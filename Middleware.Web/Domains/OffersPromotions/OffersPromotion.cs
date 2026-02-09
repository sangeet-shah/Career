using System;

namespace Middleware.Web.Domains.OffersPromotions;

    public class OffersPromotion : BaseEntity
    {
	public string Title { get; set; }

	public bool LimitedToStores { get; set; }

	public string Anchor { get; set; }

	public string Description { get; set; }

	public int DisplayOrder { get; set; }

	public DateTime? StartDateUtc { get; set; }

	public DateTime? EndDateUtc { get; set; }
}
