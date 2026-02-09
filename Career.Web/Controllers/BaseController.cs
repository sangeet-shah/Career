using Career.Web.Filters;
using Career.Web.Domains.Common;
using Microsoft.AspNetCore.Mvc;

namespace Career.Web.Controllers;

[WwwRequirement]
public abstract class BaseController : Controller
{
    #region Methods       

    protected  IActionResult AccessDeniedView()
    {
        return RedirectToAction("AccessDenied", "Security");
    }

    public IActionResult InvokeHttp404()
    {
        Response.StatusCode = 404;
        Response.ContentType = "text/html";
        return View("~/Views/Shared/PageNotFound.cshtml");
    }

    /// <summary>
    /// Helper to return 404 view when a model from API is null.
    /// Usage: var model = await _apiClient.GetAsync&lt;T&gt;(...); var maybe = NotFoundIfNull(model); if (maybe != null) return maybe;
    /// </summary>
    protected IActionResult? NotFoundIfNull(object? model)
    {
        if (model == null) return InvokeHttp404();
        return null;
    }   

    #endregion        
}
