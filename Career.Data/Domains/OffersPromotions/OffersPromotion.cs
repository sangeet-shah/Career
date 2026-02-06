using System;

namespace Career.Data.Domains.OffersPromotions;

    public partial class OffersPromotion : BaseEntity
    {
	public string Title { get; set; }

	public bool LimitedToStores { get; set; }

	public string Anchor { get; set; }

	public string Description { get; set; }

	public int DisplayOrder { get; set; }

	public DateTime? StartDateUtc { get; set; }

	public DateTime? EndDateUtc { get; set; }
}
