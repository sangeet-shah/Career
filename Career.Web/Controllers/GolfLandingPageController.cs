using Career.Web.Models.GolfLanding;
using Career.Web.Models.Security;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Career.Web.Controllers;

public class GolfLandingPageController : BaseController
{
    private readonly IApiClient _apiClient;

    public GolfLandingPageController(IApiClient apiClient)
    {
        _apiClient = apiClient;
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
        var model = await _apiClient.GetAsync<GolfLandingPageModel>("api/GolfLandingPage/Register");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(GolfLandingPageModel model)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _apiClient.GetAsync<GolfLandingPageModel>("api/GolfLandingPage/Register") ?? new GolfLandingPageModel();
            prepared.CompanyName = model.CompanyName;
            prepared.PhoneNumber = model.PhoneNumber;
            prepared.Email = model.Email;
            prepared.SponsorshipLevelId = model.SponsorshipLevelId;
            prepared.Contact1 = model.Contact1;
            prepared.Contact2 = model.Contact2;
            prepared.Contact3 = model.Contact3;
            prepared.Contact4 = model.Contact4;
            prepared.PictureId = model.PictureId;

            return View(prepared);
        }

        var response = await _apiClient.PostAsync<GolfLandingPageModel, GolfLandingPageModel>("api/GolfLandingPage/Register", model);
        if (response == null)
        {
            ModelState.AddModelError(string.Empty, "Unable to submit the form. Please try again.");
            var prepared = await _apiClient.GetAsync<GolfLandingPageModel>("api/GolfLandingPage/Register") ?? model;
            if (prepared != model)
            {
                prepared.CompanyName = model.CompanyName;
                prepared.PhoneNumber = model.PhoneNumber;
                prepared.Email = model.Email;
                prepared.SponsorshipLevelId = model.SponsorshipLevelId;
                prepared.Contact1 = model.Contact1;
                prepared.Contact2 = model.Contact2;
                prepared.Contact3 = model.Contact3;
                prepared.Contact4 = model.Contact4;
                prepared.PictureId = model.PictureId;
            }
            return View(prepared);
        }

        return View(response);
    }

}
