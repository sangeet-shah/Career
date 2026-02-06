using Career.Data.Services.Seo;
using Career.Data.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;

namespace Career.Web;

public class RouteValueTransformer : DynamicRouteValueTransformer
{
    #region Fields

    private readonly IUrlRecordService _urlRecordService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public RouteValueTransformer(IUrlRecordService urlRecordService, IStoreService storeService)
    {
        _urlRecordService = urlRecordService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        if (values == null)
            return new ValueTask<RouteValueDictionary>(values);

        if (!values.TryGetValue("SeName", out var slugValue) || string.IsNullOrEmpty(slugValue as string))
            return new ValueTask<RouteValueDictionary>(values);

        var slug = slugValue as string;
        if (slug == "news/rss/")
        {
            values["controller"] = "Home";
            values["action"] = "NewsRss";
            return new ValueTask<RouteValueDictionary>(values);
        }

        // fmusa seo name should be /news/seo-name/
        if (slug.Split('/').Length > 2 && !slug.Split('/')[1].Contains("page="))
            slug = slug.Replace("news/", "inspiration/");

        var newsSlug = string.Empty;
        if (slug.Split('/').Length > 1 && slug.Split('/')[1].Contains("page="))
        {
            newsSlug = slug.Split('/')[1];

            int index = slug.IndexOf("/");
            if (index >= 0)
                slug = slug.Substring(0, index);
        }

        var urlRecord = _urlRecordService.GetBySlugAsync(slug).Result;
        //no URL record found
        if (urlRecord == null)
            return new ValueTask<RouteValueDictionary>(values);

        //if URL record is not active let's find the latest one
        if (!urlRecord.IsActive)
        {
            //redirect to active slug if found
            values["controller"] = "Common";
            values["action"] = "InternalRedirect";
            values["permanentRedirect"] = true;
            httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;
            return new ValueTask<RouteValueDictionary>(values);
        }

        var queryParameters = QueryHelpers.ParseQuery(httpContext.Request.QueryString.ToString());

        //since we are here, all is ok with the slug, so process URL
        switch (urlRecord.EntityName.ToLowerInvariant())
        {
            case "landingpage":
                // find store wise topic page(we have created topic page for both stores)
                var urlRecordStoreWiseContest = _urlRecordService.GetBySlugAsync(slug, (_storeService.GetCurrentStoreAsync()).Result?.Id ?? 0).Result;
                if (urlRecordStoreWiseContest == null)
                {
                    values["controller"] = "Common";
                    values["action"] = "PageNotFound";
                }
                else
                {
                    urlRecord = urlRecordStoreWiseContest;

                    values["controller"] = "ContestPage";
                    values["action"] = "ContestPage";
                    values["contestId"] = urlRecord.EntityId;
                    values["SeName"] = urlRecord.Slug;
                }
                break;
            case "blogpost":
                values["controller"] = "Home";
                values["action"] = "NewsArticle";
                values["id"] = urlRecord.EntityId;
                break;
            case "news":
                values["controller"] = "Home";
                values["action"] = "ArticleList";
                break;
            case "customer":
                values["controller"] = "OurTeam";
                values["action"] = "Detail";
                values["id"] = urlRecord.EntityId;
                break;
            default:
                //no record found, thus generate an event this way developers could insert their own types

                break;
        }

        return new ValueTask<RouteValueDictionary>(values);
    }

    #endregion
}
