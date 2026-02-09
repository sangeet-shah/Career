using System;

namespace Middleware.Web.Domains.Locations;

/// <summary>
/// Represents a location
/// </summary>
public class Location : BaseEntity
{
	/// <summary>
	/// Gets or sets the location identifier
	/// </summary>
	public int LocationId { get; set; }

	/// <summary>
	/// Gets or sets the name of the location
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the alias name of the location
	/// </summary>
	public string AliasName { get; set; }

	/// <summary>
	/// Gets or sets the FMUSA display name
	/// </summary>
	public string FMUSAName { get; set; }

	/// <summary>
	/// Gets or sets the description of the location
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// Gets or sets the address identifier (maps to an existing record in the Address table)
	/// </summary>
	public int AddressId { get; set; }

	/// <summary>
	/// Gets or sets the meta description for SEO
	/// </summary>
	public string MetaDescription { get; set; }

	/// <summary>
	/// Gets or sets the meta title for SEO
	/// </summary>
	public string MetaTitle { get; set; }

	/// <summary>
	/// Gets or sets the latitude of the location
	/// </summary>
	public string Latitude { get; set; }

	/// <summary>
	/// Gets or sets the longitude of the location
	/// </summary>
	public string Longitude { get; set; }

	/// <summary>
	/// Gets or sets whether the location is published (enabled)
	/// </summary>
	public bool Published { get; set; }

	/// <summary>
	/// Gets or sets the picture ID
	/// </summary>
	public int PictureId { get; set; }

	/// <summary>
	/// Gets or sets the mobile picture ID
	/// </summary>
	public int MobilePictureId { get; set; }

	/// <summary>
	/// Gets or sets the banner picture ID
	/// </summary>
	public int BannerPictureId { get; set; }

	/// <summary>
	/// Gets or sets whether to display store hours
	/// </summary>
	public bool ShowStoreHours { get; set; }

	/// <summary>
	/// Gets or sets whether to display pickup hours
	/// </summary>
	public bool ShowPickupHours { get; set; }

	/// <summary>
	/// Gets or sets the tour video URL
	/// </summary>
	public string TourVideoUrl { get; set; }

	/// <summary>
	/// Gets or sets the Google Maps URL for the location
	/// </summary>
	public string GoogleUrl { get; set; }

	/// <summary>
	/// Gets or sets the Facebook page URL
	/// </summary>
	public string FacebookUrl { get; set; }

	/// <summary>
	/// Gets or sets the Twitter profile URL
	/// </summary>
	public string TwitterUrl { get; set; }

	/// <summary>
	/// Gets or sets the Pinterest profile URL
	/// </summary>
	public string PinterestUrl { get; set; }

	/// <summary>
	/// Gets or sets the LinkedIn profile URL
	/// </summary>
	public string LinkedInUrl { get; set; }

	/// <summary>
	/// Gets or sets the instagram profile URL
	/// </summary>
	public string InstagramUrl { get; set; }

	/// <summary>
	/// Gets or sets the youtube profile URL
	/// </summary>
	public string YouTubeUrl { get; set; }

	/// <summary>
	/// Gets or sets the Google Maps directions URL
	/// </summary>
	public string GoogleDirectionUrl { get; set; }

	/// <summary>
	/// Gets or sets the division ID (Website ID)
	/// </summary>
	public int WebsiteId { get; set; }

	/// <summary>
	/// Gets or sets whether this location is a pickup point for FM
	/// </summary>
	public bool DisplayAsPickupForFM { get; set; }

	/// <summary>
	/// Gets or sets whether this location is a pickup point for UCF
	/// </summary>
	public bool DisplayAsPickupForUCF { get; set; }

	/// <summary>
	/// Gets or sets whether this location is a pickup point for FSS
	/// </summary>
	public bool DisplayAsPickupForFSS { get; set; }

	/// <summary>
	/// Gets or sets the name of the store manager
	/// </summary>
	public string StoreManager { get; set; }

	/// <summary>
	/// Gets or sets the name of the assistant manager
	/// </summary>
	public string AssistantManager { get; set; }

	/// <summary>
	/// Gets or sets the name of the office supervisor
	/// </summary>
	public string OfficeSupervisor { get; set; }

	/// <summary>
	/// Gets or sets the name of the warehouse supervisor
	/// </summary>
	public string WarehouseSupervisor { get; set; }

	/// <summary>
	/// Gets or sets whether the location is verified
	/// </summary>
	public bool Verified { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the location was verified (in UTC)
	/// </summary>
	public DateTime? VerifiedDateTimeUtc { get; set; }

	/// <summary>
	/// Gets or sets the email address of the verifier
	/// </summary>
	public string VerifiedByEmail { get; set; }

	/// <summary>
	/// Gets or sets the store URL
	/// </summary>
	public string StoreUrl { get; set; }

	/// <summary>
	/// Gets or sets the first SEO picture ID
	/// </summary>
	public int SEOPictureId1 { get; set; }

	/// <summary>
	/// Gets or sets the second SEO picture ID
	/// </summary>
	public int SEOPictureId2 { get; set; }

	/// <summary>
	/// Gets or sets the third SEO picture ID
	/// </summary>
	public int SEOPictureId3 { get; set; }

	/// <summary>
	/// Gets or sets the unique UKG identifier (GUID)
	/// </summary>
	public string UKGGuid { get; set; }

	/// <summary>
	/// Gets or sets whether a custom area is enabled for the location
	/// </summary>
	public bool EnableCustomArea { get; set; }

	/// <summary>
	/// Gets or sets the custom area description
	/// </summary>
	public string CustomArea { get; set; }

	/// <summary>
	///  Gets or sets the store opening year
	/// </summary>
	public int? StoreOpeningYear { get; set; }

	/// <summary>
	///  Gets or sets the store opening month
	/// </summary>
	public int? StoreOpeningMonth { get; set; }

	/// <summary>
	///  Gets or sets the store opening date
	/// </summary>
	public int? StoreOpeningDay { get; set; }

}