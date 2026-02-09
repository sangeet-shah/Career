namespace Career.Web.Domains.Common;

/// <summary>
/// Shared constants (replaced Career.Data.Domains.Common.NopDefaults when removing Career.Data).
/// </summary>
public static class NopDefaults
{
    public static string AuthenticationKey => "authentication";
    public const string EmailValidationExpression = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,63}$";
}
