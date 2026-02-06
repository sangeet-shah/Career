using Career.Data.Services.Media;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class PictureController : BaseController
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
    //do not validate request token (XSRF)
    public async Task<IActionResult> AsyncUpload()
    {
        var httpPostedFile = Request.Form.Files.FirstOrDefault();
        if (httpPostedFile == null)
            return Json(new { success = false, message = "No file uploaded" });

        const string qqFileNameParameter = "qqfilename";

        var qqFileName = Request.Form.ContainsKey(qqFileNameParameter)
            ? Request.Form[qqFileNameParameter].ToString()
            : string.Empty;

        var picture = await _pictureService.InsertPictureAsync(httpPostedFile, qqFileName);

        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.

        if (picture == null)
            return Json(new { success = false, message = "Wrong file format" });

        var pictureBinary = (await _pictureService.GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData;

        return Json(new
        {
            success = true,
            pictureId = picture.Id,
            image = $"data:{picture.MimeType};base64,{System.Convert.ToBase64String(pictureBinary)}"
        });
    }

    #endregion
}