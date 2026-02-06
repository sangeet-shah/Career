namespace Middleware.Web.Options;

public sealed class PaycorEmployeeOptions
{
    public string BaseUrl { get; init; } = "https://api.paycor.com/"; // confirm your real base
    public string SubscriptionKey { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public string ClientId { get; init; } = "";
    public string ClientSecret { get; init; } = "";
    public string LegalEntityId { get; init; } = "";
    public int DaysBack { get; init; } = 1;                 // default: last 1 day
    public int MaxConcurrency { get; init; } = 6;           // throttle Paycor calls
    public int HttpTimeoutSeconds { get; init; } = 100;
}

public sealed class PaycorJobOptions
{
    public string BaseUrl { get; init; } = "https://api.paycor.com/"; // confirm your real base
    public string SubscriptionKey { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public string ClientId { get; init; } = "";
    public string ClientSecret { get; init; } = "";
    public string LegalEntityId { get; init; } = "";
    public int MaxConcurrency { get; init; } = 6;           // throttle Paycor calls
    public int HttpTimeoutSeconds { get; init; } = 100;
}