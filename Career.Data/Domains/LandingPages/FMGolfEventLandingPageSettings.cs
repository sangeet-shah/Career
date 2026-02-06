using Career.Data.Configuration;

namespace Career.Data.Domains.LandingPages;
public class FMGolfEventLandingPageSettings : ISettings
{
    public bool Enabled { get; set; }
    public string Emails { get; set; }
    public string Description { get; set; }
}