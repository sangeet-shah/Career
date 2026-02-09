using Middleware.Web.Domains.CDN;
using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Models.Hellobar;
using Middleware.Web.Services.HelloBar;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class HelloBarController : ControllerBase
{
    #region Fields

    private readonly IHelloBarService _helloBarService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public HelloBarController(IHelloBarService helloBarService,
        ISettingService settingService,
        IStoreService storeService)
    {
        _helloBarService = helloBarService;
        _settingService = settingService;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    private static string CDNUrlChange(string description, string cdnImageUrl)
    {
        if (string.IsNullOrEmpty(description) || description.IndexOf("/images/uploaded/", StringComparison.OrdinalIgnoreCase) <= 0 || string.IsNullOrEmpty(cdnImageUrl))
            return description;

        return description.Replace("/images/uploaded/", cdnImageUrl.TrimEnd('/') + "/images/uploaded/");
    }

    #endregion

    #region Methods

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool isMobile = false)
    {
        var helloBars = await _helloBarService.GetActiveHelloBarsAsync();
        if (helloBars == null || !helloBars.Any())
            return Ok(Array.Empty<HelloBarModel>());

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var nopAdvanceCDNSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>(storeId);

        var helloBarListModel = helloBars.Select(helloBar => new HelloBarModel
        {
            Id = helloBar.Id,
            Name = helloBar.Name,
            Content = isMobile && !string.IsNullOrEmpty(helloBar.MobileContent)
                ? CDNUrlChange(helloBar.MobileContent, nopAdvanceCDNSettings.CDNImageUrl)
                : CDNUrlChange(helloBar.Content, nopAdvanceCDNSettings.CDNImageUrl),
            BGColorRgb = helloBar.BGColorRgb,
            Height = helloBar.Height,
            CustomURL = helloBar.CustomUrl,
            PopupDisclaimer = helloBar.PopupDisclaimer,
            DisclaimerTitle = helloBar.PopupDisclaimerTitle,
            Disclaimer = CDNUrlChange(helloBar.PopupDisclaimerContent, nopAdvanceCDNSettings.CDNImageUrl),
            MobileContent = helloBar.MobileContent
        }).ToList();

        return Ok(helloBarListModel);
    }

    #endregion
}
