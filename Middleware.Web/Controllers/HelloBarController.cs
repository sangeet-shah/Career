using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Services.HelloBar;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloBarController : ControllerBase
{
    private readonly IHelloBarService _helloBarService;

    public HelloBarController(IHelloBarService helloBarService)
    {
        _helloBarService = helloBarService;
    }

    /// <summary>
    /// Get active hello bars
    /// </summary>
    [HttpGet("GetActiveHelloBars")]
    public async Task<IActionResult> GetActiveHelloBars()
    {
        var helloBars = await _helloBarService.GetActiveHelloBarsAsync();
        return Ok(helloBars);
    }
}
