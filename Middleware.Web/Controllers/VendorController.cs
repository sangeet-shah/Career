using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Services.Vendors;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet("GetAllVendors")]
    public async Task<IActionResult> GetAllVendors()
    {
        var vendors = await _vendorService.GetAllVendorsAsync();
        return Ok(vendors);
    }
}
