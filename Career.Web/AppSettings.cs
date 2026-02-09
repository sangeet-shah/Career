namespace Career.Web;

/// <summary>
/// App settings bound from configuration (replaced Career.Data.AppSettings when removing Career.Data).
/// </summary>
public class AppSettings
{
    public bool IsTestSite { get; set; }
    public string fmebridgeUrl { get; set; } = "";
    public ConnectionString ConnectionStrings { get; set; }

    public class ConnectionString
    {
        public string DbConnection { get; set; }
    }
}
