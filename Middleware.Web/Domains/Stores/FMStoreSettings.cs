using Career.Data.Configuration;

namespace Career.Data.Domains.Stores;

public class FMStoreSettings : ISettings
{
    public string ApiGatewayKey { get; set; }
}
