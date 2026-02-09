using Newtonsoft.Json;

namespace Middleware.Web.Models.ScheduleOrder;

public record PaymentRequest
{
    [JsonProperty(PropertyName = "CCVN")]
    public string CCVN { get; set; }

    [JsonProperty(PropertyName = "creditCardNumber")]
    public string CreditCardNumber { get; set; }

    [JsonProperty(PropertyName = "customerId")]
    public string CustomerId { get; set; }

    [JsonProperty(PropertyName = "emvToken")]
    public string EmvToken { get; set; }

    [JsonProperty(PropertyName = "expirationMonth")]
    public string ExpirationMonth { get; set; }

    [JsonProperty(PropertyName = "expirationYear")]
    public string ExpirationYear { get; set; }

    [JsonProperty(PropertyName = "locationId")]
    public string LocationId { get; set; }

    [JsonProperty(PropertyName = "overrideBlockPayment")]
    public bool OverrideBlockPayment { get; set; }

    [JsonProperty(PropertyName = "paymentAmount")]
    public decimal PaymentAmount { get; set; }

    [JsonProperty(PropertyName = "paymentType")]
    public string PaymentType { get; set; }
}
