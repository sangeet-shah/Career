using Middleware.Web.Domains.Common;
using Middleware.Web.Services.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Common;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogService _logService;

    public HttpClientService(IHttpClientFactory httpClientFactory,
        ILogService logService)
    {
        _httpClientFactory = httpClientFactory;
        _logService = logService;
    }

    #region Methods

    /// <summary>
    /// Get Oportunities
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="logEnable"></param>
    /// <returns>string</returns>
    public async Task<(string responseResult, HttpStatusCode httpStatusCode)> GetAsync(string requestUri, IDictionary<string, string> requestHeaders, bool logEnable = true)
    {
        var httpStatusCode = HttpStatusCode.NoContent;
        try
        {
            var httpClient = _httpClientFactory.CreateClient(NopDefaults.DefaultHttpClient);

            // add request header if exist
            if (requestHeaders != null && requestHeaders.Any())
                foreach (var requestHeader in requestHeaders)
                    httpClient.DefaultRequestHeaders.Add(requestHeader.Key, requestHeader.Value);

            // Get HttpClient Async
            var response = await httpClient.GetAsync(requestUri);
            httpStatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadAsStringAsync(), httpStatusCode);
            else if (logEnable)
                 _logService.Error("status code: " + response.StatusCode + " " + await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }

        return (string.Empty, httpStatusCode);
    }

    /// <summary>
    /// Put method
    /// </summary>
    /// <param name="requestUri">requestUri</param>
    /// <param name="content">content</param>
    /// <returns>string</returns>
    public async Task<string> PutAsync(string requestUri, HttpContent content)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(NopDefaults.DefaultHttpClient);

            // Put HttpClient Async
            var response = await httpClient.PutAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }

        return string.Empty;
    }

    /// <summary>
    /// Post Async
    /// </summary>
    /// <param name="uri">uri</param>        
    /// <param name="HttpContent">HttpContent</param>
    /// <returns>ApiResponse</returns>
    public async Task<ApiResponse> PostAsync(string requestUri, HttpContent content)
    {
        var apiResponse = new ApiResponse();
        try
        {
            // Post HttpClient Async
            var httpClient = _httpClientFactory.CreateClient(NopDefaults.DefaultHttpClient);
            var response = await httpClient.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                apiResponse.ResponseResult = await response.Content.ReadAsStringAsync();
                apiResponse.Success = true;
            }
            else
                 _logService.Error("status code: "+ response.StatusCode +" " + await response.Content.ReadAsStringAsync());                
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }

        return apiResponse;
    }

    #endregion
}
