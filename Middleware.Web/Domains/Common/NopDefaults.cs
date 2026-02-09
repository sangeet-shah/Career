using System;

namespace Middleware.Web.Domains.Common;

public class NopDefaults
{
    public static string AuthenticationKey => "authentication";


    public const string EmailValidationExpression = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,63}$";        

    public static string TesterRoleName => "testers";

    public static string AdminRoleName => "administrators";

    public static string FAQAuthenticationKey => "FAQ-authentication";

    /// <summary>
    /// Gets the name of the default HTTP client
    /// </summary>
    public static string DefaultHttpClient => "default";

    /// <summary>
    /// Get difference in years
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
    {
        //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
        //this assumes you are looking for the western idea of age and not using East Asian reckoning.
        var age = endDate.Year - startDate.Year;
        if (startDate > endDate.AddYears(-age))
            age--;
        return age;
    }   
}
