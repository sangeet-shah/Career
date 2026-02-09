using System;

namespace Middleware.Web.Domains.DeliveryCharges;

public class DeliveryCharge : BaseEntity
{
    /// <summary>
    /// Gets or sets the state/province identifier
    /// </summary>
    public int? StateProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the zip/postal code
    /// </summary>
    public string ZipPostalCode { get; set; }

    /// <summary>
    /// Gets or sets the days
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Gets or sets the route code
    /// </summary>
    public string RouteCode { get; set; }

    /// <summary>
    /// Gets or sets the charge
    /// </summary>
    public decimal Charge { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance update
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}
