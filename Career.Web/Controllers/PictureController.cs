using Career.Web.Models.Api;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Career.Web.Controllers;

public class PictureController : BaseController
{
    private readonly IApiClient _apiClient;

    public PictureController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpPost]
    public async Task<IActionResult> AsyncUpload()
    {
        var httpPostedFile = Request.Form.Files.FirstOrDefault();
        if (httpPostedFile == null)
            return Json(new { success = false, message = "No file uploaded" });

        const string qqFileNameParameter = "qqfilename";
        var qqFileName = Request.Form.ContainsKey(qqFileNameParameter)
            ? Request.Form[qqFileNameParameter].ToString()
            : string.Empty;

        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(httpPostedFile.OpenReadStream()), "formFile", httpPostedFile.FileName ?? "file");
        content.Add(new StringContent(qqFileName), "defaultFileName");

        var picture = await _apiClient.PostMultipartAsync<PictureUploadResponse>("api/Picture/AsyncUpload", content);
        if (picture == null)
            return Json(new { success = false, message = "Wrong file format" });

        var binaryResponse = await _apiClient.GetAsync<PictureBinaryResponse>("api/Picture/GetPictureBinary", new { pictureId = picture.Id });
        var pictureBinary = binaryResponse?.BinaryData;
        if (pictureBinary == null)
            return Json(new { success = false, message = "Could not load picture binary" });

        return Json(new
        {
            success = true,
            pictureId = picture.Id,
            image = $"data:{picture.MimeType};base64,{System.Convert.ToBase64String(pictureBinary)}"
        });
    }
}
