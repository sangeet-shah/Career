using System;

namespace Career.Data.Domains.Career;

public class JobApplication : BaseEntity
{
    public string ApplicationContent { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int JobId { get; set; }
}
