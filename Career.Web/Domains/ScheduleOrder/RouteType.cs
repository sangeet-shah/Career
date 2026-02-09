using System.ComponentModel;

namespace Career.Web.Domains.ScheduleOrder;

/// <summary>
/// Represents a route type enumeration.
/// </summary>
public enum RouteType
{
    /// <summary>
    /// Delivery route.
    /// </summary>
    [Description("Delivery Route")]
    DeliveryRoute = 1,

    /// <summary>
    /// Service route.
    /// </summary>
    [Description("Service Route")]
    ServiceRoute = 2,

    /// <summary>
    /// Transfer route.
    /// </summary>
    [Description("Transfer Route")]
    TransferRoute = 3
}
