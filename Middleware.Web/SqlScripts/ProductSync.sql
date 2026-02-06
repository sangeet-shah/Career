DROP PROCEDURE [dbo].[Storis_ProductsSync];

DROP TYPE [IF EXISTS] [dbo].[StorisProductSync];

CREATE TYPE [dbo].[StorisProductSync] AS TABLE(
	[id] [nvarchar](800) NOT NULL,
	[originalDescription] [nvarchar](800) NULL,
	[vendorModelNumber] [nvarchar](800) NULL,
	[vendorUPCCode] [nvarchar](800) NULL,
	[currentPrice] [decimal](18, 4) NOT NULL,
	[msrp] [decimal](18, 4) NOT NULL,
	[AverageCost] [decimal](18, 4) NOT NULL,
	[weight] [decimal](18, 4) NOT NULL,
	[depth] [decimal](18, 4) NOT NULL,
	[width] [decimal](18, 4) NOT NULL,
	[height] [decimal](18, 4) NOT NULL,
	[beginningPromoDate] [datetime2](7) NULL,
	[endingPromoDate] [datetime2](7) NULL,
	[promoPrice] [decimal](18, 4) NULL,
	[brandDescirption] [nvarchar](800) NULL,
	[brandId] [nvarchar](800) NULL,
	[VendorName] [nvarchar](800) NULL,
	[vendorid] [nvarchar](800) NULL,
	[status] [nvarchar](400) NULL,
	[GroupID] [nvarchar](400) NULL,
	[DateChanged] [datetime2](7) NULL,
	[DateCreated] [datetime2](7) NULL
)
GO

CREATE OR ALTER PROCEDURE [dbo].[Storis_ProductsSync]
    @StorisProductSync [dbo].[StorisProductSync] READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @currentDateTime DATETIME2(7) =
            CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Central Standard Time' AS DATETIME2(7));

        --------------------------------------------------------------------
        -- Load blocked group ids once (trimmed)
        --------------------------------------------------------------------
        DECLARE @BlockedGroups TABLE (GroupID NVARCHAR(400) NOT NULL PRIMARY KEY);

        INSERT INTO @BlockedGroups(GroupID)
        SELECT DISTINCT LTRIM(RTRIM(s.value))
        FROM dbo.[Setting] t
        CROSS APPLY STRING_SPLIT(t.[Value], ',') s
        WHERE t.[Name] = 'dwdatabasesetting.blockedproductgroupids'
          AND LTRIM(RTRIM(s.value)) <> '';

        --------------------------------------------------------------------
        -- Stage input into temp table with same types as TVP
        -- + de-duplicate by Id within this batch (optional but recommended)
        --------------------------------------------------------------------
        CREATE TABLE #src
        (
            Id NVARCHAR(800) NOT NULL,
            OriginalDescription NVARCHAR(800) NULL,
            VendorModelNumber NVARCHAR(800) NULL,
            VendorUPCCode NVARCHAR(800) NULL,
            CurrentPrice DECIMAL(18,4) NOT NULL,
            Msrp DECIMAL(18,4) NOT NULL,
            AverageCost DECIMAL(18,4) NOT NULL,
            Weight DECIMAL(18,4) NOT NULL,
            Depth DECIMAL(18,4) NOT NULL,
            Width DECIMAL(18,4) NOT NULL,
            Height DECIMAL(18,4) NOT NULL,
            BeginningPromoDate DATETIME2(7) NULL,
            EndingPromoDate DATETIME2(7) NULL,
            PromoPrice DECIMAL(18,4) NULL,
            BrandDescirption NVARCHAR(800) NULL,
            BrandId NVARCHAR(800) NULL,
            VendorName NVARCHAR(800) NULL,
            VendorId NVARCHAR(800) NULL,
            [Status] NVARCHAR(400) NULL,
            GroupID NVARCHAR(400) NULL,
            DateChanged DATETIME2(7) NULL,
            DateCreated DATETIME2(7) NULL
        );

        ;WITH Dedup AS
        (
            SELECT
                sp.*,
                rn = ROW_NUMBER() OVER
                (
                    PARTITION BY sp.id
                    ORDER BY
                        ISNULL(sp.DateChanged, '19000101') DESC,
                        ISNULL(sp.DateCreated, '19000101') DESC
                )
            FROM @StorisProductSync sp
        )
        INSERT INTO #src
        (
            Id, OriginalDescription, VendorModelNumber, VendorUPCCode,
            CurrentPrice, Msrp, AverageCost,
            Weight, Depth, Width, Height,
            BeginningPromoDate, EndingPromoDate, PromoPrice,
            BrandDescirption, BrandId, VendorName, VendorId,
            [Status], GroupID, DateChanged, DateCreated
        )
        SELECT
            d.id,
            d.originalDescription,
            d.vendorModelNumber,
            d.vendorUPCCode,
            d.currentPrice,
            d.msrp,
            d.AverageCost,
            d.[weight],
            d.[depth],
            d.[width],
            d.[height],
            d.beginningPromoDate,
            d.endingPromoDate,
            d.promoPrice,
            d.brandDescirption,
            d.brandId,
            d.VendorName,
            d.vendorid,
            d.[status],
            NULLIF(LTRIM(RTRIM(d.GroupID)), ''), -- normalize once
            d.DateChanged,
            d.DateCreated
        FROM Dedup d
        WHERE d.rn = 1;

        IF NOT EXISTS (SELECT 1 FROM #src)
        BEGIN
            SELECT
                CAST(0 AS INT) AS UpdatedCount,
                CAST(0 AS INT) AS InsertedCount,
                CAST(0 AS INT) AS SkippedBlockedCount,
                CAST(0 AS INT) AS InputCount;
            RETURN;
        END

        -- Ensure dbo.Product(Sku) is indexed/unique for best performance.
        CREATE INDEX IX_src_Id ON #src(Id);

        --------------------------------------------------------------------
        -- Partition: existing SKUs -> update list
        --------------------------------------------------------------------
        CREATE TABLE #upd
        (
            RowId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            Id NVARCHAR(800) NOT NULL,
            OriginalDescription NVARCHAR(800) NULL,
            VendorModelNumber NVARCHAR(800) NULL,
            VendorUPCCode NVARCHAR(800) NULL,
            CurrentPrice DECIMAL(18,4) NOT NULL,
            Msrp DECIMAL(18,4) NOT NULL,
            AverageCost DECIMAL(18,4) NOT NULL,
            Weight DECIMAL(18,4) NOT NULL,
            Depth DECIMAL(18,4) NOT NULL,
            Width DECIMAL(18,4) NOT NULL,
            Height DECIMAL(18,4) NOT NULL,
            BeginningPromoDate DATETIME2(7) NULL,
            EndingPromoDate DATETIME2(7) NULL,
            PromoPrice DECIMAL(18,4) NULL,
            BrandDescirption NVARCHAR(800) NULL,
            BrandId NVARCHAR(800) NULL,
            VendorName NVARCHAR(800) NULL,
            VendorId NVARCHAR(800) NULL,
            [Status] NVARCHAR(400) NULL,
            DateChanged DATETIME2(7) NULL,
            DateCreated DATETIME2(7) NULL
        );

        INSERT INTO #upd
        (
            Id, OriginalDescription, VendorModelNumber, VendorUPCCode,
            CurrentPrice, Msrp, AverageCost,
            Weight, Depth, Width, Height,
            BeginningPromoDate, EndingPromoDate, PromoPrice,
            BrandDescirption, BrandId, VendorName, VendorId,
            [Status], DateChanged, DateCreated
        )
        SELECT
            s.Id, s.OriginalDescription, s.VendorModelNumber, s.VendorUPCCode,
            s.CurrentPrice, s.Msrp, s.AverageCost,
            s.Weight, s.Depth, s.Width, s.Height,
            s.BeginningPromoDate, s.EndingPromoDate, s.PromoPrice,
            s.BrandDescirption, s.BrandId, s.VendorName, s.VendorId,
            s.[Status], s.DateChanged, s.DateCreated
        FROM #src s
        INNER JOIN dbo.Product p ON p.Sku = s.Id;

        --------------------------------------------------------------------
        -- Partition: missing SKUs -> insert list (excluding blocked groups)
        --------------------------------------------------------------------
        CREATE TABLE #ins
        (
            RowId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            Id NVARCHAR(800) NOT NULL,
            OriginalDescription NVARCHAR(800) NULL,
            VendorModelNumber NVARCHAR(800) NULL,
            VendorUPCCode NVARCHAR(800) NULL,
            CurrentPrice DECIMAL(18,4) NOT NULL,
            Msrp DECIMAL(18,4) NOT NULL,
            AverageCost DECIMAL(18,4) NOT NULL,
            Weight DECIMAL(18,4) NOT NULL,
            Depth DECIMAL(18,4) NOT NULL,
            Width DECIMAL(18,4) NOT NULL,
            Height DECIMAL(18,4) NOT NULL,
            BeginningPromoDate DATETIME2(7) NULL,
            EndingPromoDate DATETIME2(7) NULL,
            PromoPrice DECIMAL(18,4) NULL,
            BrandDescirption NVARCHAR(800) NULL,
            BrandId NVARCHAR(800) NULL,
            VendorName NVARCHAR(800) NULL,
            VendorId NVARCHAR(800) NULL,
            [Status] NVARCHAR(400) NULL,
            GroupID NVARCHAR(400) NULL,
            DateChanged DATETIME2(7) NULL,
            DateCreated DATETIME2(7) NULL
        );

        INSERT INTO #ins
        (
            Id, OriginalDescription, VendorModelNumber, VendorUPCCode,
            CurrentPrice, Msrp, AverageCost,
            Weight, Depth, Width, Height,
            BeginningPromoDate, EndingPromoDate, PromoPrice,
            BrandDescirption, BrandId, VendorName, VendorId,
            [Status], GroupID, DateChanged, DateCreated
        )
        SELECT
            s.Id, s.OriginalDescription, s.VendorModelNumber, s.VendorUPCCode,
            s.CurrentPrice, s.Msrp, s.AverageCost,
            s.Weight, s.Depth, s.Width, s.Height,
            s.BeginningPromoDate, s.EndingPromoDate, s.PromoPrice,
            s.BrandDescirption, s.BrandId, s.VendorName, s.VendorId,
            s.[Status], s.GroupID, s.DateChanged, s.DateCreated
        FROM #src s
        LEFT JOIN dbo.Product p ON p.Sku = s.Id
        WHERE p.Sku IS NULL
          AND NOT EXISTS (
                SELECT 1
                FROM @BlockedGroups bg
                WHERE bg.GroupID = ISNULL(s.GroupID, '')
          );

        DECLARE @SkippedBlockedCount INT =
        (
            SELECT COUNT(1)
            FROM #src s
            LEFT JOIN dbo.Product p ON p.Sku = s.Id
            WHERE p.Sku IS NULL
              AND EXISTS (
                    SELECT 1
                    FROM @BlockedGroups bg
                    WHERE bg.GroupID = ISNULL(s.GroupID, '')
              )
        );

        --------------------------------------------------------------------
        -- Execute updates row-by-row (calls your existing SP)
        --------------------------------------------------------------------
        DECLARE
            @RowId INT,
            @Id NVARCHAR(800),
            @OriginalDescription NVARCHAR(800),
            @VendorModelNumber NVARCHAR(800),
            @VendorUPCCode NVARCHAR(800),
            @CurrentPrice DECIMAL(18,4),
            @Msrp DECIMAL(18,4),
            @AverageCost DECIMAL(18,4),
            @Weight DECIMAL(18,4),
            @Depth DECIMAL(18,4),
            @Width DECIMAL(18,4),
            @Height DECIMAL(18,4),
            @BeginningPromoDate DATETIME2(7),
            @EndingPromoDate DATETIME2(7),
            @PromoPrice DECIMAL(18,4),
            @BrandDescirption NVARCHAR(800),
            @BrandId NVARCHAR(800),
            @VendorName NVARCHAR(800),
            @VendorId NVARCHAR(800),
            @Status NVARCHAR(400);

        DECLARE @UpdatedCount INT = 0;
        DECLARE @InsertedCount INT = 0;

        SET @RowId = 1;
        DECLARE @UpdMax INT = (SELECT ISNULL(MAX(RowId), 0) FROM #upd);

        WHILE @RowId <= @UpdMax
        BEGIN
            SELECT
                @Id = u.Id,
                @OriginalDescription = u.OriginalDescription,
                @VendorModelNumber = u.VendorModelNumber,
                @VendorUPCCode = u.VendorUPCCode,
                @CurrentPrice = u.CurrentPrice,
                @Msrp = u.Msrp,
                @AverageCost = u.AverageCost,
                @Weight = u.Weight,
                @Depth = u.Depth,
                @Width = u.Width,
                @Height = u.Height,
                @BeginningPromoDate = u.BeginningPromoDate,
                @EndingPromoDate = u.EndingPromoDate,
                @PromoPrice = u.PromoPrice,
                @BrandDescirption = u.BrandDescirption,
                @BrandId = u.BrandId,
                @VendorName = u.VendorName,
                @VendorId = u.VendorId,
                @Status = u.[Status]
            FROM #upd u
            WHERE u.RowId = @RowId;

            EXEC dbo.[sp_Product_Update]
                @Name = @OriginalDescription,
                @Sku = @Id,
                @Price = @CurrentPrice,
                @OldPrice = @Msrp,
                @ProductCost = @AverageCost,
                @UpdatedOnUtc = @currentDateTime,
                @CurrentDateTime = @currentDateTime,
                @beginningPromoDate = @BeginningPromoDate,
                @EndingPromoDate = @EndingPromoDate,
                @promoPrice = @PromoPrice,
                @ManufacturerName = @BrandDescirption,
                @ManufacturerCustomId = @BrandId,
                @VendorName = @VendorName,
                @VendorEmail = @VendorId,
                @ManufacturerPartNumber = @VendorModelNumber,
                @IsPriceUpdate = 1,
                @Status = @Status,
                @Gtin = @VendorUPCCode,
                @Weight = @Weight,
                @Length = @Depth,
                @Width = @Width,
                @Height = @Height;

            SET @UpdatedCount += 1;
            SET @RowId += 1;
        END

        --------------------------------------------------------------------
        -- Execute inserts row-by-row (calls your existing SP)
        --------------------------------------------------------------------
        SET @RowId = 1;
        DECLARE @InsMax INT = (SELECT ISNULL(MAX(RowId), 0) FROM #ins);

        WHILE @RowId <= @InsMax
        BEGIN
            SELECT
                @Id = i.Id,
                @OriginalDescription = i.OriginalDescription,
                @VendorModelNumber = i.VendorModelNumber,
                @VendorUPCCode = i.VendorUPCCode,
                @CurrentPrice = i.CurrentPrice,
                @Msrp = i.Msrp,
                @AverageCost = i.AverageCost,
                @Weight = i.Weight,
                @Depth = i.Depth,
                @Width = i.Width,
                @Height = i.Height,
                @BeginningPromoDate = i.BeginningPromoDate,
                @EndingPromoDate = i.EndingPromoDate,
                @PromoPrice = i.PromoPrice,
                @BrandDescirption = i.BrandDescirption,
                @BrandId = i.BrandId,
                @VendorName = i.VendorName,
                @VendorId = i.VendorId,
                @Status = i.[Status]
            FROM #ins i
            WHERE i.RowId = @RowId;

            EXEC dbo.[sp_Product_Insert]
                @Name = @OriginalDescription,
                @Sku = @Id,
                @ManufacturerPartNumber = @VendorModelNumber,
                @Gtin = @VendorUPCCode,
                @Price = @CurrentPrice,
                @OldPrice = @Msrp,
                @ProductCost = @AverageCost,
                @Weight = @Weight,
                @Length = @Depth,
                @Width = @Width,
                @Height = @Height,
                @CreatedOnUtc = @currentDateTime,
                @UpdatedOnUtc = @currentDateTime,
                @ManufacturerName = @BrandDescirption,
                @ManufacturerCustomId = @BrandId,
                @VendorName = @VendorName,
                @VendorEmail = @VendorId,
                @Status = @Status;

            SET @InsertedCount += 1;
            SET @RowId += 1;
        END

        --------------------------------------------------------------------
        -- Return counts (single result set)
        --------------------------------------------------------------------
        SELECT
            @UpdatedCount AS UpdatedCount,
            @InsertedCount AS InsertedCount,
            @SkippedBlockedCount AS SkippedBlockedCount,
            (SELECT COUNT(1) FROM #src) AS InputCount;

    END TRY
    BEGIN CATCH
        -- Preserve original error details and stack
        THROW;
    END CATCH
END
GO

GO
