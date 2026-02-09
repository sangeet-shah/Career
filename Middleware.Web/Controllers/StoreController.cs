using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Services.Stores;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    /// <summary>
    /// Get current store
    /// </summary>
    [HttpGet("GetCurrentStore")]
    public async Task<IActionResult> GetCurrentStore()
    {
        var store = await _storeService.GetCurrentStoreAsync();
        return Ok(store);
    }
}
