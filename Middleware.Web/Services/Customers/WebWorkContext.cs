using Middleware.Web.Domains.Customers;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Customers;

public class WebWorkContext : IWorkContext
{
    #region Fields

    private Customer _cachedCustomer;
    private readonly ICustomerService _customerService;

    #endregion

    #region Ctor
    public WebWorkContext(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current customer
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task<Customer> GetCurrentCustomerAsync()
    {
        //whether there is a cached value
        if (_cachedCustomer != null)
            return _cachedCustomer;

        await SetCurrentCustomerAsync();

        return _cachedCustomer;
    }

    /// <summary>
    /// Sets the current customer
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task SetCurrentCustomerAsync()
    {
        var cookieValue = await _customerService.GetCustomerCookieAsync();
        if (cookieValue.HasValue)
        {
            _cachedCustomer = await _customerService.GetCustomerByGuidAsync(cookieValue.Value);
        }
    }

    #endregion
}
