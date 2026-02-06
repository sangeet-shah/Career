using Middleware.Web.Data;
using Middleware.Web.Services;
using Quartz;
using Serilog;
using System.Diagnostics;

namespace Middleware.Web.Jobs;

[DisallowConcurrentExecution]
public sealed class SyncEmployeesJob : IJob
{
    private readonly IPaycorClient _paycor;
    private readonly IEBridgeRepository _erp; // eBridge is your ERP DB    

    public SyncEmployeesJob(IPaycorClient paycor, 
        IEBridgeRepository erp)
    {
        _paycor = paycor;
        _erp = erp;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
        var sw = Stopwatch.StartNew();
        var job = nameof(SyncEmployeesJob);

        Log.Information("{Job} started", job);

        var json = await _paycor.GetEmployeesWithHoursAndPunchesAsync(ct);

        if (string.IsNullOrWhiteSpace(json))
        {
            Log.Information("{Job}: no employee data returned from Paycor", job);
            return;
        }

        var affected = await _erp.PaycorEmployeeSyncAsync(json, ct);

        Log.Information("{Job} completed. ERP sync executed. Ms={Ms} Affected={Affected}",
            job, sw.ElapsedMilliseconds, affected);
    }
}
