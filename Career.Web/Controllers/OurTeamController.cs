using Career.Data.Domains.Blogs;
using Career.Data.Domains.Common;
using Career.Data.Extensions;
using Career.Data.Services.Blogs;
using Career.Data.Services.Common;
using Career.Data.Services.Customers;
using Career.Data.Services.Media;
using Career.Data.Services.Seo;
using Career.Data.Services.Stores;
using Career.Web.Models.Blogs;
using Career.Web.Models.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class OurTeamController : BaseController
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

	public async Task<IActionResult> List()
	{
		var customerListModel = new CustomerListModel
		{
			CustomerList = await (await _customerService.GetCustomersByRoleAsync(NopCustomerDefaults.OurTeamRoleName)).SelectAwait(async x =>
			{
				var fmCustomer = await _customerService.GetFMCustomersByCustomerIdAsync(x.Id);

				return new CustomerModel
				{
					FirstName = x.FirstName,
					LastName = x.LastName,
					AvatarId = await _genericAttributeService.GetAttributeAsync<int>(x, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer"),
					AvatarUrl = await _pictureService.GetPictureUrlAsync(await _genericAttributeService.GetAttributeAsync<int>(x, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer")),
					AvatarAuthorPosition = fmCustomer != null ? fmCustomer.AuthorPosition : string.Empty,
					DisplayOrder = fmCustomer.DisplayOrder,
					FMUSABio = fmCustomer.FMUSABio,
				};
			}).OrderBy(x => x.DisplayOrder == 0).ThenBy(x => x.DisplayOrder).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync()
		};

		return View(customerListModel);
	}

	public async Task<IActionResult> Detail(int id)
	{
		var customer = await _customerService.GetCustomerByIdAsync(id);
		if (customer == null)
			return InvokeHttp404();

		var fmCustomer = await _customerService.GetFMCustomersByCustomerIdAsync(customer.Id);
		if (fmCustomer == null || !fmCustomer.FMUSABio)
			return InvokeHttp404();

		var model = new CustomerModel
		{
			FirstName = customer.FirstName,
			LastName = customer.LastName,
			AvatarAuthorPosition = fmCustomer != null ? fmCustomer.AuthorPosition : string.Empty,
			BioImageId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer"),
			BioImageUrl = await _pictureService.GetPictureUrlAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer")),
			Biography = fmCustomer.Biography
		};
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

			if (!string.IsNullOrEmpty(model.SocialMediaURLs))
				model.SocialMediaURLs = string.Join(",", model.SocialMediaURLs.Split(',').Select(x => $"\"{x}\""));
		}

		var blogPosts = (await _blogService.GetBlogPostsByAuthorIdAsync((await _storeService.GetCurrentStoreAsync())?.Id ?? 0, customer.Id)).Take(3);
		model.BlogPostList = await blogPosts.SelectAwait(async x => new BlogPostModel
		{
			Id = x.Id,
			Title = x.Title,
			BodyOverview = x.BodyOverview,
			SeName = await _urlRecordService.GetSeNameAsync(x.Id, nameof(BlogPost))
		}).ToListAsync();

		return View(model);
	}
}
