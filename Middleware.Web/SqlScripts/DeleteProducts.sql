CREATE TYPE dbo.SkuList AS TABLE
(
    Sku NVARCHAR(800) NOT NULL PRIMARY KEY
);
GO

DROP PROCEDURE [dbo].[sp_DeleteProductsBySKUs];

CREATE PROCEDURE dbo.sp_DeleteProductsBySKUs
(
    @Skus dbo.SkuList READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @RemovedProductStatusId INT = 3;
    DECLARE @storisCustomerId INT;

    SELECT TOP 1 @storisCustomerId = Id
    FROM Customer
    WHERE Email = 'storis@fm-usa.com';

    -- stage matching products to delete
    DECLARE @ToProcess TABLE
    (
        RowId INT IDENTITY(1,1) PRIMARY KEY,
        ProductId INT NOT NULL,
        Sku NVARCHAR(800) NOT NULL
    );

    INSERT INTO @ToProcess(ProductId, Sku)
    SELECT p.Id, p.Sku
    FROM Product p
    INNER JOIN @Skus s ON s.Sku = p.Sku
    INNER JOIN FM_Product fp ON fp.ProductId = p.Id
    WHERE p.Published <> 0 OR fp.StatusId <> @RemovedProductStatusId;

    -- product table changes (set-based)
    UPDATE p
    SET p.Published = 0,
        p.Deleted = 1
    FROM Product p
    INNER JOIN @ToProcess t ON t.ProductId = p.Id;

    -- activity log (set-based)
    INSERT INTO ActivityLog([ActivityLogTypeId], [CustomerId], [Comment], [CreatedOnUtc])
    SELECT 39, 27750532,
           'Storis Delete Product Status Update for sku: ' + p.Sku +
           ' <br/> Published: 0 <br/> Status: Removed',
           GETUTCDATE()
    FROM Product p
    INNER JOIN @ToProcess t ON t.ProductId = p.Id;

    -- per-product steps (queue + history + FM_Product status)
    DECLARE @i INT = 1, @count INT, @sku NVARCHAR(800), @productId INT;

    SELECT @count = COUNT(1) FROM @ToProcess;

    WHILE @i <= @count
    BEGIN
        SELECT @productId = ProductId, @sku = Sku
        FROM @ToProcess
        WHERE RowId = @i;

        EXEC UpdateElasticProductQueue @productId, @storisCustomerId, 2;

        UPDATE fp
        SET fp.StatusId = @RemovedProductStatusId
        FROM FM_Product fp
        WHERE fp.ProductId = @productId;

        EXEC dbo.UpdateProductStatusHistory @productId, @RemovedProductStatusId;

        SET @i += 1;
    END

    -- Optional: return count processed (useful for C#)
    SELECT @count AS DeletedCandidates;
END
GO
