using System;

namespace Middleware.Web.Services.Common;

public interface ICommonService
{
    #region Methods

    /// <summary>
    /// Get enum description value
    /// </summary>
    /// <param name="value">value</param>
    /// <returns>description value</returns>
    string GetEnumDescription(Enum value);

    // <summary>
    /// Get a value indicating whether the request is made by mobile device
    /// </summary>
    /// <returns></returns>
    bool IsMobileDevice();

    /// <summary>
    /// Converts the date and time to current user date and time
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <param name="sourceDateTimeKind">The source datetimekind</param>
    /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
    DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind);

    #endregion
}
