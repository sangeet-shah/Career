namespace Middleware.Web.Options;

public sealed class JobSchedulesOptions
{
    public JobScheduleEntry ProductPipeline { get; set; } = new();
    public JobScheduleEntry SyncProductImageSourceType { get; set; } = new();
    public JobScheduleEntry SyncProductNotAvailableOnWeb { get; set; } = new();
    public JobScheduleEntry SyncEmployees { get; set; } = new();
}

public sealed class JobScheduleEntry
{
    public bool Enabled { get; set; } = true;
    public int IntervalMinutes { get; set; } = 60;
}
