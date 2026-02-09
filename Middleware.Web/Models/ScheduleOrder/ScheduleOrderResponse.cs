namespace Middleware.Web.Models.ScheduleOrder;

public record ScheduleOrderResponse
{
    public string message { get; set; }
    public int status { get; set; }
    public SalesOrder salesOrder { get; set; }
}

public record Address
{
    public int addressType { get; set; }
    public string name { get; set; }
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zipCode { get; set; }

    public string phoneNumber { get; set; } = "7894561230";
}

public record LineItem
{
    public DateTime? ATPDate { get; set; }
    public string LineItemNumber { get; set; }
    public string brandId { get; set; }
    public string categoryDescription { get; set; }
    public string categoryId { get; set; }
    public string description { get; set; }
    public string directShipTrackingNumber { get; set; }
    public string id { get; set; }
    public object images { get; set; }
    public int orderLineType { get; set; }
    public string poLineKey { get; set; }
    public double price { get; set; }
    public string productPoDate { get; set; }
    public string productTransferDate { get; set; }
    public int quantity { get; set; }
    public int reservedQuantity { get; set; }
    public string trackingNumber { get; set; }
    public string vendorModelNumber { get; set; }
    public string webCategoryDescription { get; set; }
    public string webCategoryId { get; set; }
}

public record OrderTotals
{
    public double balance { get; set; }
    public double discount { get; set; }
    public double delivery { get; set; }
    public double fees { get; set; }
    public double install { get; set; }
    public double invoiceTotal { get; set; }
    public double payments { get; set; }
    public double subTotal { get; set; }
    public double tax { get; set; }
}

public record SpecialOrderInformation
{
    public string comment { get; set; }
    public string info1 { get; set; }
    public string info2 { get; set; }
    public string info3 { get; set; }
    public string info4 { get; set; }
    public string label1 { get; set; }
    public string label2 { get; set; }
    public string label3 { get; set; }
    public string label4 { get; set; }
}

public record SalesOrder
{
    public List<Address> addresses { get; set; }
    public string customerId { get; set; }
    public DateTime? deliveryDate { get; set; }
    public string deliveryStatus { get; set; }
    public object deliveryTime { get; set; }
    public List<LineItem> lineItems { get; set; }
    public string locationId { get; set; }
    public DateTime orderDate { get; set; }
    public string orderId { get; set; }
    public OrderTotals orderTotals { get; set; }
    public int orderType { get; set; }
    public object shipDate { get; set; }
    public string shippingInstructions { get; set; }
    public List<SpecialOrderInformation> specialOrderInformation { get; set; }
    public string voidReasonCode { get; set; }
    public string voidReasonDescription { get; set; }
}
