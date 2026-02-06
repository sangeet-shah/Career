using Career.Data;
using Career.Data.Infrastructure;
using Career.Web.Infrastructure;
using Career.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Career.Web;

public class Startup
{
    private IConfiguration _configuration { get; }
    private IWebHostEnvironment _env { get; set; }

    public Startup(IConfiguration configuration,
        IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        //add appSettings configuration parameters
        var appSettings = services.ConfigureStartupConfig<AppSettings>(_configuration);
        services.RegisterDependencies(_configuration, appSettings.ConnectionStrings.DbConnection);

        services.AddDirectoryBrowser();
        services.AddHttpContextAccessor();

        // Register MVC views
        var mvcBuilder = services.AddControllersWithViews();

        // Register Razor Pages
        services.AddRazorPages();

        // Runtime compilation only in development
        if (_env.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        services.Configure<RouteOptions>(options =>
        {
            options.AppendTrailingSlash = true;
            options.LowercaseUrls = true;
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        EngineContext.Current.ConfigureRequestPipeline(app);
        app.UseTrailingSlashUrlMiddleware();
        //exception handling
        app.UseCareerExceptionHandler(env, _configuration);
        //handle 404 errors (not found)
        app.UsePageNotFound();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        // routes
        app.UseEndpoints();
    }
}