using System.Threading.Tasks;

namespace Middleware.Web.Services.Localization;

/// <summary>
/// Localization service interface
/// </summary>
public interface ILocalizationService
{
    Task<string> GetLocaleStringResourceByNameAsync(string resourceName);
}
