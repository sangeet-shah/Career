using System;

namespace Career.Data.Domains.Career;

public class CareerDetail
{
    public int JobId { get; set; }
    public int DepartmentId { get; set; }
    public string JobTitle { get; set; }
    public bool IsNew { get; set; }
    public int StoreAddressID { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string JobSummary { get; set; }
    public string StoreOffice { get; set; }
    public DateTime CreatedDate { get; set; }
    public int DivisionId { get; set; }
}
