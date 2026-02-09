namespace Middleware.Web.Infrastructure;

/// <summary>
/// Sets the current store alias for the request from X-Store-Alias header (e.g. FMUSA).
/// Used by StoreService to resolve store from FM_Store by Alias for store-wise LoadSettingAsync.
/// </summary>
public class StoreAliasMiddleware
{
    public const string StoreAliasHeaderName = "XStoreAlias";
    public const string StoreAliasAltHeaderName = "X-Store-Alias";
    public const string StoreAliasItemsKey = "StoreAlias";

    private readonly RequestDelegate _next;

    public StoreAliasMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var alias = context.Request.Headers[StoreAliasHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(alias))
            alias = context.Request.Headers[StoreAliasAltHeaderName].FirstOrDefault();

        context.Items[StoreAliasItemsKey] = alias?.Trim();
        return _next(context);
    }
}
