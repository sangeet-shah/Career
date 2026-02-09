using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Domains.Seo;
using Middleware.Web.Filters;
using Middleware.Web.Models.Seo;
using Middleware.Web.Services.Seo;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class SeoController : ControllerBase
{
    private readonly IUrlRecordService _urlRecordService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;

    public SeoController(IUrlRecordService urlRecordService,
        ISettingService settingService,
        IStoreService storeService)
    {
        _urlRecordService = urlRecordService;
        _settingService = settingService;
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBySlug([FromQuery] string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return BadRequest();

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var record = await _urlRecordService.GetBySlugAsync(slug, storeId);
        if (record == null)
            return Ok((UrlRecordResponse)null);

        return Ok(new UrlRecordResponse
        {
            Id = record.Id,
            EntityId = record.EntityId,
            EntityName = record.EntityName,
            Slug = record.Slug,
            IsActive = record.IsActive,
            LanguageId = record.LanguageId
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetWwwRequirement()
    {
        var store = await _storeService.GetCurrentStoreAsync();
        var storeId = store?.Id ?? 0;
        var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeId);
        return Ok(new WwwRequirementResponse
        {
            StoreId = storeId,
            WwwRequirement = (int)seoSettings.WwwRequirement
        });
    }
}
