namespace Middleware.Web.Services;

public interface INopCacheClient
{
    Task ClearAllProductCacheAsync(CancellationToken ct);
}

