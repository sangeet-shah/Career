using Quartz;
using Serilog;
using Mediator.Web.Data;
using Mediator.Web.Services;

namespace Mediator.Web.Jobs;

[DisallowConcurrentExecution] // prevents overlap if previous run still executing
public sealed class SyncProductQuantityJob : IJob
{
    private readonly IEBridgeRepository _erp;
    private readonly INopRepository _nop;
    private readonly INopCacheClient _cache;

    public SyncProductQuantityJob(IEBridgeRepository erp,
        INopRepository nop,
        INopCacheClient cache)
    {
        _erp = erp;
        _nop = nop;
        _cache = cache;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Log.Information("SyncProductQuantityJob started");

        var rows = await _erp.GetProductQuantitiesAsync(context.CancellationToken);
        Log.Information("Fetched {Count} stock rows from ERP", rows.Count);

        var data = rows.Select(x => (x.Sku, x.StockQty)).ToList();

        var changed = await _nop.SyncProductQuantitiesAsync(data, context.CancellationToken);
        if (changed > 0)
            await _cache.ClearAllProductCacheAsync(context.CancellationToken);


        Log.Information("SyncProductQuantityJob completed");
    }
}
