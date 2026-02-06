using System;

namespace Career.Data.Domains.Customers;

public class CustomerPassword : BaseEntity
{
    public int CustomerId { get; set; }

    public string Password { get; set; }

    public int PasswordFormatId { get; set; }

    public string PasswordSalt { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}
