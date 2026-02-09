using Career.Web.Domains.LandingPages;
using Career.Web.Models.Api;
using Career.Web.Models.GolfLanding;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class GolfLandingPageController : BaseController
{
    private readonly IApiClient _apiClient;

    public GolfLandingPageController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private async Task PrepareGolfLandingPageModelAsync(GolfLandingPageModel model)
    {
        model.AvailableSponsorshipLevels.Insert(0, new SelectListItem { Text = "Select sponsorship level" });
        var enumTypeName = "Career.Web.Domains.LandingPages.SponsorshipLevelEnum, Career.Web";
        foreach (var sponsorshipLevel in Enum.GetValues(typeof(SponsorshipLevelEnum)).Cast<SponsorshipLevelEnum>())
        {
            var descResp = await _apiClient.GetAsync<EnumDescriptionResponse>("api/Common/GetEnumDescription", new { enumTypeName, enumValue = sponsorshipLevel.ToString() });
            model.AvailableSponsorshipLevels.Add(new SelectListItem { Text = descResp?.Description ?? sponsorshipLevel.ToString(), Value = ((int)sponsorshipLevel).ToString() });
        }

        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;
        var settings = await _apiClient.GetAsync<FMGolfEventLandingPageSettingsDto>("api/Setting/GetFMGolfEventLandingPageSettings", new { storeId });
        if (settings != null)
        {
            model.IsActive = settings.Enabled;
            model.Description = settings.Description;
        }
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Sponsor()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var model = new GolfLandingPageModel();
        await PrepareGolfLandingPageModelAsync(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(GolfLandingPageModel model)
    {
        ModelState.Remove("PictureId");

        if (ModelState.IsValid)
        {
            var request = new
            {
                CompanyName = model.CompanyName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                SponsorshipLevelId = model.SponsorshipLevelId,
                Contact1 = model.Contact1,
                Contact2 = model.Contact2,
                Contact3 = model.Contact3,
                Contact4 = model.Contact4,
                CreatedOnUtc = DateTime.UtcNow,
                PictureId = model.PictureId
            };
            await _apiClient.PostAsync<object, object>("api/GolfLandingPage/Insert", request);
            model.Success = true;
        }

        await PrepareGolfLandingPageModelAsync(model);
        return View(model);
    }
}
