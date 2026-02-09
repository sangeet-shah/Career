using Middleware.Web.Domains.Blogs;
using Middleware.Web.Domains.Common;
using Middleware.Web.Models.Blogs;
using Middleware.Web.Models.Customers;
using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Services.Blogs;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Customers;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Seo;
using Middleware.Web.Services.Stores;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class OurTeamController : ControllerBase
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IPictureService _pictureService;
    private readonly IBlogService _blogService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor
    public OurTeamController(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IPictureService pictureService,
        IBlogService blogService,
        IUrlRecordService urlRecordService,
        IStoreService storeService)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _pictureService = pictureService;
        _blogService = blogService;
        _urlRecordService = urlRecordService;
        _storeService = storeService;
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> List()
    {
        dynamic customerListModel = new CustomerListModel();
        var customers = await _customerService.GetCustomersByRoleAsync(NopCustomerDefaults.OurTeamRoleName);
        var customerList = await Task.WhenAll(customers.Select(async x =>
                {
                    var fmCustomer = await _customerService.GetFMCustomersByCustomerIdAsync(x.Id);
                    var avatarId = await _genericAttributeService.GetAttributeAsync<int>(x, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer");

                    return new
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        AvatarId = avatarId,
                        AvatarUrl = await _pictureService.GetPictureUrlAsync(avatarId),
                        AvatarAuthorPosition = fmCustomer?.AuthorPosition ?? string.Empty,
                        DisplayOrder = fmCustomer?.DisplayOrder ?? 0,
                        FMUSABio = fmCustomer?.FMUSABio
                    };
                })
            );

        customerListModel.CustomerList = customerList
            .OrderBy(x => x.DisplayOrder == 0)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList();

        return Ok(customerListModel);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound();

        var fmCustomer = await _customerService.GetFMCustomersByCustomerIdAsync(customer.Id);
        if (fmCustomer == null || !fmCustomer.FMUSABio)
            return NotFound();

        var model = new CustomerModel();
        model.FirstName = customer.FirstName;
        model.LastName = customer.LastName;
        model.AvatarAuthorPosition = fmCustomer != null ? fmCustomer.AuthorPosition : string.Empty;
        model.BioImageId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer");
        model.BioImageUrl = await _pictureService.GetPictureUrlAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer"));
        model.Biography = fmCustomer.Biography;


        if (fmCustomer != null)
        {
            var socialMediaUrls = new List<string>();
            socialMediaUrls.Add(fmCustomer.LinkedInUrl);
            socialMediaUrls.Add(fmCustomer.FacebookUrl);
            socialMediaUrls.Add(fmCustomer.InstagramUrl);
            socialMediaUrls.Add(fmCustomer.PinterestUrl);
            socialMediaUrls.Add(fmCustomer.TwitterUrl);
            if (socialMediaUrls.Any())
                model.SocialMediaURLs = string.Join(",", socialMediaUrls);

            //if (!string.IsNullOrEmpty(model.SocialMediaURLs))
            //    model.SocialMediaURLs = string.Join(",", model.SocialMediaURLs.Split(',').Select(x => $"\"{x}\""));
        }

        var blogPosts = (await _blogService.GetBlogPostsByAuthorIdAsync((await _storeService.GetCurrentStoreAsync())?.Id ?? 0, customer.Id)).Take(3);
        model.BlogPostList = (await Task.WhenAll(blogPosts.Select(async x => new BlogPostModel
        {
            Id = x.Id,
            Title = x.Title,
            BodyOverview = x.BodyOverview,
            SeName = await _urlRecordService.GetSeNameAsync(x.Id, nameof(BlogPost))
        }))).ToList();

        return Ok(model);
    }
}
