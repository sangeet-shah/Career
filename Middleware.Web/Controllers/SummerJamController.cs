using Middleware.Web.Domains.LandingPages;
using Middleware.Web.Models.SummerJams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Middleware.Web.Filters;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Localization;
using Middleware.Web.Services.Locations;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Middleware.Web.Services.SummerJams;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class SummerJamController : ControllerBase
{
    #region Fields

    private readonly ISummerJamService _summerJamService;
    private readonly ICommonService _commonService;
    private readonly ILocationService _locationService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;
    private readonly IPictureService _pictureService;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public SummerJamController(ISummerJamService summerJamService,
        ICommonService commonService,
        ISettingService settingService,
        IStoreService storeService,
        IPictureService pictureService,
        ILocalizationService localizationService,
        ILocationService locationService)
    {
        _summerJamService = summerJamService;
        _commonService = commonService;
        _settingService = settingService;
        _storeService = storeService;
        _pictureService = pictureService;
        _localizationService = localizationService;
        _locationService = locationService;
    }

    #endregion

    #region Utilities

    public async Task<SummerJamModel> PrepareSummerJamModelAsync(SummerJamModel model)
    {
        foreach (var item in (await _locationService.GetAllStatesAsync()).Where(k => k.CountryId == 1))
            model.States.Add(new SelectListItem { Text = item.Name, Value = item.Name });

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var fmSummerJamSettings = await _settingService.LoadSettingAsync<FMSummerJamSettings>(storeId);

        model.FMUSASummerJamSetting = fmSummerJamSettings;

        var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);

        model.FMUSASummerJamSetting.StartDateUtc = fmSummerJamSettings.StartDateUtc;
        model.FMUSASummerJamSetting.EndDateUtc = fmSummerJamSettings.EndDateUtc;

        if (model.FMUSASummerJamSetting.StartDateUtc.HasValue && currentDate < model.FMUSASummerJamSetting.StartDateUtc.Value)
            return null;

        if (model.FMUSASummerJamSetting.EndDateUtc.HasValue && currentDate > model.FMUSASummerJamSetting.EndDateUtc.Value)
            return null;

        model.FMUSASummerJamSetting.ViewPatioSetMobileImageIdUrl = await _pictureService.GetPictureUrlAsync(model.FMUSASummerJamSetting.ViewPatioSetMobileImageId);

        model.FMUSASummerJamSetting.ViewPatioSetWebImageIdUrl = await _pictureService.GetPictureUrlAsync(model.FMUSASummerJamSetting.ViewPatioSetImageId);

        model.FMUSASummerJamSetting.DailyLineUpMobileImageIdUrl = await _pictureService.GetPictureUrlAsync(model.FMUSASummerJamSetting.DailyLineUpMobileImageId);

        model.FMUSASummerJamSetting.DailyLineUpWebImageIdUrl = await _pictureService.GetPictureUrlAsync(model.FMUSASummerJamSetting.DailyLineUpImageId);

        return model;
    }

    #endregion

    #region Method

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new SummerJamModel();
        model = await PrepareSummerJamModelAsync(model);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromQuery] string slug, [FromBody] SummerJamModel model)
    {
        if (ModelState.IsValid)
        {
            var summerJam = new Middleware.Web.Domains.LandingPages.SummerJam
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Address1 = model.AddressLine1,
                Address2 = model.AddressLine,
                City = model.City,
                StateProvinceId = (await _locationService.GetAllStatesAsync()).Where(k => k.Name == model.State).FirstOrDefault().Id,
                Phone = model.Phone,
                ZipCode = model.ZipCode,
                DOB = model.DOB.Value,
                CreatedDateUtc = DateTime.UtcNow,
                StoreId = 3
            };
            await _summerJamService.InsertSummerJamAsync(summerJam);

            if (summerJam.Id != 0)
            {
                model.StateName = (await _locationService.GetAllStatesAsync()).Where(x => x.Id == summerJam.StateProvinceId).FirstOrDefault().Name;

                model.Success = true;
            }
        }

        model = await PrepareSummerJamModelAsync(model);
        return Ok(model);
    }

    #endregion
}
