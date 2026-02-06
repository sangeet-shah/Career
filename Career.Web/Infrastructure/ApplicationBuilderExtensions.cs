using Career.Data.Infrastructure;
using Career.Data.Services.Logs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;

namespace Career.Web.Infrastructure;

/// <summary>
/// Represents extensions of IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
    private static string oldUrl { get; set; }
    private static string originalUrl { get; set; }

    /// <summary>
    /// Adds a special handler that checks for responses with the 404 status code that do not have a body
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UsePageNotFound(this IApplicationBuilder application)
    {
        application.UseStatusCodePagesWithReExecute("/page-not-found");
    }

    /// <summary>
    /// Add exception handling
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseCareerExceptionHandler(this IApplicationBuilder application, IWebHostEnvironment env, IConfiguration configuration)
    {
        if (env.IsDevelopment())
        {
            //get detailed exceptions for developing and testing purposes
            application.UseDeveloperExceptionPage();
        }
        else
        {
            //or use special exception handler
            application.UseExceptionHandler("/Common/Error/");
        }


        //log errors
        application.UseExceptionHandler(handler =>
        {
            handler.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (exception == null)
                    return;

                try
                {
                    var logger = EngineContext.Current.Resolve<ILogService>();
                    //log error
                    logger.InsertLog(exception.Message, exception?.ToString() ?? string.Empty);
                }
                finally
                {
                    //rethrow the exception to show the error page
                    ExceptionDispatchInfo.Throw(exception);
                }
            });
        });
    }

    /// <summary>
    /// Adds a special handler that checks for responses with the 404 status code that do not have a body
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseEndpoints(this IApplicationBuilder application)
    {
        // routes collection 
        application.UseEndpoints(endpoints =>
        {
            endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{**SeName}");
            endpoints.MapDynamicControllerRoute<RouteValueTransformer>("ourteam/{SeName}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
              name: "Schedule",
              pattern: "scheduleorder/{orderid?}",
              defaults: new { controller = "ScheduleOrder", action = "Schedule" });

            endpoints.MapControllerRoute(
              name: "ScheduleOrderDetail",
              pattern: "scheduleorderdetail/{orderid}",
              defaults: new { controller = "ScheduleOrder", action = "ScheduleOrderVerifyInformation" });

            endpoints.MapControllerRoute(
              name: "ScheduleOrderPayment",
              pattern: "scheduleorderpayment/{orderid}",
              defaults: new { controller = "ScheduleOrder", action = "ScheduleOrderPayment" });

            endpoints.MapControllerRoute(
              name: "ScheduleOrderFulfillment",
              pattern: "scheduleorderfulfillment/{orderid}",
              defaults: new { controller = "ScheduleOrder", action = "ScheduleOrderFulfillment" });

            endpoints.MapControllerRoute(
              name: "page-not-found",
              pattern: "page-not-found",
              defaults: new { controller = "Common", action = "PageNotFound" });

            endpoints.MapControllerRoute(
              name: "clearcache",
              pattern: "clearcache",
              defaults: new { controller = "Common", action = "ClearCache" });

            endpoints.MapControllerRoute(
              name: "WeeklyAds",
              pattern: "viewad",
              defaults: new { controller = "Common", action = "WeeklyAds" });

            endpoints.MapControllerRoute(
              name: "ourstory",
              pattern: "ourstory",
              defaults: new { controller = "Home", action = "OurStory" });

            endpoints.MapControllerRoute(
              name: "brands",
              pattern: "brands",
              defaults: new { controller = "Home", action = "OurBrands" });

            endpoints.MapControllerRoute(
              name: "locations",
              pattern: "locations",
              defaults: new { controller = "PhysicalStore", action = "List" });

            endpoints.MapControllerRoute(
              name: "privacy-policy",
              pattern: "privacy-policy",
              defaults: new { controller = "Home", action = "PrivacyPolicy" });

            endpoints.MapControllerRoute(
              name: "terms-conditions",
              pattern: "terms-conditions",
              defaults: new { controller = "Home", action = "TermsConditions" });

            endpoints.MapControllerRoute(
              name: "conditions-of-use",
              pattern: "conditions-of-use",
              defaults: new { controller = "Home", action = "TermsOfUse" });

            endpoints.MapControllerRoute(
              name: "careers",
              pattern: "careers",
              defaults: new { controller = "Career", action = "List" });

            endpoints.MapControllerRoute(
                name: "careerdetail",
                pattern: "/careers/careerdetail/{id?}",
                defaults: new { controller = "Career", action = "CareerDetail" });

            endpoints.MapControllerRoute(
                name: "applyjob",
                pattern: "/careers/applyjob/{id?}",
                defaults: new { controller = "Career", action = "ApplyJob" });

            endpoints.MapControllerRoute(
              name: "sitemap.xml",
              pattern: "careers/sitemap.xml",
              defaults: new { controller = "Career", action = "SitemapXml" });

            endpoints.MapControllerRoute(
              name: "sitemap.xml",
              pattern: "sitemap.xml",
              defaults: new { controller = "Career", action = "SitemapXml" });

            endpoints.MapControllerRoute(
             name: "summerjam",
             pattern: "summerjam",
             defaults: new { controller = "SummerJam", action = "Index" });

            endpoints.MapControllerRoute(
             name: "locations",
             pattern: "locations/{location}",
             defaults: new { controller = "PhysicalStore", action = "PhysicalStoreDetail" });

            endpoints.MapControllerRoute(
              name: "golf",
              pattern: "golf",
              defaults: new { controller = "GolfLandingPage", action = "Index" });

            endpoints.MapControllerRoute(
              name: "golfSponsor",
              pattern: "golf/sponsors/",
              defaults: new { controller = "GolfLandingPage", action = "Sponsor" });

            endpoints.MapControllerRoute(
              name: "golfRegister",
              pattern: "golf/register/",
              defaults: new { controller = "GolfLandingPage", action = "Register" });

            endpoints.MapControllerRoute(
              name: "ourteam",
              pattern: "ourteam",
              defaults: new { controller = "OurTeam", action = "List" });

            endpoints.MapControllerRoute(
              name: "CatalogAds",
              pattern: "catalog",
              defaults: new { controller = "Common", action = "CatalogAds" });

            endpoints.MapControllerRoute(
              name: "robots.txt",
              pattern: "robots.txt",
              defaults: new { controller = "Common", action = "RobotsTextFile" });

            endpoints.MapControllerRoute(
             name: "OffersPromotions",
             pattern: "offers-promotions",
             defaults: new { controller = "Common", action = "OffersPromotions" });
        });
    }

    /// <summary>
    /// Trailing slash url middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseTrailingSlashUrlMiddleware(this IApplicationBuilder application)
    {
        application.Use(async (context, next) =>
        {
            originalUrl = context.Request.GetEncodedUrl();
            if (originalUrl.Any(char.IsUpper))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                context.Response.Redirect(originalUrl.ToLower());
            }

            if (!originalUrl.EndsWith("/") &&
            !originalUrl.Contains("/?") &&
            !originalUrl.Contains("/customer/login") &&
            !context.Request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase) &&
            !originalUrl.Contains("/images/") &&
            !originalUrl.Contains(".xml") &&
            string.IsNullOrEmpty(Path.GetExtension(originalUrl)))
            {
                oldUrl = originalUrl;
                originalUrl = originalUrl.Replace("?", "/?");
                if (!originalUrl.Contains("/?") && !originalUrl.EndsWith('/'))
                    originalUrl += "/";

                if (originalUrl.Length != oldUrl.Length)
                {
                    //redirect
                    context.Response.Redirect(originalUrl);
                    return;
                }
            }

            //or call the next middleware in the request pipeline
            await next();
            return;
        });
    }
}
