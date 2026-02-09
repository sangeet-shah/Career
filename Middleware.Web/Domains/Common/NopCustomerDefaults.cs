namespace Middleware.Web.Domains.Common;

/// <summary>
/// Represents default values related to customers data
/// </summary>
public static partial class NopCustomerDefaults
{
    #region System customer roles

    /// <summary>
    /// Gets a system name of 'author' customer role
    /// </summary>
    public static string AuthorRoleName => "Author";

    /// <summary>
    /// Gets a system name of 'ourteam' customer role
    /// </summary>
    public static string OurTeamRoleName => "OurTeam";

    #endregion

    #region Customer attributes

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'AvatarPictureId'
    /// </summary>
    public static string AvatarPictureIdAttribute => "AvatarPictureId";    

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'TimeZoneId'
    /// </summary>
    public static string TimeZoneIdAttribute => "TimeZoneId";

    #endregion
}