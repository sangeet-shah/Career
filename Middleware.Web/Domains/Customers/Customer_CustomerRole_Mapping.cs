namespace Middleware.Web.Domains.Customers;

public class Customer_CustomerRole_Mapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int Customer_Id { get; set; }

    /// <summary>
    /// Gets or sets the customer role identifier
    /// </summary>
    public int CustomerRole_Id { get; set; }
}
