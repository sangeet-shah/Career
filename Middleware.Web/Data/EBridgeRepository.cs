using Dapper;
using Microsoft.Extensions.Options;
using Middleware.Web.Options;
using Newtonsoft.Json;
using System.Data;

namespace Middleware.Web.Data;

public sealed class EBridgeRepository : IEBridgeRepository
{
    private readonly DbConnectionFactory _db;
    private readonly MiddlewareOptions _opt;
    private readonly INopSettingsRepository _nopSettings;

    public EBridgeRepository(DbConnectionFactory db,
        IOptions<MiddlewareOptions> opt,
        INopSettingsRepository nopSettings)
    {
        _db = db;
        _opt = opt.Value;
        _nopSettings = nopSettings;
    }

    public async Task<IReadOnlyList<ProductQtyRow>> GetProductQuantitiesAsync(CancellationToken ct)
    {
        // Use your predefined query here:
        const string sql = @"Select table1.ProductID as Sku, ISNULL(SUM(table1.StockQty), 0) as StockQty
                              From
                              (
	                            (
		                            Select ProductID as ProductID, SUM(ISNULL(NetAvailable, 0)) as StockQty
		                            From storis.fn_GetProductNetAvailable(NULL)
		                            Where StoreID in ('29', '99')
		                            Group by ProductID
	                            )
	                            UNION
	                            (
		                            Select KitMaster_ProductID as ProductID, Sum(NetAvailable) as StockQty
		                            From storis.fn_GetKitNetAvailable(NULL)
		                            Where StoreID in ('29','99')
		                            Group by KitMaster_ProductID
	                            )
                              ) table1
                              Group by table1.ProductID";

        using var con = _db.CreateEBridge();
        var rows = await con.QueryAsync<ProductQtyRow>(
            new CommandDefinition(sql, commandTimeout: _opt.CommandTimeoutSeconds, cancellationToken: ct));

        return rows.AsList();
    }

    public async Task<IReadOnlyList<ProductRow>> GetProductsAsync(CancellationToken ct, string sku = "")
    {
        // 1) read last sync datetime from nop settings
        var lastSyncRaw = !string.IsNullOrEmpty(sku) ? null : await _nopSettings.GetSettingValueAsync(_opt.LastProductSyncSettingName, ct);

        DateTime? lastSync = null;
        if (string.IsNullOrEmpty(sku) && !string.IsNullOrWhiteSpace(lastSyncRaw) && DateTime.TryParse(lastSyncRaw, out var dt))
            lastSync = dt;

        // 2) call eBridge SP
        using var con = _db.CreateEBridge();

        var args = new DynamicParameters();
        args.Add("@sku", value: sku, dbType: DbType.String); // null means fetch all changed since last sync
        args.Add("@LastProductSyncDateTime", value: lastSync, dbType: DbType.DateTime2);

        var rows = await con.QueryAsync<ProductRow>(new CommandDefinition(
            "fm.GetAllProductForSync",
            args,
            commandType: CommandType.StoredProcedure,
            commandTimeout: _opt.CommandTimeoutSeconds,
            cancellationToken: ct));

        if (string.IsNullOrEmpty(sku))
        {
            DateTime lastProductSyncDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            await _nopSettings.SetSettingValueAsync(_opt.LastProductSyncSettingName,
                lastProductSyncDateTime.ToString(),
                ct);
        }

        return rows.AsList();
    }

    public async Task<IReadOnlyList<ImageSourceTypeRow>> GetProductImageSourceTypesAsync(CancellationToken ct)
    {
        using var con = _db.CreateEBridge();

        var rows = await con.QueryAsync<ImageSourceTypeRow>(new CommandDefinition(
            "dbo.fm_GetAllProductImageSourceTypes",
            commandType: CommandType.StoredProcedure,
            commandTimeout: _opt.CommandTimeoutSeconds,
            cancellationToken: ct));

        return rows.AsList();
    }

    public async Task<IReadOnlyList<string>> GetProductNotAvailableOnWebAsync(CancellationToken ct)
    {
        const string sql = @"
        SELECT p.ProductID
        FROM storis.DW_Product p WITH (NOLOCK)
        WHERE p.WebAvailableOnWeb != 1
          AND (p.DateCreated >= @Since OR p.DateChanged >= @Since);";

        var since = DateTime.UtcNow.AddDays(-1); // or local if ERP stores local dates

        using var con = _db.CreateEBridge();
        var rows = await con.QueryAsync<string>(new CommandDefinition(
            sql,
            new { Since = since },
            commandTimeout: _opt.CommandTimeoutSeconds,
            cancellationToken: ct));

        return rows.AsList();
    }

    public async Task<IReadOnlyList<string>> GetDeletedProductSkusAsync(CancellationToken ct)
    {
        const string sql = "SELECT ProductID FROM [storis].[DW_Product] WITH (NOLOCK) WHERE Status like '%hs pe%'";
        using var con = _db.CreateEBridge();
        var rows = await con.QueryAsync<string>(
            new CommandDefinition(sql, commandTimeout: _opt.CommandTimeoutSeconds, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> PaycorEmployeeSyncAsync(string json, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(json))
            return 0;

        using var con = _db.CreateEBridge();
        await con.OpenAsync(ct);

        var p = new DynamicParameters();
        p.Add("@EmployeesJson", json, DbType.String);

        // If proc returns no count, ExecuteAsync may be -1; return 1 to indicate "ran".
        var r = await con.ExecuteAsync(new CommandDefinition(
            "fm.PaycorEmployeeSync",
            p,
            commandType: CommandType.StoredProcedure,
            commandTimeout: _opt.CommandTimeoutSeconds,
            cancellationToken: ct));

        return r < 0 ? 1 : r;
    }

    public async Task<IReadOnlyList<string>> GetLocationKeysByProductKeyAsync(string productKey, CancellationToken ct)
    {
        const string sql = @"
                            SELECT [Location Key]
                            FROM bi.ProductInventory_Current
                            WHERE LOWER([Product Key]) = @ProductKey
                              AND Date = FORMAT(GETDATE(), 'yyyy-MM-dd')
                              AND [Qty NonSaleable] > 0
                            ORDER BY [Location Key]";

        using var con = _db.CreateEBridge();
        var rows = await con.QueryAsync<string>(new CommandDefinition(
            sql,
            new { ProductKey = productKey.ToLowerInvariant() },
            commandTimeout: _opt.CommandTimeoutSeconds,
            cancellationToken: ct));

        return rows.AsList();
    }    

    public async Task<string> GetPowerReviewInvoiceAsync()
    {
        using var con = _db.CreateEBridge();
        var table = await con.QueryAsync(new CommandDefinition(
            "fm.GetPowerReviewInvoice",
            commandType: CommandType.StoredProcedure,
            commandTimeout: _opt.CommandTimeoutSeconds));

        return JsonConvert.SerializeObject(table, Formatting.Indented);
    }

    public async Task<string> GetChatMeterReviewFeedAsync()
    {
        using var con = _db.CreateEBridge();
        var table = await con.QueryAsync(new CommandDefinition(
            "fm.GetChatMeterReviewFeed",
            commandType: CommandType.StoredProcedure,
            commandTimeout: _opt.CommandTimeoutSeconds));

        return JsonConvert.SerializeObject(table, Formatting.Indented);
    }

    public async Task<ProductDeliveryDate> GetProductDeliveryDateAsync(string sku, string zipcode)
    {
        using var con = _db.CreateEBridge();
        var rows = await con.QueryAsync<ProductDeliveryDate>(new CommandDefinition(
            "fm.GetProductDeliveryDate",
            new
            {
                ZipCode = zipcode,
                SKU = sku                
            },
            commandType: CommandType.StoredProcedure,
            commandTimeout: _opt.CommandTimeoutSeconds));

        return rows.First();
    }

}
