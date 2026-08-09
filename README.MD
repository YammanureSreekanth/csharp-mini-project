# Catalog Console App — Domain & Database Diagrams

This document collects the diagrams for each table/class in the catalog model,
plus one diagram showing how they all map together end to end.

Paste each diagram where indicated. If you're exporting images from VS Code,
save them under a `diagrams/` folder next to this README and reference them
with a relative path, e.g. `![Categories](./diagrams/categories.png)`.
Mermaid/PlantUML source blocks work too — just paste the code block in place
of the placeholder.

---

## Overall Mapping

_The full picture — how `Category`, `Product` (and its subtypes), and their
supporting tables relate to one another._

![alt text](Catalog.ConsoleApp/diagrams/overallFlow.png)
---

## Category

_`Categories` table — self-referencing tree via `ParentCategoryId`._

![alt text](Catalog.ConsoleApp/diagrams/categories.png)
---

## Products (BaseProduct / MasterProduct / StandardProduct / VariationProduct)

_`Products` table — single table with a `Type` discriminator, plus the
self-referencing `MasterProductId` for variations._

![alt text](Catalog.ConsoleApp/diagrams/products.png)

---

## MasterProduct.VariationAttributes

_`MasterProductVariationAttributes` table — one row per attribute name a
master varies by (e.g. "Size")._

![alt text](Catalog.ConsoleApp/diagrams/products.png)
---

## VariationProduct.AttributeValues

_`VariationProductAttributeValues` table — one row per attribute name/value
pair for a given variation (e.g. "Size" → "48")._

![alt text](Catalog.ConsoleApp/diagrams/products.png)

---

## ProductImages

_`ProductImages` table — one product to many images._

![alt text](Catalog.ConsoleApp/diagrams/productImages.png)

---

## ProductCategoryAssignments

_`ProductCategoryAssignments` table — many-to-many join between products and
categories, with one `IsPrimary` flag per product._

![Catalog.ConsoleApp/diagrams/productCategoryAssignments.png](Catalog.ConsoleApp/diagrams/productCategoryAssignments.png)

---

## Notes / Open Questions

_