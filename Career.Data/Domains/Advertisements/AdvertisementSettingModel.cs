using Career.Data.Configuration;

namespace Career.Data.Domains.Advertisements;

public class AdvertisementSettingModel : ISettings
{
    public string WeeklyAdEcommPlugin { get; set; }

    public string WeeklyAdProductCode { get; set; }
    public string CatalogAdEcommPlugin { get; set; }
}
