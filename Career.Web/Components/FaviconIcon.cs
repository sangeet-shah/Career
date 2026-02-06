using Microsoft.AspNetCore.Mvc;

namespace Career.Web.Components;

public class FaviconIconViewComponent : ViewComponent
{
    #region Methods

    public IViewComponentResult Invoke()
    {
        return View("Default", "/favicon.ico");
    }

    #endregion
}
