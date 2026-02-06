using Newtonsoft.Json;
using System;

namespace Career.Web.Models.ScheduleOrder;

public record UpdateOrderRequest
{
    public UpdateOrderRequest()
    {
        order = new UpdatedOrder();
    }

    [JsonProperty(PropertyName = "orderId")]
    public int OrderId { get; set; }

    [JsonProperty(PropertyName = "order")]
    public UpdatedOrder order { get; set; }

    public class UpdatedOrder
    {
        public UpdatedOrder()
        {
            ShippingAddress = new Address();
        }

        [JsonProperty(PropertyName = "deliveryDate")]
        public DateTime? DeliveryDate { get; set; }

        [JsonProperty(PropertyName = "deliveryInstructions")]
        public string DeliveryInstructions { get; set; }

        [JsonProperty(PropertyName = "orderComments")]
        public string OrderComments { get; set; }

        [JsonProperty(PropertyName = "shippingAddress")]
        public Address ShippingAddress { get; set; }
    }

    public class Address
    {
        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "zipCode")]
        public string ZipCode { get; set; }
    }
}
