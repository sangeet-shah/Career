using Career.Web.Models.Api;
using Career.Web.Models.Customers;
using Career.Web.Services.ApiClient;
using Career.Web.Domains.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class CustomerController : Controller
{
    private readonly IApiClient _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;

    public CustomerController(IApiClient apiClient,
        IHttpContextAccessor httpContextAccessor,
        AppSettings appSettings)
    {
        _apiClient = apiClient;
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings;
    }

    public async Task<IActionResult> Login()
    {
        var model = new TestCustomerModel();
        if (_appSettings.IsTestSite)
        {
            model.UAGoogleAnalyticsId = "UA-57932286-6";
            var localeResp = await _apiClient.GetAsync<LocaleStringResponse>("api/Localization/GetLocaleStringResourceByName", new { resourceName = "account.login.career.googlesiteverification" });
            model.GoogleSiteVerification = localeResp?.Value;
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
        var customer = await _apiClient.GetAsync<CustomerDto>("api/Customer/GetTestCustomer", new { emailId = model.Email, password = model.Password });
        if (customer != null)
        {
            var option = new CookieOptions { Expires = DateTime.Now.AddDays(30) };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(NopDefaults.AuthenticationKey, customer.CustomerGuid.ToString());
            return Redirect("~/");
        }
        model.ErrorMessage = "Wrong emailid or password";
        return View(model);
    }
}
