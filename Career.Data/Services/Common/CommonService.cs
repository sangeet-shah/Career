using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Career.Data.Services.Common;

public class CommonService : ICommonService
{
    #region Fields

    private readonly IHttpContextAccessor _httpContextAccessor;

    #endregion

    #region Ctor

    public CommonService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion        

    #region methods

    /// <summary>
    /// Get enum description value
    /// </summary>
    /// <param name="value">value</param>
    /// <returns>description value</returns>
    public string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        if (attributes != null && attributes.Any())
            return attributes.First().Description;

        return value.ToString();
    }

    // <summary>
    /// Get a value indicating whether the request is made by mobile device
    /// </summary>
    /// <returns></returns>
    public bool IsMobileDevice()
    {
        if (_httpContextAccessor?.HttpContext == null)
            return false;
        try
        {
            var ua = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            //string ret = "";
            // Check if user agent is a smart TV - http://goo.gl/FocDk
            if (Regex.IsMatch(ua, @"GoogleTV|SmartTV|Internet.TV|NetCast|NETTV|AppleTV|boxee|Kylo|Roku|DLNADOC|CE\-HTML", RegexOptions.IgnoreCase))
                return false;
            // Check if user agent is a TV Based Gaming Console
            else if (Regex.IsMatch(ua, "Xbox|PLAYSTATION.3|Wii", RegexOptions.IgnoreCase))
                return false;
            // Check if user agent is a Tablet
            if ((Regex.IsMatch(ua, "iP(a|ro)d", RegexOptions.IgnoreCase) || (Regex.IsMatch(ua, "tablet", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "RX-34", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "FOLIO", RegexOptions.IgnoreCase))))
                return false;
            // Check if user agent is an Android Tablet
            else if ((Regex.IsMatch(ua, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Android", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Fennec|mobi|HTC.Magic|HTCX06HT|Nexus.One|SC-02B|fone.945", RegexOptions.IgnoreCase)))
                return true;
            // Check if user agent is a Kindle or Kindle Fire
            else if ((Regex.IsMatch(ua, "Kindle", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "Mac.OS", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Silk", RegexOptions.IgnoreCase)))
                return true;
            // Check if user agent is a pre Android 3.0 Tablet
            else if ((Regex.IsMatch(ua, @"GT-P10|SC-01C|SHW-M180S|SGH-T849|SCH-I800|SHW-M180L|SPH-P100|SGH-I987|zt180|HTC(.Flyer|\\_Flyer)|Sprint.ATP51|ViewPad7|pandigital(sprnova|nova)|Ideos.S7|Dell.Streak.7|Advent.Vega|A101IT|A70BHT|MID7015|Next2|nook", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "MB511", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "RUTEM", RegexOptions.IgnoreCase)))
                return true;
            // Check if user agent is unique Mobile User Agent
            else if ((Regex.IsMatch(ua, "BOLT|Fennec|Iris|Maemo|Minimo|Mobi|mowser|NetFront|Novarra|Prism|RX-34|Skyfire|Tear|XV6875|XV6975|Google.Wireless.Transcoder", RegexOptions.IgnoreCase)))
                return true;
            // Check if user agent is an odd Opera User Agent - http://goo.gl/nK90K
            else if ((Regex.IsMatch(ua, "Opera", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Windows.NT.5", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, @"HTC|Xda|Mini|Vario|SAMSUNG\-GT\-i8000|SAMSUNG\-SGH\-i9", RegexOptions.IgnoreCase)))
                return true;
            // Check if user agent is Windows Desktop
            else if ((Regex.IsMatch(ua, "Windows.(NT|XP|ME|9)")) && (!Regex.IsMatch(ua, "Phone", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "Win(9|.9|NT)", RegexOptions.IgnoreCase)))
                return false;
            // Check if agent is Mac Desktop
            else if ((Regex.IsMatch(ua, "Macintosh|PowerPC", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Silk", RegexOptions.IgnoreCase)))
                return false;
            // Check if user agent is a Linux Desktop
            else if ((Regex.IsMatch(ua, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "X11", RegexOptions.IgnoreCase)))
                return false;
            // Check if user agent is a Solaris, SunOS, BSD Desktop
            else if ((Regex.IsMatch(ua, "Solaris|SunOS|BSD", RegexOptions.IgnoreCase)))
                return false;
            // Check if user agent is a Desktop BOT/Crawler/Spider
            else if ((Regex.IsMatch(ua, "Bot|Crawler|Spider|Yahoo|ia_archiver|Covario-IDS|findlinks|DataparkSearch|larbin|Mediapartners-Google|NG-Search|Snappy|Teoma|Jeeves|TinEye", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Mobile", RegexOptions.IgnoreCase)))
                return false;
            // Otherwise assume it is a Mobile Device
            else
                return true;
        }
        catch
        {
            // ignored
        }
        return false;
    }

    /// <summary>
    /// Converts the date and time to current user date and time
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <param name="sourceDateTimeKind">The source datetimekind</param>
    /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
    public  DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind)
    {
        dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
        if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
            return dt;

        return TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
    }

    #endregion

}
