using Middleware.Web.Models.Blogs;
using Middleware.Web.Models.Career;
using Middleware.Web.Models.Vendors;

namespace Middleware.Web.Models.CorporateManagement;

public record CorporateManagementSettingsModel
{
    public CorporateManagementSettingsModel()
    {
        CorporateVendors = new List<VendorModel>();
        BlogPostList = new List<BlogPostModel>();
        CareerBrandListModel = new List<CareerBrandModel>();
    }

    public int HomeWebBannerId { get; set; }

    public string HomeWebBannerUrl { get; set; }

    public string HomeMobileBannerUrl { get; set; }

    public string HomeBannerAltText { get; set; }

    public string HomeBannerTitle { get; set; }

    public string HomeBannerLink { get; set; }

    public string ContentDescription { get; set; }

    public string ContentMobileDescription { get; set; }

    public int LeftWebBannerId { get; set; }

    public string LeftWebBannerUrl { get; set; }

    public string LeftMobileBannerUrl { get; set; }

    public string LeftBannerAltText { get; set; }

    public string LeftBannerTitle { get; set; }

    public string LeftBannerLink { get; set; }

    public int RightWebBannerId { get; set; }

    public string RightWebBannerUrl { get; set; }

    public string RightMobileBannerUrl { get; set; }

    public string RightBannerAltText { get; set; }

    public string RightBannerTitle { get; set; }

    public string RightBannerLink { get; set; }

    public int NumberOfNewsArticles { get; set; }

    public int OurStoryBanner { get; set; }

    public string OurStoryBannerUrl { get; set; }

    public string OurStoryBannerAltText { get; set; }

    public string OurStoryBannerTitle { get; set; }

    public int OurStoryMobileBanner { get; set; }

    public string OurStoryMobileBannerUrl { get; set; }

    public string OurStoryMobileBannerAltText { get; set; }

    public string OurStoryMobileBannerTitle { get; set; }

    public string OurStoryTitle { get; set; }

    public string OurStoryFullDescription { get; set; }

    public int OurBrandsImage1 { get; set; }

    public string OurBrandsImage1Url { get; set; }

    public string OurBrandsImage1AltText { get; set; }

    public string OurBrandsImage1Title { get; set; }

    public string OurBrandsShortDescription1 { get; set; }

    public string OurBrandsUrl1 { get; set; }

    public int OurBrandsImage2 { get; set; }

    public string OurBrandsImage2Url { get; set; }

    public string OurBrandsImage2AltText { get; set; }

    public string OurBrandsImage2Title { get; set; }

    public string OurBrandsShortDescription2 { get; set; }

    public string OurBrandsUrl2 { get; set; }

    public int OurBrandsImage3 { get; set; }

    public string OurBrandsImage3Url { get; set; }

    public string OurBrandsImage3AltText { get; set; }

    public string OurBrandsImage3Title { get; set; }

    public string OurBrandsShortDescription3 { get; set; }

    public string OurBrandsUrl3 { get; set; }

    public int OurBrandsImage4 { get; set; }

    public string OurBrandsImage4Url { get; set; }

    public string OurBrandsImage4AltText { get; set; }

    public string OurBrandsImage4Title { get; set; }

    public string OurBrandsShortDescription4 { get; set; }

    public string OurBrandsUrl4 { get; set; }

    public IList<VendorModel> CorporateVendors { get; set; }

    public string PhotoGalleryTitle { get; set; }

    public bool IsMobile { get; set; }

    public IList<BlogPostModel> BlogPostList { get; set; }

    public string FacebookImageUrl { get; set; }

    public string FacebookImageAltText { get; set; }

    public string FacebookImageTitle { get; set; }

    public string FacebookURL { get; set; }

    public string YouTubeImageUrl { get; set; }

    public string YouTubeImageAltText { get; set; }

    public string YouTubeImageTitle { get; set; }

    public string YouTubeURL { get; set; }

    public string TwitterImageUrl { get; set; }

    public string TwitterImageAltText { get; set; }

    public string TwitterImageTitle { get; set; }

    public string TwitterURL { get; set; }

    public string LinkedInImageUrl { get; set; }

    public string LinkedInImageAltText { get; set; }

    public string LinkedInImageTitle { get; set; }

    public string LinkedInURL { get; set; }

    public IList<CareerBrandModel> CareerBrandListModel { get; set; }
}
