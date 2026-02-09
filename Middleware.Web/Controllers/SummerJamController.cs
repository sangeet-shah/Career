using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Domains.LandingPages;
using Middleware.Web.Services.SummerJams;
using System;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummerJamController : ControllerBase
{
    private readonly ISummerJamService _summerJamService;

    public SummerJamController(ISummerJamService summerJamService)
    {
        _summerJamService = summerJamService;
    }

    [HttpPost("Insert")]
    public async Task<IActionResult> Insert([FromBody] SummerJamInsertRequest request)
    {
        if (request == null)
            return BadRequest();

        var summerJam = new SummerJam
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Address1 = request.Address1,
            Address2 = request.Address2,
            City = request.City,
            StateProvinceId = request.StateProvinceId,
            Phone = request.Phone,
            ZipCode = request.ZipCode,
            DOB = request.DOB,
            CreatedDateUtc = request.CreatedDateUtc,
            StoreId = request.StoreId
        };
        await _summerJamService.InsertSummerJamAsync(summerJam);
        return Ok(new { Success = true, Id = summerJam.Id });
    }
}

public class SummerJamInsertRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public int StateProvinceId { get; set; }
    public string ZipCode { get; set; }
    public string Phone { get; set; }
    public DateTime? DOB { get; set; }
    public DateTime CreatedDateUtc { get; set; }
    public int StoreId { get; set; }
}
