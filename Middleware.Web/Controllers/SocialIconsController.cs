using Middleware.Web.Domains.CorporateManagement;
using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Models.CorporateManagement;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class SocialIconsController : ControllerBase
{
    #region Fields

    private readonly IPictureService _pictureService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public SocialIconsController(IPictureService pictureService,
        ISettingService settingService,
        IStoreService storeService)
    {
        _pictureService = pictureService;
        _settingService = settingService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new CorporateManagementSettingsModel();
        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>(storeId);
        var fbpicture = await _pictureService.GetPictureByIdAsync(corporateManagementSettings.FacebookIconId);

        model.FacebookImageUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.FacebookIconId, showDefaultPicture: false);
        model.FacebookImageAltText = fbpicture?.AltAttribute;
        model.FacebookImageTitle = fbpicture?.TitleAttribute;
        model.FacebookURL = corporateManagementSettings.FacebookUrl;

        var youTubeImage = await _pictureService.GetPictureByIdAsync(corporateManagementSettings.YouTubeIconId);
        model.YouTubeImageUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.YouTubeIconId, showDefaultPicture: false);
        model.YouTubeImageAltText = youTubeImage?.AltAttribute;
        model.YouTubeImageTitle = youTubeImage?.TitleAttribute;
        model.YouTubeURL = corporateManagementSettings.YouTubeUrl;

        var twitterImage = await _pictureService.GetPictureByIdAsync(corporateManagementSettings.TwitterIconId);
        model.TwitterImageUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.TwitterIconId, showDefaultPicture: false);
        model.TwitterImageAltText = twitterImage?.AltAttribute;
        model.TwitterImageTitle = twitterImage?.TitleAttribute;
        model.TwitterURL = corporateManagementSettings.TwitterUrl;

        var linkedInImage = await _pictureService.GetPictureByIdAsync(corporateManagementSettings.LinkedInIconId);
        model.LinkedInImageUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LinkedInIconId, showDefaultPicture: false);
        model.LinkedInImageAltText = linkedInImage?.AltAttribute;
        model.LinkedInImageTitle = linkedInImage?.TitleAttribute;
        model.LinkedInURL = corporateManagementSettings.LinkedInUrl;

        return Ok(model);
    }

    #endregion
}
