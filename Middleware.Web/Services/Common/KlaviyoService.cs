using Middleware.Web.Domains.Common;
using Middleware.Web.Services.Directory;
using Middleware.Web.Services.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Common;

public class KlaviyoService : IKlaviyoService
{
    #region Fields

    private readonly ILogService _logService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IHttpClientService _httpClientService;

    #endregion

    #region Ctor

    public KlaviyoService(ILogService logService,
        IStateProvinceService stateProvinceService,
        IHttpClientService httpClientService)
    {
        _logService = logService;
        _stateProvinceService = stateProvinceService;
        _httpClientService = httpClientService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get profile id by email
    /// </summary>
    /// <param name="customerEmail"></param>
    /// <returns></returns>
    public async Task<string> GetProfileIdByEmailAsync(string customerEmail, string privateAPIKey)
    {
        var requestUri = $"https://a.klaviyo.com/api/profiles/?filter=equals(email,\"{customerEmail}\")";

        try
        {
            var requestHeaders = new Dictionary<string, string>();
            requestHeaders.Add("accept", "application/json");
            requestHeaders.Add("revision", "2024-07-15");
            requestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");

            var response = (await _httpClientService.GetAsync(requestUri, requestHeaders)).responseResult;
            if (string.IsNullOrEmpty(response) || response == "[]")
                return string.Empty;

            var responseResult = JsonConvert.DeserializeObject<KlaviyoProfileExistReponse>(response);
            if (response == null || responseResult.data == null || !responseResult.data.Any() || string.IsNullOrEmpty(responseResult.data.FirstOrDefault().id))
                return string.Empty;

            return responseResult.data.FirstOrDefault().id;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return string.Empty;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Subscribe email
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="newsLetterListId">newsLetterListId</param>
    /// <returns>ApiResponse</returns>
    public async Task<string> SubscribeEmailAsync(string email, string privateAPIKey, string newsLetterListId)
    {
        var klaviyoEmailSubscribeProfileRequest = new KlaviyoEmailSubscribeProfileRequest
        {
            data = new KlaviyoEmailSubscribeProfileRequest.Data
            {
                type = "profile-subscription-bulk-create-job",
                attributes = new KlaviyoEmailSubscribeProfileRequest.Attributes
                {
                    custom_source = "Marketing Event",
                    profiles = new KlaviyoEmailSubscribeProfileRequest.Profiles
                    {
                        data = new List<KlaviyoEmailSubscribeProfileRequest.Profile>
                        {
                            new KlaviyoEmailSubscribeProfileRequest.Profile
                            {
                                type = "profile",
                                attributes = new KlaviyoEmailSubscribeProfileRequest.ProfileAttributes
                                {
                                    email = email,
                                    subscriptions = new KlaviyoEmailSubscribeProfileRequest.Subscriptions
                                    {
                                        email = new KlaviyoEmailSubscribeProfileRequest.EmailSubscription
                                        {
                                            marketing = new KlaviyoEmailSubscribeProfileRequest.Marketing
                                            {
                                                consent = "SUBSCRIBED"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                relationships = new KlaviyoEmailSubscribeProfileRequest.Relationships
                {
                    list = new KlaviyoEmailSubscribeProfileRequest.ListData
                    {
                        data = new KlaviyoEmailSubscribeProfileRequest.ListType
                        {
                            type = "list",
                            id = newsLetterListId
                        }
                    }
                }
            }
        };

        try
        {
            var requestUri = "https://a.klaviyo.com/api/profile-subscription-bulk-create-jobs/";

            using var client = new HttpClient();

            // clear header
            client.DefaultRequestHeaders.Clear();

            // add request header                
            client.DefaultRequestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");
            client.DefaultRequestHeaders.Add("revision", "2024-07-15");
            client.DefaultRequestHeaders.Add("accept", "application/json");

            var body = JsonConvert.SerializeObject(klaviyoEmailSubscribeProfileRequest);

            using var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(requestUri, stringContent);
            if (!response.IsSuccessStatusCode)
            {
                _logService.InsertLog( await response.Content.ReadAsStringAsync());

                var klaviyoSubsribeProfileErrorResponse = JsonConvert.DeserializeObject<KlaviyoSubsribeProfileErrorResponse>(await response.Content.ReadAsStringAsync());
                if (klaviyoSubsribeProfileErrorResponse.errors.Any())
                    return klaviyoSubsribeProfileErrorResponse.errors.Select(x => x.detail).FirstOrDefault();
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return string.Empty;
    }

    /// <summary>
    /// UnSubscribe email
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="newsLetterListId">newsLetterListId</param>
    /// <returns>ApiResponse</returns>
    public async Task<string> UnSubscribeEmailAsync(string email, string privateAPIKey, string newsLetterListId)
    {
        var klaviyoEmailUnSubscribeProfileRequest = new KlaviyoEmailUnSubscribeProfileRequest
        {
            data = new KlaviyoEmailUnSubscribeProfileRequest.Data
            {
                type = "profile-unsubscription-bulk-create-job",
                attributes = new KlaviyoEmailUnSubscribeProfileRequest.Attributes
                {
                    list_id = newsLetterListId,
                    emails = email
                }
            }
        };

        try
        {
            var requestUri = "https://a.klaviyo.com/api/profile-unsubscription-bulk-create-jobs/";

            using var client = new HttpClient();

            // clear header
            client.DefaultRequestHeaders.Clear();

            // add request header                
            client.DefaultRequestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");
            client.DefaultRequestHeaders.Add("revision", "2024-07-15");
            client.DefaultRequestHeaders.Add("accept", "application/json");

            var body = JsonConvert.SerializeObject(klaviyoEmailUnSubscribeProfileRequest);

            using var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(requestUri, stringContent);

            if (!response.IsSuccessStatusCode)
            {
                _logService.InsertLog( await response.Content.ReadAsStringAsync());

                var klaviyoSubsribeProfileErrorResponse = JsonConvert.DeserializeObject<KlaviyoSubsribeProfileErrorResponse>(await response.Content.ReadAsStringAsync());
                if (klaviyoSubsribeProfileErrorResponse.errors.Any())
                    return klaviyoSubsribeProfileErrorResponse.errors.Select(x => x.detail).FirstOrDefault();
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return string.Empty;
    }

    /// <summary>
    /// Subscribe sms
    /// </summary>
    /// <param name="phone">phone</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="smsListId">smsListId</param>
    /// <returns>ApiResponse</returns>
    public async Task<string> SubscribeSMSAsync(string phone, string privateAPIKey, string smsListId)
    {
        var klaviyoSmsSubscribeProfileRequest = new KlaviyoSMSSubscribeProfileRequest
        {
            data = new KlaviyoSMSSubscribeProfileRequest.Data
            {
                type = "profile-subscription-bulk-create-job",
                attributes = new KlaviyoSMSSubscribeProfileRequest.Attributes
                {
                    custom_source = "Marketing Event",
                    profiles = new KlaviyoSMSSubscribeProfileRequest.Profiles
                    {
                        data = new List<KlaviyoSMSSubscribeProfileRequest.Profile>
                {
                    new KlaviyoSMSSubscribeProfileRequest.Profile
                    {
                        type = "profile",
                        attributes = new KlaviyoSMSSubscribeProfileRequest.ProfileAttributes
                        {
                            phone_number = "+1" + phone,
                            subscriptions = new KlaviyoSMSSubscribeProfileRequest.Subscriptions
                            {
                                sms = new KlaviyoSMSSubscribeProfileRequest.SmsSubscription
                                {
                                    marketing = new KlaviyoSMSSubscribeProfileRequest.Marketing
                                    {
                                        consent = "SUBSCRIBED"
                                    }
                                }
                            }
                        }
                    }
                }
                    }
                },
                relationships = new KlaviyoSMSSubscribeProfileRequest.Relationships
                {
                    list = new KlaviyoSMSSubscribeProfileRequest.ListData
                    {
                        data = new KlaviyoSMSSubscribeProfileRequest.ListType
                        {
                            type = "list",
                            id = smsListId
                        }
                    }
                }
            }
        };

        try
        {
            var requestUri = "https://a.klaviyo.com/api/profile-subscription-bulk-create-jobs/";

            using var client = new HttpClient();

            // clear header
            client.DefaultRequestHeaders.Clear();

            // add request header                
            client.DefaultRequestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");
            client.DefaultRequestHeaders.Add("revision", "2024-07-15");
            client.DefaultRequestHeaders.Add("accept", "application/json");

            var body = JsonConvert.SerializeObject(klaviyoSmsSubscribeProfileRequest);

            using var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(requestUri, stringContent);
            if (!response.IsSuccessStatusCode)
            {
                _logService.InsertLog( await response.Content.ReadAsStringAsync());

                var klaviyoSubsribeProfileErrorResponse = JsonConvert.DeserializeObject<KlaviyoSubsribeProfileErrorResponse>(await response.Content.ReadAsStringAsync());
                if (klaviyoSubsribeProfileErrorResponse.errors.Any())
                    return klaviyoSubsribeProfileErrorResponse.errors.Select(x => x.detail).FirstOrDefault();
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return string.Empty;
    }

    /// <summary>
    /// UnSubscribe sms
    /// </summary>
    /// <param name="phone">phone</param>
    /// <returns>ApiResponse</returns>
    public async Task<string> UnSubscribeSMSAsync(string phone, string privateAPIKey, string smsListId)
    {
        var klaviyoSMSUnSubscribeProfileRequest = new KlaviyoSMSUnSubscribeProfileRequest
        {
            data = new KlaviyoSMSUnSubscribeProfileRequest.Data
            {
                type = "profile-unsubscription-bulk-create-job",
                attributes = new KlaviyoSMSUnSubscribeProfileRequest.Attributes
                {
                    list_id = smsListId,
                    phone_numbers = "+1" + phone
                }
            }
        };

        try
        {
            var requestUri = "https://a.klaviyo.com/api/profile-unsubscription-bulk-create-jobs/";

            using var client = new HttpClient();

            // clear header
            client.DefaultRequestHeaders.Clear();

            // add request header                
            client.DefaultRequestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");
            client.DefaultRequestHeaders.Add("revision", "2024-07-15");
            client.DefaultRequestHeaders.Add("accept", "application/json");

            var body = JsonConvert.SerializeObject(klaviyoSMSUnSubscribeProfileRequest);

            using var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(requestUri, stringContent);

            if (!response.IsSuccessStatusCode)
            {
                _logService.InsertLog( await response.Content.ReadAsStringAsync());

                var klaviyoSubsribeProfileErrorResponse = JsonConvert.DeserializeObject<KlaviyoSubsribeProfileErrorResponse>(await response.Content.ReadAsStringAsync());
                if (klaviyoSubsribeProfileErrorResponse.errors.Any())
                    return klaviyoSubsribeProfileErrorResponse.errors.Select(x => x.detail).FirstOrDefault();
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return string.Empty;
    }

    /// <summary>
    /// Subscribe event list
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="email">Email</param>
    /// <param name="phone">Phone</param>
    /// <param name="ZipPostalCode">Zip postal code</param>
    /// <param name="city">City</param>
    /// <param name="stateId">State</param>
    /// <param name="eventName">Event name</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="eventListId">eventListId</param>
    /// <returns>ApiResponse</returns>
    public async Task<string> SubscribeEventListAsync(string firstName, string lastName, string email, string phone, string ZipPostalCode, string city, int stateId, string eventName, string privateAPIKey, string eventListId)
    {
        var klaviyoEventListSubscribeRequest = new KlaviyoEventListSubscribeRequest
        {
            data = new KlaviyoEventListSubscribeRequest.Data
            {
                type = "event",
                attributes = new KlaviyoEventListSubscribeRequest.Attributes
                {
                    profile = new KlaviyoEventListSubscribeRequest.Profile
                    {
                        data = new KlaviyoEventListSubscribeRequest.ProfileData
                        {
                            type = "profile",
                            attributes = new KlaviyoEventListSubscribeRequest.ProfileAttributes
                            {
                                email = email,
                                first_name = firstName,
                                last_name = lastName,
                                phone_number = "+1" + phone,
                                location = new KlaviyoEventListSubscribeRequest.Location
                                {
                                    city = city,
                                    region = stateId > 0 ? (await _stateProvinceService.GetStateProvinceByIdAsync(stateId)).Name : "",
                                    zip = ZipPostalCode
                                }
                            }
                        }
                    },
                    metric = new KlaviyoEventListSubscribeRequest.Metric
                    {
                        data = new KlaviyoEventListSubscribeRequest.MetricData
                        {
                            type = "metric",
                            attributes = new KlaviyoEventListSubscribeRequest.MetricAttributes
                            {
                                name = eventName
                            }
                        }
                    },
                    properties = new KlaviyoEventListSubscribeRequest.Properties
                    {
                        subscribed = "yes"
                    },
                    unique_id = eventListId
                }
            }
        };


        try
        {
            var requestUri = "https://a.klaviyo.com/api/events/";

            using var client = new HttpClient();

            // clear header
            client.DefaultRequestHeaders.Clear();

            // add request header                
            client.DefaultRequestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");
            client.DefaultRequestHeaders.Add("revision", "2024-07-15");

            var body = JsonConvert.SerializeObject(klaviyoEventListSubscribeRequest);

            using var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(requestUri, stringContent);
            if (!response.IsSuccessStatusCode)
            {
                _logService.InsertLog( await response.Content.ReadAsStringAsync());

                var klaviyoSubsribeProfileErrorResponse = JsonConvert.DeserializeObject<KlaviyoEventListSubscribeErrorResponse>(await response.Content.ReadAsStringAsync());
                if (klaviyoSubsribeProfileErrorResponse.errors.Any())
                    return klaviyoSubsribeProfileErrorResponse.errors.Select(x => x.detail).FirstOrDefault();
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return string.Empty;
    }

    /// <summary>
    /// Check if klaviyo profile exist by email
    /// </summary>
    /// <param name="email"></param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="newsLetterListId">newsLetterListId</param>
    /// <returns></returns>
    public async Task<bool> IsKlaviyoProfileExistByEmailAsync(string email, string privateAPIKey, string newsLetterListId)
    {
        if (string.IsNullOrEmpty(newsLetterListId) || string.IsNullOrEmpty(email))
            return false;

        try
        {
            var requestUri = $"https://a.klaviyo.com/api/lists/{newsLetterListId}/profiles?filter=equals(email,\"{email}\")";

            var requestHeaders = new Dictionary<string, string>();
            requestHeaders.Add("accept", "application/json");
            requestHeaders.Add("revision", "2024-07-15");
            requestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");

            var response = (await _httpClientService.GetAsync(requestUri, requestHeaders)).responseResult;

            if (string.IsNullOrEmpty(response) || response == "[]")
                return false;

            var responseResult = JsonConvert.DeserializeObject<KlaviyoProfileExistReponse>(response);
            if (response == null || responseResult.data == null || !responseResult.data.Any() || string.IsNullOrEmpty(responseResult.data.FirstOrDefault().id))
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return false;
    }

    /// <summary>
    /// Check if klaviyo profile exist by phone
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="smsListId">smsListId</param>
    /// <returns></returns>
    public async Task<bool> IsKlaviyoProfileExistByPhoneAsync(string phone, string privateAPIKey, string smsListId)
    {
        if (string.IsNullOrEmpty(smsListId) || string.IsNullOrEmpty(phone))
            return false;

        try
        {
            var requestUri = $"https://a.klaviyo.com/api/lists/{smsListId}/profiles?filter=equals(phone_number,\"{phone}\")";
            var requestHeaders = new Dictionary<string, string>();
            requestHeaders.Add("accept", "application/json");
            requestHeaders.Add("revision", "2024-07-15");
            requestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");

            var response = (await _httpClientService.GetAsync(requestUri, requestHeaders)).responseResult;

            if (string.IsNullOrEmpty(response) || response == "[]")
                return false;

            var responseResult = JsonConvert.DeserializeObject<KlaviyoProfileExistReponse>(response);
            if (responseResult == null || responseResult.data == null || !responseResult.data.Any() || string.IsNullOrEmpty(responseResult.data.FirstOrDefault().id))
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logService.InsertLog( ex.Message, ex?.ToString() ?? string.Empty);
        }

        return false;
    }

    /// <summary>
    /// AddToList 
    /// </summary>
    /// <param name="listId">listId</param>
    /// <param name="profileProperties">profileProperties</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <returns>ApiResponse</returns>
    public async Task<ApiResponse> AddToListAsync(string listId, IDictionary<string, object> profileProperties, string privateAPIKey)
    {
        if (string.IsNullOrEmpty(listId))
            return new ApiResponse();

        var requestUri = "https://a.klaviyo.com/api/lists/" + listId + "/relationships/profiles/";

        var requestParameter = new KlaviyoAddToListRequest
        {
            data = new List<KlaviyoAddToListRequest.Data>
            {
            new KlaviyoAddToListRequest.Data
                {
                    type = "profile",
                    id = await GetProfileIdByEmailAsync(profileProperties["email"].ToString(), privateAPIKey)
                }
            }
        };

        using var client = new HttpClient();

        // clear header
        client.DefaultRequestHeaders.Clear();

        // add request header                
        client.DefaultRequestHeaders.Add("Authorization", $"Klaviyo-API-Key {privateAPIKey}");
        client.DefaultRequestHeaders.Add("revision", "2024-07-15");

        var body = JsonConvert.SerializeObject(requestParameter);
        using var stringContent = new StringContent(body, Encoding.UTF8, "application/json");

        using var response = await client.PostAsync(requestUri, stringContent);

        var responseMessage = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseMessage);

        if (!response.IsSuccessStatusCode)
        {
            apiResponse.Error = !string.IsNullOrEmpty(apiResponse.Message) ? apiResponse.Message : responseMessage;
            _logService.InsertLog( apiResponse.Error);
        }

        return apiResponse;
    }

    #endregion
}