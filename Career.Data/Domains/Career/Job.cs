using System;

namespace Career.Data.Domains.Career;

public class Job : BaseEntity
{
    public string JobTitle { get; set; }
    public string JobSummaryTitle1 { get; set; }
    public string JobSummaryContent1 { get; set; }
    public string JobSummaryTitle2 { get; set; }
    public string JobSummaryContent2 { get; set; }
    public string JobSummaryTitle3 { get; set; }
    public string JobSummaryContent3 { get; set; }
    public int DepartmentId { get; set; }
    public bool Published { get; set; }
    public bool MarkAsNew { get; set; }
    public bool IsFeatured { get; set; }
    public int PictureId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int EmployementTypeId { get; set; }
    public int PhysicalStoreId { get; set; }
    public int BaseSalaryId { get; set; }
    public decimal BaseSalaryAmount { get; set; }
    public int ParentId { get; set; }
    public bool Deleted { get; set; }
}
