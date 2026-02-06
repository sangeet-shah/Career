using Microsoft.Extensions.Options;
using Middleware.Web.Data;
using Middleware.Web.Options;
using Middleware.Web.Services;
using Quartz;
using Serilog;
using System.Diagnostics;

namespace Middleware.Web.Jobs;

[DisallowConcurrentExecution]
public sealed class ProductPipelineJob : IJob
{
    #region Fields

    private readonly IEBridgeRepository _erp;
    private readonly INopRepository _nop;
    private readonly INopCacheClient _cache;
    private readonly MiddlewareOptions _opt;

    #endregion

    #region Ctor

    public ProductPipelineJob(IEBridgeRepository erp,
        INopRepository nop,
        INopCacheClient cache,
        IOptions<MiddlewareOptions> opt)
    {
        _erp = erp;
        _nop = nop;
        _cache = cache;
        _opt = opt.Value;
    }

    #endregion

    public async Task Execute(IJobExecutionContext context)
    {
        var sw = Stopwatch.StartNew();
        var job = nameof(ProductPipelineJob);
        Log.Information("ProductPipelineJob started");

        var anyChanges = false;

        // 1) Upsert products
        var productRows = await _erp.GetProductsAsync(context.CancellationToken,sku:string.Empty);
        Log.Information("{Job}: {Count} product upsert rows fetched from eBridge", job, productRows.Count);
        if (productRows.Count > 0)
        {
            var affected = await _nop.UpsertProductsAsync(productRows, _opt.BatchSize, context.CancellationToken);
            anyChanges |= affected > 0;
            Log.Information("{Job}: product upsert rows affected={Affected}", job, affected);
        }
        Log.Information("{Job}: products completed in {Ms} ms", job, sw.ElapsedMilliseconds);
        sw.Restart();

        // 2) Sync product quantities
        var productQtyRows = await _erp.GetProductQuantitiesAsync(context.CancellationToken);
        Log.Information("{Job}: {Count} product quantity rows fetched from eBridge", job, productQtyRows.Count);
        if (productQtyRows.Count > 0)
        {
            var data = productQtyRows.Select(x => (x.Sku, x.StockQty)).ToList();
            var affected = await _nop.SyncProductQuantitiesAsync(data, context.CancellationToken);
            anyChanges |= affected > 0;
            Log.Information("{Job}: product quantity sync affected={Affected}", job, affected);
        }
        Log.Information("{Job}: product quantity sync completed in {Ms} ms", job, sw.ElapsedMilliseconds);
        sw.Restart();

        // 3) Delete products
        var deletedProductSkus = await _erp.GetDeletedProductSkusAsync(context.CancellationToken);
        Log.Information("{Job}: {Count} product skus fetched from eBridge to delete", job, deletedProductSkus.Count);
        if(deletedProductSkus.Count > 0)
        {
            var deleted = await _nop.DeleteProductsBySkuAsync(deletedProductSkus, _opt.BatchSize, context.CancellationToken);
            anyChanges |= deleted > 0;
            Log.Information("{Job}: products deleted={Deleted}", job, deleted);
        }
        Log.Information("{Job}: delete products completed in {Ms} ms", job, sw.ElapsedMilliseconds);

        if (anyChanges)
            await _cache.ClearAllProductCacheAsync(context.CancellationToken);

        Log.Information("ProductPipelineJob completed. AnyChanges={AnyChanges}", anyChanges);
    }
}
