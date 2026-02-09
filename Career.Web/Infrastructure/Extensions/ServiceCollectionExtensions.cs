using System;
using Career.Web.Services.ApiClient;
using Career.Web.Services.Caching;
using Career.Web.Services.FmeHttpClient;
using Career.Web.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Career.Web.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Career.Web.Infrastructure.Extensions;

/// <summary>
/// Registers Career.Web dependencies only (no Career.Data).
/// </summary>
public static class ServiceCollectionExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration, string connectionString = null)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IWebHelper, WebHelper>();
        services.AddMemoryCache();
        services.AddScoped<IApiCache, MemoryApiCache>();
        services.AddScoped<RouteValueTransformer>();

        services.AddHttpClient();
        // Register delegating handler used by ApiClient HttpClient
        services.AddTransient<ForwardCookiesHandler>();
        // Register user agent helper used by controllers
        services.AddScoped<IUserAgentHelper, UserAgentHelper>();
        services.AddHttpClient<IApiClient, ApiClient>()
            .AddHttpMessageHandler<ForwardCookiesHandler>()
            .ConfigureHttpClient((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(config["Api:BaseUrl"]);
                client.DefaultRequestHeaders.Add("XApiKey", config["Api:XApiKey"]);
                client.DefaultRequestHeaders.Add("XStoreAlias", config["Api:XStoreAlias"]?.Trim());
            });

        services.AddHttpClient(HttpClientService.FmeBridgeClientName, (sp, client) =>
        {
            var appSettings = sp.GetService<AppSettings>();
            if (!string.IsNullOrEmpty(appSettings?.fmebridgeUrl))
                client.BaseAddress = new Uri(appSettings.fmebridgeUrl);
        });
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IDateTimeHelper, DateTimeHelper>();
    }

    public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
        var config = new TConfig();
        configuration.Bind(config);
        services.AddSingleton(config);
        return config;
    }
}
