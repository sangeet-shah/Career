using Career.Data.Domains.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Career.Web.Models.Customers;

public record TestCustomerModel
{
    public Guid CustomerGuid { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(NopDefaults.EmailValidationExpression, ErrorMessage = "Wrong email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public string UAGoogleAnalyticsId { get; set; }

    public string GTMGoogleAnalyticsId { get; set; }

    public string GoogleSiteVerification { get; set; }
}
