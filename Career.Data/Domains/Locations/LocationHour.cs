namespace Career.Data.Domains.Locations;

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
    /// Gets or sets the opening hour
    /// </summary>
    public string OpeningHour { get; set; }

    /// <summary>
    /// Gets or sets the closing hour
    /// </summary>
    public string ClosingHour { get; set; }

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