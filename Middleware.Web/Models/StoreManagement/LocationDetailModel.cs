using Middleware.Web.Domains.PhysicalStores;

namespace Middleware.Web.Models.StoreManagement;

public record LocationDetailModel
{
    public LocationDetailModel()
    {
        StoreInfoModel = new StoreInfoModels();
        PhysicalStoreBannerModel = new BannerModel();
        PhysicalStoreReviews = new List<GoogleResponseCache.PhysicalStoreReview>();
        SchemaPictureUrls = new List<string>();
    }
    public int Id { get; set; }

    public string StoreOpenCloseMessage { get; set; }

    public string StorePictureURL { get; set; }

    public string StoreAddresFromDb { get; set; }
    public string StoreHoursFromDb { get; set; }
    public string PickupHoursFromDb { get; set; }
    public string GoogleUrlFromDb { get; set; }
    public string FacebookUrlFromDb { get; set; }
    public string TwitterUrlFromDb { get; set; }
    public string PinterestUrlFromDb { get; set; }
    public string LinkedInUrlFromDb { get; set; }
    public string InstagramUrlFromDb { get; set; }
    public string YouTubeUrlFromDb { get; set; }

    public StoreInfoModels StoreInfoModel { get; set; }
    public string StoreName { get; set; }
    public string TourVideoLink { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public int StoreId { get; set; }
    public int LocationId { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string Zipcode { get; set; }

    public string Description { get; set; }
    public string StoreDetail { get; set; }
    public string StoreAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }

    public string PhoneNumber { get; set; }
    public BannerModel PhysicalStoreBannerModel { get; set; }

    public IList<string> SchemaPictureUrls { get; set; }

    public string Vicinity { get; set; }

    public double? Rating { get; set; }

    public double? TotalGoogleRating { get; set; }

    public string GooglePhysicalStoreName { get; set; }

    public string Place_id { get; set; }
    public string LocationFaceBookIconUrl { get; set; }
    public string LocationGoogleIconUrl { get; set; }
    public string LocationTwitterIconUrl { get; set; }
    public string LocationPinterestIconUrl { get; set; }
    public string LocationLinkedInIconUrl { get; set; }
    public string LocationInstagramIconUrl { get; set; }
    public string LocationYouTubeIconUrl { get; set; }

    public IList<GoogleResponseCache.PhysicalStoreReview> PhysicalStoreReviews { get; set; }

    public record BannerModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        public string ImageUrl { get; set; }
    }

    public record StoreInfoModels
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int PictureId { get; set; }
        public string RedirectionURL { get; set; }
        public int StoreId { get; set; }
        public string PictureURL { get; set; }
    }
}
