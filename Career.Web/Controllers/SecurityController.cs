using Microsoft.AspNetCore.Mvc;

namespace Career.Web.Controllers;
public class SecurityController : Controller
{
    public  IActionResult AccessDenied()
    {
        TempData["Error"] = "You do not have permission to perform the selected operation.";
        return View();
    }
}

