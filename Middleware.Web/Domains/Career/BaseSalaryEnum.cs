using System.ComponentModel;

namespace Middleware.Web.Domains.Career;

public enum BaseSalaryEnum
{
    /// <summary>
    /// Base salary not to show
    /// </summary>
    [Description("")]
    NOT_SHOWN = 1,

    /// <summary>
    /// Base salary per hour
    /// </summary>
    [Description("HOUR")]
    HOUR = 2,

    /// <summary>
    /// Base salary per Day
    /// </summary>
    [Description("Daily")]
    DAY = 3,

    /// <summary>
    /// Base salary per week
    /// </summary>
    [Description("Weekly")]
    WEEK = 4,

    /// <summary>
    /// Base salary per month
    /// </summary>
    [Description("Monthly")]
    MONTH = 5,

    /// <summary>
    /// Base salary per year
    /// </summary>
    [Description("Yearly")]
    YEAR = 6,
}
