-- =========================================================
-- Drop existing tables (children first, respecting FK order)
-- =========================================================
IF OBJECT_ID('dbo.ProductCategoryAssignments', 'U') IS NOT NULL DROP TABLE dbo.ProductCategoryAssignments;
IF OBJECT_ID('dbo.VariationProductAttributeValues', 'U') IS NOT NULL DROP TABLE dbo.VariationProductAttributeValues;
IF OBJECT_ID('dbo.MasterProductVariationAttributes', 'U') IS NOT NULL DROP TABLE dbo.MasterProductVariationAttributes;
IF OBJECT_ID('dbo.ProductImages', 'U') IS NOT NULL DROP TABLE dbo.ProductImages;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
GO

-- =========================================================
-- Categories (self-referencing tree)
-- Id is a business code (e.g. "suits-tuxedo"), matching the
-- real category-id values from the SFCC export.
-- =========================================================
CREATE TABLE dbo.Categories (
    Id                  VARCHAR(50)         NOT NULL,
    Name                NVARCHAR(150)       NOT NULL,
    ParentCategoryId    VARCHAR(50)         NULL,

    CONSTRAINT PK_Categories PRIMARY KEY (Id),
    CONSTRAINT FK_Categories_Parent
        FOREIGN KEY (ParentCategoryId) REFERENCES dbo.Categories(Id)
        ON DELETE NO ACTION -- self-referencing: SQL Server disallows CASCADE here
);
GO

CREATE INDEX IX_Categories_ParentCategoryId ON dbo.Categories(ParentCategoryId);
GO

-- =========================================================
-- Products (single table for BaseProduct/MasterProduct/
-- StandardProduct/VariationProduct, discriminated by Type).
-- Id is the product's business code (e.g. "B125902"),
-- matching the real product-id values from the SFCC export.
--
-- Price*/Stock* columns are only populated for Standard and
-- Variation products (MasterProduct has neither — its display
-- price and availability are derived from its variations).
-- MasterProductId is only populated for Variation products.
-- =========================================================
CREATE TABLE dbo.Products (
    Id                  VARCHAR(50)         NOT NULL,
    Name                NVARCHAR(200)       NOT NULL,
    IsOnline            BIT                 NOT NULL CONSTRAINT DF_Products_IsOnline DEFAULT (0),
    IsSearchable        BIT                 NOT NULL CONSTRAINT DF_Products_IsSearchable DEFAULT (1),
    ShortDescription    NVARCHAR(500)       NULL,
    Type                TINYINT             NOT NULL,   -- 1=Master, 2=Standard, 3=Variation
    SeoPageTitle        NVARCHAR(200)       NULL,
    SeoPageKeywords     NVARCHAR(300)       NULL,

    -- StandardProduct / VariationProduct only
    PriceAmount         DECIMAL(18,4)       NULL CONSTRAINT CK_Products_PriceAmount CHECK (PriceAmount IS NULL OR PriceAmount >= 0),
    PriceCurrency       CHAR(3)             NULL,

    -- VariationProduct only — points back to its MasterProduct
    MasterProductId     VARCHAR(50)         NULL,

    -- StandardProduct / VariationProduct only (StockStatus struct)
    StockAts                INT             NULL,
    StockPreorder            INT            NULL,
    StockInstockDate        DATE            NULL,
    StockIsOrderable        BIT             NULL,
    StockIsPerpetual        BIT             NULL,
    StockAvailableStatus    TINYINT         NULL,        -- 1=InStock, 2=PreOrder, 3=OutOfStock

    CONSTRAINT PK_Products PRIMARY KEY (Id),
    CONSTRAINT FK_Products_Master
        FOREIGN KEY (MasterProductId) REFERENCES dbo.Products(Id)
        ON DELETE NO ACTION -- self-referencing: SQL Server disallows CASCADE here
);
GO

CREATE INDEX IX_Products_MasterProductId ON dbo.Products(MasterProductId);
GO

-- =========================================================
-- MasterProduct.VariationAttributes (List<string>)
-- e.g. ("Color"), ("Size") for a given master
-- =========================================================
CREATE TABLE dbo.MasterProductVariationAttributes (
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProductId           VARCHAR(50)         NOT NULL,
    AttributeName       NVARCHAR(50)        NOT NULL,

    CONSTRAINT PK_MasterProductVariationAttributes PRIMARY KEY (Id),
    CONSTRAINT FK_MPVA_Product
        FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_MPVA_ProductId ON dbo.MasterProductVariationAttributes(ProductId);
GO

-- =========================================================
-- VariationProduct.AttributeValues (Dictionary<string,string>)
-- e.g. ("Color", "Navy") for a given variation
-- =========================================================
CREATE TABLE dbo.VariationProductAttributeValues (
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProductId           VARCHAR(50)         NOT NULL,
    AttributeName        NVARCHAR(50)       NOT NULL,
    AttributeValue        NVARCHAR(100)     NOT NULL,

    CONSTRAINT PK_VariationProductAttributeValues PRIMARY KEY (Id),
    CONSTRAINT FK_VPAV_Product
        FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_VPAV_ProductId ON dbo.VariationProductAttributeValues(ProductId);
GO

-- =========================================================
-- Product images (1 product -> many images)
-- =========================================================
CREATE TABLE dbo.ProductImages (
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProductId           VARCHAR(50)         NOT NULL,
    Title               NVARCHAR(150)       NULL,
    Alt                 NVARCHAR(200)       NULL,
    Path                NVARCHAR(500)       NOT NULL,

    CONSTRAINT PK_ProductImages PRIMARY KEY (Id),
    CONSTRAINT FK_ProductImages_Product
        FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_ProductImages_ProductId ON dbo.ProductImages(ProductId);
GO

-- =========================================================
-- Product <-> Category assignments (many-to-many, one primary)
-- =========================================================
CREATE TABLE dbo.ProductCategoryAssignments (
    ProductId           VARCHAR(50)         NOT NULL,
    CategoryId           VARCHAR(50)        NOT NULL,
    IsPrimary            BIT                NOT NULL CONSTRAINT DF_PCA_IsPrimary DEFAULT (0),

    CONSTRAINT PK_ProductCategoryAssignments PRIMARY KEY (ProductId, CategoryId),
    CONSTRAINT FK_PCA_Product
        FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
        ON DELETE CASCADE,
    CONSTRAINT FK_PCA_Category
        FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_PCA_CategoryId ON dbo.ProductCategoryAssignments(CategoryId);
GO