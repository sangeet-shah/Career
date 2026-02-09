using Career.Web.Models.Media;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class PictureController : BaseController
{
    private readonly IApiClient _apiClient;

    public PictureController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

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

        using var content = new MultipartFormDataContent();
        await using var stream = httpPostedFile.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(httpPostedFile.ContentType);
        content.Add(fileContent, "file", httpPostedFile.FileName);
        if (!string.IsNullOrWhiteSpace(qqFileName))
            content.Add(new StringContent(qqFileName), qqFileNameParameter);

        var result = await _apiClient.PostMultipartAsync<PictureUploadResult>("api/Picture/AsyncUpload", content);
        if (result == null || !result.Success)
            return Json(new { success = false, message = result?.Message ?? "Wrong file format" });

        return Json(new
        {
            success = true,
            pictureId = result.PictureId,
            image = result.Image
        });
    }

    #endregion
}