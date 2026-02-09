using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Services.Stores;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<IActionResult> CurrentId()
    {
        var store = await _storeService.GetCurrentStoreAsync();
        return Ok(new { storeId = store?.Id ?? 0 });
    }
}
