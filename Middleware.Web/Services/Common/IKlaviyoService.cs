using Career.Data.Domains.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Common;

public interface IKlaviyoService
{
    /// <summary>
    /// Subscribe email
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="newsLetterListId">newsLetterListId</param>
    /// <returns>ApiResponse</returns>
    Task<string> SubscribeEmailAsync(string email, string privateAPIKey, string newsLetterListId);

    /// <summary>
    /// UnSubscribe email
    /// </summary>
    /// <param name="email">email</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="newsLetterListId">newsLetterListId</param>
    /// <returns>ApiResponse</returns>
    Task<string> UnSubscribeEmailAsync(string email, string privateAPIKey, string newsLetterListId);

    /// <summary>
    /// Subscribe sms
    /// </summary>
    /// <param name="phone">phone</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="smsListId">smsListId</param>
    /// <returns>ApiResponse</returns>
    Task<string> SubscribeSMSAsync(string phone, string privateAPIKey, string smsListId);

    /// <summary>
    /// UnSubscribe sms
    /// </summary>
    /// <param name="phone">phone</param>
    /// <returns>ApiResponse</returns>
    Task<string> UnSubscribeSMSAsync(string phone, string privateAPIKey, string smsListId);

    /// <summary>
    /// Subscribe event list
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="email">Email</param>
    /// <param name="phone">Phone</param>
    /// <param name="ZipPostalCode">Zip postal code</param>
    /// <param name="city">City</param>
    /// <param name="stateId">State identifier</param>
    /// <param name="eventName">Event name</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="eventListId">eventListId</param>
    /// <returns>ApiResponse</returns>
    Task<string> SubscribeEventListAsync(string firstName, string lastName, string email, string phone, string ZipPostalCode, string city, int stateId, string eventName, string privateAPIKey, string eventListId);

    /// <summary>
    /// Check if klaviyo profile exist by email
    /// </summary>
    /// <param name="email"></param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="newsLetterListId">newsLetterListId</param>
    /// <returns></returns>
    Task<bool> IsKlaviyoProfileExistByEmailAsync(string email, string privateAPIKey, string newsLetterListId);

    /// <summary>
    /// Check if klaviyo profile exist by phone
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <param name="smsListId">smsListId</param>
    /// <returns></returns>
    Task<bool> IsKlaviyoProfileExistByPhoneAsync(string phone, string privateAPIKey, string smsListId);

    /// <summary>
    /// AddToList 
    /// </summary>
    /// <param name="listId">listId</param>
    /// <param name="profileProperties">profileProperties</param>
    /// <param name="privateAPIKey">privateAPIKey</param>
    /// <returns>ApiResponse</returns>
    Task<ApiResponse> AddToListAsync(string listId, IDictionary<string, object> profileProperties, string privateAPIKey);
}
