using Middleware.Web.Data.Configuration;

namespace Middleware.Web.Domains.Advertisements;

public class AdvertisementSettingModel : ISettings
{
    public string WeeklyAdEcommPlugin { get; set; }

    public string WeeklyAdProductCode { get; set; }
    public string CatalogAdEcommPlugin { get; set; }
}
