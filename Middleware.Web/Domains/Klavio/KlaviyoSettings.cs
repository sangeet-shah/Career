using Career.Data.Configuration;

namespace Career.Data.Domains.Klaviyo;

public class KlaviyoSettings : ISettings
{
    /// <summary>
    /// Gets or sets enable
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets public api key
    /// </summary>
    public string PublicAPIKey { get; set; }

    /// <summary>
    /// Gets or sets private api key
    /// </summary>
    public string PrivateAPIKey { get; set; }     

    /// <summary>
    /// Gets or sets news letter list id
    /// </summary>
    public string NewsLetterListId { get; set; }

    /// <summary>
    /// Gets or sets out of stock product request email id
    /// </summary>
    public string OutOfStockProductRequestEmailId { get; set; }

    /// <summary>
    /// Gets or sets out of stock product request list id
    /// </summary>
    public string OutOfStockProductRequestListId { get; set; }

    /// <summary>
    /// Gets or sets sms list id
    /// </summary>
    public string SMSListId { get; set; }

    /// <summary>
    /// Gets or sets event list id
    /// </summary>
    public string EventListId { get; set; }

    /// <summary>
    /// Gets or sets event newsletter list id
    /// </summary>
    public string EventNewsletterListId { get; set; }
}
