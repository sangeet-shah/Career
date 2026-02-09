using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Customers;
using Dapper;
using Microsoft.AspNetCore.Http;
using Middleware.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Customers;

public class CustomerService : ICustomerService
{
    private const string CustomerTable = "Customer";
    private const string FMCustomerTable = "FM_Customer";
    private const string CustomerRoleTable = "CustomerRole";
    private const string CustomerRoleMappingTable = "Customer_CustomerRole_Mapping";
    private const string CustomerPasswordTable = "CustomerPassword";
    private const string CustomerSocialMediaMappingTable = "Customer_SocialMedia_Mapping";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    public CustomerService(IHttpContextAccessor httpContextAccessor,
        DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    protected bool PasswordsMatch(CustomerPassword customerPassword, string enteredPassword)
    {
        if (customerPassword == null || string.IsNullOrEmpty(enteredPassword))
            return false;
        var savedPassword = CreatePasswordHash(enteredPassword, customerPassword.PasswordSalt, "SHA1");
        if (customerPassword.Password == null)
            return false;
        return customerPassword.Password.Equals(savedPassword);
    }

    public string CreatePasswordHash(string password, string saltkey, string passwordFormat)
    {
        return CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltkey)), passwordFormat);
    }

    public string CreateHash(byte[] data, string hashAlgorithm)
    {
        if (string.IsNullOrEmpty(hashAlgorithm))
            throw new ArgumentNullException(nameof(hashAlgorithm));
        var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);
        if (algorithm == null)
            throw new ArgumentException("Unrecognized hash name");
        var hashByteArray = algorithm.ComputeHash(data);
        return BitConverter.ToString(hashByteArray).Replace("-", string.Empty);
    }

    public async Task<Customer> GetTestCustomerAsync(string emailId, string password)
    {
        if (string.IsNullOrEmpty(emailId) || string.IsNullOrEmpty(password))
            return null;

        Customer customer;
        using (var conn = _db.CreateNop())
        {
            var sql = $@"
SELECT c.* FROM [{CustomerTable}] c
INNER JOIN [{CustomerRoleMappingTable}] crm ON c.Id = crm.Customer_Id
INNER JOIN [{CustomerRoleTable}] cr ON crm.CustomerRole_Id = cr.Id
WHERE c.Active = 1 AND c.RegisteredInStoreId = 1 AND c.Deleted = 0
  AND c.Email = @Email AND (LOWER(cr.SystemName) = 'testers' OR LOWER(cr.SystemName) = 'administrators')";
            customer = await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = emailId });
        }

        if (customer == null)
            return null;

        using (var conn = _db.CreateNop())
        {
            var sql = $"SELECT TOP 1 * FROM [{CustomerPasswordTable}] WHERE CustomerId = @CustomerId ORDER BY CreatedOnUtc DESC";
            var passwordEntity = await conn.QueryFirstOrDefaultAsync<CustomerPassword>(sql, new { CustomerId = customer.Id });
            if (!PasswordsMatch(passwordEntity, password))
                return null;
        }

        return customer;
    }

    public async Task<bool> AuthorizeForTestingSiteAsync(Guid guid)
    {
        using var conn = _db.CreateNop();
        var sql = $@"
SELECT 1 FROM [{CustomerTable}] c
INNER JOIN [{CustomerRoleMappingTable}] crm ON c.Id = crm.Customer_Id
INNER JOIN [{CustomerRoleTable}] cr ON crm.CustomerRole_Id = cr.Id
WHERE c.Active = 1 AND c.Deleted = 0 AND c.CustomerGuid = @Guid
  AND (LOWER(cr.SystemName) = @TesterRole OR LOWER(cr.SystemName) = @AdminRole)";
        var exists = await conn.ExecuteScalarAsync<int?>(sql, new { Guid = guid, TesterRole = NopDefaults.TesterRoleName, AdminRole = NopDefaults.AdminRoleName });
        return exists.HasValue && exists.Value == 1;
    }

    public async Task<Guid?> GetCustomerCookieAsync()
    {
        var cookieValue = _httpContextAccessor.HttpContext?.Request.Cookies[NopDefaults.AuthenticationKey];
        if (string.IsNullOrEmpty(cookieValue))
            return null;
        return await Task.FromResult(Guid.Parse(cookieValue));
    }

    public async Task<Customer> GetCustomerByGuidAsync(Guid customerGuid)
    {
        if (customerGuid == Guid.Empty)
            return null;
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CustomerByGuidCacheKey, customerGuid);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT TOP 1 * FROM [{CustomerTable}] WHERE CustomerGuid = @CustomerGuid ORDER BY Id";
            return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerGuid = customerGuid });
        });
    }

    public async Task<IList<Customer>> GetCustomersByRoleAsync(string role)
    {
        if (string.IsNullOrEmpty(role))
            return null;
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CustomersRoleKey, role);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT c.* FROM [{CustomerTable}] c
INNER JOIN [{CustomerRoleMappingTable}] cm ON c.Id = cm.Customer_Id
INNER JOIN [{CustomerRoleTable}] cr ON cm.CustomerRole_Id = cr.Id
WHERE LOWER(cr.SystemName) = LOWER(@Role) AND c.Deleted = 0";
            var list = (await conn.QueryAsync<Customer>(sql, new { Role = role })).AsList();
            return list;
        });
    }

    public async Task<Customer> GetCustomerByIdAsync(int customerId)
    {
        if (customerId == 0)
            return null;
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{CustomerTable}] WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = customerId });
    }

    public async Task<IList<Customer>> GetCustomersByIdsAsync(int[] customerIds)
    {
        if (customerIds == null || customerIds.Length == 0)
            return new List<Customer>();
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{CustomerTable}] WHERE Id IN @Ids";
        var list = (await conn.QueryAsync<Customer>(sql, new { Ids = customerIds })).AsList();
        return list.OrderBy(c => Array.IndexOf(customerIds, c.Id)).ToList();
    }

    public async Task<FMCustomer> GetFMCustomersByCustomerIdAsync(int customerId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.FMCustomerByCustomerIdCacheKey, customerId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT TOP 1 * FROM [{FMCustomerTable}] WHERE CustomerId = @CustomerId";
            return await conn.QueryFirstOrDefaultAsync<FMCustomer>(sql, new { CustomerId = customerId });
        });
    }

    public async Task<bool> IsInCustomerRoleAsync(Customer customer, string customerRoleSystemName, bool onlyActiveCustomerRoles = true)
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
        using var conn = _db.CreateNop();
        var sql = $@"
SELECT cr.* FROM [{CustomerRoleTable}] cr
INNER JOIN [{CustomerRoleMappingTable}] crm ON cr.Id = crm.CustomerRole_Id
WHERE crm.Customer_Id = @CustomerId AND (@ShowHidden = 1 OR cr.Active = 1)";
        var list = (await conn.QueryAsync<CustomerRole>(sql, new { CustomerId = customer.Id, ShowHidden = showHidden })).AsList();
        return list;
    }
}
