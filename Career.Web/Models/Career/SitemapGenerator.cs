using Career.Web.Domains.Common;
using Career.Web.Models.Api;
using Career.Web.Models.Customers;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Career.Web.Models.Career;

public record SitemapGenerator
{
    #region Fields

    private XmlTextWriter _writer;
    private readonly IApiClient _apiClient;
    private readonly IMemoryCache _memoryCache;

    #endregion

    #region Ctor

    public SitemapGenerator(
        IApiClient apiClient,
        IMemoryCache memoryCache)
    {
        _apiClient = apiClient;
        _memoryCache = memoryCache;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Method that is overridden, that handles creation of child urls.
    /// Use the method WriteUrlLocation() within this method.
    /// </summary>
    /// <param name="urlHelper">URL helper</param>
    protected async Task GenerateUrlNodesAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;

        // our story
        var ourstoryPageUrl = urlHelper.RouteUrl("ourstory").ToLower();
        WriteUrlLocation(scheme + ourstoryPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // careers
        var careersUrl = urlHelper.RouteUrl("careers").ToLower();
        WriteUrlLocation(scheme + careersUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // All jobs detail and Apply job
        await WriteJobDetailsAsync(urlHelper);

        // our brands
        var ourbrandsPageUrl = urlHelper.RouteUrl("brands").ToLower();
        WriteUrlLocation(scheme + ourbrandsPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // locations
        var locationsPageUrl = urlHelper.RouteUrl("locations").ToLower();
        WriteUrlLocation(scheme + locationsPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
        await WritePhysicalStoreDetailsAsync(urlHelper);

        // news
        var newsUrl = "/news/";
        WriteUrlLocation(scheme + newsUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // articles
        await WriteArticlesAsync(urlHelper);

        // privacy-policy
        var privacypolicyPageUrl = urlHelper.RouteUrl("privacy-policy").ToLower();
        WriteUrlLocation(scheme + privacypolicyPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // terms-conditions
        var termsconditionsPageUrl = urlHelper.RouteUrl("terms-conditions").ToLower();
        WriteUrlLocation(scheme + termsconditionsPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // conditions-of-use
        var conditionsofusePageUrl = urlHelper.RouteUrl("conditions-of-use").ToLower();
        WriteUrlLocation(scheme + conditionsofusePageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // summerjam
        var summerjamPageUrl = urlHelper.RouteUrl("summerjam").ToLower();
        WriteUrlLocation(scheme + summerjamPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // contest
        await WriteContestsAsync(urlHelper);

        // golf
        var golfPageUrl = urlHelper.RouteUrl("golf").ToLower();
        WriteUrlLocation(scheme + golfPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // golfSponsor
        var golfSponsorPageUrl = urlHelper.RouteUrl("golfSponsor").ToLower();
        WriteUrlLocation(scheme + golfSponsorPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // golf
        var golfRegisterPageUrl = urlHelper.RouteUrl("golfRegister").ToLower();
        WriteUrlLocation(scheme + golfRegisterPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // ourteam
        var ourteamPageUrl = urlHelper.RouteUrl("ourteam").ToLower();
        WriteUrlLocation(scheme + ourteamPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // bio
        await WriteBioAsync(urlHelper);

        var furnitureMartUsa = "http://furnituremartusa.com";
        WriteUrlLocation(furnitureMartUsa, UpdateFrequency.Weekly, DateTime.UtcNow);
    }

    protected void WriteUrlLocation(string url, UpdateFrequency updateFrequency, DateTime lastUpdated)
    {

        _writer.WriteStartElement("url");
        string loc = XmlHelper.XmlEncode(url);
        _writer.WriteElementString("loc", loc);
        _writer.WriteEndElement();
    }

    protected async Task WriteJobDetailsAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;
        var jobs = await _apiClient.GetAsync<JobDto[]>("api/Career/GetAllJobs");
        if (jobs != null)
        {
            foreach (var opportunity in jobs)
            {
                var url = "/careers/careerdetail/?id=" + opportunity.Id;
                WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
        }
    }

    protected async Task WriteArticlesAsync(IUrlHelper urlHelper)
    {
        var scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;
        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var blogPosts = await _apiClient.GetAsync<BlogPostDto[]>("api/Blog/GetBlogPosts", new { storeId = store?.Id ?? 0, pageIndex = 0, pageSize = int.MaxValue });
        if (blogPosts != null)
        {
            foreach (var blogPost in blogPosts)
            {
                var seNameRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = blogPost.Id, entityName = "BlogPost" });
                var seName = seNameRes?.SeName;
                if (!string.IsNullOrEmpty(seName))
                    seName = seName.Replace("inspiration/", "news/");

                var url = "/" + seName;
                WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
        }
    }

    protected async Task WritePhysicalStoreDetailsAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;
        var physicalStores = await _apiClient.GetAsync<LocationDto[]>("api/Location/GetLocations", new { websiteId = (int)WebsiteEnum.FMUSA });
        if (physicalStores != null)
        {
            foreach (var physicalStore in physicalStores)
                WriteUrlLocation(scheme+ "/locations/"+ physicalStore.LocationId +"/", UpdateFrequency.Weekly, DateTime.UtcNow);
        }
    }

    protected async Task WriteBioAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;

        var customers = await _apiClient.GetAsync<OurTeamCustomerDto[]>("api/Customer/GetCustomersByRole", new { role = NopCustomerDefaults.OurTeamRoleName });
        if (customers == null)
            return;

        var customerListModel = new CustomerListModel { CustomerList = new System.Collections.Generic.List<CustomerModel>() };
        foreach (var x in customers)
        {
            var fmCustomer = await _apiClient.GetAsync<FMCustomerDto>("api/Customer/GetFMCustomerByCustomerId", new { customerId = x.Id });
            customerListModel.CustomerList.Add(new CustomerModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DisplayOrder = fmCustomer?.DisplayOrder ?? 0
            });
        }
        customerListModel.CustomerList = customerListModel.CustomerList
            .OrderBy(x => x.DisplayOrder == 0).ThenBy(x => x.DisplayOrder).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

        foreach (var author in customerListModel.CustomerList)
        {
            var seNameRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = author.Id, entityName = "Customer" });
            var seName = seNameRes?.SeName;
            if (string.IsNullOrEmpty(seName))
                continue;

            var url = "/ourteam/" + seName.TrimEnd('/') + "/";
            WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
        }
    }

    protected async Task WriteContestsAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;

        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var urlRecords = await _apiClient.GetAsync<UrlRecordDto[]>("api/UrlRecord/GetSlugs", new { entityName = "LandingPage", storeId = store?.Id ?? 0 });
        if (urlRecords == null)
            return;

        foreach (var urlRecord in urlRecords)
        {
            try
            {
                var contest = await _apiClient.GetAsync<LandingPageDto>("api/LandingPage/GetById", new { id = urlRecord.EntityId });
                var currentDate = DateTime.UtcNow; // ConvertToUserTime is handled by middleware
                if (contest == null)
                    continue;
                if (!contest.Published)
                    continue;
                if (contest.StartDateUtc != null && contest.StartDateUtc >= currentDate)
                    continue;
                if (contest.EndDateUtc != null && contest.EndDateUtc <= currentDate)
                    continue;

                var url = "/" + urlRecord.Slug;
                WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// This will build an xml sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="urlHelper">URL helper</param>
    /// <returns>Sitemap.xml as string</returns>
    public async Task<string> GenerateAsync(IUrlHelper urlHelper)
    {
        if (!_memoryCache.TryGetValue(CacheKeys.SitemapCacheKey, out string cachedSitemap))
        {
            using var stream = new MemoryStream();
            await GenerateAsync(urlHelper, stream);
            cachedSitemap = Encoding.UTF8.GetString(stream.ToArray());
            _memoryCache.Set(CacheKeys.SitemapCacheKey, cachedSitemap, TimeSpan.FromHours(24));
        }
        return cachedSitemap;
    }

    /// <summary>
    /// This will build an xml sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="urlHelper">URL helper</param>
    /// <param name="stream">Stream of sitemap.</param>
    public async Task GenerateAsync(IUrlHelper urlHelper, Stream stream)
    {
        using (_writer = new XmlTextWriter(stream, Encoding.UTF8))
        {
            _writer.Formatting = Formatting.Indented;
            _writer.WriteStartDocument();
            _writer.WriteStartElement("urlset");
            _writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            _writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            await GenerateUrlNodesAsync(urlHelper);

            _writer.WriteEndElement();
        }
    }

    #endregion
}
