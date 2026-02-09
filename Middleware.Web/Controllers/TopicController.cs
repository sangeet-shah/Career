using Middleware.Web.Domains.LegalPages;
using Middleware.Web.Infrastructure;
using Middleware.Web.Models.Legal;
using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Localization;
using Middleware.Web.Services.Stores;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class TopicController : ControllerBase
{
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly IStoreService _storeService;

    public TopicController(
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IStoreService storeService)
    {
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<IActionResult> PrivacyPolicy()
    {
        var model = new LegalModel
        {
            Title = await _localizationService.GetLocaleStringResourceByNameAsync(string.Format(ContentManagementDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.PrivacyPolicy.ToString())),
            Body = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.PrivacyPolicy }, ContentManagementDefaults.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0),
            SeName = "privacy-policy"
        };

        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> TermsConditions()
    {
        var model = new LegalModel
        {
            Title = await _localizationService.GetLocaleStringResourceByNameAsync(string.Format(ContentManagementDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.TermsAndConditions.ToString())),
            Body = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.TermsAndConditions }, ContentManagementDefaults.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0),
            SeName = "terms-conditions"
        };

        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> TermsOfUse()
    {
        var model = new LegalModel
        {
            Title = await _localizationService.GetLocaleStringResourceByNameAsync(string.Format(ContentManagementDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.TermsOfUse.ToString())),
            Body = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.TermsOfUse }, ContentManagementDefaults.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0),
            SeName = "conditions-of-use"
        };

        return Ok(model);
    }
}
