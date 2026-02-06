namespace Middleware.Web.Options;

public sealed class MiddlewareOptions
{
    public int BatchSize { get; set; } = 500;
    public int CommandTimeoutSeconds { get; set; } = 60;

    public string NopCacheClearUrl { get; set; } = "";
    public string LastProductSyncSettingName { get; set; } = "";
    public string NopCacheApiKeySettingName { get; set; } = "";

    public string XApiKey { get; set; } = "";
}
