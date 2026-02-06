namespace Career.Web.Models.GoogleAnalytics;

public record GoogleAnalyticsModel
{
    public string eventName { get; set; }

    public string method { get; set; }

    public string contentID { get; set; }

    public string contentType { get; set; }

    public string eventCategory { get; set; }

    public string eventLabel { get; set; }

    public string nonInteraction { get; set; }
}
