# Wishlist API — End-to-End Test Report

**Environment:** `http://localhost:7071/api` (local Azure Functions host)
**Date:** 2026-09-30
**Scope:** Happy-path and well-formed-request scenarios only (400-on-malformed-input handling remains out of scope — the consuming client is expected to send well-formed requests).
**Goal:** Verify that a customer's wishlist ends up containing all 10 shared Suitsupply products, and validate the 404 / 409 error responses with valid, well-formed requests, against the **latest API contract** (route changes below).

**Products used** (SKU-style item codes, per team convention):
`SNS10014TN1`, `SNS10014CQ1`, `SNS10014CM1`, `SNS10017LB1`, `SNS10050TD1`, `SNS10038CK1`, `SNS10054N01`, `SNS10054SK1`, `SNS10068CK1`, `SNT10001S91`

**Test customer:** `cust-300`, list `My Wishlist` (`listId: 044223e3-06dd-4260-a5d2-2e8ef14e704a`)

---

## API contract changes since the last report

Pulled from the latest `http://localhost:7071/api/swagger.json`:

| Old | New | Notes |
|---|---|---|
| `POST /customer/list` (customerId in body) | `POST /customer/{customerId}/list` | `customerId` moved from body to path; `createListRequest` no longer carries it |
| `PATCH /list/{listId}/visibility` | `PATCH /list/{listId}` | List visibility update route simplified, no `/visibility` suffix |
| `PATCH /items/{itemId}/visibility` | `PATCH /lists/{listId}/items/{itemId}` | Item visibility route now nests under its parent list, and adds `listId` as a path param |
| `DELETE /items/{itemId}` | `DELETE /lists/{listId}/items/{itemId}` | Item delete route likewise nested under `listId` |
| `updateItemVisibilityRequest` = `{isPublic}` | `updateItemVisibilityRequest` = `{isPublic, quantity}` | Item visibility PATCH can now also update `quantity` in the same call |
| `productListDto` / `productListWithItemsDto` | now include `customerId` | List responses now echo the owning customer |

---

## Result summary

| # | Endpoint | Scenario | Expected | Actual | Pass? |
|---|---|---|---|---|---|
| 1 | `POST /customer/{customerId}/list` | Valid payload | 201 | 201, list created with `customerId:"cust-300"` echoed back | ✅ |
| 2 | `GET /customers/{customerId}/lists` | Valid customer | 200 | 200, list returned | ✅ |
| 3 | `GET /customers/{customerId}/lists` | Unknown customer | 200 empty array | 200, `[]` | ✅ |
| 4 | `GET /list/{listId}` | Valid list | 200 | 200 | ✅ |
| 5 | `GET /list/{listId}` | Valid GUID format, no matching row | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 6 | `POST /list/{listId}/items` | All 10 real Suitsupply SKUs | 201 each | 201 each — **all 10 added** | ✅ |
| 7 | `POST /list/{listId}/items` | Duplicate product (`SNS10014TN1` again) | 409 | 409, `{"error":"Product 'SNS10014TN1' is already in this list."}` | ✅ |
| 8 | `POST /list/{listId}/items` | Valid GUID, non-existent list | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 9 | `PATCH /list/{listId}` | Valid, set `isPublic:true` | 200 | 200, `isPublic:true` | ✅ |
| 10 | `PATCH /list/{listId}` | Valid GUID, non-existent list | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 11 | `PATCH /lists/{listId}/items/{itemId}` | Valid, set `isPublic:true, quantity:2` | 200 | 200, both `isPublic:true` and `quantity:2` returned and persisted, with a correct current `modifiedDate` | ✅ |
| 12 | `PATCH /lists/{listId}/items/{itemId}` | Valid GUIDs, non-existent item | 404 | 404, `{"error":"No ProductListItem found for id '...'."}` | ✅ |
| 13 | `DELETE /lists/{listId}/items/{itemId}` | Valid item (scratch list, not main wishlist) | 204 | 204 | ✅ |
| 14 | `DELETE /lists/{listId}/items/{itemId}` | Valid GUIDs, non-existent item | 404 | 404, `{"error":"No ProductListItem found for id '...'."}` | ✅ |
| 15 | `DELETE /list/{listId}` | Valid list (scratch list) | 204 | 204 | ✅ |
| 16 | `DELETE /list/{listId}` | Valid GUID, non-existent list | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 17 | `GET /list/{listId}` (final check) | Confirm all 10 products present in main wishlist | all 10 in `items[]` | **Confirmed** — all 10 SKUs present | ✅ |

**17/17 passed.**

---

## Primary goal: confirmed ✅

Customer `cust-300`'s wishlist (`listId: 044223e3-06dd-4260-a5d2-2e8ef14e704a`) contains exactly the 10 requested products after the run, verified via `GET /list/{listId}`:

`SNS10050TD1`, `SNT10001S91`, `SNS10054N01`, `SNS10014CM1`, `SNS10014CQ1`, `SNS10014TN1`, `SNS10017LB1`, `SNS10054SK1`, `SNS10068CK1`, `SNS10038CK1`

All 10 were added with `201 Created`. Duplicate/not-found error paths and the delete flows were verified separately against a disposable scratch customer/list/item, so the main wishlist's 10 products remain untouched throughout the run.

---

## Notes

- The previously-tracked `modifiedDate` bug on the item-visibility endpoint remains fixed under the new route (`PATCH /lists/{listId}/items/{itemId}`) — both the response and the persisted row show the correct, current timestamp.
- The item-visibility endpoint's new `quantity` field was exercised (`1 → 2`) and persists correctly alongside `isPublic`.

---

## Out of scope for this run

Malformed/invalid-input handling (bad GUIDs in path params, missing required fields, invalid enum values, empty-string `productId`, negative `quantity`) was **not** tested here, per prior direction that the calling party is expected to always send well-formed requests. Findings from the earlier run on the old routes still stand if validation hardening is revisited later, and should be re-verified against the new routes if/when that work happens.
