namespace Middleware.Web.Domains.Locations;

public class LocationHour : BaseEntity
{
    /// <summary>
    /// Gets or sets the location identifier
    /// </summary>
    public int LocationId { get; set; }

    /// <summary>
    /// Gets or sets the hour type identifier (e.g., regular hours, holiday hours)
    /// </summary>
    public int HourTypeId { get; set; }

    /// <summary>
    /// Gets or sets the day identifier (e.g., Monday, Tuesday)
    /// </summary>
    public int DayId { get; set; }

    /// <summary>
    /// Gets or sets the opening hour (stored as SQL TIME -> maps to TimeSpan)
    /// </summary>
    public TimeSpan? OpeningHour { get; set; }

    /// <summary>
    /// Gets or sets the closing hour (stored as SQL TIME -> maps to TimeSpan)
    /// </summary>
    public TimeSpan? ClosingHour { get; set; }

    public HourTypeEnum HourType
    {
        get => (HourTypeEnum)HourTypeId;
        set => HourTypeId = (int)value;
    }

    public DaysEnum Day
    {
        get => (DaysEnum)DayId;
        set => DayId = (int)value;
    }
}