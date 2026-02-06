using Career.Data.Domains.PaycorAPI;

namespace Career.Data;

public class AppSettings
{
    public AppSettings()
    {
        PaycorAPISettings = new PaycorAPISettings();
    }
    public bool IsTestSite { get; set; }

    public string FAQUserName { get; set; }

    public string FAQPassword { get; set; }

    public string PhysicalStoreListByStateOrder { get; set; }

    public string fmebridgeUrl { get; set; }

    public ConnectionString ConnectionStrings { get; set; }

    public class ConnectionString
    {
        public string DbConnection { get; set; }
    }

    public PaycorAPISettings PaycorAPISettings { get; set; }
}
