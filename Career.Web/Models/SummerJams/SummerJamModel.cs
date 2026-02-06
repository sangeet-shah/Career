using Career.Data.Domains.Common;
using Career.Data.Domains.LandingPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Career.Web.Models.SummerJams;

public record SummerJamModel
{
    public SummerJamModel()
    {
        States = new List<SelectListItem>();
        FMUSASummerJamSetting = new FMSummerJamSettings();
    }
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Valid email address is required.")]
    [RegularExpression(NopDefaults.EmailValidationExpression, ErrorMessage = "Wrong email")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Address line1 is required.")]
    public string AddressLine1 { get; set; }

    public string AddressLine { get; set; }
    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; }
    [Required(ErrorMessage = "State is required.")]
    public string State { get; set; }

    public string StateName { get; set; }

    [Required(ErrorMessage = "Zip is required.")]
    public string ZipCode { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$", ErrorMessage = "This phone number is invalid or not from USA")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid Date Format"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    [MinAge(18)] // 18 is the parameter of constructor.         
    public DateTime? DOB { get; set; }
    public int StoreId { get; set; }
    public DateTime? CreatedDate { get; set; }

    public bool Success { get; set; }
    public IList<SelectListItem> States { get; set; }
    public FMSummerJamSettings FMUSASummerJamSetting { get; set; }

    public class MinAge : ValidationAttribute
    {
        private int _Limit;
        public MinAge(int Limit)
        { // The constructor which we use in modal.
            this._Limit = Limit;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return null;

            DateTime bday = DateTime.Parse(value.ToString());
            DateTime today = DateTime.Today;
            int age = today.Year - bday.Year;
            if (bday > today.AddYears(-age))
            {
                age--;
            }
            if (age < _Limit)
            {
                var result = new ValidationResult("Must be 18 years or older to be eligible for this registration.");
                return result;
            }
            return null;

        }
    }
}
