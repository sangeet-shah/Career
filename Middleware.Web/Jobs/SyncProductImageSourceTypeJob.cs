using Middleware.Web.Data;
using Middleware.Web.Options;
using Microsoft.Extensions.Options;
using Quartz;
using Serilog;
using System.Diagnostics;

namespace Middleware.Web.Jobs;

[DisallowConcurrentExecution] // prevents overlap if previous run still executing
public sealed class SyncProductImageSourceTypeJob : IJob
{
    private readonly IEBridgeRepository _erp;
    private readonly INopRepository _nop;
    private readonly MiddlewareOptions _opt;

    public SyncProductImageSourceTypeJob(IEBridgeRepository erp,
        INopRepository nop,
        IOptions<MiddlewareOptions> opt)
    {
        _erp = erp;
        _nop = nop;
        _opt = opt.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var sw = Stopwatch.StartNew();
        var job = nameof(SyncProductImageSourceTypeJob);

        try
        {
            Log.Information("{Job} started", job);

            var rows = await _erp.GetProductImageSourceTypesAsync(context.CancellationToken);
            Log.Information("{Job} fetched {Count} rows from ERP", job, rows.Count);

            var affected = await _nop.UpdateImageSourceTypeAsync(rows, _opt.BatchSize, context.CancellationToken);
            Log.Information("{Job} updated nop rows affected={Affected}", job, affected);

            Log.Information("{Job} completed in {Ms} ms", job, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{Job} failed after {Ms} ms", job, sw.ElapsedMilliseconds);
            throw; // let Quartz mark it as failed
        }
    }
}
