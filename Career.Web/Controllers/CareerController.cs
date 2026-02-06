using Career.Data.Domains.Career;
using Career.Data.Extensions;
using Career.Data.Services.Common;
using Career.Data.Services.Directory;
using Career.Data.Services.Locations;
using Career.Data.Services.Media;
using Career.Data.Services.PaycorAPI;
using Career.Data.Services.Security;
using Career.Data.Services.Settings;
using Career.Web.Models.Career;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Career.Web.Controllers;

public class CareerController : BaseController
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IPictureService _pictureService;
    private readonly ICommonService _commonService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocationService _locationService;
    private readonly SitemapGenerator _sitemapGenerator;
    private readonly IPermissionService _permissionService;
    private readonly IPaycorAPIService _paycoreAPIService;
    private readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public CareerController(ISettingService settingService,
        IPictureService pictureService,
        ICommonService commonService,
        IHttpContextAccessor httpContextAccessor,
        SitemapGenerator sitemapGenerator,
        IPermissionService permissionService,
        IPaycorAPIService paycoreAPIService,
        ILocationService locationService,
        IStateProvinceService stateProvinceService)
    {
        _settingService = settingService;
        _pictureService = pictureService;
        _commonService = commonService;
        _httpContextAccessor = httpContextAccessor;
        _sitemapGenerator = sitemapGenerator;
        _permissionService = permissionService;
        _paycoreAPIService = paycoreAPIService;
        _locationService = locationService;
        _stateProvinceService = stateProvinceService;
    }

    #endregion

    #region Utilities

    public static Dictionary<string, string> ParseQueryString(String query)
    {
        var queryDict = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(query))
            return queryDict;

        var tokens = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (String token in tokens)
        {
            var parts = token.Split('=', 2); // split only once
            var key = parts.Length > 0 ? parts[0].Trim() : "";
            var value = parts.Length > 1 ? HttpUtility.UrlDecode(parts[1]).Trim() : "";

            // avoid empty key like "?="
            if (!string.IsNullOrEmpty(key))
            {
                queryDict[key] = value;
            }
        }
        return queryDict;
    }

    #endregion

    #region Method

    public async Task<IActionResult> List(CareerSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var carrierSettings = await _settingService.LoadSettingAsync<CareerSettings>();
        searchModel.TopImage1 = carrierSettings.TopImage1Id;
        searchModel.TopImage1Url = await _pictureService.GetPictureUrlAsync(carrierSettings.TopImage1Id, showDefaultPicture: false);
        searchModel.TopImage1AltText = carrierSettings.TopImage1Alt;
        searchModel.TopImage1Title = carrierSettings.TopImage1Title;
        searchModel.TopImage2 = carrierSettings.TopImage2Id;
        searchModel.TopImage2Url = await _pictureService.GetPictureUrlAsync(carrierSettings.TopImage2Id, showDefaultPicture: false);
        searchModel.TopImage2AltText = carrierSettings.TopImage2Alt;
        searchModel.TopImage2Title = carrierSettings.TopImage2Title;
        searchModel.TopImage3 = carrierSettings.TopImage3Id;
        searchModel.TopImage3Url = await _pictureService.GetPictureUrlAsync(carrierSettings.TopImage3Id, showDefaultPicture: false);
        searchModel.TopImage3AltText = carrierSettings.TopImage3Alt;
        searchModel.TopImage3Title = carrierSettings.TopImage3Title;
        searchModel.TopDescription = carrierSettings.TopImageDescription;
        searchModel.Image = carrierSettings.WebImageId;
        searchModel.ImageUrl = await _pictureService.GetPictureUrlAsync(carrierSettings.WebImageId, showDefaultPicture: false);
        searchModel.AltText = carrierSettings.WebImageAlt;
        searchModel.MobileImage = carrierSettings.MobileImageId;
        searchModel.MobileImageUrl = await _pictureService.GetPictureUrlAsync(carrierSettings.MobileImageId, showDefaultPicture: false);
        searchModel.MobileAltText = carrierSettings.MobileImageAlt;
        searchModel.IsMobile = _commonService.IsMobileDevice();

        // state filter
        if (!string.IsNullOrEmpty(searchModel.State) && searchModel.State != "0")
        {
            searchModel.SelectedStates = searchModel.State.Split('~').ToArray();
            searchModel.State = string.Empty;
        }

        // city filter
        if (!string.IsNullOrEmpty(searchModel.City) && searchModel.City != "0")
        {
            searchModel.SelectedCities = searchModel.City.Split('~').ToArray();
            searchModel.City = string.Empty;
        }

        // job category filter
        if (!string.IsNullOrEmpty(searchModel.JobCategory) && searchModel.JobCategory != "0")
        {
            searchModel.SelectedJobCategories = searchModel.JobCategory.Split('~').ToArray();
            searchModel.JobCategory = string.Empty;
        }

        // get jobs from api request only for filter to get all filter value
        var jobs = await _paycoreAPIService.GetAllJobsAsync();
        if (jobs == null || jobs.Count == 0)
            return View(searchModel);

        // States
        var allStates = await _stateProvinceService.GetAllStateProvincesAsync();
        var states = jobs.Where(x => !string.IsNullOrEmpty(x.AtsLocation.State)).Select(x => x.AtsLocation.State).Distinct().OrderBy(x => x).ToList();
        if (states != null)
            searchModel.AvailableStates = states.Select(state => new SelectListItem { Text = allStates.Where(s => s.Abbreviation.ToLower() == state.ToLower()).Select(s => s.Name).FirstOrDefault(), Value = state.ToLower() }).ToList();
        if (searchModel.IsMobile)
            searchModel.AvailableStates.Insert(0, new SelectListItem { Text = "State", Value = "0" });

        // Cities            
        var cities = jobs.Where(x => !string.IsNullOrEmpty(x.AtsLocation.City)).Select(x => x.AtsLocation.City).Distinct().OrderBy(x => x).ToList();
        if (cities != null)
            searchModel.AvailableCities = cities.Select(c => new SelectListItem { Text = c, Value = c.ToLower() }).ToList();
        if (searchModel.IsMobile)
            searchModel.AvailableCities.Insert(0, new SelectListItem { Text = "City", Value = "0" });

        // Job categories
        var jobCategories = jobs.Where(x => x.AtsDepartment != null && !string.IsNullOrEmpty(x.AtsDepartment.Title)).Select(x => x.AtsDepartment).GroupBy(x => x.Title).Select(x => x.First()).OrderBy(x => x.Title).ToList();
        if (jobCategories != null)
            searchModel.AvailableJobCategories = jobCategories.Select(jobCategory => new SelectListItem { Text = jobCategory.Title, Value = jobCategory.Id.ToString() }).ToList();
        if (searchModel.IsMobile)
            searchModel.AvailableJobCategories.Insert(0, new SelectListItem { Text = "Job Category", Value = "0" });

        searchModel.Parameters = ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value);
        jobs = await _paycoreAPIService.GetAllJobsAsync(selectedStates: searchModel.SelectedStates, selectedCities: searchModel.SelectedCities,
            selectedJobCategories: searchModel.SelectedJobCategories, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        searchModel.CarrerList = await jobs.SelectAwait(async job => new CareerDetailModel
        {
            JobId = job.Id,
            JobTitle = job.Title,
            JobCategory = job.AtsDepartment.Title,
            State = job.AtsLocation.State,
            City = job.AtsLocation.City,
            WebsiteId = job.AtsLocation != null ? (await _locationService.GetLocationByUKGGuidIdAsync(job.AtsLocation.Id))?.WebsiteId ?? 0 : 0,
            DatePostedString = job.CreatedDate.ToString("MM/dd/yyyy")
        }).ToListAsync();
        searchModel.TotalCount = jobs.TotalCount;
        searchModel.TotalPages = jobs.TotalPages;
        searchModel.Page = searchModel.Page == 0 ? 1 : searchModel.Page;

        return View(searchModel);
    }

    public async Task<IActionResult> CareerDetail(string id)
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        if (string.IsNullOrEmpty(id))
            return InvokeHttp404();

        var jobs = await _paycoreAPIService.GetAllJobsAsync();
        if (jobs == null || jobs.Count == 0)
            return InvokeHttp404();

        var job = jobs.Where(j => j.Id == id).FirstOrDefault();
        if (job == null)
            return InvokeHttp404();

        var model = new CareerJobDetailModel
        {
            JobTitle = job.Title,
            JobCategory = job.AtsDepartment.Title,
            JobSummaryContent = job.Description,
            PostedDate = job.CreatedDate,
            JobId = job.Id,
            //ExpirationDate = carrerJobDetail.closed_date.HasValue ? carrerJobDetail.closed_date.Value : (DateTime?)null,
            //BaseSalaryAmount = job.PayRange != null && job.PayRange.Minimum > 0 ? Convert.ToDecimal(job.PayRange.Minimum) : decimal.Zero,
            IsMobile = _commonService.IsMobileDevice()
        };

        // store location detail
        if (job.AtsLocation != null)
        {
            model.City = job.AtsLocation.City;
            model.State = job.AtsLocation.State;
            model.StoreName = job.AtsLocation.Address1;
            model.Address = job.AtsLocation.Address2;
            model.ZipCode = job.AtsLocation.PostalCode;
        }

        var httpContext = _httpContextAccessor.HttpContext.Request;
        model.baseURL = string.Format("{0}://{1}", httpContext.Scheme, httpContext.Host.Value);
        model.CurrentPageURL = string.Format("{0}://{1}{2}{3}", httpContext.Scheme, httpContext.Host.Value, httpContext.Path, httpContext.QueryString);
        model.Type = job.PayRange.PayPeriod != "hour" ? "Full Time" : "Part Time";

        return View(model);
    }

    public async Task<IActionResult> SitemapXml()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var siteMap = await _sitemapGenerator.GenerateAsync(this.Url);
        if (siteMap != null)
            return Content(siteMap, "text/xml");

        return View();
    }

    #endregion
}
