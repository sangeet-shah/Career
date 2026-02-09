using Middleware.Web.Domains.Customers;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Customers;

public interface IWorkContext
{
    //// <summary>
    /// Gets the current customer
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<Customer> GetCurrentCustomerAsync();

    /// <summary>
    /// Sets the current customer
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SetCurrentCustomerAsync();
}
