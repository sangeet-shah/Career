DROP PROCEDURE [dbo].[ProductsQtySync];

DROP TYPE [IF EXISTS] [dbo].[ProductQtySync];

CREATE TYPE [dbo].[ProductQtySync] AS TABLE
(
    Sku NVARCHAR(800) NOT NULL,
    StockQty INT NOT NULL
);
GO

CREATE OR ALTER PROCEDURE [dbo].[ProductsQtySync]
    @tblProductQtySync [ProductQtySync] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRAN;

    -- Capture only products that will change
    DECLARE @changed TABLE
    (
        ProductId INT NOT NULL,
        Sku NVARCHAR(400) NOT NULL,
        OldQty INT NOT NULL,
        NewQty INT NOT NULL
    );

    -- Update Product and capture changes
    UPDATE p
        SET p.StockQuantity = t.StockQty
    OUTPUT
        inserted.Id,
        inserted.Sku,
        deleted.StockQuantity,
        inserted.StockQuantity
    INTO @changed (ProductId, Sku, OldQty, NewQty)
    FROM dbo.Product p
    INNER JOIN @tblProductQtySync t ON p.Sku = t.Sku
    WHERE ISNULL(p.StockQuantity, -2147483648) <> ISNULL(t.StockQty, -2147483648);

    -- ActivityLog for changed products only
    INSERT INTO dbo.ActivityLog (ActivityLogTypeId, CustomerId, Comment, CreatedOnUtc)
    SELECT
        39,
        27750532,
        'Storis edited a product(''' + c.Sku + ''')' +
        '<br/> StockQuantity: ' + CAST(c.OldQty as varchar(20)) + '/' + CAST(c.NewQty as varchar(20)),
        GETUTCDATE()
    FROM @changed c;

    -- Resolve storis customer id once
    DECLARE @storisCustomerId INT;
    SELECT TOP 1 @storisCustomerId = Id
    FROM dbo.Customer
    WHERE Email = 'storis@fm-usa.com';

    -- Mark Elastic queue as dirty for the changed products
    UPDATE ep
        SET ep.IsSynced = 0,
            ep.UpdatedOnUtc = GETUTCDATE(),
            ep.ActionTypeId = 2,
            ep.CustomerId = @storisCustomerId,
            ep.SourceId = 2
    FROM dbo.FM_ProductIndexQueue ep
    INNER JOIN @changed c ON c.ProductId = ep.ProductId;
	
	SELECT COUNT(*) AS ChangedCount FROM @changed;

    COMMIT TRAN;
END