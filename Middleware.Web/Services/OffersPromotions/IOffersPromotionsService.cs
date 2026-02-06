using Career.Data.Domains.OffersPromotions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.OffersPromotions;

/// <summary>
/// Represents the offers promotions service
/// </summary>
public interface IOffersPromotionsService
{
    /// <summary>
    /// Gets all active offers and promotions
    /// </summary>
    /// <returns></returns>
    Task<IList<OffersPromotion>> GetAllActiveOffersPromotionsCachedAsync();
}