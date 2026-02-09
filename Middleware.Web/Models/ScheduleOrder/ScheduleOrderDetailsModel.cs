namespace Middleware.Web.Models.ScheduleOrder;

public record ScheduleOrderDetailsModel
{
    public ScheduleOrderDetailsModel()
    {
        DeliverTo = new Address();
        Items = new List<OrderItemModel>();
        CardPaymentInfo = new CardPaymentInfoModel();
    }

    public int OrderId { get; set; }

    public string ZipCode { get; set; }

    public string PhoneNumber { get; set; }

    public string ErrorMessage { get; set; }

    public bool IsZipCodeVerified { get; set; }

    public DateTime OrderDate { get; set; }

    public Address DeliverTo { get; set; }

    public IList<OrderItemModel> Items { get; set; }

    public decimal MerchandiseTotal { get; set; }

    public decimal DeliveryCharge { get; set; }

    public decimal Tax { get; set; }

    public decimal TotalSalesOrder { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal AmountDue { get; set; }

    public CardPaymentInfoModel CardPaymentInfo { get; set; }

    #region Nested Classes

    public partial class OrderItemModel
    {
        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }

    public class Address
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CardPaymentInfoModel
    {
        public string CardholderName { get; set; }

        public string CardNumber { get; set; }

        public string ExpireMonth { get; set; }

        public string ExpireYear { get; set; }

        public string CardCode { get; set; }
    }

    #endregion
}
