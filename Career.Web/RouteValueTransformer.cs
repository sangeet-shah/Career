using Career.Web.Models.Api;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;

namespace Career.Web;

public class RouteValueTransformer : DynamicRouteValueTransformer
{
    private readonly IApiClient _apiClient;

    public RouteValueTransformer(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        if (values == null)
            return values;

        if (!values.TryGetValue("SeName", out var slugValue) || string.IsNullOrEmpty(slugValue as string))
            return values;

        var slug = slugValue as string;
        if (slug == "news/rss/")
        {
            values["controller"] = "Home";
            values["action"] = "NewsRss";
            return values;
        }

        if (slug.Split('/').Length > 2 && !slug.Split('/')[1].Contains("page="))
            slug = slug.Replace("news/", "inspiration/");

        if (slug.Split('/').Length > 1 && slug.Split('/')[1].Contains("page="))
        {
            var index = slug.IndexOf("/");
            if (index >= 0)
                slug = slug.Substring(0, index);
        }

        var urlRecord = await _apiClient.GetAsync<UrlRecordDto>("api/UrlRecord/GetBySlug", new { slug });
        if (urlRecord == null)
            return values;

        if (!urlRecord.IsActive)
        {
            values["controller"] = "Common";
            values["action"] = "InternalRedirect";
            values["permanentRedirect"] = true;
            httpContext.Items["nop.RedirectFromGenericPathRoute"] = true;
            return values;
        }

        switch (urlRecord.EntityName?.ToLowerInvariant())
        {
            case "landingpage":
                var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
                var storeId = store?.Id ?? 0;
                var urlRecordStoreWiseContest = await _apiClient.GetAsync<UrlRecordDto>("api/UrlRecord/GetBySlug", new { slug, storeId });
                if (urlRecordStoreWiseContest == null)
                {
                    values["controller"] = "Common";
                    values["action"] = "PageNotFound";
                }
                else
                {
                    values["controller"] = "ContestPage";
                    values["action"] = "ContestPage";
                    values["contestId"] = urlRecordStoreWiseContest.EntityId;
                    values["SeName"] = urlRecordStoreWiseContest.Slug;
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
        }

        return values;
    }
}
