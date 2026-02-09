using Middleware.Web.Data.Configuration;

namespace Middleware.Web.Domains.Common;

public class FMCommonSettings : ISettings
{
    #region Store Locator GeocodingAPIKey

    public string StoreLocatorGeocodingAPIKey { get; set; }

    #endregion
}
