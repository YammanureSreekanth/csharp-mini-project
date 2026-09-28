# Wishlist infrastructure (Bicep)

Bicep creates the Azure resources only — no tables, no app code. Those are
separate steps below. Debugging context for anything marked ⚠️ is in
`docs/LEARNINGS.md`.

## Layout

```
infra/
├── main.bicep                entry point (subscription scope) — creates the resource group
├── main.bicepparam           local values, gitignored
├── main.bicepparam.example   committed template with placeholders
├── modules/
│   ├── monitoring.bicep      Log Analytics + Application Insights
│   ├── storage.bicep         storage account + app-package container
│   ├── sql.bicep             SQL server (Entra-only auth) + Wishlist database
│   └── functionapp.bicep     Flex Consumption plan + Function App + storage role
└── sql/
    ├── migrations.sql            generated from EF, gitignored, regenerate every deploy
    └── grant-app-identity.sql    creates the app's DB user, committed, no secrets
```

## What gets created

| Resource | Notes |
|---|---|
| Resource group `rg-wishlist-dev` | Delete it to remove everything |
| Log Analytics + App Insights | 1 GB/day cap |
| Storage account | Host storage + `app-package` container (Flex needs this) |
| SQL server + `Wishlist` DB | Serverless, auto-pause, **Entra-only** — no SQL passwords |
| Flex Consumption (`FC1`) + Function App | System-assigned managed identity, no storage keys |
| Role assignment | App identity → `Storage Blob Data Owner` on storage |

**Flex Consumption, not Y1:** ⚠️ new/trial subscriptions can have zero Y1
quota everywhere. FC1 uses a different allocation, is Linux-only, and
configures the runtime under `functionAppConfig`, not `siteConfig`.

## 1. One-time prerequisites

```bash
az login
az account set --subscription "<subscription name>"

for ns in Microsoft.Web Microsoft.Sql Microsoft.Insights Microsoft.OperationalInsights Microsoft.Storage; do
  az provider register --namespace $ns
done
# re-run to confirm all say "Registered"
for ns in Microsoft.Web Microsoft.Sql Microsoft.Insights Microsoft.OperationalInsights Microsoft.Storage; do
  echo -n "$ns: "; az provider show --namespace $ns --query registrationState -o tsv
done
```

Copy `main.bicepparam.example` → `main.bicepparam`, fill in:

```bash
az ad signed-in-user show --query userPrincipalName -o tsv   # → sqlAdminLogin
az ad signed-in-user show --query id -o tsv                  # → sqlAdminObjectId
```

## 2. Deploy the infrastructure

Run from the folder containing `infra/`.

```bash
az bicep build --file infra/main.bicep
az deployment sub what-if --location northeurope \
  --template-file infra/main.bicep --parameters infra/main.bicepparam
az deployment sub create --location northeurope \
  --template-file infra/main.bicep --parameters infra/main.bicepparam
```

`create` is safe to re-run — Bicep only touches what's missing/different. If
one resource fails partway, fix the parameter and re-run the same command; a
resource stuck mid-creation may need deleting first (`az sql server delete
...`) if a region retry says "already exists in location X".

⚠️ **Regions are inconsistent on new subscriptions** and `what-if` doesn't
catch capacity issues — only real deploys do. What worked here (Sept 2026):
`northeurope` for everything except SQL, `swedencentral` for SQL (separate
`sqlLocation` param — SQL is often restricted independently). `westeurope`
refused every resource type.

## 3. Create the tables

```bash
dotnet ef migrations script --idempotent \
  --project src/Wishlist.Functions --output infra/sql/migrations.sql
```

Run it against the `Wishlist` DB — portal **Query editor (preview)** with
Entra auth, or `sqlcmd -S <fqdn> -d Wishlist -G -i infra/sql/migrations.sql`
(allow your IP on the SQL firewall first). Idempotent — safe to re-run.

```sql
SELECT name FROM sys.tables;
SELECT MigrationId FROM __EFMigrationsHistory;
```

## 4. Grant the app's identity database access

Bicep can't do this — the identity only exists once the Function App is
created. On the `Wishlist` database (not `master`):

```sql
CREATE USER [<function-app-name>] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [<function-app-name>];
ALTER ROLE db_datawriter ADD MEMBER [<function-app-name>];
```

or:

```bash
sqlcmd -S <sql-fqdn> -d Wishlist -G -v AppName="<function-app-name>" \
  -i infra/sql/grant-app-identity.sql
```

Skipping this → **500 with a SQL login failure** on the first real request.

## 5. Publish the code

```bash
cd src/Wishlist.Functions
func azure functionapp publish <function-app-name>
```

⚠️ If prompted about `dotnet-isolated|10.0` (local) vs `10` (Azure), answer
**yes** — treating the two strings as equivalent broke trigger sync and left
the app unhealthy (see LEARNINGS.md). Keep `dotnetVersion` in
`functionapp.bicep` set to whatever value actually worked, so a later Bicep
deploy doesn't revert it.

## 6. Look up generated names

Names include `uniqueString(resourceGroup().id)` — never hardcode them:

```bash
az functionapp list -g rg-wishlist-dev --query "[0].name" -o tsv
az sql server list -g rg-wishlist-dev --query "[0].fullyQualifiedDomainName" -o tsv
```

## 7. Smoke test

```bash
BASE=https://<function-app-name>.azurewebsites.net/api

curl -i -X POST $BASE/customer/list -H "Content-Type: application/json" \
  -d '{"name":"Birthday","customerId":"c1","type":10,"isPublic":true}'

curl -i $BASE/customers/c1/lists
```

`201` then `200` with data = full chain works (host, worker, identity,
storage, SQL). ⚠️ A `401` means the trigger's `AuthorizationLevel` defaulted
to `Function` — see LEARNINGS.md.

## Tear down

```bash
az group delete --name rg-wishlist-dev --yes --no-wait
```

Deletes everything, including all data. Rebuild = steps 2–5.

## Costs

- Function App (Flex Consumption): pay-per-use, ~zero when idle.
- SQL serverless + auto-pause: billed only while active; first request after
  a pause is slow (code uses `EnableRetryOnFailure()` — bump `Connection
  Timeout` if that's not enough).
- Log Analytics capped at 1 GB/day.
- Set a **budget alert** (Cost Management → Budgets) — it only notifies, never
  stops spend.

## Known gaps

- Endpoints are anonymous (or default to `Function`-key auth — pick one
  consistently). No caller identity is verified; `customerId` is trusted as
  given. Next step: Entra ID / JWT auth (Phase 5).
- Flex Consumption Bicep shape was current at write time — verify against
  `az functionapp list-flexconsumption-runtimes` /
  `list-flexconsumption-locations` before assuming it still holds.
- Pipeline doesn't run steps 3–4 (SQL) automatically — see
  `docs/GITHUB-WORKFLOWS.md`.