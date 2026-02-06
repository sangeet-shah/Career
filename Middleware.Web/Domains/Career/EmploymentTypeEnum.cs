using System.ComponentModel;

namespace Career.Data.Domains.Career;

// Employement Type 
public enum EmploymentTypeEnum
{
    /// <summary>
    /// FULL_TIME
    /// </summary>
    [Description("Full Time")]
    FULL_TIME = 1,

    /// <summary>
    /// PART_TIME
    /// </summary>
    [Description("Part Time")]
    PART_TIME = 2,

    /// <summary>
    /// CONTRACTOR
    /// </summary>
    [Description("Contractor")]
    CONTRACTOR = 3,

    /// <summary>
    /// TEMPORARY
    /// </summary>
    [Description("Temporary")]
    TEMPORARY = 4,

    /// <summary>
    /// INTERN
    /// </summary>
    [Description("Intern")]
    INTERN = 5,

    /// <summary>
    /// VOLUNTEER
    /// </summary>
    [Description("Volunteer")]
    VOLUNTEER = 6,

    /// <summary>
    /// PER_DIEM
    /// </summary>
    [Description("Per Diem")]
    PER_DIEM = 7,

    /// <summary>
    /// OTHER
    /// </summary>
    [Description("Other")]
    OTHER = 8
}
