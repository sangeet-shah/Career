using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.Customers;
using Career.Data.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Career.Data.Services.Customers;

public class CustomerService : ICustomerService
{
    #region Fields

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<FMCustomer> _fmcustomerRepository;
    private readonly IRepository<CustomerRole> _customerRoleRepository;
    private readonly IRepository<Customer_CustomerRole_Mapping> _customerRoleMappingRepository;
    private readonly IRepository<CustomerPassword> _customerPasswordRepository;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IRepository<Customer_SocialMedia_Mapping> _customerSocialMediaMappingRepository;

    #endregion

    #region Ctor

    public CustomerService(IHttpContextAccessor httpContextAccessor,
        IRepository<Customer> customerRepository,
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<Customer_CustomerRole_Mapping> customerRoleMappingRepository,
        IRepository<CustomerPassword> customerPasswordRepository,
        IStaticCacheManager staticCacheManager,
        IRepository<Customer_SocialMedia_Mapping> customerSocialMediaMappingRepository,
        IRepository<FMCustomer> fmcustomerRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _customerRepository = customerRepository;
        _customerRoleRepository = customerRoleRepository;
        _customerRoleMappingRepository = customerRoleMappingRepository;
        _customerPasswordRepository = customerPasswordRepository;
        _staticCacheManager = staticCacheManager;
        _customerSocialMediaMappingRepository = customerSocialMediaMappingRepository;
        _fmcustomerRepository = fmcustomerRepository;
    }

    #endregion

    #region Utilities

    protected bool PasswordsMatch(CustomerPassword customerPassword, string enteredPassword)
    {
        if (customerPassword == null || string.IsNullOrEmpty(enteredPassword))
            return false;

        var savedPassword = CreatePasswordHash(enteredPassword, customerPassword.PasswordSalt, "SHA1");
        if (customerPassword.Password == null)
            return false;

        return customerPassword.Password.Equals(savedPassword);
    }

    public  string CreatePasswordHash(string password, string saltkey, string passwordFormat)
    {
        return CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltkey)), passwordFormat);
    }

    public  string CreateHash(byte[] data, string hashAlgorithm)
    {
        if (string.IsNullOrEmpty(hashAlgorithm))
            throw new ArgumentNullException(nameof(hashAlgorithm));

        var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);
        if (algorithm == null)
            throw new ArgumentException("Unrecognized hash name");

        var hashByteArray = algorithm.ComputeHash(data);
        return BitConverter.ToString(hashByteArray).Replace("-", string.Empty);
    }

    #endregion

    #region Methods   

    /// <summary>
    /// check customer is authorize or not
    /// </summary>
    /// <param name="emailId">emailId</param>
    /// <param name="password">password</param>
    /// <returns>check authorize or not</returns>
    public async Task<Customer> GetTestCustomerAsync(string emailId, string password)
    {
        if (string.IsNullOrEmpty(emailId) || string.IsNullOrEmpty(password))
            return null;

        var customer = await (from c in _customerRepository.Table
                              join crm in _customerRoleMappingRepository.Table on c.Id equals crm.Customer_Id
                              join cr in _customerRoleRepository.Table on crm.CustomerRole_Id equals cr.Id
                              where c.Active && c.RegisteredInStoreId == 1 && !c.Deleted &&
                              c.Email == emailId && (cr.SystemName.ToLower() == "testers" || cr.SystemName.ToLower() == "administrators")
                              select c).FirstOrDefaultAsync();

        if (customer == null)
            return null;

        var passwordEntity = await (from c in _customerPasswordRepository.Table
                                    where c.CustomerId == customer.Id
                                    orderby c.CreatedOnUtc descending
                                    select c).FirstOrDefaultAsync();

        if (!PasswordsMatch(passwordEntity, password))
            customer = null;

        return customer;
    }

    public async Task<bool> AuthorizeForTestingSiteAsync(Guid guid)
    {
        var customer = await (from c in _customerRepository.Table
                              join crm in _customerRoleMappingRepository.Table on c.Id equals crm.Customer_Id
                              join cr in _customerRoleRepository.Table on crm.CustomerRole_Id equals cr.Id
                              where c.Active && !c.Deleted && c.CustomerGuid == guid &&
                              (cr.SystemName.ToLower() == NopDefaults.TesterRoleName || cr.SystemName.ToLower() == NopDefaults.AdminRoleName)
                              select c).FirstOrDefaultAsync();

        if (customer == null)
            return false;

        return true;
    }

    public async Task<Guid?> GetCustomerCookieAsync()
    {
        var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[NopDefaults.AuthenticationKey];
        if (string.IsNullOrEmpty(cookieValue))
            return null;

        return await Task.FromResult(Guid.Parse(cookieValue));
    }

    /// <summary>
    /// Gets a customer by GUID
    /// </summary>
    /// <param name="customerGuid">Customer GUID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer
    /// </returns>
    public async Task<Customer> GetCustomerByGuidAsync(Guid customerGuid)
    {
        if (customerGuid == Guid.Empty)
            return null;

        var query = from c in _customerRepository.Table
                    where c.CustomerGuid == customerGuid
                    orderby c.Id
                    select c;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CustomerByGuidCacheKey, customerGuid);
        return await _staticCacheManager.GetAsync(cacheKey, async () => await query.FirstOrDefaultAsync());
    }

    /// <summary>
    /// Get customers by role
    /// </summary>
    /// <param name="role">role</param>
    /// <returns>Customers</returns>
    public async Task<IList<Customer>> GetCustomersByRoleAsync(string role)
    {
        if (string.IsNullOrEmpty(role))
            return null;

        //cacheable copy
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CustomersRoleKey, role);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from c in _customerRepository.Table
                          join cm in _customerRoleMappingRepository.Table on c.Id equals cm.Customer_Id
                          join cr in _customerRoleRepository.Table on cm.CustomerRole_Id equals cr.Id
                          where cr.SystemName.ToLower() == role.ToLower() && !c.Deleted
                          select c).ToListAsync();
        });
    }

    /// <summary>
    /// Gets a customer by id
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a customer
    /// </returns>
    public async Task<Customer> GetCustomerByIdAsync(int customerId)
    {
        if (customerId == 0)
            return null;

        return await _customerRepository.GetByIdAsync(customerId, cache => default);
    }

    /// <summary>
    /// Get customers by identifiers
    /// </summary>
    /// <param name="customerIds">Customer identifiers</param>
    /// <returns>Customers</returns>
    public async Task<IList<Customer>> GetCustomersByIdsAsync(int[] customerIds)
    {
        return await _customerRepository.GetByIdsAsync(customerIds, cache => default);
    }

    public async Task<FMCustomer> GetFMCustomersByCustomerIdAsync(int customerId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.FMCustomerByCustomerIdCacheKey, customerId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await _fmcustomerRepository.Table.Where(c => c.CustomerId == customerId).FirstOrDefaultAsync();
        });
    }


    public async Task<bool> IsInCustomerRoleAsync(Customer customer,
        string customerRoleSystemName, bool onlyActiveCustomerRoles = true)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (string.IsNullOrEmpty(customerRoleSystemName))
            throw new ArgumentNullException(nameof(customerRoleSystemName));

        var customerRoles = await GetCustomerRolesAsync(customer, !onlyActiveCustomerRoles);

        return customerRoles?.Any(cr => cr.SystemName == customerRoleSystemName) ?? false;
    }

    public async Task<IList<CustomerRole>> GetCustomerRolesAsync(Customer customer, bool showHidden = false)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        return await (from cr in _customerRoleRepository.Table
                      join crm in _customerRoleMappingRepository.Table on cr.Id equals crm.CustomerRole_Id
                      where crm.Customer_Id == customer.Id &&
                            (showHidden || cr.Active)
                      select cr).ToListAsync();
    }

    #endregion
}
