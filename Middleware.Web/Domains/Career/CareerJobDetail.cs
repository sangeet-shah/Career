using System;

namespace Middleware.Web.Domains.Career;

public class CareerJobDetail
{
    public int JobId { get; set; }
    public string JobTitle { get; set; }
    public string JobSummaryTitle1 { get; set; }
    public string JobSummaryContent1 { get; set; }
    public string JobSummaryTitle2 { get; set; }
    public string JobSummaryContent2 { get; set; }
    public string JobSummaryTitle3 { get; set; }
    public string JobSummaryContent3 { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string StoreName { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string CurrentPageURL { get; set; }
    public DateTime PostedDate { get; set; }
    public string DepartmentName { get; set; }
    public int StoreId { get; set; }
    public int EmployementTypeId { get; set; }
    public string ZipCode { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int BaseSalaryId { get; set; }
    public decimal BaseSalaryAmount { get; set; }
    public string BaseSalary { get; set; }
    public string PrimaryStoreCurrencyCode { get; set; }

    public string Country { get; set; }

    public int DivisionId { get; set; }
}
