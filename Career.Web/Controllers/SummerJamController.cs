using Career.Web.Models.Api;
using Career.Web.Models.SummerJams;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class SummerJamController : BaseController
{
    private readonly IApiClient _apiClient;

    public SummerJamController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private async Task<SummerJamModel> PrepareSummerJamModelAsync(SummerJamModel model)
    {
        var states = await _apiClient.GetAsync<System.Collections.Generic.List<StateProvinceDto>>("api/Location/GetAllStates");
        if (states != null)
        {
            foreach (var item in states.Where(k => k.CountryId == 1))
                model.States.Add(new SelectListItem { Text = item.Name, Value = item.Name });
        }

        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;
        var fmSummerJamSettings = await _apiClient.GetAsync<FMSummerJamSettingsDto>("api/Setting/GetFMSummerJamSettings", new { storeId });
        if (fmSummerJamSettings != null)
        {
            model.FMUSASummerJamSetting.StartDateUtc = fmSummerJamSettings.StartDateUtc;
            model.FMUSASummerJamSetting.EndDateUtc = fmSummerJamSettings.EndDateUtc;
            model.FMUSASummerJamSetting.ViewPatioSetMobileImageIdUrl = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = fmSummerJamSettings.ViewPatioSetMobileImageId, showDefaultPicture = false }))?.Url;
            model.FMUSASummerJamSetting.ViewPatioSetWebImageIdUrl = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = fmSummerJamSettings.ViewPatioSetImageId, showDefaultPicture = false }))?.Url;
            model.FMUSASummerJamSetting.DailyLineUpMobileImageIdUrl = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = fmSummerJamSettings.DailyLineUpMobileImageId, showDefaultPicture = false }))?.Url;
            model.FMUSASummerJamSetting.DailyLineUpWebImageIdUrl = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = fmSummerJamSettings.DailyLineUpImageId, showDefaultPicture = false }))?.Url;
        }

        var convertResp = await _apiClient.GetAsync<ConvertToUserTimeResponse>("api/Common/ConvertToUserTime", new { dateTime = DateTime.UtcNow, sourceDateTimeKind = "Utc" });
        var currentDate = convertResp?.Result ?? DateTime.UtcNow;

        if (model.FMUSASummerJamSetting.StartDateUtc.HasValue && currentDate < model.FMUSASummerJamSetting.StartDateUtc.Value)
            return null;
        if (model.FMUSASummerJamSetting.EndDateUtc.HasValue && currentDate > model.FMUSASummerJamSetting.EndDateUtc.Value)
            return null;

        return model;
    }

    public async Task<IActionResult> Index()
    {
        var model = new SummerJamModel();
        model = await PrepareSummerJamModelAsync(model);
        if (model == null)
            return InvokeHttp404();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string slug, SummerJamModel model)
    {
        if (ModelState.IsValid)
        {
            var states = await _apiClient.GetAsync<System.Collections.Generic.List<StateProvinceDto>>("api/Location/GetAllStates");
            var stateProvinceId = states?.Where(k => k.Name == model.State).Select(k => k.Id).FirstOrDefault() ?? 0;

            var request = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Address1 = model.AddressLine1,
                Address2 = model.AddressLine,
                City = model.City,
                StateProvinceId = stateProvinceId,
                Phone = model.Phone,
                ZipCode = model.ZipCode,
                DOB = model.DOB,
                CreatedDateUtc = DateTime.UtcNow,
                StoreId = 3
            };
            var insertResp = await _apiClient.PostAsync<object, SummerJamInsertResponse>("api/SummerJam/Insert", request);
            if (insertResp != null && insertResp.Id != 0)
            {
                model.StateName = model.State;
                model.Success = true;
            }
        }

        model = await PrepareSummerJamModelAsync(model);
        return View(model);
    }
}

public class FMSummerJamSettingsDto
{
    public int DailyLineUpImageId { get; set; }
    public int DailyLineUpMobileImageId { get; set; }
    public int ViewPatioSetImageId { get; set; }
    public int ViewPatioSetMobileImageId { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
}

public class SummerJamInsertResponse
{
    public bool Success { get; set; }
    public int Id { get; set; }
}
