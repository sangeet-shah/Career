using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Services.Media;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class PictureController : ControllerBase
{
    #region Fields

    private readonly IPictureService _pictureService;

    #endregion

    #region Ctor

    public PictureController(IPictureService pictureService)
    {
        _pictureService = pictureService;
    }

    #endregion

    #region Method

    [HttpPost]
    public async Task<IActionResult> AsyncUpload()
    {
        var httpPostedFile = Request.Form.Files.FirstOrDefault();
        if (httpPostedFile == null)
            return Ok(new { success = false, message = "No file uploaded" });

        const string qqFileNameParameter = "qqfilename";

        var qqFileName = Request.Form.ContainsKey(qqFileNameParameter)
            ? Request.Form[qqFileNameParameter].ToString()
            : string.Empty;

        var picture = await _pictureService.InsertPictureAsync(httpPostedFile, qqFileName);

        if (picture == null)
            return Ok(new { success = false, message = "Wrong file format" });

        var pictureBinary = (await _pictureService.GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData;

        return Ok(new
        {
            success = true,
            pictureId = picture.Id,
            image = $"data:{picture.MimeType};base64,{System.Convert.ToBase64String(pictureBinary)}"
        });
    }

    #endregion
}
