using Middleware.Web.Data;
using Middleware.Web.Options;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Http.Headers;

namespace Middleware.Web.Services;

public sealed class NopCacheClient : INopCacheClient
{
    private readonly HttpClient _http;
    private readonly INopSettingsRepository _settings;
    private readonly MiddlewareOptions _opt;

    public NopCacheClient(HttpClient http, 
        INopSettingsRepository settings, 
        IOptions<MiddlewareOptions> opt)
    {
        _http = http;
        _settings = settings;
        _opt = opt.Value;
    }

    public async Task ClearAllProductCacheAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_opt.NopCacheClearUrl))
            throw new InvalidOperationException("Middleware:NopCacheClearUrl is not configured.");

        if (string.IsNullOrWhiteSpace(_opt.NopCacheApiKeySettingName))
            throw new InvalidOperationException("Middleware:NopCacheApiKeySettingName is not configured.");

        var apiKey = await _settings.GetSettingValueAsync(_opt.NopCacheApiKeySettingName, ct);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException($"XApiKey not found in dbo.Setting for name '{_opt.NopCacheApiKeySettingName}'.");

        using var req = new HttpRequestMessage(HttpMethod.Get, _opt.NopCacheClearUrl);
        req.Headers.Add("XApiKey", apiKey);

        // optional: accept json/text
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var res = await _http.SendAsync(req, ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
        {
            Log.Warning("Clear cache call failed. Status={Status} Body={Body}", (int)res.StatusCode, body);
            res.EnsureSuccessStatusCode();
        }

        Log.Information("Clear cache call success. Status={Status}", (int)res.StatusCode);
    }
}
