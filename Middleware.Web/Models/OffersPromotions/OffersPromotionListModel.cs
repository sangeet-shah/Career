namespace Middleware.Web.Models.OffersPromotions;

public record OffersPromotionListModel
{
    public OffersPromotionListModel()
    {
        OffersPromotions = new List<OffersPromotionModel>();
    }

    public IList<OffersPromotionModel> OffersPromotions { get; set; }
}