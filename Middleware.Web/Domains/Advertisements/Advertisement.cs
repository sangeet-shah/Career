using System;

namespace Middleware.Web.Domains.Advertisements;

public class Advertisement : BaseEntity
{
    /// <summary>
    /// Gets or sets the title of the advertisement
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the store id for the advertisement
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the URL for the advertisement
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the start date in UTC
    /// </summary>
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the end date in UTC
    /// </summary>
    public DateTime? EndDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the advertisement type id
    /// </summary>
    public int AdTypeId { get; set; }

    /// <summary>
    /// Geta or sets Published 
    /// </summary>
    public bool Published { get; set; }

    public AdvertisementTypeEnum AdType
    {
        get => (AdvertisementTypeEnum)AdTypeId;
        set => AdTypeId = (int)value;
    }
}