using Career.Web.Domains.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Career.Web.Models.Career;

public partial record ApplicationFormModel
{
    public ApplicationFormModel()
    {
        States = new List<SelectListItem>();                        
        Education = new Education();
        WorkExperience = new WorkExperience();
    }
    public string JobTitle { get; set; }
    public string GreetingText { get; set; }
    public string ApplicationAuthorizationText { get; set; }
    public string Location { get; set; }
    public string StoreName { get; set; }
    public string Department { get; set; }
    public int JobId { get; set; }
    public string baseURL { get; set; }


    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; }

    [Required(ErrorMessage = "State is required")]
    public string State { get; set; }

    [Required(ErrorMessage = "Zipcode is required")]
    public string ZipCode { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$", ErrorMessage = "Please enter a valid mobile no")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Email is required")]        
    public string Email { get; set; }

    public string ResumeFilePath { get; set; }
    public string CoverLetterFilePath { get; set; }
    public string LinkedinURL { get; set; }
    public string FacebookURL { get; set; }
    public string PortFolioURL { get; set; }
    public Education Education { get; set; }
    public WorkExperience WorkExperience { get; set; }
    public string DesiredSalary { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime StartDate { get; set; }
    public string DoYouPreferPartTime { get; set; }
    public string CanYouWork { get; set; }
    public string PreferredDaysNotAvailable { get; set; }
    public string NotAvailable { get; set; }
    public bool AreYouAboveEighteen { get; set; }
    public string AffiliatedCompany { get; set; }
    public string AffiliatedCompanyYESorNO { get; set; }
    public string AffiliatedCompanyListString { get; set; }
    public string CanYouProvideDocuments { get; set; }
    public string DoYouHaveAnyCertifications { get; set; }
    public string DocumentList { get; set; }
    public string YouFindTheOpeningFrom { get; set; }
    public string ESignature { get; set; }
    public bool MayWeContactYourCurrentEmployer { get; set; }
    public bool IsReviewedApplication { get; set; }
    public IList<SelectListItem> States { get; set; }
    public bool ApplicationStatus { get; set; }
    public string EmployeeReferralName { get; set; }
    public string OtherOption { get; set; }

    public Reference Reference { get; set; }               
}

public class ResumeAndProfessional
{
    public string Other { get; set; }
}

public class Education
{
    public Education()
    {
        States = new List<SelectListItem>();
    }
    public string HighestEducation { get; set; }
    public string Institution { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime EndDate { get; set; }
    public string DegreeAccomplishments { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public IList<SelectListItem> States { get; set; }
    public int Count { get; set; }
}

public class Reference
{
    public string ReferenceName { get; set; }
    public string Relationship { get; set; }

    [RegularExpression(@"^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$", ErrorMessage = "Please enter a valid mobile no")]
    public string PhoneNumber { get; set; }
    
    public string ReferenceEmail { get; set; }
    public string ReferenceOtherOption { get; set; }
    public string ReferenceName1 { get; set; }
    public string Relationship1 { get; set; }

    [RegularExpression(@"^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$", ErrorMessage = "Please enter a valid mobile no")]
    public string PhoneNumber1 { get; set; }

    [RegularExpression(NopDefaults.EmailValidationExpression, ErrorMessage = "Wrong email")]
    public string ReferenceEmail1 { get; set; }
    public string ReferenceOtherOption1 { get; set; }

    public string ReferenceName2 { get; set; }
    public string Relationship2 { get; set; }

    [RegularExpression(@"^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$", ErrorMessage = "Please enter a valid mobile no")]
    public string PhoneNumber2 { get; set; }

    [RegularExpression(NopDefaults.EmailValidationExpression, ErrorMessage = "Wrong email")]
    public string ReferenceEmail2 { get; set; }
    public string ReferenceOtherOption2 { get; set; }
}

public class WorkExperience
{
    public WorkExperience()
    {
        States = new List<SelectListItem>();
    }        

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime EndDate { get; set; }
    public string JobTitle { get; set; }
    public string JobDuties { get; set; }
    public string Company { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PayUpOnLeaving { get; set; }
    public string ResionForLeaving { get; set; }
    public string Supervisor { get; set; }

    [RegularExpression(@"^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$", ErrorMessage = "Please enter a valid mobile no")]
    public string Phone { get; set; }
    public bool MayWeContactYourCurrentEmployer { get; set; }
    public IList<SelectListItem> States { get; set; }

    public int Count { get; set; }
}
public class SocialUrl
{
    public string OtherURL { get; set; }        

    public int Count { get; set; }
}
