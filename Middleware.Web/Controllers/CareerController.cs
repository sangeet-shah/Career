using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Services.Career;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CareerController : ControllerBase
{
    private readonly ICareerService _careerService;

    public CareerController(ICareerService careerService)
    {
        _careerService = careerService;
    }

    /// <summary>
    /// Get all departments
    /// </summary>
    [HttpGet("GetAllDepartments")]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departments = await _careerService.GetAllDepartmentAsync();
        return Ok(departments);
    }

    /// <summary>
    /// Get all corporate brand pages
    /// </summary>
    [HttpGet("GetAllCorporateBrandPages")]
    public async Task<IActionResult> GetAllCorporateBrandPages()
    {
        var brands = await _careerService.GetAllCorporateBrandPagesAsync();
        return Ok(brands);
    }
}
