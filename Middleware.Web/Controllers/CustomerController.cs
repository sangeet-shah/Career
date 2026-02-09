using Middleware.Web.Models.Customers;
using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Services.Customers;
using Middleware.Web.Services.Localization;

namespace Middleware.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
public class CustomerController : ControllerBase
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    #endregion

    #region Ctor
    public CustomerController(ICustomerService customerService,
                             ILocalizationService localizationService,
                             Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _customerService = customerService;
        _localizationService = localizationService;
        _configuration = configuration;
    }

    #endregion

    #region Methods

    [HttpGet("Login")]
    public async Task<IActionResult> LoginGet()
    {
        var model = new TestCustomerModel();

        model.UAGoogleAnalyticsId = "UA-57932286-5";
        model.GTMGoogleAnalyticsId = "GTM-TVQ65TT";

        return Ok(new LoginResultModel { IsValid = false, Model = model });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginPost()
    {
        // Support both JSON body and form posts (e.g. from browser form submit)
        TestCustomerModel model = null;
        try
        {
            var contentType = Request.ContentType ?? string.Empty;
            if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // read JSON body
                model = await System.Text.Json.JsonSerializer.DeserializeAsync<TestCustomerModel>(Request.Body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                model = new TestCustomerModel
                {
                    Email = form["email"],
                    Password = form["password"]
                };
            }
        }
        catch
        {
            // fall through - model may be null
        }

        var response = new LoginResultModel { IsValid = false, Model = model ?? new TestCustomerModel() };
        if (model != null && !string.IsNullOrEmpty(model.Email))
        {
            var customer = await _customerService.GetTestCustomerAsync(emailId: model.Email, password: model.Password);
            if (customer != null)
            {
                response.IsValid = true;
                response.Model.CustomerGuid = customer.CustomerGuid;
                return Ok(response);
            }

            response.Model.ErrorMessage = "Wrong emailid or password";
            return Ok(response);
        }

        // Bad request if no credentials provided
        return BadRequest(new { message = "Invalid request payload" });
    }

    #endregion
}
