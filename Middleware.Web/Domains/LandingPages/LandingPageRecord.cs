using System;

namespace Middleware.Web.Domains.LandingPages;
public class LandingPageRecord : BaseEntity
{
    public int LandingPageId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string City { get; set; }
    public int? StateProvinceId { get; set; }
    public int StoreId { get; set; }
    public string ZipCode { get; set; }
    public string Email { get; set; }
    public DateTime? DOB { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string InstagramHandle { get; set; }
    public string TwitterHandle { get; set; }
    public bool EventSubscribed { get; set; }
    public bool EmailSubscribed { get; set; }
    public bool SMSSubscribed { get; set; }
    public int? LocationId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}