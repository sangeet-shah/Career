using System;

namespace Career.Data.Domains.LandingPages;
public class GolfEventLandingPage : BaseEntity
{
    public string CompanyName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public int SponsorshipLevelId { get; set; }
    public string Contact1 { get; set; }
    public string Contact2 { get; set; }
    public string Contact3 { get; set; }
    public string Contact4 { get; set; }
    public int PictureId { get; set; }
    public DateTime CreatedOnUtc { get; set; }

    public SponsorshipLevelEnum SponsorshipLevel
    {
        get => (SponsorshipLevelEnum)SponsorshipLevelId;
        set => SponsorshipLevelId = (int)value;
    }
}