using Career.Web.Domains.Common;
using Career.Web.Models.Api;
using Career.Web.Models.Blogs;
using Career.Web.Models.Customers;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class OurTeamController : BaseController
{
    private readonly IApiClient _apiClient;

    public OurTeamController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> List()
    {
        var customers = await _apiClient.GetAsync<OurTeamCustomerDto[]>("api/Customer/GetCustomersByRole", new { role = NopCustomerDefaults.OurTeamRoleName });
        if (customers == null || !customers.Any())
        {
            return View(new CustomerListModel { CustomerList = new List<CustomerModel>() });
        }

        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;

        var list = new List<CustomerModel>();
        foreach (var x in customers)
        {
            var fmCustomer = await _apiClient.GetAsync<FMCustomerDto>("api/Customer/GetFMCustomerByCustomerId", new { customerId = x.Id });
            var avatarAttr = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute", new { entityId = x.Id, keyGroup = "Customer", key = NopCustomerDefaults.AvatarPictureIdAttribute, storeId = 0 });
            var pictureId = 0;
            int.TryParse(avatarAttr?.Value, out pictureId);
            var pictureUrl = "";
            if (pictureId > 0)
            {
                var picRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId });
                pictureUrl = picRes?.Url ?? "";
            }

            list.Add(new CustomerModel
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                AvatarId = pictureId,
                AvatarUrl = pictureUrl,
                AvatarAuthorPosition = fmCustomer?.AuthorPosition ?? "",
                DisplayOrder = fmCustomer?.DisplayOrder ?? 0,
                FMUSABio = fmCustomer?.FMUSABio ?? false,
            });
        }

        var customerListModel = new CustomerListModel
        {
            CustomerList = list.OrderBy(x => x.DisplayOrder == 0).ThenBy(x => x.DisplayOrder).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToList()
        };
        return View(customerListModel);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var customer = await _apiClient.GetAsync<OurTeamCustomerDto>("api/Customer/GetCustomerById", new { customerId = id });
        if (customer == null)
            return InvokeHttp404();

        var fmCustomer = await _apiClient.GetAsync<FMCustomerDto>("api/Customer/GetFMCustomerByCustomerId", new { customerId = id });
        if (fmCustomer == null || !fmCustomer.FMUSABio)
            return InvokeHttp404();

        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;

        var avatarAttr = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute", new { entityId = id, keyGroup = "Customer", key = NopCustomerDefaults.AvatarPictureIdAttribute, storeId = 0 });
        var pictureId = 0;
        int.TryParse(avatarAttr?.Value, out pictureId);
        var pictureUrl = "";
        if (pictureId > 0)
        {
            var picRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId });
            pictureUrl = picRes?.Url ?? "";
        }

        var model = new CustomerModel
        {
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            AvatarAuthorPosition = fmCustomer.AuthorPosition ?? "",
            BioImageId = pictureId,
            BioImageUrl = pictureUrl,
            Biography = fmCustomer.Biography
        };

        var socialMediaUrls = new List<string> { fmCustomer.LinkedInUrl, fmCustomer.FacebookUrl, fmCustomer.InstagramUrl, fmCustomer.PinterestUrl, fmCustomer.TwitterUrl };
        socialMediaUrls.RemoveAll(string.IsNullOrEmpty);
        if (socialMediaUrls.Any())
            model.SocialMediaURLs = string.Join(",", socialMediaUrls.Select(u => $"\"{u}\""));

        var blogPosts = await _apiClient.GetAsync<BlogPostDto[]>("api/Blog/GetBlogPostsByAuthorId", new { storeId, blogAuthorId = id });
        if (blogPosts != null && blogPosts.Any())
        {
            var blogList = new List<BlogPostModel>();
            foreach (var post in blogPosts.Take(3))
            {
                var seNameRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = post.Id, entityName = "BlogPost" });
                blogList.Add(new BlogPostModel { Id = post.Id, Title = post.Title, BodyOverview = post.BodyOverview, SeName = seNameRes?.SeName ?? "" });
            }
            model.BlogPostList = blogList;
        }

        return View(model);
    }
}
