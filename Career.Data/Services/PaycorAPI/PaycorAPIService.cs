using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.PaycorAPI;
using Career.Data.Repository;
using Career.Data.Services.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Career.Data.Services.PaycorAPI;

// <summary>
/// paycor api interface
/// </summary>
public class PaycorAPIService : IPaycorAPIService
{
    #region Fields

    private readonly ILogService _logService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly AppSettings _appSettings;
    private readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public PaycorAPIService(ILogService logService,
        IStaticCacheManager staticCacheManager,
        AppSettings appSettings,
        HttpClient httpClient)
    {
        _logService = logService;
        _staticCacheManager = staticCacheManager;
        _appSettings = appSettings;
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
                
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_appSettings.PaycorAPISettings.BaseUrl}{_appSettings.PaycorAPISettings.TokenEndpoint}?subscription-key={_appSettings.PaycorAPISettings.SubscriptionKey}");
                var collection = new List<KeyValuePair<string, string>>
                {
                    new("refresh_token", _appSettings.PaycorAPISettings.RefreshToken),
                    new("client_id", _appSettings.PaycorAPISettings.ClientId),
                    new("client_secret", _appSettings.PaycorAPISettings.ClientSecret),
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
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_appSettings.PaycorAPISettings.BaseUrl}{_appSettings.PaycorAPISettings.JobsEndpoint}?status=ACTIVE");

                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", "Bearer " + token);
                request.Headers.Add("Ocp-Apim-Subscription-Key", _appSettings.PaycorAPISettings.SubscriptionKey);

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

