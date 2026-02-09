using System;

namespace Middleware.Web.Domains.LandingPages;

public class LandingPage : BaseEntity
{
    public int LandingPageTypeId { get; set; }
    public string Title { get; set; }
    public bool DisplayTitle { get; set; }
    public int StoreId { get; set; }
    public string Url { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public int BannerPictureId { get; set; }
    public int BannerMobilePictureId { get; set; }
    public string Disclaimer { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string Description { get; set; }
    public string MobileDescription { get; set; }
    public bool MaxRegistrationEnabled { get; set; }
    public int? MaxRegistrations { get; set; }
    public bool EventFlowEnabled { get; set; }
    public string EventFlow { get; set; }
    public bool EventListEnabled { get; set; }
    public bool NewsLetterEnabled { get; set; }
    public bool SMSEnabled { get; set; }
    public bool FirstNameEnabled { get; set; }
    public bool FirstNameRequired { get; set; }
    public bool LastNameEnabled { get; set; }
    public bool LastNameRequired { get; set; }
    public bool DOBEnabled { get; set; }
    public bool DOBRequired { get; set; }
    public bool EmailEnabled { get; set; }
    public bool EmailRequired { get; set; }
    public bool PhoneNumberEnabled { get; set; }
    public bool PhoneNumberRequired { get; set; }
    public bool AddressEnabled { get; set; }
    public bool AddressRequired { get; set; }
    public bool CityEnabled { get; set; }
    public bool CityRequired { get; set; }
    public bool StateProvinceEnabled { get; set; }
    public bool StateProvinceRequired { get; set; }
    public bool ZipCodeEnabled { get; set; }
    public bool ZipCodeRequired { get; set; }
    public bool ZipCodeVerification { get; set; }
    public bool InstagrameHandleEnabled { get; set; }
    public bool InstagrameHandleRequired { get; set; }
    public bool TwitterHandleEnabled { get; set; }
    public bool TwitterHandleRequired { get; set; }
    public bool LocationEnabled { get; set; }
    public bool LocationRequired { get; set; }
    public bool Published { get; set; }
    public string FormFieldsDisplayOrder { get; set; }
    public LandingPageTypeEnum LandingPageType
    {
        get => (LandingPageTypeEnum)LandingPageTypeId;
        set => LandingPageTypeId = (int)value;
    }
}