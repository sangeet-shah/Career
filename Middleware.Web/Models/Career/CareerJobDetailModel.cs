namespace Middleware.Web.Models.Career;

public record CareerJobDetailModel
{
    public string JobId { get; set; }
    public string JobTitle { get; set; }
    public string JobSummaryContent { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }

    public string StoreName { get; set; }
    public string CurrentPageURL { get; set; }
    public string baseURL { get; set; }
    public DateTime PostedDate { get; set; }
    public string JobCategory { get; set; }
    public string ZipCode { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal BaseSalaryAmount { get; set; }
    public string BaseSalary { get; set; }
    public string BaseSalaryUnitText { get; set; }
    public string PrimaryStoreCurrencyCode { get; set; }

    public string Type { get; set; }

    public bool IsMobile { get; set; }
}
