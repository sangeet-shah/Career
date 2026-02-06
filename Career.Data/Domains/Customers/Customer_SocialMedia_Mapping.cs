namespace Career.Data.Domains.Customers;

/// <summary>
/// Represents a customer-social media mapping class
/// </summary>
public class Customer_SocialMedia_Mapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the social media enum identifier
    /// </summary>
    public int SocialMediaId { get; set; }

    /// <summary>
    /// Gets or sets the social media URL
    /// </summary>
    public string SocialMediaUrl { get; set; }
}