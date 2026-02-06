using Career.Web.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Career.Web.Models.LandingPage;

public record LandingPageModel
{
    public LandingPageModel()
    {
        AvailableStoreLocation = new List<SelectListItem>();
        AvailableStates = new List<SelectListItem>();
        ClosedFormModel = new ClosedFormModel();
    }

    public int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }        
    
    public string Email { get; set; }
                     
    public string Phone { get; set; }        
    
    public DateTime? DateOfBirth { get; set; }

    public string StreetAddress { get; set; }

    public string City { get; set; }

    public int StateProvinceId { get; set; }
    public IList<SelectListItem> AvailableStates { get; set; }

    public string InstagramHandle { get; set; }

    public string TwitterHandle { get; set; }
    
    public string ZipPostalCode { get; set; }
    public bool EmailSubscribed { get; set; }
    public string Title { get; set; }
    public int PictureId { get; set; }
    public string PictureUrl { get; set; }
    public string Description { get; set; }
    public string DisclaimerText { get; set; }
    public bool ZipCodeVerification { get; set; }
    public bool IsActive { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }        
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool DisplayTitle { get; set; }

    public bool FirstNameEnabled { get; set; }

    public bool FirstNameRequired { get; set; }

    public bool LastNameEnabled { get; set; }

    public bool LastNameRequired { get; set; }

    public bool DateOfBirthEnabled { get; set; }

    public bool DateOfBirthRequired { get; set; }

    public bool DateOfBirth21 { get; set; }

    public bool EmailAddressEnabled { get; set; }

    public bool EmailAddressRequired { get; set; }

    public bool PhoneNumberEnabled { get; set; }

    public bool PhoneNumberRequired { get; set; }

    public bool StreetAddressEnabled { get; set; }

    public bool StreetAddressRequired { get; set; }

    public bool CityEnabled { get; set; }

    public bool CityRequired { get; set; }

    public bool StateEnabled { get; set; }

    public bool StateRequired { get; set; }

    public bool ZipEnabled { get; set; }

    public bool ZipRequired { get; set; }

    public bool InstagramHandleEnabled { get; set; }

    public bool InstagramHandleRequired { get; set; }

    public bool TwitterHandleEnabled { get; set; }

    public bool TwitterHandleRequired { get; set; }

    public bool StoreDropdownEnabled { get; set; }

    public bool StoreDropdownRequired { get; set; }

    public int LocationId { get; set; }

    public IList<SelectListItem> AvailableStoreLocation { get; set; }

    public int LandingPageId { get; set; }

    public string MobilePictureUrl { get; set; }

    public string MobileDescription { get; set; }

    public ClosedFormModel ClosedFormModel { get; set; }

    public bool ClosedFormEnabled { get; set; }

    public int LandingPageTypeId { get; set; }

    public string LandingPageFormFieldsSortOrder { get; set; }

    public bool SubscribedEventList { get; set; }
    
    public bool EventListEnabled { get; set; }
    
    public bool NewsLetterEnabled { get; set; }
    
    public bool SMSSubscribed { get; set; }
    
    public bool SMSEnabled { get; set; }
    
    public bool IsMobileDevice { get; set; }

    public bool EventFlowEnabled { get; set; }

    public string EventFlow { get; set; }

    public int ContestId { get; set; }
}
