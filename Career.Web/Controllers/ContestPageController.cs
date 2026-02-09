using Career.Web.Infrastructure;
using Career.Web.Models.LandingPage;
using Career.Web.Models.Security;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Career.Web.Controllers;

public class ContestPageController : BaseController
{
    private readonly IApiClient _apiClient;
    private readonly IUserAgentHelper _userAgentHelper;

    public ContestPageController(IApiClient apiClient, IUserAgentHelper userAgentHelper)
    {
        _apiClient = apiClient;
        _userAgentHelper = userAgentHelper;
    }

    

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> ContestPage(int contestId)
    {
        

        ContestPageResponseModel response;
        try
        {
            response = await _apiClient.GetAsync<ContestPageResponseModel>($"api/ContestPage/ContestPage/{contestId}");
        }
        catch
        {
            return InvokeHttp404();
        }

        response.Model.IsMobileDevice = _userAgentHelper.IsMobileDevice();
        return View("ContestPage", response.Model);
    }

    [HttpPost]
    public async Task<IActionResult> ContestPage(int contestId, LandingPageModel model)
    {
        ContestPageResponseModel response;
        try
        {
            response = await _apiClient.PostAsync<LandingPageModel, ContestPageResponseModel>($"api/ContestPage/ContestPage/{contestId}", model);
        }
        catch
        {
            return InvokeHttp404();
        }

        if (response == null)
        {
            ModelState.AddModelError(string.Empty, "Unable to submit the form. Please try again.");
            var fallback = await _apiClient.GetAsync<ContestPageResponseModel>($"api/ContestPage/ContestPage/{contestId}");
            var fallbackModel = fallback?.Model ?? model;
            if (fallback?.Model != null)
            {
                fallbackModel.FirstName = model.FirstName;
                fallbackModel.LastName = model.LastName;
                fallbackModel.DateOfBirth = model.DateOfBirth;
                fallbackModel.Email = model.Email;
                fallbackModel.Phone = model.Phone;
                fallbackModel.StreetAddress = model.StreetAddress;
                fallbackModel.City = model.City;
                fallbackModel.StateProvinceId = model.StateProvinceId;
                fallbackModel.ZipPostalCode = model.ZipPostalCode;
                fallbackModel.InstagramHandle = model.InstagramHandle;
                fallbackModel.TwitterHandle = model.TwitterHandle;
                fallbackModel.LocationId = model.LocationId;
                fallbackModel.EmailSubscribed = model.EmailSubscribed;
                fallbackModel.SMSSubscribed = model.SMSSubscribed;
                fallbackModel.SubscribedEventList = model.SubscribedEventList;
                fallbackModel.EventFlow = model.EventFlow;
                fallbackModel.ContestId = model.ContestId;
                fallbackModel.LandingPageId = model.LandingPageId;
            }

            fallbackModel.IsMobileDevice = _userAgentHelper.IsMobileDevice();
            return View("ContestPage", fallbackModel);
        }

        if (response.Errors?.Count > 0)
        {
            foreach (var error in response.Errors)
                ModelState.AddModelError(error.Key, error.Value);
        }

        response.Model.IsMobileDevice = _userAgentHelper.IsMobileDevice();
        if (response.ShowResult)
            return View("ContestPageResult", response.Model);

        return View("ContestPage", response.Model);
    }

}
