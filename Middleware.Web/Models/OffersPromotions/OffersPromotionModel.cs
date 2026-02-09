namespace Middleware.Web.Models.OffersPromotions;

public record OffersPromotionModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Anchor { get; set; }

    public string Description { get; set; }
}