using System.ComponentModel;

namespace Middleware.Web.Domains.Media;

public enum DisplaySections
{
    /// <summary>
    /// FM top banner on home page
    /// </summary>
    FM_HomePage_TopBanner = 1,

    /// <summary>
    /// FM top side banner 1 on home page
    /// </summary>
    FM_HomePage_SideBanner1 = 2,

    /// <summary>
    /// FM top side banner 2 on home page
    /// </summary>
    FM_HomePage_SideBanner2 = 3,

    /// <summary>
    /// FM banner 2 on home page
    /// </summary>
    FM_HomePage_Banner2 = 4,

    /// <summary>
    /// FM display limited time deals on home page 
    /// </summary>
    FM_HomePage_LimitedTimeDeals = 5,

    /// <summary>
    /// UCF main banner on home page
    /// </summary>
    UCF_HomePage_TopBanner = 6,

    /// <summary>
    /// UCF banner 2 on home page
    /// </summary>
    UCF_HomePage_Banner2 = 7,

    /// <summary>
    /// UCF smart buye on home  page
    /// </summary>
    UCF_HomePage_SmartBuys = 8,

    /// <summary>
    /// FM top banner for category
    /// </summary>
    FM_CategoryPage_TopBanner = 9,

    /// <summary>
    /// UCF top banner for category
    /// </summary>
    UCF_CategoryPage_TopBanner = 10,

    /// <summary>
    /// UCF additional ways to save 1 on category page
    /// </summary>
    UCF_CategoryPage_AWTSBoxBanner1 = 11,

    /// <summary>
    /// UCF additional ways to save 2 on category page
    /// </summary>
    UCF_CategoryPage_AWTSBoxBanner2 = 12,

    /// <summary>
    /// FM banner 1 on sale page
    /// </summary>
    FM_SalesPage_Banner1 = 13,

    /// <summary>
    /// FM banner 2 on sale page
    /// </summary>
    FM_SalesPage_Banner2 = 14,

    /// <summary>
    /// FM banner 3 on sale page
    /// </summary>
    FM_SalesPage_Banner3 = 15,

    /// <summary>
    /// FM banner 4 on sale page
    /// </summary>
    FM_SalesPage_Banner4 = 16,

    /// <summary>
    /// FM banner 5 on sale page
    /// </summary>
    FM_SalesPage_Banner5 = 17,

    /// <summary>
    /// UCF banner 1 on sale page
    /// </summary>
    UCF_SalesPage_Banner1 = 18,

    /// <summary>
    /// UCF banner 2 on sale page
    /// </summary>
    UCF_SalesPage_Banner2 = 19,

    /// <summary>
    /// UCF banner 3 on sale page
    /// </summary>
    UCF_SalesPage_Banner3 = 20,

    /// <summary>
    /// UCF banner 4 on sale page
    /// </summary>
    UCF_SalesPage_Banner4 = 21,

    /// <summary>
    /// UCF banner 5 on sale page
    /// </summary>
    UCF_SalesPage_Banner5 = 22,

    /// <summary>
    /// UCF additional ways to save 1 on sale page
    /// </summary>
    UCF_SalesPage_AWTSBoxBanner1 = 23,

    /// <summary>
    /// UCF additional ways to save 2 on sale page
    /// </summary>
    UCF_SalesPage_AWTSBoxBanner2 = 24,

    /// <summary>
    /// UCF top banner on mattress category page
    /// </summary>
    UCF_CategoryMattressPage_TopBanner = 26,

    /// <summary>
    /// UCF banner 1 on mattress category page
    /// </summary>
    UCF_CategoryMattressPage_Banner2 = 27,

    /// <summary>
    /// FM banner on sale mattress page
    /// </summary>
    FM_SaleMattressPage_Banner = 28,

    /// <summary>
    /// UCF banner on sale mattress page
    /// </summary>
    UCF_SaleMattressPage_Banner = 29,

    /// <summary>
    /// FM banner 1 on inspiration page
    /// </summary>
    FM_InspirationPage_Banner1 = 30,

    /// <summary>
    /// FM banner 2 on inspiration page
    /// </summary>
    FM_InspirationPage_Banner2 = 31,

    /// <summary>
    /// FM banner 1 on search page
    /// </summary>
    FM_SearchPage_Banner1 = 32,

    /// <summary>
    /// UCF banner 1 on search page
    /// </summary>
    UCF_SearchPage_Banner1 = 33,

    /// <summary>
    /// UCF Category pager bottom banner display
    /// </summary>
    UCF_CategoryPage_Banner2 = 34,

    /// <summary>
    /// Top menu category thumbnail image for promotional
    /// </summary>
    FM_TopMenuThumbnail = 37,

    /// <summary>
		/// Top menu category thumbnail image for promotional
		/// </summary>
		UCF_TopMenuThumbnail = 38,

    /// <summary>
    /// Image display on FM Category page
    /// </summary>
    FM_CategoryPage_Thumbnail = 39,

    /// <summary>
		/// Career Store detail page banner. this enum value used into career page 
		/// </summary>
		Career_StoreDetailPage = 40,

    /// <summary>
    /// Global image 
    /// </summary>
    GlobalBanner = 41,

    /// <summary>
    /// Global cart page image 
    /// </summary>
    [Description("Cart")]
    GlobalBanner_Cart = 42,

    /// <summary>
    /// Global checkout page image 
    /// </summary>
    [Description("Checkout")]
    GlobalBanner_Checkout = 43,

    /// <summary>
    /// Sales page main banner
    /// </summary>
    SalePages_MainBanner = 44,

    /// <summary>
    /// Sales page feature category banner
    /// </summary>
    SalePages_FeaturedCategory = 45,

    /// <summary>
    /// Sales page feature ad banners
    /// </summary>
    SalePages_FeaturedAdBanner = 46,

    /// <summary>
    /// FM search page grid banner
    /// </summary>
    FM_Search_GridBanner = 47,

    /// <summary>
    /// UCF search page grid banner
    /// </summary>
    UCF_Search_GridBanner = 48,

    /// <summary>
    /// FM mattress page main banner
    /// </summary>
    FM_MattressPage_MainBanner = 49,

    /// <summary>
    /// FM mattress page Tab7
    /// </summary>
    FM_MattressPage_Tab7Banner = 50,

    /// <summary>
    /// FMUSA top banner on home page
    /// </summary>
    FMUSA_HomePage_TopBanner = 51,

    /// <summary>
    /// FMUSA top banner on home page 1
    /// </summary>
    FMUSA_HomePage_Banner1 = 52,

    /// <summary>
    /// FMUSA top banner on home page 2
    /// </summary>
    FMUSA_HomePage_Banner2 = 53
}
