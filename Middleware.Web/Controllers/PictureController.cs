using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Services.Media;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PictureController : ControllerBase
{
    private readonly IPictureService _pictureService;

    public PictureController(IPictureService pictureService)
    {
        _pictureService = pictureService;
    }

    /// <summary>
    /// Get picture by ID
    /// </summary>
    [HttpGet("GetById")]
    public async Task<IActionResult> GetById([FromQuery] int pictureId)
    {
        var picture = await _pictureService.GetPictureByIdAsync(pictureId);
        if (picture == null)
            return NotFound();
        return Ok(picture);
    }

    /// <summary>
    /// Get picture URL
    /// </summary>
    [HttpGet("GetPictureUrl")]
    public async Task<IActionResult> GetPictureUrl(
        [FromQuery] int pictureId,
        [FromQuery] int targetSize = 0,
        [FromQuery] bool showDefaultPicture = true,
        [FromQuery] string storeLocation = null)
    {
        var url = await _pictureService.GetPictureUrlAsync(pictureId, targetSize, showDefaultPicture, storeLocation);
        return Ok(new { Url = url });
    }

    /// <summary>
    /// Upload picture (multipart form data)
    /// </summary>
    [HttpPost("AsyncUpload")]
    public async Task<IActionResult> AsyncUpload([FromForm] Microsoft.AspNetCore.Http.IFormFile formFile, [FromForm] string defaultFileName = "", [FromForm] string virtualPath = "")
    {
        if (formFile == null || formFile.Length == 0)
            return BadRequest("No file uploaded");

        var picture = await _pictureService.InsertPictureAsync(formFile, defaultFileName, virtualPath);
        return Ok(picture);
    }

    /// <summary>
    /// Get picture binary by picture ID
    /// </summary>
    [HttpGet("GetPictureBinary")]
    public async Task<IActionResult> GetPictureBinary([FromQuery] int pictureId)
    {
        var binary = await _pictureService.GetPictureBinaryByPictureIdAsync(pictureId);
        if (binary == null)
            return NotFound();
        return Ok(binary);
    }
}
