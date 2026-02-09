using Middleware.Web.Domains.Career;
using Middleware.Web.Domains.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Middleware.Web.Filters;
using Middleware.Web.Infrastructure;
using Middleware.Web.Models.Career;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Directory;
using Middleware.Web.Services.LandingPages;
using Middleware.Web.Services.Locations;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.PaycorAPI;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using System.Xml;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class CareerController : ControllerBase
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IPictureService _pictureService;
    private readonly ICommonService _commonService;
    private readonly ILocationService _locationService;
    private readonly IPaycorAPIService _paycoreAPIService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public CareerController(ISettingService settingService,
        IPictureService pictureService,
        ICommonService commonService,
        IPaycorAPIService paycoreAPIService,
        ILocationService locationService,
        IStateProvinceService stateProvinceService,
        IStoreService storeService)
    {
        _settingService = settingService;
        _pictureService = pictureService;
        _commonService = commonService;
        _paycoreAPIService = paycoreAPIService;
        _locationService = locationService;
        _stateProvinceService = stateProvinceService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] CareerSearchModel searchModel)
    {
        if (searchModel.Page < 1)
            searchModel.Page = 1;
        if (searchModel.PageSize < 1)
            searchModel.PageSize = 10;

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var carrierSettings = await _settingService.LoadSettingAsync<CareerSettings>(storeId);
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
            return Ok(searchModel);

        // States
        var allStates = await _stateProvinceService.GetAllStateProvincesAsync();
        var states = jobs.Where(x => !string.IsNullOrEmpty(x.AtsLocation.State)).Select(x => x.AtsLocation.State).Distinct().OrderBy(x => x).ToList();
        if (states != null)
            searchModel.AvailableStates = states.Select(state => new SelectListItem { Text = allStates.Where(s => s.Abbreviation.ToLower() == state.ToLower()).Select(s => s.Name).FirstOrDefault(), Value = state.ToLower() }).ToList();

        // Cities            
        var cities = jobs.Where(x => !string.IsNullOrEmpty(x.AtsLocation.City)).Select(x => x.AtsLocation.City).Distinct().OrderBy(x => x).ToList();
        if (cities != null)
            searchModel.AvailableCities = cities.Select(c => new SelectListItem { Text = c, Value = c.ToLower() }).ToList();

        // Job categories
        var jobCategories = jobs.Where(x => x.AtsDepartment != null && !string.IsNullOrEmpty(x.AtsDepartment.Title)).Select(x => x.AtsDepartment).GroupBy(x => x.Title).Select(x => x.First()).OrderBy(x => x.Title).ToList();
        if (jobCategories != null)
            searchModel.AvailableJobCategories = jobCategories.Select(jobCategory => new SelectListItem { Text = jobCategory.Title, Value = jobCategory.Id.ToString() }).ToList();

        jobs = await _paycoreAPIService.GetAllJobsAsync(selectedStates: searchModel.SelectedStates, selectedCities: searchModel.SelectedCities,
            selectedJobCategories: searchModel.SelectedJobCategories, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        searchModel.CarrerList = await Task.WhenAll(jobs.Select(async job =>new CareerDetailModel
        {
            JobId = job.Id,
            JobTitle = job.Title,
            JobCategory = job.AtsDepartment?.Title,
            State = job.AtsLocation?.State,
            City = job.AtsLocation?.City,
            WebsiteId = job.AtsLocation != null
            ? (await _locationService
                .GetLocationByUKGGuidIdAsync(job.AtsLocation.Id))?.WebsiteId ?? 0
            : 0,
            DatePostedString = job.CreatedDate.ToString("MM/dd/yyyy")
        }));

        searchModel.TotalCount = jobs.TotalCount;
        searchModel.TotalPages = jobs.TotalPages;
        searchModel.Page = searchModel.Page == 0 ? 1 : searchModel.Page;

        return Ok(searchModel);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> CareerDetail(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var jobs = await _paycoreAPIService.GetAllJobsAsync();
        if (jobs == null || jobs.Count == 0)
            return NotFound();

        var job = jobs.Where(j => j.Id == id).FirstOrDefault();
        if (job == null)
            return NotFound();

        var model = new CareerJobDetailModel
        {
            JobTitle = job.Title,
            JobCategory = job.AtsDepartment.Title,
            JobSummaryContent = job.Description,
            PostedDate = job.CreatedDate,
            JobId = job.Id,
            Type = job.PayRange.PayPeriod != "hour" ? "Full Time" : "Part Time"
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

        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> SitemapXml([FromQuery] string? baseUrl = null)
    {
        var hostUrl = string.IsNullOrWhiteSpace(baseUrl)
            ? $"{Request.Scheme}://{Request.Host.Value}"
            : baseUrl!.TrimEnd('/');

        string Normalize(string path) => $"{hostUrl}{path}";

        using var stream = new System.IO.MemoryStream();
        using var writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8)
        {
            Formatting = Formatting.Indented
        };
        writer.WriteStartDocument();
        writer.WriteStartElement("urlset");
        writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

        void WriteUrlLocation(string url)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", Helpers.XmlHelper.XmlEncode(url));
            writer.WriteEndElement();
        }

        // static pages
        WriteUrlLocation(Normalize("/ourstory/"));
        WriteUrlLocation(Normalize("/careers/"));
        WriteUrlLocation(Normalize("/brands/"));
        WriteUrlLocation(Normalize("/locations/"));
        WriteUrlLocation(Normalize("/news/"));
        WriteUrlLocation(Normalize("/privacy-policy/"));
        WriteUrlLocation(Normalize("/terms-conditions/"));
        WriteUrlLocation(Normalize("/conditions-of-use/"));
        WriteUrlLocation(Normalize("/summerjam/"));
        WriteUrlLocation(Normalize("/golf/"));
        WriteUrlLocation(Normalize("/golf/sponsors/"));
        WriteUrlLocation(Normalize("/golf/register/"));
        WriteUrlLocation(Normalize("/ourteam/"));
        WriteUrlLocation("http://furnituremartusa.com");

        // job details
        var jobs = await _paycoreAPIService.GetAllJobsAsync();
        if (jobs != null)
        {
            foreach (var opportunity in jobs)
                WriteUrlLocation(Normalize($"/careers/careerdetail/?id={opportunity.Id}"));
        }

        // physical store details
        var physicalStores = await _locationService.GetLocationsAsync(((int)WebsiteEnum.FMUSA));
        foreach (var physicalStore in physicalStores)
            WriteUrlLocation(Normalize($"/locations/{physicalStore.LocationId}/"));

        // articles
        var blogService = HttpContext.RequestServices.GetRequiredService<Middleware.Web.Services.Blogs.IBlogService>();
        var urlRecordService = HttpContext.RequestServices.GetRequiredService<Middleware.Web.Services.Seo.IUrlRecordService>();
        var storeService = HttpContext.RequestServices.GetRequiredService<Middleware.Web.Services.Stores.IStoreService>();
        var blogPosts = await blogService.GetBlogPostsAsync(storeId: (await storeService.GetCurrentStoreAsync())?.Id ?? 0);
        foreach (var blogPost in blogPosts)
        {
            var seName = await urlRecordService.GetSeNameAsync(blogPost.Id, nameof(Middleware.Web.Domains.Blogs.BlogPost));
            if (!string.IsNullOrEmpty(seName))
                seName = seName.Replace("inspiration/", "news/");
            if (!string.IsNullOrEmpty(seName))
                WriteUrlLocation(Normalize("/" + seName));
        }

        // contest pages
        var landingPageService = HttpContext.RequestServices.GetRequiredService<ILandingPageService>();
        var commonService = _commonService;
        var urlRecords = await urlRecordService.GetSlugsAsync(nameof(Middleware.Web.Domains.LandingPages.LandingPage), (await storeService.GetCurrentStoreAsync())?.Id ?? 0);
        foreach (var urlRecord in urlRecords)
        {
            try
            {
                var contest = await landingPageService.GetlandingPageByIdAsync(urlRecord.EntityId);
                var currentDate = commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
                if (contest == null)
                    continue;
                if (!contest.Published)
                    continue;
                if (contest.StartDateUtc != null && contest.StartDateUtc >= currentDate)
                    continue;
                if (contest.EndDateUtc != null && contest.EndDateUtc <= currentDate)
                    continue;

                WriteUrlLocation(Normalize("/" + urlRecord.Slug));
            }
            catch
            {
                continue;
            }
        }

        // our team bios
        var customerService = HttpContext.RequestServices.GetRequiredService<Middleware.Web.Services.Customers.ICustomerService>();      
        var customers = await customerService.GetCustomersByRoleAsync(CustomerRoleDefaults.OURTEAM_ROLE_NAME);
        var customerList = await Task.WhenAll(customers.Select(async x => new Middleware.Web.Models.Customers.CustomerModel
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            DisplayOrder = (await customerService
            .GetFMCustomersByCustomerIdAsync(x.Id))?.DisplayOrder ?? 0
        }));

        foreach (var author in customerList)
        {
            var seName = await urlRecordService.GetSeNameAsync(author.Id, nameof(Middleware.Web.Domains.Customers.Customer));
            if (string.IsNullOrEmpty(seName))
                continue;
            WriteUrlLocation(Normalize("/ourteam/" + seName.TrimEnd('/') + "/"));
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Flush();

        var xml = System.Text.Encoding.UTF8.GetString(stream.ToArray());
        return Ok(xml);
    }

    #endregion
}
