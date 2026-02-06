using System.ComponentModel;

namespace Career.Data.Domains.ScheduleOrder;

/// <summary>
/// Represents an route type enumeration
/// </summary>
public enum RouteType
{
    /// <summary>
    /// DeliveryRoute
    /// </summary>
    [Description("Delivery Route")]
    DeliveryRoute = 1,

    /// <summary>
    /// ServiceRoute
    /// </summary>
    [Description("Service Route")]
    ServiceRoute = 2,

    /// <summary>
    /// TransferRoute 
    /// </summary>
    [Description("Transfer Route")]
    TransferRoute = 3
}
