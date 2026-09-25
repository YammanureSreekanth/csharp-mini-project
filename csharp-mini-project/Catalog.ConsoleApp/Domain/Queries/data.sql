-- =========================================================
-- Seed data sourced from the real SFCC exports:
--   navigation_catalog.xml (categories, category-assignments)
--   master_catalog.xml     (products, variations, images)
--   eur-list-prices.xml    (price, quantity=1 tier only)
--   int-inventory.xml      (ats, perpetual, preorder-backorder-handling)
--
-- Scope: 2 master products (jacket + trousers) with 2 variants
-- each, 8 standalone accessory products. All under the real
-- category tree these products are actually assigned to.
-- =========================================================

-- ---------------------------------------------------------
-- Categories (parents before children)
-- ---------------------------------------------------------
INSERT INTO dbo.Categories (Id, Name, ParentCategoryId) VALUES
    ('root', 'Suitsupply', NULL),
    ('Men', 'Men', 'root'),
    ('Suits', 'Suits', 'Men'),
    ('Trousers', 'Trousers', 'Men'),
    ('Accessories', 'Accessories', 'Men'),
    ('black-tie-collection', 'Black Tie', 'Men'),
    ('Ties', 'Ties & Bow Ties', 'Accessories'),
    ('Handkerchiefs', 'Pocket Squares', 'Accessories'),
    ('Cufflinks', 'Cufflinks', 'Accessories'),
    ('TiesSub', 'Ties', 'Ties'),
    ('Unlined_Ties', 'Unlined Ties', 'Ties');
GO

-- ---------------------------------------------------------
-- Products: Master products first (Type = 1)
-- ---------------------------------------------------------
INSERT INTO dbo.Products
    (Id, Name, IsOnline, IsSearchable, ShortDescription, Type, SeoPageTitle,
     PriceAmount, PriceCurrency, MasterProductId,
     StockAts, StockPreorder, StockInstockDate, StockIsOrderable, StockIsPerpetual, StockAvailableStatus)
VALUES
    ('C1199', 'Black Tailored Fit Lazio Dinner Jacket', 0, 0,
     'Mid-weight fabric, made from 100% wool.', 1, 'Black Tailored Fit Lazio Dinner Jacket',
     NULL, NULL, NULL,
     NULL, NULL, NULL, NULL, NULL, NULL),

    ('B1299', 'Black Slim Leg Straight Tuxedo Trousers', 0, 0,
     'Structured, durable pure S110''s wool, woven by Vitale Barberis Canonico, Italy.', 1, 'Black Slim Leg Straight Tuxedo Trousers',
     NULL, NULL, NULL,
     NULL, NULL, NULL, NULL, NULL, NULL);
GO

-- ---------------------------------------------------------
-- Products: Standard products (Type = 2)
-- ---------------------------------------------------------
INSERT INTO dbo.Products
    (Id, Name, IsOnline, IsSearchable, ShortDescription, Type, SeoPageTitle,
     PriceAmount, PriceCurrency, MasterProductId,
     StockAts, StockPreorder, StockInstockDate, StockIsOrderable, StockIsPerpetual, StockAvailableStatus)
VALUES
    ('D221032', 'Black Plain Tie', 1, 1, NULL, 2, 'Black Plain Tie',
     59.00, 'EUR', NULL,
     124, 0, NULL, 1, 0, 1),

    ('D005', 'Black Self-tied Bow Tie', 0, 1, NULL, 2, 'Black Self-tied Bow Tie',
     29.00, 'EUR', NULL,
     125, 0, NULL, 1, 0, 1),

    ('PS984', 'White Pocket Square', 1, 1, NULL, 2, 'White Pocket Square',
     19.00, 'EUR', NULL,
     124, 0, NULL, 1, 0, 1),

    ('D015', 'Navy Pre-tied Bow Tie', 0, 1, NULL, 2, 'Navy Pre-tied Bow Tie',
     29.00, 'EUR', NULL,
     125, 0, NULL, 1, 0, 1),

    ('SS002', 'Black Tuxedo Shirt Studs', 1, 1, NULL, 2, 'Black Tuxedo Shirt Studs',
     19.00, 'EUR', NULL,
     95, 0, NULL, 1, 0, 1),

    ('D016', 'Black Pre-tied Bow Tie', 0, 1, NULL, 2, 'Black Pre-tied Bow Tie',
     29.00, 'EUR', NULL,
     125, 0, NULL, 1, 0, 1),

    ('D221023', 'Pink Tie', 1, 1, NULL, 2, 'Pink Tie',
     79.00, 'EUR', NULL,
     125, 0, NULL, 1, 0, 1),

    ('D222035', 'Navy Tie', 1, 1, NULL, 2, 'Navy Tie',
     59.00, 'EUR', NULL,
     125, 0, NULL, 1, 0, 1);
GO

-- ---------------------------------------------------------
-- Products: Variation products (Type = 3)
-- Name/Description/Images are not present on variant records
-- in the source data (only the master carries them) — Name
-- below is synthesized from the master + size for display.
-- ---------------------------------------------------------
INSERT INTO dbo.Products
    (Id, Name, IsOnline, IsSearchable, ShortDescription, Type, SeoPageTitle,
     PriceAmount, PriceCurrency, MasterProductId,
     StockAts, StockPreorder, StockInstockDate, StockIsOrderable, StockIsPerpetual, StockAvailableStatus)
VALUES
    ('C119905', 'Black Tailored Fit Lazio Dinner Jacket - Size 48', 1, 1, NULL, 3, NULL,
     379.00, 'EUR', 'C1199',
     125, 0, NULL, 1, 0, 1),

    ('C119909', 'Black Tailored Fit Lazio Dinner Jacket - Size 56', 1, 1, NULL, 3, NULL,
     379.00, 'EUR', 'C1199',
     125, 0, NULL, 1, 0, 1),

    ('B129929', 'Black Slim Leg Straight Tuxedo Trousers - Size 28', 1, 1, NULL, 3, NULL,
     169.00, 'EUR', 'B1299',
     125, 0, NULL, 1, 0, 1),

    ('B129903', 'Black Slim Leg Straight Tuxedo Trousers - Size 44', 1, 1, NULL, 3, NULL,
     169.00, 'EUR', 'B1299',
     125, 0, NULL, 1, 0, 1);
GO

-- ---------------------------------------------------------
-- MasterProduct.VariationAttributes (List<string>)
-- Both masters vary by Size only, per the source data.
-- ---------------------------------------------------------
INSERT INTO dbo.MasterProductVariationAttributes (ProductId, AttributeName) VALUES
    ('C1199', 'Size'),
    ('B1299', 'Size');
GO

-- ---------------------------------------------------------
-- VariationProduct.AttributeValues (Dictionary<string,string>)
-- ---------------------------------------------------------
INSERT INTO dbo.VariationProductAttributeValues (ProductId, AttributeName, AttributeValue) VALUES
    ('C119905', 'Size', '48'),
    ('C119909', 'Size', '56'),
    ('B129929', 'Size', '28'),
    ('B129903', 'Size', '44');
GO

-- ---------------------------------------------------------
-- Product images
-- Paths are relative, matching the source export — prepend
-- your CDN base URL when rendering (no title/alt data exists
-- per-image in the source; both default to the product Name).
-- ---------------------------------------------------------
INSERT INTO dbo.ProductImages (ProductId, Title, Alt, Path) VALUES
    ('C1199', 'Black Tailored Fit Lazio Dinner Jacket', 'Black Tailored Fit Lazio Dinner Jacket', 'products/Jackets/default/Winter/C1199_1'),
    ('C1199', 'Black Tailored Fit Lazio Dinner Jacket', 'Black Tailored Fit Lazio Dinner Jacket', 'products/Jackets/default/C1199_1'),

    ('B1299', 'Black Slim Leg Straight Tuxedo Trousers', 'Black Slim Leg Straight Tuxedo Trousers', 'products/Trousers/default/Winter/B1299_1'),
    ('B1299', 'Black Slim Leg Straight Tuxedo Trousers', 'Black Slim Leg Straight Tuxedo Trousers', 'products/Trousers/default/Summer/B1299_1'),

    ('D221032', 'Black Plain Tie', 'Black Plain Tie', 'products/Ties/default/Winter/D221032_1'),
    ('D221032', 'Black Plain Tie', 'Black Plain Tie', 'products/Ties/default/Summer/D221032_1'),

    ('D005', 'Black Self-tied Bow Tie', 'Black Self-tied Bow Tie', 'products/ties/default/Winter/D005_1'),
    ('D005', 'Black Self-tied Bow Tie', 'Black Self-tied Bow Tie', 'products/ties/default/Summer/D005_1'),

    ('PS984', 'White Pocket Square', 'White Pocket Square', 'products/Handkerchiefs/default/Winter/PS984_1'),
    ('PS984', 'White Pocket Square', 'White Pocket Square', 'products/Handkerchiefs/default/PS984_1'),

    ('D015', 'Navy Pre-tied Bow Tie', 'Navy Pre-tied Bow Tie', 'products/ties/default/Summer/D015_1'),
    ('D015', 'Navy Pre-tied Bow Tie', 'Navy Pre-tied Bow Tie', 'products/Ties/default/Winter/D015_1'),

    ('SS002', 'Black Tuxedo Shirt Studs', 'Black Tuxedo Shirt Studs', 'products/Cufflinks/default/SS002_1'),

    ('D016', 'Black Pre-tied Bow Tie', 'Black Pre-tied Bow Tie', 'products/ties/default/Summer/D016_1'),
    ('D016', 'Black Pre-tied Bow Tie', 'Black Pre-tied Bow Tie', 'products/Ties/default/Winter/D016_1'),

    ('D221023', 'Pink Tie', 'Pink Tie', 'products/Ties/default/Winter/D221023_1'),
    ('D221023', 'Pink Tie', 'Pink Tie', 'products/Ties/default/Summer/D221023_1'),

    ('D222035', 'Navy Tie', 'Navy Tie', 'products/ties/default/D222035_1'),
    ('D222035', 'Navy Tie', 'Navy Tie', 'products/Ties/default/Winter/D222035_1');
GO

-- ---------------------------------------------------------
-- Category assignments — primary categories are the real,
-- specific SFCC leaf categories each product is assigned to
-- (not a generic "accessories" bucket); secondary is
-- black-tie-collection where the source data actually
-- assigns it (not all products get one — two are
-- cross-assigned to Accessories instead).
-- ---------------------------------------------------------
INSERT INTO dbo.ProductCategoryAssignments (ProductId, CategoryId, IsPrimary) VALUES
    ('C1199', 'Suits', 1),
    ('C1199', 'black-tie-collection', 0),

    ('B1299', 'Trousers', 1),
    ('B1299', 'black-tie-collection', 0),

    ('D221032', 'TiesSub', 1),
    ('D221032', 'black-tie-collection', 0),

    ('D005', 'Ties', 1),
    ('D005', 'black-tie-collection', 0),

    ('PS984', 'Handkerchiefs', 1),
    ('PS984', 'black-tie-collection', 0),

    ('D015', 'Ties', 1),
    ('D015', 'black-tie-collection', 0),

    ('SS002', 'Cufflinks', 1),
    ('SS002', 'black-tie-collection', 0),

    ('D016', 'Ties', 1),
    ('D016', 'black-tie-collection', 0),

    ('D221023', 'Unlined_Ties', 1),
    ('D221023', 'Accessories', 0),

    ('D222035', 'TiesSub', 1),
    ('D222035', 'Accessories', 0);
GO