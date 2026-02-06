using Career.Data.Domains.LandingPages;
using Career.Data.Services.Common;
using Career.Data.Services.Localization;
using Career.Data.Services.Locations;
using Career.Data.Services.Media;
using Career.Data.Services.Security;
using Career.Data.Services.Settings;
using Career.Data.Services.SummerJams;
using Career.Web.Models.SummerJams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class SummerJamController : BaseController
{
    #region Fields

    private readonly ISummerJamService _summerJamService;
    private readonly ICommonService _commonService;
    private readonly ILocationService _locationService;
    private readonly ISettingService _settingService;
    private readonly IPictureService _pictureService;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public SummerJamController(ISummerJamService summerJamService,
        ICommonService commonService,
        ISettingService settingService,
        IPictureService pictureService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        ILocationService locationService)
    {
        _summerJamService = summerJamService;
        _commonService = commonService;
        _settingService = settingService;
        _pictureService = pictureService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _locationService = locationService;
    }

    #endregion

    #region Utilities

    public async Task<SummerJamModel> PrepareSummerJamModelAsync(SummerJamModel model)
    {
        //AddStates country 1 for USA
        foreach (var item in (await _locationService.GetAllStatesAsync()).Where(k => k.CountryId == 1))
            model.States.Add(new SelectListItem { Text = item.Name, Value = item.Name });

        var fmSummerJamSettings = await _settingService.LoadSettingAsync<FMSummerJamSettings>();


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

    public  async Task<IActionResult> Index()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var model = new SummerJamModel();
        model = await PrepareSummerJamModelAsync(model);

        if (model == null)
            return InvokeHttp404();

        return View(model);
    }

    [HttpPost]
    public  async Task<IActionResult> Index(string slug, SummerJamModel model)
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        if (ModelState.IsValid)
        {
            var summerJam = new SummerJam();
            {
                summerJam.FirstName = model.FirstName;
                summerJam.LastName = model.LastName;
                summerJam.Email = model.Email;
                summerJam.Address1 = model.AddressLine1;
                summerJam.Address2 = model.AddressLine;
                summerJam.City = model.City;
                summerJam.StateProvinceId = (await _locationService.GetAllStatesAsync()).Where(k => k.Name == model.State).FirstOrDefault().Id;
                summerJam.Phone = model.Phone;
                summerJam.ZipCode = model.ZipCode;
                summerJam.DOB = model.DOB.Value;
                summerJam.CreatedDateUtc = DateTime.UtcNow;
                summerJam.StoreId = 3;
            }
            await _summerJamService.InsertSummerJamAsync(summerJam);

            if (summerJam.Id != 0)
            {
                model.StateName = (await _locationService.GetAllStatesAsync()).Where(x => x.Id == summerJam.StateProvinceId).FirstOrDefault().Name;

                model.Success = true;
            }
        }

        model = await PrepareSummerJamModelAsync(model);
        return View(model);
    }

    #endregion
}
