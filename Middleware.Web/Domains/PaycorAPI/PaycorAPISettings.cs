namespace Career.Data.Domains.PaycorAPI;

public class PaycorAPISettings
{
    public string SubscriptionKey { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RefreshToken { get; set; }
    public string BaseUrl { get; set; }
    public string TokenEndpoint { get; set; }
    public string JobsEndpoint { get; set; }
}