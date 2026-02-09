using Middleware.Web.Data.Configuration;

namespace Middleware.Web.Domains.Stores;

public class FMStoreSettings : ISettings
{
    public string ApiGatewayKey { get; set; }
}
