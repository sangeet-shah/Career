using System.ComponentModel;

namespace Middleware.Web.Domains.ScheduleOrder;

/// <summary>
/// Represents an address type enumeration
/// </summary>
public enum AddressType
{
    /// <summary>
    /// Undefined
    /// </summary>
    [Description("Undefined")]
    Undefined = 0,

    /// <summary>
    /// Customer
    /// </summary>
    [Description("Customer")]
    Customer = 1,

    /// <summary>
    /// Billing 
    /// </summary>
    [Description("Billing")]
    Billing = 2,

    /// <summary>
    /// Shipping
    /// </summary>
    [Description("Shipping")]
    Shipping = 3
}
