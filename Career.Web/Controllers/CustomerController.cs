using Career.Web.Domains.Common;
using Career.Web.Models.Customers;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Career.Web.Controllers;

public class CustomerController : Controller
{
    #region Fields

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApiClient _apiClient;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    #endregion

    #region Ctor
    public CustomerController(IHttpContextAccessor httpContextAccessor,
                             IApiClient apiClient,
                             Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiClient = apiClient;
        _configuration = configuration;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Login()
    {
        var response = await _apiClient.GetAsync<LoginResultModel>("api/Customer/Login") ?? new LoginResultModel();
        return View("~/Views/Customer/Login.cshtml", response.Model ?? new TestCustomerModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(TestCustomerModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Customer/Login.cshtml", model);

        var response = await _apiClient.PostAsync<TestCustomerModel, LoginResultModel>("api/Customer/Login", model) ?? new LoginResultModel();
        if (response.IsValid)
        {
            var option = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(24),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(NopDefaults.AuthenticationKey, response.Model.CustomerGuid.ToString(), option);
            return Redirect("~/");
        }

        return View(response.Model ?? model);
    }

    #endregion
}
