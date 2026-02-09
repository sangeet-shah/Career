using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.PaycorAPI;
using Middleware.Web.Data.Repository;
using Microsoft.Extensions.Options;
using Middleware.Web.Options;
using Middleware.Web.Services.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Middleware.Web.Services.PaycorAPI;

// <summary>
/// paycor api interface
/// </summary>
public class PaycorAPIService : IPaycorAPIService
{
    #region Fields

    private readonly ILogService _logService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly PaycorJobOptions _paycorJobOptions;
    private readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public PaycorAPIService(ILogService logService,
        IStaticCacheManager staticCacheManager,
        IOptions<PaycorJobOptions> paycorJobOptions,
        HttpClient httpClient)
    {
        _logService = logService;
        _staticCacheManager = staticCacheManager;
        _paycorJobOptions = paycorJobOptions.Value;
        _httpClient = httpClient;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get token
    /// </summary>
    private async Task<string> GetTokenAsync(bool forceRefresh = false)
    {
        try
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.JobsAPITokenCacheKey);
            if (forceRefresh)
                await _staticCacheManager.RemoveAsync(cacheKey); // Force remove old token

            cacheKey.CacheTime = NopCachingDefaults.ShortTermCacheTime; 
            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_paycorJobOptions.BaseUrl}sts/v1/common/token?subscription-key={_paycorJobOptions.SubscriptionKey}");
                var collection = new List<KeyValuePair<string, string>>
                {
                    new("refresh_token", _paycorJobOptions.RefreshToken),
                    new("client_id", _paycorJobOptions.ClientId),
                    new("client_secret", _paycorJobOptions.ClientSecret),
                    new("grant_type", "refresh_token")
                };
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logService.InsertLog( "Paycor GetTokenAsync API Error ", await response.Content.ReadAsStringAsync());
                    return null;
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response == null || string.IsNullOrEmpty(responseContent))
                    return string.Empty;

                // With this line:
                var token = JsonConvert.DeserializeObject<PaycorAPITokenResponse>(responseContent);
                if (token != null)
                    return token.AccessToken;

                return null;
            });

        }
        catch (Exception e)
        {
            _logService.Error(e?.InnerException?.Message);
        }

        return null;
    }

    #endregion

    #region Methods 

    public async Task<IPagedList<PaycorAPIJobsResponse.JobRecord>> GetAllJobsAsync(IList<string> selectedStates = null,
        IList<string> selectedCities = null, IList<string> selectedJobCategories = null, int pageIndex = 0, int pageSize =10000)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.JobsCacheKey);
        cacheKey.CacheTime = NopCachingDefaults.ShortTermCacheTime; 
        var jobs = await _staticCacheManager.GetAsync<IList<PaycorAPIJobsResponse.JobRecord>>(cacheKey, async () =>
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return null;

                var repeatCount = 0;
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_paycorJobOptions.BaseUrl}v1/legalEntities/182655/ats/8a7883c6974d4d58019783f945de0e4b/jobs?status=ACTIVE");

                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", "Bearer " + token);
                request.Headers.Add("Ocp-Apim-Subscription-Key", _paycorJobOptions.SubscriptionKey);

                var response = await _httpClient.SendAsync(request);
                // If unauthorized, force regenerate the token once
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && repeatCount <= 2)
                {
                    token = await GetTokenAsync(true);
                    if (string.IsNullOrEmpty(token))
                        return null;

                    repeatCount++;
                    request.Headers.Remove("Authorization");
                    request.Headers.Add("Authorization", "Bearer " + token); // force refresh token
                    response = await _httpClient.SendAsync(request); // retry request
                }
                if (!response.IsSuccessStatusCode)
                {
                    _logService.InsertLog( "Paycor GetAllJobsAsync API Error ", await response.Content.ReadAsStringAsync());
                    return null;
                }               

                var responseData = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseData))
                    return null;

                var paycorAPIJobsResponse = JsonConvert.DeserializeObject<PaycorAPIJobsResponse>(responseData);
                if (paycorAPIJobsResponse == null || paycorAPIJobsResponse.Records == null || paycorAPIJobsResponse.Records.Count == 0)
                    return null;

                return paycorAPIJobsResponse.Records.Where(x => !string.IsNullOrEmpty(x.Title)).OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception e)
            {
                _logService.Error(e?.InnerException?.Message ?? e.Message);
                return null;
            }
        });

        // state filter
        if (selectedStates != null && selectedStates.Any())
            jobs = (from o in jobs
                    where selectedStates.Any(ss => o.AtsLocation.State.ToLower() == ss.ToLower())
                    select o).ToList();

        // city filter
        if (selectedCities != null && selectedCities.Any())
            jobs = (from o in jobs
                    where selectedCities.Any(ss => o.AtsLocation.City.ToLower() == ss.ToLower())
                    select o).ToList();

        // job category filter
        if (selectedJobCategories != null && selectedJobCategories.Any())
            jobs = (from o in jobs
                    where selectedJobCategories.Contains(o.AtsDepartment.Id)
                    select o).ToList();

        if(jobs == null)
            return null;

        return new PagedList<PaycorAPIJobsResponse.JobRecord>(jobs, pageIndex, pageSize);
    }

    #endregion
}

