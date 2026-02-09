using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Models.Blogs;
using Middleware.Web.Domains.Blogs;
using Middleware.Web.Domains.Common;
using Middleware.Web.Models.Media;
using Middleware.Web.Services.Blogs;
using Middleware.Web.Services.Customers;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Seo;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Middleware.Web.Services.Common;
using Middleware.Web.Domains.CDN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class BlogController : ControllerBase
{
    private readonly IGalleryService _galleryService;
    private readonly IPictureService _pictureService;
    private readonly IBlogService _blogService;
    private readonly IStoreService _storeService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ISettingService _settingService;

    public BlogController(
        IGalleryService galleryService,
        IPictureService pictureService,
        IBlogService blogService,
        IStoreService storeService,
        IUrlRecordService urlRecordService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ISettingService settingService)
    {
        _galleryService = galleryService;
        _pictureService = pictureService;
        _blogService = blogService;
        _storeService = storeService;
        _urlRecordService = urlRecordService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _settingService = settingService;
    }

    [HttpGet]
    public async Task<IActionResult> ArticleList([FromQuery] BlogPostListModel model)
    {
        var corporateGallery = await _galleryService.GetCorporateGalleryAsync();
        if (corporateGallery != null)
        {
            model.PhotoGalleryTitle = corporateGallery?.Title;

            var galleries = await _galleryService.GetHighlightedGalleryAsync(corporateGallery.Id);
            if (galleries.Any())
            {
                model.GalleryPictures = await galleries.SelectAwait(async x =>
                {
                    var picture = await _pictureService.GetPictureByIdAsync(x.PictureId);
                    return new GalleryPictureMappingModel
                    {
                        Title = picture?.TitleAttribute,
                        AltText = picture?.AltAttribute,
                        ShortDescription = x.ShortDescription,
                        PictureURL = await _pictureService.GetPictureUrlAsync(x.PictureId),
                        Link = x.LinkUrl,
                        DisplayOrder = x.DisplayOrder
                    };
                }).ToListAsync();
            }
        }
        var blogPosts = await _blogService.GetBlogPostsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0, pageIndex: model.Page - 1, pageSize: model.PageSize);
        if (blogPosts.Any())
        {
            model.BlogPostList = await blogPosts.SelectAwait(async x =>
            {
                var fmBlog = await _blogService.GetFMBlogByBlogPostIdAsync(x.Id);
                var pictureUrl = await _pictureService.GetPictureUrlAsync(fmBlog?.ThumbnailPictureId ?? 0);
                return new BlogPostModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartDateUtc = x.StartDateUtc,
                    BodyOverview = x.BodyOverview,
                    FMUSAPictureURL = pictureUrl,
                    SeName = await _urlRecordService.GetSeNameAsync(x.Id, nameof(BlogPost))
                };
            }).ToListAsync();
        }

        model.TotalCount = blogPosts.TotalCount;
        model.TotalPages = (int)Math.Ceiling(decimal.Divide(model.TotalCount, model.PageSize));

        return Ok(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> NewsArticle(int id)
    {
        var nopAdvanceCDNSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>((await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        var blogPost = await _blogService.GetBlogPostByIdAsync((await _storeService.GetCurrentStoreAsync())?.Id ?? 0, id);
        if (blogPost == null)
            return NotFound();

        var fmBlog = await _blogService.GetFMBlogByBlogPostIdAsync(blogPost.Id);
        var model = new ArticleModel();
        model.BlogPost.Title = blogPost.Title;
        model.BlogPost.SeName = (await _urlRecordService.GetSeNameAsync(blogPost.Id, nameof(BlogPost))).Replace("inspiration/", "").TrimEnd('/');
        model.BlogPost.MetaTitle = blogPost.MetaTitle;
        model.BlogPost.StartDateUtc = blogPost.StartDateUtc;
        if (blogPost.Body != null && blogPost.Body.Contains("/images/uploaded/"))
            model.BlogPost.Body = blogPost.Body.Replace("/images/uploaded/", nopAdvanceCDNSettings.CDNImageUrl.TrimEnd('/') + "/images/uploaded/");
        else
            model.BlogPost.Body = blogPost.Body.Replace("/Images/uploaded/", nopAdvanceCDNSettings.CDNImageUrl.TrimEnd('/') + "/images/uploaded/");
        model.BlogPost.BodyOverview = blogPost.BodyOverview;
        model.BlogPost.UpdatedDateUtc = fmBlog?.ModifiedDateUtc ?? null;

        var blogPictureId = fmBlog?.SEOPictureId1 > 0 ? fmBlog?.SEOPictureId1 : (fmBlog?.ThumbnailPictureId ?? 0);
        model.BlogPost.BlogBoxPicturePictureURL = await _pictureService.GetPictureUrlAsync(blogPictureId.HasValue ? blogPictureId.Value : 0);

        model.BlogPost.SEOPictureUrl1 = await _pictureService.GetPictureUrlAsync(fmBlog?.SEOPictureId1 ?? 0);
        model.BlogPost.SEOPictureUrl2 = await _pictureService.GetPictureUrlAsync(fmBlog?.SEOPictureId2 ?? 0);
        model.BlogPost.SEOPictureUrl3 = await _pictureService.GetPictureUrlAsync(fmBlog?.SEOPictureId3 ?? 0);

        var authors = await _blogService.GetBlogPostAuthorsByBlogPostIdAsync(blogPost.Id);
        if (authors.Any())
        {
            var customers = await _customerService.GetCustomersByIdsAsync(authors.Select(c => c.AuthorId).ToArray());
            if (!customers.Any())
                return NotFound();

            foreach (var customer in customers)
            {
                var fmCustomer = await _customerService.GetFMCustomersByCustomerIdAsync(customer.Id);
                var fisrtName = customer.FirstName;
                var lastName = customer.LastName;
                var isAuthorRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AuthorRoleName, true);
                var hostUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var authorUrl = isAuthorRole && fmCustomer != null && fmCustomer.FMUSABio ? hostUrl + "/ourteam/" + fisrtName.ToLower().Replace(" ", "-") + "-" + lastName.ToLower().Replace(" ", "-") + "/" : string.Empty;

                var blogAutherDetailsModel = new BlogAuthorDetailsModel
                {
                    AuthorId = customer.Id,
                    AuthorFirstName = fisrtName,
                    AuthorLastName = lastName,
                    FMUSABio = fmCustomer?.FMUSABio ?? false,
                    AuthorPictureUrl = await _pictureService.GetPictureUrlAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer")),
                    AuthorPosition = fmCustomer != null ? fmCustomer.AuthorPosition : string.Empty,
                    AuthorType = fmCustomer != null ? fmCustomer.IsOrganization : false,
                    AuthorUrl = authorUrl
                };

                model.BlogAuthors.Add(blogAutherDetailsModel);
            }
        }

        var blogPosts = await _blogService.GetBlogPostsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0, pageSize: 3);
        model.BlogPostList = await blogPosts.SelectAwait(async x => new BlogPostModel
        {
            Id = x.Id,
            Title = x.Title,
            MetaTitle = x.MetaTitle,
            StartDateUtc = x.StartDateUtc,
            BodyOverview = x.BodyOverview,
            SeName = await _urlRecordService.GetSeNameAsync(x.Id, nameof(BlogPost))
        }).ToListAsync();

        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> NewsRss()
    {
        var items = new List<BlogPostModel>();
        var recentBlogPosts = await _blogService.GetLatestBlogsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0, numberOfBlogs: 12);
        foreach (var recentBlogPost in recentBlogPosts)
        {
            var seName = await _urlRecordService.GetSeNameAsync(recentBlogPost.Id, nameof(BlogPost));
            var content = string.IsNullOrEmpty(recentBlogPost.BodyOverview) ? string.Empty : Regex.Replace(recentBlogPost.BodyOverview, "<.*?>", String.Empty);
            items.Add(new BlogPostModel
            {
                Id = recentBlogPost.Id,
                Title = recentBlogPost.Title.Replace("<br>", "").Replace("<br/>", "").Replace("<br />", ""),
                BodyOverview = content,
                SeName = seName,
                StartDateUtc = recentBlogPost.StartDateUtc ?? DateTime.UtcNow
            });
        }

        return Ok(items);
    }
}

