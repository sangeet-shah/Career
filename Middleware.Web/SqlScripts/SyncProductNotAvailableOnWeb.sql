CREATE PROCEDURE dbo.Storis_ProductNotAvailableOnWebSync
(
    @Skus dbo.SkuList READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ProductNoteId INT = 6;

    -- Stage matching product IDs
    ;WITH Matched AS
    (
        SELECT p.Id AS ProductId
        FROM dbo.Product p
        INNER JOIN @Skus s ON s.Sku = p.Sku
    )
    INSERT INTO dbo.FM_Product_Note_Mapping (ProductId, ProductNoteId)
    SELECT m.ProductId, @ProductNoteId
    FROM Matched m
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.FM_Product_Note_Mapping x
        WHERE x.ProductId = m.ProductId
          AND x.ProductNoteId = @ProductNoteId
    );

    -- return affected count
    SELECT @@ROWCOUNT AS InsertedCount;
END
GO
