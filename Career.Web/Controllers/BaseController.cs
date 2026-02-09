using Career.Web.Filters;
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

    #endregion        
}
