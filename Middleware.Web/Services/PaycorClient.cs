using Middleware.Web.Data;
using Middleware.Web.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Http.Headers;

namespace Middleware.Web.Services;

public sealed class PaycorClient : IPaycorClient
{
    private readonly HttpClient _http;
    private readonly PaycorEmployeeOptions _opt;

    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private PaycorTokenCache? _tokenCache;

    public PaycorClient(HttpClient http, IOptions<PaycorEmployeeOptions> opt)
    {
        _http = http;
        _opt = opt.Value;
    }

    public async Task<string?> GetEmployeesWithHoursAndPunchesAsync(CancellationToken ct)
    {
        // 1) fetch employee list
        var employeeJson = await GetAllEmployeesAsync(ct);
        if (string.IsNullOrWhiteSpace(employeeJson))
            return null;

        var root = JObject.Parse(employeeJson);
        if (root["records"] is not JArray employees || employees.Count == 0)
            return null;

        // 2) token once (then reused)
        var token = await GetAccessTokenAsync(ct);

        // 3) bounded concurrency (each employee = 2 calls)
        var max = Math.Max(1, _opt.MaxConcurrency);
        using var gate = new SemaphoreSlim(max, max);

        var startDate = DateTime.UtcNow.Date.AddDays(-Math.Max(1, _opt.DaysBack));
        var endDate = DateTime.UtcNow.Date;

        var tasks = new List<Task<JObject>>(employees.Count);

        foreach (var emp in employees)
        {
            var employeeId = emp?["id"]?.ToString();
            if (string.IsNullOrWhiteSpace(employeeId))
                continue;

            tasks.Add(FetchOneAsync(emp, employeeId, token, startDate, endDate, gate, ct));
        }

        var results = await Task.WhenAll(tasks);

        return new JObject
        {
            ["employees"] = new JArray(results)
        }.ToString(Formatting.None);
    }

    private static JObject EmptyRecordsObject() => new() { ["records"] = new JArray() };

    private async Task<JObject> FetchOneAsync(
        JToken? employee,
        string employeeId,
        string token,
        DateTime startDateUtc,
        DateTime endDateUtc,
        SemaphoreSlim gate,
        CancellationToken ct)
    {
        await gate.WaitAsync(ct);
        try
        {
            var hoursTask = GetEmployeeHoursAsync(token, employeeId, startDateUtc, endDateUtc, ct);
            var punchesTask = GetEmployeePunchesAsync(token, employeeId, startDateUtc, endDateUtc, ct);
            var timecardTask = GetEmployeeTimecardAsync(token, employeeId, startDateUtc, endDateUtc, ct);

            await Task.WhenAll(hoursTask, punchesTask, timecardTask);

            JObject hoursObj;
            if (string.IsNullOrWhiteSpace(hoursTask.Result))
                hoursObj = EmptyRecordsObject();
            else
                hoursObj = JObject.Parse(hoursTask.Result!);

            JObject punchesObj;
            if (string.IsNullOrWhiteSpace(punchesTask.Result))
                punchesObj = EmptyRecordsObject();
            else
                punchesObj = JObject.Parse(punchesTask.Result!);

            JObject timcardObj;
            if (string.IsNullOrWhiteSpace(timecardTask.Result))
                timcardObj = EmptyRecordsObject();
            else
                timcardObj = JObject.Parse(timecardTask.Result!);

            return new JObject
            {
                ["employee"] = employee,
                ["hours"] = hoursObj,
                ["punches"] = punchesObj,
                ["timecard"] = timcardObj
            };
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            // Log and keep employee in payload so stage isn't missing
            Log.Warning(ex, "Paycor fetch failed for employeeId={EmployeeId}", employeeId);

            return new JObject
            {
                ["employee"] = employee,
                ["hours"] = EmptyRecordsObject(),
                ["punches"] = EmptyRecordsObject(),
                ["timecard"] = EmptyRecordsObject()
            };
        }
        finally
        {
            gate.Release();
        }
    }


    private async Task<string?> GetAllEmployeesAsync(CancellationToken ct)
    {
        var token = await GetAccessTokenAsync(ct);
        var url = $"v1/legalentities/{_opt.LegalEntityId}/employees?include=All";
        return await SendAsync(HttpMethod.Get, url, token, ct);
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        // refresh early by 1 minute
        if (_tokenCache != null && _tokenCache.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
            return _tokenCache.AccessToken;

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (_tokenCache != null && _tokenCache.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
                return _tokenCache.AccessToken;

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"sts/v1/common/token?subscription-key={_opt.SubscriptionKey}")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = _opt.RefreshToken,
                    ["client_id"] = _opt.ClientId,
                    ["client_secret"] = _opt.ClientSecret
                })
            };

            var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadAsStringAsync(ct);
            var token = JsonConvert.DeserializeObject<PaycorAPITokenResponse>(payload)
                        ?? throw new InvalidOperationException("Paycor token response was empty");

            _tokenCache = new PaycorTokenCache
            {
                AccessToken = token.AccessToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn)
            };

            return token.AccessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private Task<string?> GetEmployeeHoursAsync(
        string token,
        string employeeId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct)
    {
        var url = $"v1/employees/{employeeId}/employeeHours?startDate={startUtc:yyyy-MM-dd}&endDate={endUtc:yyyy-MM-dd}";
        return SendAsync(HttpMethod.Get, url, token, ct);
    }

    private Task<string?> GetEmployeePunchesAsync(
        string token,
        string employeeId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct)
    {
        var url = $"v1/employees/{employeeId}/employeePunches?startDate={startUtc:yyyy-MM-dd}&endDate={endUtc:yyyy-MM-dd}";
        return SendAsync(HttpMethod.Get, url, token, ct);
    }

    public Task<string?> GetEmployeeTimecardAsync(
        string token,
        string employeeId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct)
    {        
        var url = $"v1/employees/{employeeId}/timecard?startDate={startUtc:yyyy-MM-dd}&endDate={endUtc:yyyy-MM-dd}";
        return SendAsync(HttpMethod.Get, url, token,ct);
    }

    private async Task<string?> SendAsync(HttpMethod method, string url, string accessToken, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("Ocp-Apim-Subscription-Key", _opt.SubscriptionKey);

        var response = await _http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            Log.Warning("Paycor request failed. Url={Url} Status={Status} Body={Body}", url, (int)response.StatusCode, body);
            return null;
        }

        return await response.Content.ReadAsStringAsync(ct);
    }
}
