using System.Threading.Tasks;

namespace Career.Data.Services.Localization;

/// <summary>
/// Localization service interface
/// </summary>
public interface ILocalizationService
{
    Task<string> GetLocaleStringResourceByNameAsync(string resourceName);
}
