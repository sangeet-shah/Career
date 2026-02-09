using Newtonsoft.Json;

namespace Middleware.Web.Models.ScheduleOrder;

public record FulfillmentRequest
{
    [JsonProperty(PropertyName = "fulfillmentLocation")]
    public string FulfillmentLocation { get; set; }

    [JsonProperty(PropertyName = "routeCode")]
    public string RouteCode { get; set; }

    [JsonProperty(PropertyName = "routeStartDate")]
    public DateTime? RouteStartDate { get; set; }

    [JsonProperty(PropertyName = "routeEndDate")]
    public DateTime? RouteEndDate { get; set; }

    [JsonProperty(PropertyName = "deliveryStatus")]
    public string DeliveryStatus { get; set; }

    [JsonProperty(PropertyName = "fullyReservedFulfillmentsOnly")]
    public string FullyReservedFulfillmentsOnly { get; set; }

    [JsonProperty(PropertyName = "routeType")]
    public int RouteType { get; set; }
}
