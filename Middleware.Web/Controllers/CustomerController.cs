using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Services.Customers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Get test customer
    /// </summary>
    [HttpGet("GetTestCustomer")]
    public async Task<IActionResult> GetTestCustomer([FromQuery] string emailId, [FromQuery] string password)
    {
        var customer = await _customerService.GetTestCustomerAsync(emailId, password);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Authorize for testing site
    /// </summary>
    [HttpGet("AuthorizeForTestingSite")]
    public async Task<IActionResult> AuthorizeForTestingSite([FromQuery] Guid guid)
    {
        var authorized = await _customerService.AuthorizeForTestingSiteAsync(guid);
        return Ok(new { Authorized = authorized });
    }

    /// <summary>
    /// Get customer cookie
    /// </summary>
    [HttpGet("GetCustomerCookie")]
    public async Task<IActionResult> GetCustomerCookie()
    {
        var cookie = await _customerService.GetCustomerCookieAsync();
        return Ok(new { Cookie = cookie });
    }

    /// <summary>
    /// Get customer by GUID
    /// </summary>
    [HttpGet("GetCustomerByGuid")]
    public async Task<IActionResult> GetCustomerByGuid([FromQuery] Guid customerGuid)
    {
        var customer = await _customerService.GetCustomerByGuidAsync(customerGuid);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Get customers by role
    /// </summary>
    [HttpGet("GetCustomersByRole")]
    public async Task<IActionResult> GetCustomersByRole([FromQuery] string role)
    {
        var customers = await _customerService.GetCustomersByRoleAsync(role);
        return Ok(customers);
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("GetCustomerById")]
    public async Task<IActionResult> GetCustomerById([FromQuery] int customerId)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Get customers by IDs
    /// </summary>
    [HttpPost("GetCustomersByIds")]
    public async Task<IActionResult> GetCustomersByIds([FromBody] int[] customerIds)
    {
        var customers = await _customerService.GetCustomersByIdsAsync(customerIds);
        return Ok(customers);
    }

    /// <summary>
    /// Get FM customer by customer ID
    /// </summary>
    [HttpGet("GetFMCustomerByCustomerId")]
    public async Task<IActionResult> GetFMCustomerByCustomerId([FromQuery] int customerId)
    {
        var customer = await _customerService.GetFMCustomersByCustomerIdAsync(customerId);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Check if customer is in role by customer ID
    /// </summary>
    [HttpGet("IsInCustomerRoleById")]
    public async Task<IActionResult> IsInCustomerRoleById([FromQuery] int customerId, [FromQuery] string roleName, [FromQuery] bool onlyActiveCustomerRoles = true)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var isInRole = await _customerService.IsInCustomerRoleAsync(customer, roleName, onlyActiveCustomerRoles);
        return Ok(new { IsInRole = isInRole });
    }

    /// <summary>
    /// Check if customer is in role
    /// </summary>
    [HttpPost("IsInCustomerRole")]
    public async Task<IActionResult> IsInCustomerRole([FromBody] IsInCustomerRoleRequest request)
    {
        // Note: This requires the Customer entity to be passed, which may need adjustment
        // For now, returning a placeholder response
        return BadRequest("This endpoint requires Customer entity - may need to be refactored");
    }

    /// <summary>
    /// Get customer roles
    /// </summary>
    [HttpPost("GetCustomerRoles")]
    public async Task<IActionResult> GetCustomerRoles([FromBody] GetCustomerRolesRequest request)
    {
        // Note: This requires the Customer entity to be passed, which may need adjustment
        // For now, returning a placeholder response
        return BadRequest("This endpoint requires Customer entity - may need to be refactored");
    }
}

public class IsInCustomerRoleRequest
{
    public int CustomerId { get; set; }
    public string CustomerRoleSystemName { get; set; }
    public bool OnlyActiveCustomerRoles { get; set; } = true;
}

public class GetCustomerRolesRequest
{
    public int CustomerId { get; set; }
    public bool ShowHidden { get; set; } = false;
}
