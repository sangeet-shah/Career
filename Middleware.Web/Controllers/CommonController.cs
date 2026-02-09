using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Stores;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Stores;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.HelloBar;
using Middleware.Web.Services.OffersPromotions;
using Middleware.Web.Services.Advertisements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommonController : ControllerBase
{
    private readonly ICommonService _commonService;
    private readonly IStoreService _storeService;
    private readonly ISettingService _settingService;
    private readonly IHelloBarService _helloBarService;
    private readonly IOffersPromotionsService _offersPromotionsService;
    private readonly IAdvertisementService _advertisementService;
    private readonly IStaticCacheManager _staticCacheManager;

    public CommonController(
        ICommonService commonService,
        IStoreService storeService,
        ISettingService settingService,
        IHelloBarService helloBarService,
        IOffersPromotionsService offersPromotionsService,
        IAdvertisementService advertisementService,
        IStaticCacheManager staticCacheManager)
    {
        _commonService = commonService;
        _storeService = storeService;
        _settingService = settingService;
        _helloBarService = helloBarService;
        _offersPromotionsService = offersPromotionsService;
        _advertisementService = advertisementService;
        _staticCacheManager = staticCacheManager;
    }

    /// <summary>
    /// Get layout data for _Layout.cshtml
    /// </summary>
    [HttpGet("GetLayoutData")]
    public async Task<IActionResult> GetLayoutData()
    {
        var store = await _storeService.GetCurrentStoreAsync();
        var helloBars = await _helloBarService.GetActiveHelloBarsAsync();
        
        // You may need to adjust this based on your actual LayoutDataResponse model
        var layoutData = new
        {
            Store = store,
            HelloBars = helloBars
        };

        return Ok(layoutData);
    }

    /// <summary>
    /// Check if request is from mobile device
    /// </summary>
    [HttpGet("IsMobileDevice")]
    public IActionResult IsMobileDevice()
    {
        var isMobile = _commonService.IsMobileDevice();
        return Ok(new { IsMobile = isMobile });
    }

    /// <summary>
    /// Get enum description
    /// </summary>
    [HttpGet("GetEnumDescription")]
    public IActionResult GetEnumDescription([FromQuery] string enumTypeName, [FromQuery] string enumValue)
    {
        var enumType = Type.GetType(enumTypeName);
        if (enumType == null)
            return BadRequest($"Enum type {enumTypeName} not found");

        var enumVal = Enum.Parse(enumType, enumValue);
        var description = _commonService.GetEnumDescription((Enum)enumVal);
        return Ok(new { Description = description });
    }

    [HttpGet("ConvertToUserTime")]
    public IActionResult ConvertToUserTime([FromQuery] DateTime dateTime, [FromQuery] string sourceDateTimeKind = "Utc")
    {
        var kind = sourceDateTimeKind.Equals("Local", StringComparison.OrdinalIgnoreCase) ? DateTimeKind.Local : DateTimeKind.Utc;
        var result = _commonService.ConvertToUserTime(dateTime, kind);
        return Ok(new { Result = result });
    }

    /// <summary>
    /// Clear cache. Requires apiKey (body or XApiKey header) to match FMStoreSettings.ApiGatewayKey for the given storeId.
    /// </summary>
    [HttpPost("ClearCache")]
    public async Task<IActionResult> ClearCache([FromBody] ClearCacheRequest body = null)
    {
        var storeId = body?.StoreId ?? 0;
        var key = body?.ApiKey ?? Request.Headers["XApiKey"].FirstOrDefault();
        if (string.IsNullOrEmpty(key))
            return Unauthorized("Missing API key.");

        var fmStoreSettings = await _settingService.LoadSettingAsync<FMStoreSettings>(storeId);
        if (fmStoreSettings?.ApiGatewayKey == null || !fmStoreSettings.ApiGatewayKey.Equals(key, StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Invalid API key.");

        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.PatternCacheKey);
        await _staticCacheManager.ClearAsync();
        return Ok();
    }
}

public class ClearCacheRequest
{
    public int StoreId { get; set; }
    public string ApiKey { get; set; }
}
