using Career.Web.Infrastructure;
using Career.Web.Models.Blogs;
using Career.Web.Models.CorporateManagement;
using Career.Web.Models.Legal;
using Career.Web.Models.Security;
using Career.Web.Rss;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Career.Web.Controllers;

public class HomeController : BaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHelper _webHelper;
    private readonly IApiClient _apiClient;
    private readonly Career.Web.Infrastructure.IUserAgentHelper _userAgentHelper;

    public HomeController(IHttpContextAccessor httpContextAccessor,
        IWebHelper webHelper,
        IApiClient apiClient,
        Career.Web.Infrastructure.IUserAgentHelper userAgentHelper)
    {
        _httpContextAccessor = httpContextAccessor;
        _webHelper = webHelper;
        _apiClient = apiClient;
        _userAgentHelper = userAgentHelper;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _apiClient.GetAsync<CorporateManagementSettingsModel>("api/CareerHomePage/Index");
        var maybe = NotFoundIfNull(model);
        if (maybe != null) return maybe;
        model.IsMobile = _userAgentHelper.IsMobileDevice();
        return View(model);
    }

    public async Task<IActionResult> OurStory()
    {
        var model = await _apiClient.GetAsync<CorporateManagementSettingsModel>("api/CareerHomePage/OurStory");
        var maybe = NotFoundIfNull(model);
        if (maybe != null) return maybe;
        model.IsMobile = _userAgentHelper.IsMobileDevice();
        return View(model);
    }

    public async Task<IActionResult> OurBrands()
    {
        var model = await _apiClient.GetAsync<CorporateManagementSettingsModel>("api/Vendor/OurBrands");
        var maybe = NotFoundIfNull(model);
        if (maybe != null) return maybe;
        return View(model);
    }

    public async Task<IActionResult> PrivacyPolicy()
    {
        var model = await _apiClient.GetAsync<LegalModel>("api/Topic/PrivacyPolicy");
        var maybe = NotFoundIfNull(model);
        if (maybe != null) return maybe;
        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> TermsConditions()
    {
        var model = await _apiClient.GetAsync<LegalModel>("api/Topic/TermsConditions");
        var maybe = NotFoundIfNull(model);
        if (maybe != null) return maybe;
        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> TermsOfUse()
    {
        var model = await _apiClient.GetAsync<LegalModel>("api/Topic/TermsOfUse");
        var maybe = NotFoundIfNull(model);
        if (maybe != null) return maybe;
        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> ArticleList(BlogPostListModel model)
    {
        // Try to get "page" from route
        var query = HttpContext.Request.Query;
        if (query.ContainsKey("page") && int.TryParse(query["page"], out var parsedPage))
            model.Page = parsedPage;

        var apiModel = await _apiClient.GetAsync<BlogPostListModel>("api/Blog/ArticleList", new
        {
            model.Page,
            model.PageSize
        });
        var maybe = NotFoundIfNull(apiModel);
        if (maybe != null) return maybe;

        return View("ArticleList", apiModel);
    }

    public async Task<IActionResult> NewsArticle(int id)
    {
        ArticleModel model;
        try
        {
            model = await _apiClient.GetAsync<ArticleModel>($"api/Blog/NewsArticle/{id}");
            var maybe = NotFoundIfNull(model);
            if (maybe != null) return maybe;
        }
        catch
        {
            return InvokeHttp404();
        }

        var httpContext = _httpContextAccessor.HttpContext.Request;
        model.HostURL = string.Format("{0}://{1}", httpContext.Scheme, httpContext.Host.Value);
        model.CurrentPageURL = string.Format("{0}://{1}{2}{3}", httpContext.Scheme, httpContext.Host.Value, httpContext.Path, httpContext.QueryString);

        return View("NewsArticle", model);
    }

    public async Task<IActionResult> NewsRss()
    {
        var feed = new RssFeed(
        "News from The Furniture Mart USA",
        "The latest news articles from The Furniture Mart USA",
        new Uri(_webHelper.GetStoreLocation() + "/news/"),
        DateTime.UtcNow);

        var items = new List<RssItem>();
        var recentBlogPosts = await _apiClient.GetAsync<List<BlogPostModel>>("api/Blog/NewsRss") ?? new List<BlogPostModel>();
        foreach (var recentBlogPost in recentBlogPosts)
        {
            var blogUrl = _webHelper.GetStoreLocation() + "/" + recentBlogPost.SeName;
            var content = string.IsNullOrEmpty(recentBlogPost.BodyOverview) ? string.Empty : Regex.Replace(recentBlogPost.BodyOverview, "<.*?>", String.Empty);
            items.Add(new RssItem(recentBlogPost.Title.Replace("<br>", "").Replace("<br/>", "").Replace("<br />", ""), content, new Uri(blogUrl.Replace("/inspiration/", "/news/")), $"urn:store:0:news:blog:{recentBlogPost.Id}", recentBlogPost.StartDateUtc ?? DateTime.UtcNow));
        }
        feed.Items = items;

        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }
}
