using Career.Data.Domains.Messages;
using System.Threading.Tasks;

namespace Career.Data.Services.Messages;

/// <summary>
/// Newsletter subscription service interface
/// </summary>
public partial interface INewsLetterSubscriptionService
{
    /// <summary>
    /// Inserts a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">NewsLetter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);

    /// <summary>
    /// Gets a newsletter subscription by email and store ID
    /// </summary>
    /// <param name="email">The newsletter subscription email</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>NewsLetter subscription</returns>
    Task<NewsLetterSubscription> GetNewsLetterSubscriptionByEmailAndStoreIdAsync(string email, int storeId);

    /// <summary>
    /// Updates a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">NewsLetter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);
}
