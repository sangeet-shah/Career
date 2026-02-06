namespace Career.Data.Domains.FMVendors;

public class FMVendor : BaseEntity
{
    /// <summary>
    /// Gets or sets vender 
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets is corporate 
    /// </summary>
    public bool IsCorporate { get; set; }

    /// <summary>
    /// Gets or sets is corporate picture id 
    /// </summary>
    public int CorporatePictureId { get; set; }

    /// <summary>
    /// Gets or sets is corporate short description
    /// </summary>
    public string CorporateShortDescription { get; set; }
}
