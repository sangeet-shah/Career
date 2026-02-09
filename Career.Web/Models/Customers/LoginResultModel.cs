namespace Career.Web.Models.Customers;

public class LoginResultModel
{
    public bool IsValid { get; set; }

    public TestCustomerModel Model { get; set; } = new();
}
