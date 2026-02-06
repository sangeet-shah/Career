using Middleware.Web.Services.Customers;

namespace Middleware.Web.Services.Security;

public class PermissionService : IPermissionService
{
    #region Fields

    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PermissionService(IWorkContext workContext)
    {
        _workContext = workContext;
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
        if (await _workContext.GetCurrentCustomerAsync() != null)
            return true;

        return false;
    }

    #endregion
}
