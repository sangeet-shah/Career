using Career.Web.Models.CorporateManagement;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Career.Web.Components;

public class SocialIconsViewComponent : ViewComponent
{
    #region Fields

    private readonly IApiClient _apiClient;

    #endregion

    #region Ctor

    public SocialIconsViewComponent(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _apiClient.GetAsync<CorporateManagementSettingsModel>("api/SocialIcons/Index") ?? new CorporateManagementSettingsModel();
        return View(model);
    }

    #endregion
}