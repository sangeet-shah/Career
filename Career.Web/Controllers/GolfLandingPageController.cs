using Career.Data.Domains.LandingPages;
using Career.Data.Services.Common;
using Career.Data.Services.GolfLanding;
using Career.Data.Services.Media;
using Career.Data.Services.Messages;
using Career.Data.Services.Security;
using Career.Data.Services.Settings;
using Career.Data.Services.Stores;
using Career.Web.Models.GolfLanding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class GolfLandingPageController : BaseController
{
    #region Fields

    private readonly IGolfLandingPageService _golfLandingPageService;
    private readonly ICommonService _commonService;
    private readonly ISettingService _settingService;
    private readonly IWorkflowMessageService _workflowMessageService;
    private readonly IPermissionService _permissionService;
    private readonly IPictureService _pictureService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public GolfLandingPageController(IGolfLandingPageService golfLandingPageService,
        ICommonService commonService,
        ISettingService settingService,
        IWorkflowMessageService workflowMessageService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IWebHostEnvironment webHostEnvironment,
        IHttpContextAccessor httpContextAccessor,
        IStoreService storeService)
    {
        _golfLandingPageService = golfLandingPageService;
        _commonService = commonService;
        _settingService = settingService;
        _workflowMessageService = workflowMessageService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    private async Task PrepareGolfLandingPageModelAsync(GolfLandingPageModel model)
    {
        // sponsors list
        model.AvailableSponsorshipLevels.Insert(0, new SelectListItem { Text = "Select sponsorship level" });
        foreach (var sponsorshipLevel in Enum.GetValues(typeof(SponsorshipLevelEnum)).Cast<SponsorshipLevelEnum>())
            model.AvailableSponsorshipLevels.Add(new SelectListItem { Text = _commonService.GetEnumDescription(sponsorshipLevel), Value = ((int)sponsorshipLevel).ToString() });

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var _fmGolfEventLandingPageSettings = await _settingService.LoadSettingAsync<FMGolfEventLandingPageSettings>(storeId);
        model.IsActive = _fmGolfEventLandingPageSettings.Enabled;
        model.Description = _fmGolfEventLandingPageSettings.Description;        
    }

    #endregion

    #region Methods

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Sponsor()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var model = new GolfLandingPageModel();
        await PrepareGolfLandingPageModelAsync(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(GolfLandingPageModel model)
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        ModelState.Remove("PictureId");

        if (ModelState.IsValid)
        {
            var golfLandingPage = new GolfEventLandingPage()
            {
                CompanyName = model.CompanyName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                SponsorshipLevelId = model.SponsorshipLevelId,
                Contact1 = model.Contact1,
                Contact2 = model.Contact2,
                Contact3 = model.Contact3,
                Contact4 = model.Contact4,
                CreatedOnUtc = DateTime.UtcNow,
                PictureId = model.PictureId
            };
            await _golfLandingPageService.InsertGolfEventLandingPageAsync(golfLandingPage);

            var body = string.Empty;
            body += "<p><b>Company name: </b>" + golfLandingPage.CompanyName + "</p>";
            body += "<p><b>Main contact phone number: </b>" + golfLandingPage.PhoneNumber + "</p>";
            body += "<p><b>Main contact email: </b>" + golfLandingPage.Email + "</p>";
            body += "<p><b>Sponsorship level: </b>" + _commonService.GetEnumDescription((SponsorshipLevelEnum)golfLandingPage.SponsorshipLevelId) + "</p>";
            if (!string.IsNullOrEmpty(golfLandingPage.Contact1))
                body += "<p><b>Team member 1- main contact: </b>" + golfLandingPage.Contact1 + "</p>";
            if (!string.IsNullOrEmpty(golfLandingPage.Contact2))
                body += "<p><b>Team member 2: </b>" + golfLandingPage.Contact2 + "</p>";
            if (!string.IsNullOrEmpty(golfLandingPage.Contact3))
                body += "<p><b>Team member 3: </b>" + golfLandingPage.Contact3 + "</p>";
            if (!string.IsNullOrEmpty(golfLandingPage.Contact4))
                body += "<p><b>Team member 4: </b>" + golfLandingPage.Contact4 + "</p>";
            body += "<p><b>Created on: </b>" + _commonService.ConvertToUserTime(golfLandingPage.CreatedOnUtc, DateTimeKind.Utc) + "</p>";

            var picture = await _pictureService.GetPictureByIdAsync(golfLandingPage.PictureId);
            if (picture != null)
            {
                var pictureBinary = (await _pictureService.GetPictureBinaryByPictureIdAsync(golfLandingPage.PictureId)).BinaryData;
                var folderPath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "images\\golf-regisration");
                if (!System.IO.Directory.Exists(folderPath))
                    System.IO.Directory.CreateDirectory(folderPath);

                var fileName = picture.SeoFilename + "_" + picture.Id.ToString() + ".png";
                var base64img = System.Convert.ToBase64String(pictureBinary);
                System.IO.File.WriteAllBytes(Path.Combine(folderPath, fileName), Convert.FromBase64String(base64img));

                var httpContext = _httpContextAccessor.HttpContext.Request;
                var imageUrl = string.Format("{0}://{1}", httpContext.Scheme, httpContext.Host.Value) + "/images/golf-regisration/" + fileName;

                body += $"<p><b>Logo: </b> <img src=\"{imageUrl}\" height=100px width=100px /> </p>";
            }


            var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
            var _fmGolfEventLandingPageSettings = await _settingService.LoadSettingAsync<FMGolfEventLandingPageSettings>(storeId);
            if (!string.IsNullOrEmpty(_fmGolfEventLandingPageSettings.Emails))
                await _workflowMessageService.SendNotificationAsync(subject: "Golf landing page notification", body: body, toEmailAddress: _fmGolfEventLandingPageSettings.Emails, fromEmail: model.Email);
            model.Success = true;
        }

        await PrepareGolfLandingPageModelAsync(model);
        return View(model);
    }

    #endregion
}
