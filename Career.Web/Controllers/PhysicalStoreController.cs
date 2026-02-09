using Career.Web.Infrastructure;
using Career.Web.Models.Security;
using Career.Web.Models.StoreManagement;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Career.Web.Controllers;

public class PhysicalStoreController : BaseController
{
    private readonly IApiClient _apiClient;
    private readonly IUserAgentHelper _userAgentHelper;

    public PhysicalStoreController(IApiClient apiClient, IUserAgentHelper userAgentHelper)
    {
        _apiClient = apiClient;
        _userAgentHelper = userAgentHelper;
    }

    

    public async Task<IActionResult> PhysicalStoreDetail(string location)
    {
        if (string.IsNullOrEmpty(location) || !Regex.IsMatch(location, @"^\d+$"))
            return InvokeHttp404();

        LocationDetailModel model;
        try
        {
            model = await _apiClient.GetAsync<LocationDetailModel>($"api/PhysicalStore/PhysicalStoreDetail/{location}");
        }
        catch
        {
            return InvokeHttp404();
        }

        return View(model);
    }

    public async Task<IActionResult> List()
    {
        var model = await _apiClient.GetAsync<LocationModel>("api/PhysicalStore/List");
        model.IsMobile = _userAgentHelper.IsMobileDevice();
        return View(model);
    }
}
