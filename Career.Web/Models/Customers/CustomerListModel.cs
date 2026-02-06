using System.Collections.Generic;

namespace Career.Web.Models.Customers;

public record CustomerListModel
{
    public CustomerListModel()
    {
        CustomerList = new List<CustomerModel>();
    }

    public IList<CustomerModel> CustomerList { get; set; }
}
