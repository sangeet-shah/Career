namespace Career.Data.Domains.Customers;

public class FMCustomer : BaseEntity
{
    /// <summary>
    /// Gets or sets customer id 
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets is organization
    /// </summary>
    public bool IsOrganization { get; set; }

    /// <summary>
    /// Gets or sets biography
    /// </summary>
    public string Biography { get; set; }

    /// <summary>
    /// Gets or sets customer id 
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets is FMUSA Bio
    /// </summary>
    public bool FMUSABio { get; set; }

    /// <summary>
    /// Gets or sets linkedIn url
    /// </summary>
    public string LinkedInUrl { get; set; }

    /// <summary>
    /// Gets or sets facebook url
    /// </summary>
    public string FacebookUrl { get; set; }

    /// <summary>
    /// Gets or sets instagram url
    /// </summary>
    public string InstagramUrl { get; set; }

    /// <summary>
    /// Gets or sets pinterest url
    /// </summary>
    public string PinterestUrl { get; set; }

    /// <summary>
    /// Gets or sets twitter url
    /// </summary>
    public string TwitterUrl { get; set; }

    /// <summary>
    /// Gets or sets other url
    /// </summary>
    public string OtherUrl { get; set; }

    /// <summary>
    /// Gets or sets author position
    /// </summary>
    public string AuthorPosition { get; set; }
}
