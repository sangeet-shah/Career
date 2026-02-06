using Career.Data.Domains.CorporateManagement;
using Career.Data.Services.Media;
using Career.Data.Services.Settings;
using Career.Web.Models.CorporateManagement;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Career.Web.Components;

public class SocialIconsViewComponent : ViewComponent
{
    #region Fields

    private readonly IPictureService _pictureService;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public SocialIconsViewComponent(IPictureService pictureService,
        ISettingService settingService)
    {
        _pictureService = pictureService;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new CorporateManagementSettingsModel();
        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>();
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

        return View(model);
    }

    #endregion
}