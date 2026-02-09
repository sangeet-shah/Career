using System;

namespace Middleware.Web.Domains.LandingPages;
public class SummerJam : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public int StateProvinceId { get; set; }
    public string ZipCode { get; set; }
    public string Phone { get; set; }
    public DateTime? DOB { get; set; }
    public int StoreId { get; set; }
    public DateTime CreatedDateUtc { get; set; }
}