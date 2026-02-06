namespace Middleware.Web.Data;

public interface INopSettingsRepository
{
    Task<string?> GetSettingValueAsync(string name, CancellationToken ct);
    Task SetSettingValueAsync(string name, string value, CancellationToken ct);
}

