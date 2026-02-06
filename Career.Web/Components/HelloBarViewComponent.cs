using Career.Data.Domains.CDN;
using Career.Data.Helpers;
using Career.Data.Services.HelloBar;
using Career.Data.Services.Settings;
using Career.Data.Services.Stores;
using Career.Web.Models.Hellobar;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Components;

public class HelloBarViewComponent : ViewComponent
{
    #region Fields

    private readonly IHelloBarService _helloBarService;
    private readonly IUserAgentHelper _userAgentHelper;
    private readonly IStoreService _storeService;
    private readonly ISettingService _settingService;   

    #endregion

    #region Ctor

    public HelloBarViewComponent(IHelloBarService helloBarService,
        IUserAgentHelper userAgentHelper,
        IStoreService storeService,
        ISettingService settingService)
    {
        _helloBarService = helloBarService;
        _userAgentHelper = userAgentHelper;
        _storeService = storeService;
        _settingService= settingService;    
    }

    #endregion

    #region Utilities
    private string CDNUrlChange(string description,string cdnImageUrl)
    {
        if (string.IsNullOrEmpty(description) || description.IndexOf("/images/uploaded/") <= 0 || string.IsNullOrEmpty(cdnImageUrl))
            return description;
        
        return description.Replace("/images/uploaded/", cdnImageUrl.TrimEnd('/') + "/images/uploaded/") ;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var helloBars = await _helloBarService.GetActiveHelloBarsAsync();
        if (helloBars == null || !helloBars.Any())
            return Content(string.Empty);

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var nopAdvanceCDNSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>(storeId);

        var IsMobileDevice = _userAgentHelper.IsMobileDevice();
        var helloBarListModel = helloBars.Select(helloBar => new HelloBarModel
        {
            Id = helloBar.Id,
            Name = helloBar.Name,
            Content = IsMobileDevice && !string.IsNullOrEmpty(helloBar.MobileContent) ? CDNUrlChange(helloBar.MobileContent, nopAdvanceCDNSettings.CDNImageUrl) : CDNUrlChange(helloBar.Content,nopAdvanceCDNSettings.CDNImageUrl),
            BGColorRgb = helloBar.BGColorRgb,
            Height = helloBar.Height,
            CustomURL = helloBar.CustomUrl,
            PopupDisclaimer = helloBar.PopupDisclaimer,
            DisclaimerTitle = helloBar.PopupDisclaimerTitle,
            Disclaimer = CDNUrlChange(helloBar.PopupDisclaimerContent,nopAdvanceCDNSettings.CDNImageUrl),
            MobileContent = helloBar.MobileContent
        }).ToList();

        return View(helloBarListModel);
    }

    #endregion
}