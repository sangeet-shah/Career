using Quartz;
using Serilog;
using Mediator.Web.Data;
using Microsoft.Extensions.Options;
using Mediator.Web.Options;
using System.Diagnostics;

namespace Mediator.Web.Jobs;

[DisallowConcurrentExecution]
public sealed class SyncProductJob : IJob
{
    private readonly IEBridgeRepository _erp;
    private readonly INopRepository _nop;
    private readonly MediatorOptions _opt;

    public SyncProductJob(IEBridgeRepository erp, INopRepository nop, IOptions<MediatorOptions> opt)
    {
        _erp = erp;
        _nop = nop;
        _opt = opt.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var sw = Stopwatch.StartNew();
        var job = nameof(SyncProductJob);

        try
        {
            Log.Information("{Job} started", job);

            var rows = await _erp.GetProductsAsync(context.CancellationToken);
            Log.Information("{Job} fetched {Count} rows from ERP", job, rows.Count);

            var affected = await _nop.UpsertProductsAsync(rows, _opt.BatchSize, context.CancellationToken);
            Log.Information("{Job} upsert affected={Affected}", job, affected);

            Log.Information("{Job} completed in {Ms} ms", job, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{Job} failed after {Ms} ms", job, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
