using System.Threading.Tasks;

namespace Career.Data.Services.Security;

public interface IPermissionService
{
    /// <summary>
    /// Authorize permission
    /// </summary>       
    /// <returns>
    /// A task that represents the operation
    /// The task result contains the rue - authorized; otherwise, false
    /// </returns>
   Task<bool> AuthorizeAsync();
}
