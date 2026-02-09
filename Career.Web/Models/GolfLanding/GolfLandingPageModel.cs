using Career.Web.Domains.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Career.Web.Models.GolfLanding;

public record GolfLandingPageModel
{
    public GolfLandingPageModel()
    {
        AvailableSponsorshipLevels = new List<SelectListItem>();
    }

    [Required(ErrorMessage = "Company name is required")]
    public string CompanyName { get; set; }

    [Required(ErrorMessage = "Main contact phone number is required")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Main contact email address is required.")]
    [RegularExpression(NopDefaults.EmailValidationExpression, ErrorMessage = "Wrong email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "sponsorship level is required")]
    public int SponsorshipLevelId { get; set; }
    public IList<SelectListItem> AvailableSponsorshipLevels { get; set; }

    public string Contact1 { get; set; }

    public string Contact2 { get; set; }

    public string Contact3 { get; set; }

    public string Contact4 { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public bool Success { get; set; }

    public bool IsActive { get; set; }

    public string Description { get; set; }

    [Display(Name = "Upload current high-res logo if applicable")]
    public int PictureId { get; set; }
}