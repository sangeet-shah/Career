namespace Career.Data.Domains.Banner;

public class BannerDisplayTarget : BaseEntity
{
    /// <summary>
    /// Gets or sets the banner type identifier
    /// </summary>
    public int BannerTypeId { get; set; }

    /// <summary>
    /// Gets or sets the banner name
    /// </summary>
    public string Name { get; set; }

    public BannerTypeEnum BannerType
    {
        get => (BannerTypeEnum)BannerTypeId;
        set => BannerTypeId = (int)value;
    }
}