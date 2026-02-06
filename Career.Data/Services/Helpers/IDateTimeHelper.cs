using System;

namespace Career.Data.Services.Helpers;

/// <summary>
/// Represents a datetime helper
/// </summary>
public partial interface IDateTimeHelper
{
    /// <summary>
    /// Converts the date and time to current user date and time
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <param name="sourceDateTimeKind">The source datetimekind</param>
    /// <returns>
    /// Result contains a DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.
    /// </returns>
    DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind);

    /// <summary>
    /// Gets or sets a default store time zone
    /// </summary>
    TimeZoneInfo DefaultStoreTimeZone
    {
        get;
    }
}