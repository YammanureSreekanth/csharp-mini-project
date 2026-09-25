# Domain Model: Catalog Console App

## Classes

### `Category`
| Property | Type |
|---|---|
| `Id` | `string` |
| `Name` | `string` |
| `ParentCategory` | `Category` |
| `SubCategories?` | `List<Category>` |

### `ProductCategoryAssignment`
| Property | Type |
|---|---|
| `Product` | `BaseProduct` |
| `Category` | `Category` |
| `IsPrimary` | `bool` |

### `BaseProduct` *(abstract root)*
| Property | Type |
|---|---|
| `Id` | `int` |
| `Name` | `string` |
| `IsOnline` | `bool` |
| `IsSearchable` | `bool` |
| `ShortDescription` | `string` |
| `Type` | `ProductType` |
| `SEO` | `SeoInfo` |
| `Images` | `List<ProductImage>` |

### `MasterProduct : BaseProduct`
| Property | Type |
|---|---|
| `VariationAttributes` | `List` |
| `Variations` | `List<VariationProduct>` |

### `StandardProduct : BaseProduct`
| Property | Type |
|---|---|
| `Price` | `Money` |
| `Availability` | `StockStatus` |

### `VariationProduct : BaseProduct`
| Property | Type |
|---|---|
| `Master` | `MasterProduct` |
| `Price` | `Money` |
| `Availability` | `StockStatus` |

---

## Structs

### `Money`
| Property | Type |
|---|---|
| `Amount` | `decimal` |
| `CurrencyValue` | `Currency` |

### `SeoInfo`
| Property | Type |
|---|---|
| `PageTitle` | `string` |
| `PageKeywords` | `string` |

### `ProductImage`
| Property | Type |
|---|---|
| `Title` | `string` |
| `Alt` | `string` |
| `Path` | `string` |

### `StockStatus`
| Property | Type |
|---|---|
| `ATS` | `int` |
| `Preorder` | `int` |
| `InstockDate` | `DateOnly` |
| `IsOrderable` | `bool` |
| `IsPerectual` | `bool` |
| `AvailableStatus` | `AvailableStatus` |

---

## Enums

### `AvailableStatus`
- `IN_STOCK`
- `PRE_ORDER`
- `OUT_OF_STOCK`

### `Currency`
- `EUR`
- `USD`
- `INR`

### `ProductType`
- `MASTER_PRODUCT`
- `VARIATION_PRODUCT`
- `SET_PRODUCT`
- `STANDARD_PRODUCT`