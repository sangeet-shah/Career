using Career.Data.Domains.Advertisements;
using Career.Data.Domains.Banner;
using Career.Data.Domains.Blogs;
using Career.Data.Domains.Common;
using Career.Data.Domains.CorporateManagement;
using Career.Data.Domains.Customers;
using Career.Data.Domains.DeliveryCharges;
using Career.Data.Domains.FMVendors;
using Career.Data.Domains.LandingPages;
using Career.Data.Domains.Locations;
using Career.Data.Domains.OffersPromotions;
using Career.Data.Domains.Stores;
using System;
using System.Collections.Generic;

namespace Career.Data.Mapping;

/// <summary>
/// Base instance of backward compatibility of table naming
/// </summary>
public partial class BaseNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new()
    {
        { typeof(FMCustomer), "FM_Customer" },
        { typeof(FMBlogPost), "FM_BlogPost" },
        { typeof(FMBlogPostAuthor), "FM_BlogPost_Author_Mapping" },
        { typeof(Advertisement), "FM_Advertisement" },
        { typeof(CorporateGallery), "FM_CorporateGallery" },
        { typeof(CorporateBrandPage), "FM_Corporate_BrandPage" },
        { typeof(CorporateGalleryPicture), "FM_CorporateGalleryPicture" },
        { typeof(GolfEventLandingPage), "FM_GolfEventLandingPage" },
        { typeof(LandingPage), "FM_LandingPage" },
        { typeof(LandingPageClosed), "FM_LandingPageClosed" },
        { typeof(LandingPageRecord), "FM_LandingPageRecord" },
        { typeof(Location), "FM_Location" },
        { typeof(LocationHour), "FM_LocationHour" },
        { typeof(OffersPromotion), "FM_OfferAndPromotion" },
        { typeof(SummerJam), "FM_SummerJam" },
        { typeof(FMVendor), "FM_Vendor" },
        { typeof(Banner), "FM_Banner" },
        { typeof(DeliveryCharge), "FM_DeliveryCharge" },
        { typeof(BannerDisplayTarget), "FM_BannerDisplayTarget" },
        { typeof(BannerLocationMapping), "FM_Banner_Location_Mapping" },        
        { typeof(FMStore), "FM_Store" },
        { typeof(HelloBars), "NopAdvance_HB_HelloBar" },
    };

    public Dictionary<(Type, string), string> ColumnName => new()
    {
			{ (typeof(Customer), "BillingAddressId"), "BillingAddress_Id" },
		    { (typeof(Customer), "ShippingAddressId"), "ShippingAddress_Id" },
		};
}
