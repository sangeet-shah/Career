using Career.Data.Domains.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.HelloBar;

public interface IHelloBarService
{
    #region Methods

    /// <summary>
    /// Get active hello bars
    /// </summary>
    /// <returns>HelloBars</returns>
    Task<IList<HelloBars>> GetActiveHelloBarsAsync();

    #endregion
}
