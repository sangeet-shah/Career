using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Domains.LandingPages;
using Middleware.Web.Services.GolfLanding;
using Middleware.Web.Services.Messages;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using System;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GolfLandingPageController : ControllerBase
{
    private readonly IGolfLandingPageService _golfLandingPageService;
    private readonly IWorkflowMessageService _workflowMessageService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;

    public GolfLandingPageController(
        IGolfLandingPageService golfLandingPageService,
        IWorkflowMessageService workflowMessageService,
        ISettingService settingService,
        IStoreService storeService)
    {
        _golfLandingPageService = golfLandingPageService;
        _workflowMessageService = workflowMessageService;
        _settingService = settingService;
        _storeService = storeService;
    }

    [HttpPost("Insert")]
    public async Task<IActionResult> Insert([FromBody] GolfEventLandingPageRequest request)
    {
        if (request == null)
            return BadRequest();

        var golfLandingPage = new GolfEventLandingPage
        {
            CompanyName = request.CompanyName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            SponsorshipLevelId = request.SponsorshipLevelId,
            Contact1 = request.Contact1,
            Contact2 = request.Contact2,
            Contact3 = request.Contact3,
            Contact4 = request.Contact4,
            CreatedOnUtc = request.CreatedOnUtc,
            PictureId = request.PictureId
        };
        await _golfLandingPageService.InsertGolfEventLandingPageAsync(golfLandingPage);

        var store = await _storeService.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<FMGolfEventLandingPageSettings>(store?.Id ?? 0);
        if (settings != null && !string.IsNullOrEmpty(settings.Emails))
        {
            var body = BuildEmailBody(request);
            await _workflowMessageService.SendNotificationAsync("Golf landing page notification", body, settings.Emails, request.Email);
        }

        return Ok(new { Success = true, Id = golfLandingPage.Id });
    }

    private static string BuildEmailBody(GolfEventLandingPageRequest r)
    {
        var body = $"<p><b>Company name: </b>{r.CompanyName}</p>";
        body += $"<p><b>Main contact phone number: </b>{r.PhoneNumber}</p>";
        body += $"<p><b>Main contact email: </b>{r.Email}</p>";
        body += $"<p><b>Sponsorship level: </b>{r.SponsorshipLevelId}</p>";
        if (!string.IsNullOrEmpty(r.Contact1)) body += $"<p><b>Team member 1: </b>{r.Contact1}</p>";
        if (!string.IsNullOrEmpty(r.Contact2)) body += $"<p><b>Team member 2: </b>{r.Contact2}</p>";
        if (!string.IsNullOrEmpty(r.Contact3)) body += $"<p><b>Team member 3: </b>{r.Contact3}</p>";
        if (!string.IsNullOrEmpty(r.Contact4)) body += $"<p><b>Team member 4: </b>{r.Contact4}</p>";
        body += $"<p><b>Created on: </b>{r.CreatedOnUtc}</p>";
        return body;
    }
}

public class GolfEventLandingPageRequest
{
    public string CompanyName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public int SponsorshipLevelId { get; set; }
    public string Contact1 { get; set; }
    public string Contact2 { get; set; }
    public string Contact3 { get; set; }
    public string Contact4 { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public int PictureId { get; set; }
}
