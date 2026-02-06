using Career.Data;
using Career.Data.Domains.Common;
using Career.Data.Services.Customers;
using Career.Data.Services.Localization;
using Career.Web.Models.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class CustomerController : Controller
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly AppSettings _appSettings;

    #endregion

    #region Ctor
    public CustomerController(ICustomerService customerService,
                             IHttpContextAccessor httpContextAccessor,
                             ILocalizationService localizationService,
                             AppSettings appSettings)
    {
        _customerService = customerService;
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _appSettings = appSettings;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Login()
    {
        var model = new TestCustomerModel();
        if (_appSettings.IsTestSite)
        {
            model.UAGoogleAnalyticsId = "UA-57932286-6";
            model.GoogleSiteVerification = await _localizationService.GetLocaleStringResourceByNameAsync("account.login.career.googlesiteverification");
        }
        else
        {
            model.UAGoogleAnalyticsId = "UA-57932286-5";
            model.GTMGoogleAnalyticsId = "GTM-TVQ65TT";
        }
        return View("~/Views/Customer/Login.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(TestCustomerModel model)
    {
        var customer = await _customerService.GetTestCustomerAsync(emailId: model.Email, password: model.Password);
        if (customer != null)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1)
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(NopDefaults.AuthenticationKey, customer.CustomerGuid.ToString());
            return Redirect("~/");
        }
        else
            model.ErrorMessage = "Wrong emailid or password";

        return View(model);
    }

    #endregion
}
