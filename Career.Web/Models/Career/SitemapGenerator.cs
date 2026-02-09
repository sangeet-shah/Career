using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Career.Web.Models.Career;

public record SitemapGenerator
{
    private readonly IApiClient _apiClient;

    public SitemapGenerator(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Fetch the sitemap xml from middleware API.
    /// </summary>
    public async Task<string> GenerateAsync(IUrlHelper urlHelper)
    {
        var baseUrl = $"{urlHelper.ActionContext.HttpContext.Request.Scheme}://{urlHelper.ActionContext.HttpContext.Request.Host.Value}";
        return await _apiClient.GetAsync<string>("api/Career/SitemapXml", new { baseUrl }) ?? string.Empty;
    }

    /// <summary>
    /// Write the sitemap xml to a stream.
    /// </summary>
    public async Task GenerateAsync(IUrlHelper urlHelper, Stream stream)
    {
        var xml = await GenerateAsync(urlHelper);
        var bytes = Encoding.UTF8.GetBytes(xml);
        await stream.WriteAsync(bytes, 0, bytes.Length);
    }
}
