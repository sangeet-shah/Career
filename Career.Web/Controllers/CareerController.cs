using Career.Web.Infrastructure;
using Career.Web.Models.Api;
using Career.Web.Models.Career;
using Career.Web.Services.ApiClient;
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
    private readonly IApiClient _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAgentHelper _userAgentHelper;

    public CareerController(IApiClient apiClient, IHttpContextAccessor httpContextAccessor,
        IUserAgentHelper userAgentHelper)
    {
        _apiClient = apiClient;
        _httpContextAccessor = httpContextAccessor;
        _userAgentHelper = userAgentHelper;

    }

    public static Dictionary<string, string> ParseQueryString(string query)
    {
        var queryDict = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(query))
            return queryDict;

        var tokens = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            var parts = token.Split('=', 2);
            var key = parts.Length > 0 ? parts[0].Trim() : "";
            var value = parts.Length > 1 ? HttpUtility.UrlDecode(parts[1]).Trim() : "";
            if (!string.IsNullOrEmpty(key))
                queryDict[key] = value;
        }
        return queryDict;
    }

    public async Task<IActionResult> List(CareerSearchModel searchModel)
    {
        var carrierSettings = await _apiClient.GetAsync<CareerSettingsDto>("api/Setting/GetCareerSettings");
        if (carrierSettings == null)
            return View(searchModel);

        searchModel.TopImage1 = carrierSettings.TopImage1Id;
        searchModel.TopImage1Url = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = carrierSettings.TopImage1Id, showDefaultPicture = false }))?.Url;
        searchModel.TopImage1AltText = carrierSettings.TopImage1Alt;
        searchModel.TopImage1Title = carrierSettings.TopImage1Title;
        searchModel.TopImage2 = carrierSettings.TopImage2Id;
        searchModel.TopImage2Url = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = carrierSettings.TopImage2Id, showDefaultPicture = false }))?.Url;
        searchModel.TopImage2AltText = carrierSettings.TopImage2Alt;
        searchModel.TopImage2Title = carrierSettings.TopImage2Title;
        searchModel.TopImage3 = carrierSettings.TopImage3Id;
        searchModel.TopImage3Url = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = carrierSettings.TopImage3Id, showDefaultPicture = false }))?.Url;
        searchModel.TopImage3AltText = carrierSettings.TopImage3Alt;
        searchModel.TopImage3Title = carrierSettings.TopImage3Title;
        searchModel.TopDescription = carrierSettings.TopImageDescription;
        searchModel.Image = carrierSettings.WebImageId;
        searchModel.ImageUrl = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = carrierSettings.WebImageId, showDefaultPicture = false }))?.Url;
        searchModel.AltText = carrierSettings.WebImageAlt;
        searchModel.MobileImage = carrierSettings.MobileImageId;
        searchModel.MobileImageUrl = (await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = carrierSettings.MobileImageId, showDefaultPicture = false }))?.Url;
        searchModel.MobileAltText = carrierSettings.MobileImageAlt;

        var isMobileResp = _userAgentHelper.IsMobileDevice();
        searchModel.IsMobile = isMobileResp;

        if (!string.IsNullOrEmpty(searchModel.State) && searchModel.State != "0")
        {
            searchModel.SelectedStates = searchModel.State.Split('~').ToArray();
            searchModel.State = string.Empty;
        }
        if (!string.IsNullOrEmpty(searchModel.City) && searchModel.City != "0")
        {
            searchModel.SelectedCities = searchModel.City.Split('~').ToArray();
            searchModel.City = string.Empty;
        }
        if (!string.IsNullOrEmpty(searchModel.JobCategory) && searchModel.JobCategory != "0")
        {
            searchModel.SelectedJobCategories = searchModel.JobCategory.Split('~').ToArray();
            searchModel.JobCategory = string.Empty;
        }

        var jobsResponse = await _apiClient.GetAsync<PaycorJobsResponse>("api/PaycorAPI/GetAllJobs");
        var jobs = jobsResponse?.Records;
        if (jobs == null || jobs.Count == 0)
            return View(searchModel);

        var allStates = await _apiClient.GetAsync<List<StateProvinceDto>>("api/StateProvince/GetAll");
        var states = jobs.Where(x => x.AtsLocation != null && !string.IsNullOrEmpty(x.AtsLocation.State)).Select(x => x.AtsLocation.State).Distinct().OrderBy(x => x).ToList();
        if (states != null)
            searchModel.AvailableStates = states.Select(state => new SelectListItem { Text = allStates?.FirstOrDefault(s => string.Equals(s.Abbreviation, state, StringComparison.OrdinalIgnoreCase))?.Name, Value = state.ToLower() }).ToList();
        if (searchModel.IsMobile)
            searchModel.AvailableStates?.Insert(0, new SelectListItem { Text = "State", Value = "0" });

        var cities = jobs.Where(x => x.AtsLocation != null && !string.IsNullOrEmpty(x.AtsLocation.City)).Select(x => x.AtsLocation.City).Distinct().OrderBy(x => x).ToList();
        if (cities != null)
            searchModel.AvailableCities = cities.Select(c => new SelectListItem { Text = c, Value = c.ToLower() }).ToList();
        if (searchModel.IsMobile)
            searchModel.AvailableCities?.Insert(0, new SelectListItem { Text = "City", Value = "0" });

        var jobCategories = jobs.Where(x => x.AtsDepartment != null && !string.IsNullOrEmpty(x.AtsDepartment.Title)).Select(x => x.AtsDepartment).GroupBy(x => x.Title).Select(x => x.First()).OrderBy(x => x.Title).ToList();
        if (jobCategories != null)
            searchModel.AvailableJobCategories = jobCategories.Select(jc => new SelectListItem { Text = jc.Title, Value = jc.Id }).ToList();
        if (searchModel.IsMobile)
            searchModel.AvailableJobCategories?.Insert(0, new SelectListItem { Text = "Job Category", Value = "0" });

        searchModel.Parameters = ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value ?? "");

        var filteredJobs = jobs;
        if (searchModel.SelectedStates != null && searchModel.SelectedStates.Count > 0 && !searchModel.SelectedStates.Contains("0"))
        {
            var stateSet = new HashSet<string>(searchModel.SelectedStates.Select(s => s.ToLowerInvariant()));
            filteredJobs = filteredJobs
                .Where(j => j.AtsLocation != null && !string.IsNullOrEmpty(j.AtsLocation.State) && stateSet.Contains(j.AtsLocation.State.ToLowerInvariant()))
                .ToList();
        }
        if (searchModel.SelectedCities != null && searchModel.SelectedCities.Count > 0 && !searchModel.SelectedCities.Contains("0"))
        {
            var citySet = new HashSet<string>(searchModel.SelectedCities.Select(s => s.ToLowerInvariant()));
            filteredJobs = filteredJobs
                .Where(j => j.AtsLocation != null && !string.IsNullOrEmpty(j.AtsLocation.City) && citySet.Contains(j.AtsLocation.City.ToLowerInvariant()))
                .ToList();
        }
        if (searchModel.SelectedJobCategories != null && searchModel.SelectedJobCategories.Count > 0 && !searchModel.SelectedJobCategories.Contains("0"))
        {
            var categorySet = new HashSet<string>(searchModel.SelectedJobCategories);
            filteredJobs = filteredJobs
                .Where(j => j.AtsDepartment != null && !string.IsNullOrEmpty(j.AtsDepartment.Id) && categorySet.Contains(j.AtsDepartment.Id))
                .ToList();
        }

        var pageIndex = searchModel.Page > 0 ? searchModel.Page - 1 : 0;
        var pageSize = searchModel.PageSize > 0 ? searchModel.PageSize : 1;
        var totalCount = filteredJobs?.Count ?? 0;
        var totalPages = (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));
        jobs = filteredJobs?.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        if (jobs != null)
        {
            var list = new List<CareerDetailModel>();
            foreach (var job in jobs)
            {
                int websiteId = 0;
                if (job.AtsLocation != null && !string.IsNullOrEmpty(job.AtsLocation.Id))
                {
                    var loc = await _apiClient.GetAsync<LocationDto>("api/Location/GetByUKGGuidId", new { ukgGuidId = job.AtsLocation.Id });
                    websiteId = loc?.WebsiteId ?? 0;
                }
                list.Add(new CareerDetailModel
                {
                    JobId = job.Id,
                    JobTitle = job.Title,
                    JobCategory = job.AtsDepartment?.Title,
                    State = job.AtsLocation?.State,
                    City = job.AtsLocation?.City,
                    WebsiteId = websiteId,
                    DatePostedString = job.CreatedDate.ToString("MM/dd/yyyy")
                });
            }
            searchModel.CarrerList = list;
        }

        searchModel.TotalCount = totalCount;
        searchModel.TotalPages = totalPages;
        searchModel.Page = searchModel.Page == 0 ? 1 : searchModel.Page;

        return View(searchModel);
    }

    public async Task<IActionResult> CareerDetail(string id)
    {
        if (string.IsNullOrEmpty(id))
            return InvokeHttp404();

        var jobsResponse = await _apiClient.GetAsync<PaycorJobsResponse>("api/PaycorAPI/GetAllJobs");
        var jobs = jobsResponse?.Records;
        if (jobs == null || jobs.Count == 0)
            return InvokeHttp404();

        var job = jobs.FirstOrDefault(j => j.Id == id);
        if (job == null)
            return InvokeHttp404();

        var isMobileResp = _userAgentHelper.IsMobileDevice();
        var model = new CareerJobDetailModel
        {
            JobTitle = job.Title,
            JobCategory = job.AtsDepartment?.Title,
            JobSummaryContent = job.Description,
            PostedDate = job.CreatedDate,
            JobId = job.Id,
            IsMobile = isMobileResp
        };

        if (job.AtsLocation != null)
        {
            model.City = job.AtsLocation.City;
            model.State = job.AtsLocation.State;
            model.StoreName = job.AtsLocation.Address1;
            model.Address = job.AtsLocation.Address2;
            model.ZipCode = job.AtsLocation.PostalCode;
        }

        var httpContext = _httpContextAccessor.HttpContext.Request;
        model.baseURL = $"{httpContext.Scheme}://{httpContext.Host.Value}";
        model.CurrentPageURL = $"{httpContext.Scheme}://{httpContext.Host.Value}{httpContext.Path}{httpContext.QueryString}";
        model.Type = job.PayRange?.PayPeriod != "hour" ? "Full Time" : "Part Time";

        return View(model);
    }

    public async Task<IActionResult> SitemapXml()
    {
        var siteMap = await _apiClient.GetStringAsync("api/Sitemap/GetXml");
        if (!string.IsNullOrEmpty(siteMap))
            return Content(siteMap, "text/xml");
        return View();
    }
}
