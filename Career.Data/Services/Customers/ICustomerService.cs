using Career.Data.Domains.Customers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Career.Data.Services.Customers;

public interface ICustomerService
{
    Task<Customer> GetTestCustomerAsync(string emailId, string password);

    Task<bool> AuthorizeForTestingSiteAsync(Guid guid);

    Task<Guid?> GetCustomerCookieAsync();

    /// <summary>
    /// Gets a customer by GUID
    /// </summary>
    /// <param name="customerGuid">Customer GUID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer
    /// </returns>
    Task<Customer> GetCustomerByGuidAsync(Guid customerGuid);

    /// <summary>
    /// Get customers by role
    /// </summary>
    /// <param name="role">role</param>
    /// <returns>Customers</returns>
    Task<IList<Customer>> GetCustomersByRoleAsync(string role);

    /// <summary>
    /// Gets a customer by id
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer
    /// </returns>
    Task<Customer> GetCustomerByIdAsync(int customerId);

    /// <summary>
    /// Get customers by identifiers
    /// </summary>
    /// <param name="customerIds">Customer identifiers</param>
    /// <returns>Customers</returns>
    Task<IList<Customer>> GetCustomersByIdsAsync(int[] customerIds);

    Task<FMCustomer> GetFMCustomersByCustomerIdAsync(int customerId);

    /// <summary>
    /// Gets a value indicating whether customer is in a certain customer role
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="customerRoleSystemName">Customer role system name</param>
    /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
    Task<bool> IsInCustomerRoleAsync(Customer customer, string customerRoleSystemName, bool onlyActiveCustomerRoles = true);

    /// <summary>
    /// Gets list of customer roles
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="showHidden">A value indicating whether to load hidden records</param>
    Task<IList<CustomerRole>> GetCustomerRolesAsync(Customer customer, bool showHidden = false);   
}