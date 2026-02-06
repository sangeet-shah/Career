using Career.Data.Services.Customers;
using System.Threading.Tasks;

namespace Career.Data.Services.Security;

public class PermissionService : IPermissionService
{
    #region Fields

    private readonly IWorkContext _workContext;
    private readonly AppSettings _appSettings;

    #endregion

    #region Ctor

    public PermissionService(IWorkContext workContext,
        AppSettings appSettings)
    {
        _workContext = workContext;
        _appSettings = appSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Authorize permission
    /// </summary>       
    /// <returns>
    /// A task that represents the operation
    /// The task result contains the rue - authorized; otherwise, false
    /// </returns>
    public async Task<bool> AuthorizeAsync()
    {
        if (!_appSettings.IsTestSite)
            return true;

        if (await _workContext.GetCurrentCustomerAsync() != null)
            return true;

        return false;
    }

    #endregion
}
