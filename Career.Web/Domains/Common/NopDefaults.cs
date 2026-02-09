using System;

namespace Career.Web.Domains.Common;

/// <summary>
/// Shared constants (replaced Career.Data.Domains.Common.NopDefaults when removing Career.Data).
/// </summary>
public static class NopDefaults
{
    public static string AuthenticationKey => "authentication";
    public const string EmailValidationExpression = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,63}$";

    /// <summary>
    /// Get difference in years.
    /// </summary>
    public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
    {
        var age = endDate.Year - startDate.Year;
        if (startDate > endDate.AddYears(-age))
            age--;
        return age;
    }
}

/// <summary>
/// Customer-related constants for OurTeam and attributes.
/// </summary>
public static class NopCustomerDefaults
{
    public const string AuthorRoleName = "Author";
    public const string OurTeamRoleName = "OurTeam";
    public const string AvatarPictureIdAttribute = "AvatarPictureId";
}
