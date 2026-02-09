using Career.Web.Infrastructure;
using Career.Web.Models.Hellobar;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Components;

public class HelloBarViewComponent : ViewComponent
{
    private readonly IApiClient _apiClient;
    private readonly IUserAgentHelper _userAgentHelper;

    public HelloBarViewComponent(IApiClient apiClient, IUserAgentHelper userAgentHelper)
    {
        _apiClient = apiClient;
        _userAgentHelper = userAgentHelper;
    }

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var isMobile = _userAgentHelper.IsMobileDevice();
        var helloBars = await _apiClient.GetAsync<List<HelloBarModel>>("api/HelloBar/List", new { isMobile });
        if (helloBars == null || !helloBars.Any())
            return Content(string.Empty);

        return View(helloBars);
    }

    #endregion
}