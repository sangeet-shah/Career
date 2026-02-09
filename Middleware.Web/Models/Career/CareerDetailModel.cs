namespace Middleware.Web.Models.Career;

public record CareerDetailModel
{
    public string JobId { get; set; }
    public int DepartmentId { get; set; }
    public string JobTitle { get; set; }
    public string JobCategory { get; set; }
    public bool IsNew { get; set; }
    public int StoreAddressID { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string JobSummary { get; set; }
    public string StoreOffice { get; set; }        
    public int WebsiteId { get; set; }
    public string DatePostedString { get; set; }
}
