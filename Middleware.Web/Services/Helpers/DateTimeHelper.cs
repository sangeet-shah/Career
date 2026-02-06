using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using System;
using System.Diagnostics;

namespace Middleware.Web.Services.Helpers;

/// <summary>
/// Represents a datetime helper
/// </summary>
public class DateTimeHelper : IDateTimeHelper
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public DateTimeHelper(ISettingService settingService,
        IStoreService storeService)
    {
        _settingService = settingService;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
    /// </summary>
    /// <param name="id">The time zone identifier, which corresponds to the System.TimeZoneInfo.Id property.</param>
    /// <returns>A System.TimeZoneInfo object whose identifier is the value of the id parameter.</returns>
    protected  TimeZoneInfo FindTimeZoneById(string id)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Converts the date and time to current user date and time
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <param name="sourceDateTimeKind">The source datetimekind</param>
    /// <returns>
    /// Result contains a DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.
    /// </returns>
    public  DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind)
    {
        dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
        if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
            return dt;

        return TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
    }

    /// <summary>
    /// Gets or sets a default store time zone
    /// </summary>
    public  TimeZoneInfo DefaultStoreTimeZone
    {
        get
        {
            TimeZoneInfo timeZoneInfo = null;
            try
            {
                var dateTimeSettings = _settingService.LoadSettingAsync<DateTimeSettings>((_storeService.GetCurrentStoreAsync()).Result?.Id ?? 0).Result;
                if (!string.IsNullOrEmpty(dateTimeSettings.DefaultStoreTimeZoneId))
                    timeZoneInfo = FindTimeZoneById(dateTimeSettings.DefaultStoreTimeZoneId);
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return timeZoneInfo ?? TimeZoneInfo.Local;
        }
    }

    #endregion
}