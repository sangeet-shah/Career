using Career.Data.Domains.Common;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Common;

public interface IHttpClientService
{
    #region Methods

    /// <summary>
    /// Get Oportunities
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="logEnable"></param>
    /// <returns>string</returns>
    Task<(string responseResult, HttpStatusCode httpStatusCode)> GetAsync(string requestUri, IDictionary<string, string> requestHeaders, bool logEnable = true);

    /// <summary>
    /// Put method
    /// </summary>
    /// <param name="requestUri">requestUri</param>
    /// <param name="content">content</param>
    /// <returns>string</returns>
    Task<string> PutAsync(string requestUri, HttpContent content);

    /// <summary>
    /// Post Async
    /// </summary>
    /// <param name="uri">uri</param>        
    /// <param name="HttpContent">HttpContent</param>
    /// <returns>string</returns>
    Task<ApiResponse> PostAsync(string requestUri, HttpContent content);

    #endregion
}
