using Career.Data;
using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Helpers;
using Career.Data.Infrastructure;
using Career.Data.Services.Advertisements;
using Career.Data.Services.Blogs;
using Career.Data.Services.Career;
using Career.Data.Services.Common;
using Career.Data.Services.Customers;
using Career.Data.Services.DeliveryCharges;
using Career.Data.Services.Directory;
using Career.Data.Services.GolfLanding;
using Career.Data.Services.HelloBar;
using Career.Data.Services.Helpers;
using Career.Data.Services.LandingPages;
using Career.Data.Services.Localization;
using Career.Data.Services.Locations;
using Career.Data.Services.Logs;
using Career.Data.Services.Media;
using Career.Data.Services.Messages;
using Career.Data.Services.OffersPromotions;
using Career.Data.Services.PaycorAPI;
using Career.Data.Services.RegistrationPage;
using Career.Data.Services.Security;
using Career.Data.Services.Seo;
using Career.Data.Services.Settings;
using Career.Data.Services.Stores;
using Career.Data.Services.SummerJams;
using Career.Data.Services.Vendors;
using Career.Web.Models.Career;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Career.Web.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{

    #region Methods

    /// <summary>
    /// Adds a special handler that checks for dependencies
    /// </summary>
    /// <param name="ServiceCollection">Builder for configuring an ServiceCollection's request pipeline</param>
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration _configuration, string connectionString)
    {
        services.AddScoped<IDataProvider, CareerDataProvider>();
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<IWebHelper, WebHelper>();        
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<ICommonService, CommonService>();
        services.AddScoped<IPictureService, PictureService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<ICareerService, CareerService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IAdvertisementService, AdvertisementService>();        
        services.AddScoped<IGolfLandingPageService, GolfLandingPageService>();
        services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
        services.AddScoped<ILandingPageRecordService, LandingPageRecordService>();
        services.AddScoped<ILandingPageService, LandingPageService>();
        services.AddScoped<IDeliveryChargeService, DeliveryChargeService>();
        services.AddScoped<SitemapGenerator, SitemapGenerator>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISummerJamService, SummerJamService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IWorkContext, WebWorkContext>();
        services.AddScoped<IGenericAttributeService, GenericAttributeService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IUrlRecordService, UrlRecordService>();
        services.AddScoped<IBannerManagementService, BannerManagementService>();
        services.AddScoped<IStaticCacheManager, MemoryCacheManager>();
        services.AddScoped<ISmtpBuilder, SmtpBuilder>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<INopFileProvider, NopFileProvider>();
        services.AddScoped<IOffersPromotionsService, OffersPromotionsService>();
        services.AddScoped<IPaycorAPIService, PaycorAPIService>();
        services.AddScoped<IKlaviyoService, KlaviyoService>();
        services.AddScoped<IRegistrationPageFieldsService, RegistrationPageFieldsService>();
        services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
        services.AddScoped<ILandingPageClosedService, LandingPageClosedService>();
        services.AddScoped<IDateTimeHelper, DateTimeHelper>();
        services.AddScoped<IStateProvinceService, StateProvinceService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreMappingService, StoreMappingService>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IHelloBarService, HelloBarService>();
        services.AddScoped<IUserAgentHelper, UserAgentHelper>();

        services.AddScoped<RouteValueTransformer>();

        services.AddHttpClient();

        //register type finder
        var typeFinder = new WebAppTypeFinder();
        Singleton<ITypeFinder>.Instance = typeFinder;
        services.AddSingleton<ITypeFinder>(typeFinder);

        //create engine and configure service provider
        var engine = EngineContext.Create();
        engine.ConfigureServices(services, _configuration);
    }

    /// <summary>
    /// Create, bind and register as service the specified configuration parameters 
    /// </summary>
    /// <typeparam name="TConfig">Configuration parameters</typeparam>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Set of key/value application configuration properties</param>
    /// <returns>Instance of configuration parameters</returns>
    public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        //create instance of config
        var config = new TConfig();

        //bind it to the appropriate section of configuration
        configuration.Bind(config);

        //and register it as a service
        services.AddSingleton(config);

        return config;
    }

    #endregion
}
