DROP PROCEDURE [dbo].[Storis_ProductImageSourceTypes];

DROP TYPE [IF EXISTS] [dbo].[Storis_ProductImageSourceTypes];

CREATE TYPE [dbo].[Storis_ProductImageSourceTypes] AS TABLE(
	[ProductId] [nvarchar](800) NOT NULL,
	[ImageSourceType] [int] NOT NULL
)
GO

CREATE PROCEDURE [dbo].[Storis_ProductImageSourceTypesSync]
(
  @Storis_ProductImageSourceTypes Storis_ProductImageSourceTypes READONLY
)
AS
BEGIN
  -- Merge statement to handle both matching and non-matching cases
  MERGE INTO FM_Product AS target
  USING (
      SELECT p.id AS ProductId, s.ImageSourceType
      FROM @Storis_ProductImageSourceTypes s
      JOIN Product p ON p.SKU = s.ProductID
  ) AS source
  ON target.ProductId = source.ProductId
  WHEN MATCHED AND target.ImageSourceTypeId <> source.ImageSourceType THEN
    UPDATE SET target.ImageSourceTypeId = source.ImageSourceType
  WHEN NOT MATCHED THEN
    INSERT (ProductId, ImageSourceTypeId)
    VALUES (source.ProductId, source.ImageSourceType);
END;