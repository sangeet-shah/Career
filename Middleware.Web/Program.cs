using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Infrastructure;
using Middleware.Web.Jobs;
using Middleware.Web.Options;
using Middleware.Web.Services;
using Middleware.Web.Services.Advertisements;
using Middleware.Web.Services.Blogs;
using Middleware.Web.Services.Career;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Customers;
using Middleware.Web.Services.DeliveryCharges;
using Middleware.Web.Services.Directory;
using Middleware.Web.Services.GolfLanding;
using Middleware.Web.Services.HelloBar;
using Middleware.Web.Services.Helpers;
using Middleware.Web.Services.LandingPages;
using Middleware.Web.Services.Localization;
using Middleware.Web.Services.Locations;
using Middleware.Web.Services.Logs;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Messages;
using Middleware.Web.Services.OffersPromotions;
using Middleware.Web.Services.PaycorAPI;
using Middleware.Web.Services.RegistrationPage;
using Middleware.Web.Services.Security;
using Middleware.Web.Services.Seo;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Middleware.Web.Services.SummerJams;
using Middleware.Web.Services.Vendors;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// Options
builder.Services.Configure<JobSchedulesOptions>(builder.Configuration.GetSection("Jobs"));
builder.Services.Configure<MiddlewareOptions>(builder.Configuration.GetSection("Middleware"));
builder.Services.Configure<PaycorEmployeeOptions>(builder.Configuration.GetSection("PaycorEmployee"));
builder.Services.Configure<PaycorJobOptions>(builder.Configuration.GetSection("PaycorJob"));

// DB connection factory
builder.Services.AddSingleton<DbConnectionFactory>();

// Repositories
builder.Services.AddScoped<IEBridgeRepository, EBridgeRepository>();
builder.Services.AddScoped<INopRepository, NopRepository>();
builder.Services.AddScoped<INopSettingsRepository, NopSettingsRepository>();

// Caching
builder.Services.AddScoped<IStaticCacheManager, MemoryCacheManager>();

// Services
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ICareerService, CareerService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IGenericAttributeService, GenericAttributeService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IKlaviyoService, KlaviyoService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IWorkContext, WebWorkContext>();
builder.Services.AddScoped<IDeliveryChargeService, DeliveryChargeService>();
builder.Services.AddScoped<IStateProvinceService, StateProvinceService>();
builder.Services.AddScoped<IGolfLandingPageService, GolfLandingPageService>();
builder.Services.AddScoped<IHelloBarService, HelloBarService>();
builder.Services.AddScoped<IDateTimeHelper, DateTimeHelper>();
builder.Services.AddScoped<ILandingPageService, LandingPageService>();
builder.Services.AddScoped<ILandingPageRecordService, LandingPageRecordService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IBannerManagementService, BannerManagementService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<INopFileProvider, NopFileProvider>();
builder.Services.AddScoped<IPictureService, PictureService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
builder.Services.AddScoped<ISmtpBuilder, SmtpBuilder>();
builder.Services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
builder.Services.AddScoped<IOffersPromotionsService, OffersPromotionsService>();
builder.Services.AddScoped<IPaycorAPIService, PaycorAPIService>();
builder.Services.AddScoped<ILandingPageClosedService, LandingPageClosedService>();
builder.Services.AddScoped<IRegistrationPageFieldsService, RegistrationPageFieldsService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUrlRecordService, UrlRecordService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStoreMappingService, StoreMappingService>();
builder.Services.AddScoped<ISummerJamService, SummerJamService>();
builder.Services.AddScoped<IVendorService, VendorService>();

// HttpClients
builder.Services.AddHttpClient<INopCacheClient, NopCacheClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IPaycorClient, PaycorClient>((sp, http) =>
{
    var opt = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PaycorEmployeeOptions>>().Value;
    http.BaseAddress = new Uri(opt.BaseUrl);
    http.Timeout = TimeSpan.FromSeconds(Math.Max(10, opt.HttpTimeoutSeconds));
});

// Quartz (schedule everything here, based on config)
builder.Services.AddQuartz(q =>
{
    // Read schedules from configuration at startup (no runtime scheduling needed)
    var schedules = builder.Configuration.GetSection("Jobs").Get<JobSchedulesOptions>()
                   ?? throw new InvalidOperationException("Missing configuration section: Jobs");

    //AddSimpleIntervalJob<ProductPipelineJob>(q, schedules.ProductPipeline);
    //AddSimpleIntervalJob<SyncProductImageSourceTypeJob>(q, schedules.SyncProductImageSourceType);
    //AddSimpleIntervalJob<SyncProductNotAvailableOnWebJob>(q, schedules.SyncProductNotAvailableOnWeb);
   // AddSimpleIntervalJob<SyncEmployeesJob>(q, schedules.SyncEmployees);
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

// Health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("EBridge")!, name: "ebridge-sql")
    .AddSqlServer(builder.Configuration.GetConnectionString("Nop")!, name: "nop-azure-sql");

var app = builder.Build();
app.UseMiddleware<StoreAliasMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/", () => "Middleware is running.");

app.Run();


// -------------------------
// Helpers
// -------------------------
static void AddSimpleIntervalJob<TJob>(
    IServiceCollectionQuartzConfigurator q,
    JobScheduleEntry entry)
    where TJob : IJob
{
    var jobName = typeof(TJob).Name;
    var jobKey = new JobKey(jobName);

    // Always register the job
    q.AddJob<TJob>(j => j.WithIdentity(jobKey));

    // If disabled, no trigger (job won't run)
    if (!entry.Enabled)
        return;

    if (entry.IntervalMinutes <= 0)
        throw new InvalidOperationException($"Jobs:{jobName}: IntervalMinutes must be > 0");

    q.AddTrigger(t => t
        .WithIdentity($"{jobName}.trigger")
        .ForJob(jobKey)
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(entry.IntervalMinutes)
            .RepeatForever()
            .WithMisfireHandlingInstructionNextWithRemainingCount()));
}
