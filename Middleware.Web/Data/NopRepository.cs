using Dapper;
using Middleware.Web.Options;
using Microsoft.Extensions.Options;
using System.Data;

namespace Middleware.Web.Data;

public sealed class NopRepository : INopRepository
{
    private readonly DbConnectionFactory _db;
    private readonly MiddlewareOptions _opt;

    public NopRepository(DbConnectionFactory db, 
        IOptions<MiddlewareOptions> opt)
    {
        _db = db;
        _opt = opt.Value;
    }

    private static DataTable BuildQtyTvp(IEnumerable<(string Sku, int StockQty)> rows)
    {
        var dt = new DataTable();
        dt.Columns.Add("Sku", typeof(string));
        dt.Columns.Add("StockQty", typeof(int));

        foreach (var r in rows)
            dt.Rows.Add(r.Sku, r.StockQty);

        return dt;
    }

    private static DataTable BuildSkuTvp(IEnumerable<string> skus)
    {
        var dt = new DataTable();
        dt.Columns.Add("Sku", typeof(string));

        foreach (var sku in skus)
            dt.Rows.Add(sku);

        return dt;
    }

    private static DataTable BuildProductsTvp(IEnumerable<ProductRow> rows)
    {
        var dt = new DataTable();
        dt.Columns.Add("id", typeof(string));
        dt.Columns.Add("originalDescription", typeof(string));
        dt.Columns.Add("vendorModelNumber", typeof(string));
        dt.Columns.Add("vendorUPCCode", typeof(string));
        dt.Columns.Add("currentPrice", typeof(decimal));
        dt.Columns.Add("msrp", typeof(decimal));
        dt.Columns.Add("AverageCost", typeof(decimal));
        dt.Columns.Add("weight", typeof(decimal));
        dt.Columns.Add("depth", typeof(decimal));
        dt.Columns.Add("width", typeof(decimal));
        dt.Columns.Add("height", typeof(decimal));
        dt.Columns.Add("beginningPromoDate", typeof(DateTime));
        dt.Columns.Add("endingPromoDate", typeof(DateTime));
        dt.Columns.Add("promoPrice", typeof(decimal));
        dt.Columns.Add("brandDescirption", typeof(string));
        dt.Columns.Add("brandId", typeof(string));
        dt.Columns.Add("VendorName", typeof(string));
        dt.Columns.Add("vendorid", typeof(string));
        dt.Columns.Add("status", typeof(string));
        dt.Columns.Add("GroupID", typeof(string));
        dt.Columns.Add("DateChanged", typeof(DateTime));
        dt.Columns.Add("DateCreated", typeof(DateTime));

        foreach (var r in rows)
        {
            dt.Rows.Add(
                r.Id,
                r.OriginalDescription,
                r.VendorModelNumber,
                r.VendorUPCCode,
                r.CurrentPrice,
                r.Msrp,
                r.AverageCost,
                r.Weight,
                r.Depth,
                r.Width,
                r.Height,
                (object?)r.BeginningPromoDate ?? DBNull.Value,
                (object?)r.EndingPromoDate ?? DBNull.Value,
                (object?)r.PromoPrice ?? DBNull.Value,
                r.BrandDescirption,
                r.BrandId,
                r.VendorName,
                r.VendorId,
                r.Status,
                r.GroupID,
                (object?)r.DateChanged ?? DBNull.Value,
                (object?)r.DateCreated ?? DBNull.Value
            );
        }

        return dt;
    }

    private static DataTable BuildImageSourceTypeTvp(IEnumerable<ImageSourceTypeRow> rows)
    {
        var dt = new DataTable();
        // IMPORTANT: column names must match the TVP type exactly
        dt.Columns.Add("ProductId", typeof(string));       // NVARCHAR(800)
        dt.Columns.Add("ImageSourceType", typeof(int));    // INT

        foreach (var r in rows)
        {
            // Keep it safe with trimming
            dt.Rows.Add(r.ProductID.Trim(), r.ImageSourceType);
        }

        return dt;
    }

    //public async Task<int> UpsertProductsAsync(IReadOnlyList<ProductRow> rows, int batchSize, CancellationToken ct)
    //{
    //    if (rows.Count == 0) return 0;

    //    using var con = _db.CreateNop();
    //    await con.OpenAsync(ct);

    //    var totalAttempted = 0;

    //    foreach (var batch in rows.Chunk(batchSize))
    //    {
    //        var tvp = BuildProductsTvp(batch);

    //        var p = new DynamicParameters();
    //        p.Add("@StorisProductSync", tvp.AsTableValuedParameter("dbo.StorisProductSync"));

    //        // If your SP does NOT return affected count, ExecuteAsync returns -1 for many SPs.
    //        // So treat "attempted" as batch size for now (or modify SP to return count - recommended below).
    //        await con.ExecuteAsync(new CommandDefinition(
    //            "dbo.Storis_ProductsSync",
    //            p,
    //            commandType: CommandType.StoredProcedure,
    //            commandTimeout: _opt.CommandTimeoutSeconds,
    //            cancellationToken: ct));

    //        totalAttempted += batch.Length;
    //    }

    //    return totalAttempted; // or actual affected if you update the SP to return it
    //}

    public async Task<int> UpsertProductsAsync(IReadOnlyList<ProductRow> rows, int batchSize, CancellationToken ct)
    {
        if (rows is null || rows.Count == 0)
            return 0;

        using var con = _db.CreateNop();
        await con.OpenAsync(ct);

        var totalUpdated = 0;
        var totalInserted = 0;
        var totalSkippedBlocked = 0;
        var totalInput = 0;

        foreach (var batch in rows.Chunk(batchSize))
        {
            var tvp = BuildProductsTvp(batch);

            var p = new DynamicParameters();
            p.Add("@StorisProductSync", tvp.AsTableValuedParameter("dbo.StorisProductSync"));

            // SP returns a single row with counts
            var r = await con.QuerySingleAsync<ProductSyncResult>(
                new CommandDefinition(
                    "dbo.Storis_ProductsSync",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _opt.CommandTimeoutSeconds,
                    cancellationToken: ct));

            totalUpdated += r.UpdatedCount;
            totalInserted += r.InsertedCount;
            totalSkippedBlocked += r.SkippedBlockedCount;
            totalInput += r.InputCount;
        }

        // "Affected" means: actual writes that would require cache clear / Elastic reindex
        var affected = totalUpdated + totalInserted;

        // Optional: log these where you call the repo, not here (repo should ideally be quiet)
        // But returning only affected keeps the job clean.
        return affected;
    }


    //public async Task<int> UpdateImageSourceTypeAsync(IReadOnlyList<ImageSourceTypeRow> rows, int batchSize, CancellationToken ct)
    //{
    //    // This likely writes to a custom column/table in nopCommerce (plugin table recommended).
    //    const string sql = @"UPDATE dbo.YourCustomProductExt SET ImageSourceType=@ImageSourceType WHERE ProductId=@ProductID;";
    //    return await ExecuteInBatches(rows, batchSize, (con, tx, batch) =>
    //        con.ExecuteAsync(new CommandDefinition(sql, batch, tx, _opt.CommandTimeoutSeconds, cancellationToken: ct)), ct);
    //}

    //public async Task<int> UpdateNotAvailableOnWebAsync(IReadOnlyList<NotAvailableRow> rows, int batchSize, CancellationToken ct)
    //{
    //    // Could be Published flag or a custom flag; don't change core semantics unless you’re sure.
    //    const string sql = @"UPDATE dbo.YourCustomProductExt SET NotAvailableOnWeb=@NotAvailableOnWeb WHERE ProductId=@ProductID;";
    //    return await ExecuteInBatches(rows, batchSize, (con, tx, batch) =>
    //        con.ExecuteAsync(new CommandDefinition(sql, batch, tx, _opt.CommandTimeoutSeconds, cancellationToken: ct)), ct);
    //}

    //private async Task<int> ExecuteInBatches<T>(IReadOnlyList<T> rows, int batchSize,
    //    Func<SqlConnection, SqlTransaction, IReadOnlyList<T>, Task<int>> execBatch,
    //    CancellationToken ct)
    //{
    //    if (rows.Count == 0) return 0;

    //    using var con = _db.CreateNop();
    //    await con.OpenAsync(ct);

    //    var total = 0;
    //    for (var i = 0; i < rows.Count; i += batchSize)
    //    {
    //        var batch = rows.Skip(i).Take(batchSize).ToList();

    //        // Short transaction per batch
    //        using var tx = con.BeginTransaction();

    //        try
    //        {
    //            total += await execBatch(con, tx, batch);
    //            tx.Commit();
    //        }
    //        catch
    //        {
    //            tx.Rollback();
    //            throw;
    //        }
    //    }

    //    return total;
    //}

    public async Task<int> SyncProductQuantitiesAsync(IReadOnlyList<(string Sku, int StockQty)> rows, CancellationToken ct)
    {
        if (rows.Count == 0) return 0;

        using var con = _db.CreateNop();
        await con.OpenAsync(ct);

        var totalChanged = 0;

        foreach (var batch in rows.Chunk(_opt.BatchSize))
        {
            var tvp = BuildQtyTvp(batch);

            var p = new DynamicParameters();
            p.Add("@tblProductQtySync", tvp.AsTableValuedParameter("dbo.ProductQtySync"));

            // Expect SP returns a scalar ChangedCount
            var changed = await con.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    "dbo.ProductsQtySync",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _opt.CommandTimeoutSeconds,
                    cancellationToken: ct));

            totalChanged += changed;
        }

        return totalChanged;
    }

    public async Task<int> DeleteProductsBySkuAsync(IReadOnlyList<string> skus, int batchSize, CancellationToken ct)
    {
        if (skus is null || skus.Count == 0)
            return 0;

        var distinctSkus = skus
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (distinctSkus.Count == 0)
            return 0;

        using var con = _db.CreateNop();
        await con.OpenAsync(ct);

        var totalDeletedCandidates = 0;

        foreach (var batch in distinctSkus.Chunk(batchSize))
        {
            var tvp = BuildSkuTvp(batch);

            var p = new DynamicParameters();
            p.Add("@Skus", tvp.AsTableValuedParameter("dbo.SkuList"));

            // SP returns a single-row result: DeletedCandidates
            var deletedCandidates = await con.QuerySingleAsync<int>(
                new CommandDefinition(
                    "dbo.sp_DeleteProductsBySKUs",
                    p,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _opt.CommandTimeoutSeconds,
                    cancellationToken: ct));

            totalDeletedCandidates += deletedCandidates;
        }

        return totalDeletedCandidates;
    }

    public async Task<int> UpdateImageSourceTypeAsync(IReadOnlyList<ImageSourceTypeRow> rows, int batchSize,
        CancellationToken ct)
    {
        if (rows is null || rows.Count == 0)
            return 0;

        // Normalize + de-dupe by ProductID (keep last)
        // If ERP can return duplicates, this prevents TVP PK issues if you later add a PK.
        var distinct = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.ProductID))
            .GroupBy(x => x.ProductID.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new ImageSourceTypeRow(g.Key, g.Last().ImageSourceType))
            .ToList();

        if (distinct.Count == 0)
            return 0;

        using var con = _db.CreateNop();
        await con.OpenAsync(ct);

        var totalAttempted = 0;

        foreach (var batch in distinct.Chunk(batchSize))
        {
            var tvp = BuildImageSourceTypeTvp(batch);

            var p = new DynamicParameters();
            p.Add("@Storis_ProductImageSourceTypes", tvp.AsTableValuedParameter("dbo.Storis_ProductImageSourceTypes"));

            await con.ExecuteAsync(new CommandDefinition(
                "dbo.Storis_ProductImageSourceTypesSync", // <-- rename proc OR change this to actual proc name
                p,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _opt.CommandTimeoutSeconds,
                cancellationToken: ct));

            totalAttempted += batch.Length;
        }

        return totalAttempted;
    }

    public async Task<int> UpdateNotAvailableOnWebAsync(IReadOnlyList<string> skus, int batchSize, CancellationToken ct)
    {
        if (skus is null || skus.Count == 0)
            return 0;

        var distinctSkus = skus
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (distinctSkus.Count == 0)
            return 0;

        using var con = _db.CreateNop();
        await con.OpenAsync(ct);

        var totalInserted = 0;

        foreach (var batch in distinctSkus.Chunk(batchSize))
        {
            var tvp = BuildSkuTvp(batch);

            var p = new DynamicParameters();
            p.Add("@Skus", tvp.AsTableValuedParameter("dbo.SkuList"));

            // SP returns InsertedCount (single int)
            var inserted = await con.QuerySingleAsync<int>(new CommandDefinition(
                "dbo.Storis_ProductNotAvailableOnWebSync",
                p,
                commandType: CommandType.StoredProcedure,
                commandTimeout: _opt.CommandTimeoutSeconds,
                cancellationToken: ct));

            totalInserted += inserted;
        }

        return totalInserted;
    }

}
