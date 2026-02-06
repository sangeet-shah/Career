using Career.Data.Data;
using Career.Data.Domains.Messages;
using Career.Data.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Messages;

/// <summary>
/// Newsletter subscription service
/// </summary>
public class NewsLetterSubscriptionService : INewsLetterSubscriptionService
{
    #region Fields

    private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionService(IRepository<NewsLetterSubscription> subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inserts a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">NewsLetter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    public async Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        if (newsLetterSubscription == null)
        {
            throw new ArgumentNullException(nameof(newsLetterSubscription));
        }

        //Handle e-mail
        newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

        //Persist
        await _subscriptionRepository.InsertAsync(newsLetterSubscription);

        ////Publish the subscription event 
        //if (newsLetterSubscription.Active)
        //    PublishSubscriptionEvent(newsLetterSubscription, true, publishSubscriptionEvents);
    }

    /// <summary>
    /// Gets a newsletter subscription by email and store ID
    /// </summary>
    /// <param name="email">The newsletter subscription email</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>NewsLetter subscription</returns>
    public async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByEmailAndStoreIdAsync(string email, int storeId)
    {
        if (!CommonHelper.IsValidEmail(email))
            return null;

        return await (from nls in _subscriptionRepository.Table
                      where nls.Email == email.Trim() && nls.StoreId == storeId
                      orderby nls.Id
                      select nls).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Updates a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">NewsLetter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    public async Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        if (newsLetterSubscription == null)
        {
            throw new ArgumentNullException(nameof(newsLetterSubscription));
        }

        //Handle e-mail
        newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

        ////Get original subscription record
        //var originalSubscription = await _subscriptionRepository.LoadOriginalCopy(newsLetterSubscription);

        //Persist
        await _subscriptionRepository.UpdateAsync(newsLetterSubscription);

        ////Publish the subscription event 
        //if ((originalSubscription.Active == false && newsLetterSubscription.Active) ||
        //    (newsLetterSubscription.Active && originalSubscription.Email != newsLetterSubscription.Email))
        //{
        //    //If the previous entry was false, but this one is true, publish a subscribe.
        //    await PublishSubscriptionEventAsync(newsLetterSubscription, true, publishSubscriptionEvents);
        //}

        //if (originalSubscription.Active && newsLetterSubscription.Active &&
        //    originalSubscription.Email != newsLetterSubscription.Email)
        //{
        //    //If the two emails are different publish an unsubscribe.
        //    await PublishSubscriptionEventAsync(originalSubscription, false, publishSubscriptionEvents);
        //}

        //if (originalSubscription.Active && !newsLetterSubscription.Active)
        //    //If the previous entry was true, but this one is false
        //    await PublishSubscriptionEventAsync(originalSubscription, false, publishSubscriptionEvents);
    }

    #endregion
}