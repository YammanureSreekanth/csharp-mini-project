# Wishlist API — End-to-End Test Report

**Environment:** `http://localhost:7071/api` (local Azure Functions host)
**Date:** 2026-09-28
**Scope:** Happy-path and well-formed-request scenarios only (400-on-malformed-input handling is out of scope for this run — the consuming client is expected to send well-formed requests; see note at the end).
**Goal:** Verify that a customer's wishlist ends up containing all 10 shared Suitsupply products, and validate the 404 / 409 error responses with valid, well-formed requests.

**Products used** (SKU-style item codes, per team convention):
`SNS10014TN1`, `SNS10014CQ1`, `SNS10014CM1`, `SNS10017LB1`, `SNS10050TD1`, `SNS10038CK1`, `SNS10054N01`, `SNS10054SK1`, `SNS10068CK1`, `SNT10001S91`

**Test customer:** `cust-200`, list `My Wishlist` (`listId: 51ac03c7-1061-4ffe-98d5-3797a2800782`)

---

## Result summary

| # | Endpoint | Scenario | Expected | Actual | Pass? |
|---|---|---|---|---|---|
| 1 | `POST /customer/list` | Valid payload | 201 | 201, list created | ✅ |
| 2 | `GET /customers/{customerId}/lists` | Valid customer | 200 | 200, list returned | ✅ |
| 3 | `GET /customers/{customerId}/lists` | Unknown customer | 200 empty array | 200, `[]` | ✅ |
| 4 | `GET /list/{listId}` | Valid list | 200 | 200 | ✅ |
| 5 | `GET /list/{listId}` | Valid GUID format, no matching row | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 6 | `POST /list/{listId}/items` | All 10 real Suitsupply SKUs | 201 each | 201 each — **all 10 added** | ✅ |
| 7 | `POST /list/{listId}/items` | Duplicate product (`SNS10014TN1` again) | 409 | 409, `{"error":"Product 'SNS10014TN1' is already in this list."}` | ✅ |
| 8 | `POST /list/{listId}/items` | Valid GUID, non-existent list | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 9 | `PATCH /list/{listId}/visibility` | Valid, set `isPublic:true` | 200 | 200, `isPublic:true` | ✅ |
| 10 | `PATCH /list/{listId}/visibility` | Valid GUID, non-existent list | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 11 | `PATCH /items/{itemId}/visibility` | Valid, set `isPublic:true` | 200 | 200, `isPublic:true` and a correct, current `modifiedDate` both returned and persisted — **fixed** (see note) | ✅ |
| 12 | `PATCH /items/{itemId}/visibility` | Valid GUID, non-existent item | 404 | 404, `{"error":"No ProductListItem found for id '...'."}` | ✅ |
| 13 | `DELETE /items/{itemId}` | Valid item (on a scratch list, not the main wishlist) | 204 | 204 | ✅ |
| 14 | `DELETE /items/{itemId}` | Valid GUID, non-existent item | 404 | 404, `{"error":"No ProductListItem found for id '...'."}` | ✅ |
| 15 | `DELETE /list/{listId}` | Valid list (scratch list) | 204 | 204 | ✅ |
| 16 | `DELETE /list/{listId}` | Valid GUID, non-existent list | 404 | 404, `{"error":"No ProductList found for id '...'."}` | ✅ |
| 17 | `GET /list/{listId}` (final check) | Confirm all 10 products present in main wishlist | all 10 in `items[]` | **Confirmed** — all 10 SKUs present | ✅ |

---

## Primary goal: confirmed ✅

Customer `cust-200`'s wishlist (`listId: 51ac03c7-1061-4ffe-98d5-3797a2800782`) contains exactly the 10 requested products after the run, verified via `GET /list/{listId}`:

`SNS10038CK1`, `SNS10054SK1`, `SNS10014CQ1`, `SNS10017LB1`, `SNS10014TN1`, `SNT10001S91`, `SNS10054N01`, `SNS10068CK1`, `SNS10014CM1`, `SNS10050TD1`

All 10 were added with `201 Created`, and the duplicate/not-found error paths were verified separately without contaminating this list — the delete-flow tests (`DELETE /items/{itemId}`, `DELETE /list/{listId}`) were run against a disposable scratch list/item created for that purpose, so the main wishlist's 10 products remain untouched.

---

## Out of scope for this run

Per direction, malformed/invalid-input handling (bad GUIDs in path params, missing required fields, invalid enum values, empty-string `productId`, negative `quantity`) was **not** re-tested here, since the calling party is expected to always send well-formed requests. Those findings from the previous run still stand if validation hardening is revisited later.
