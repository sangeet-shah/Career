using Middleware.Web.Data.Caching;

namespace Middleware.Web.Domains.Common;

public static class CacheKeys
{
    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string PatternCacheKey => "middleware.";

    /// <summary>
    /// Gets a key for imagekit
    /// </summary>
    public static CacheKey ImageKitCacheKey => new("middleware.imagekit");

    /// <summary>
    /// Gets a key for homepage banner
    /// </summary>
    public static CacheKey HomePageBannerCacheKey => new("middleware.homepagebanners");

    /// <summary>
    /// Gets generic topic
    /// </summary>
    public static CacheKey GenericTopicCacheKey => new("middleware.generictopic.all");

    /// <summary>
    /// Gets states
    /// </summary>
    public static CacheKey StatesKey => new("middleware.states.all");

    /// <summary>
    /// Gets cities
    /// </summary>
    public static CacheKey CitiesKey => new("middleware.cities.all");

    /// <summary>
    /// Gets departments
    /// </summary>
    public static CacheKey DepartmentsKey => new("middleware.departments.all");

    /// <summary>
    /// Gets career brands
    /// </summary>
    public static CacheKey CareerBrandsKey => new("middleware.careerbrands.all");

    /// <summary>
    /// Gets customersrole
    /// </summary>
    public static CacheKey CustomersRoleKey => new("middleware.customersrole.all-{0}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : customer GUID
    /// </remarks>
    public static CacheKey CustomerByGuidCacheKey => new("middleware.customer.byguid.{0}");

    /// <summary>
    /// Get the key for physical store google response key
    /// </summary>
    public static CacheKey PhysicalStoreGoogleResponseCacheKey => new("middleware.physicalstore.googleresponse-{0}");

    /// <summary>
    /// Gets a key for jobs
    /// </summary>
    public static string JobsPattern => "middleware.jobs.";

    /// <summary>
    /// Get the key for jobs
    /// </summary>
    public static CacheKey JobsCacheKey => new("middleware.jobs");

    /// <summary>
    /// Get the key for jobs token
    /// </summary>
    public static CacheKey JobsAPITokenCacheKey => new("middleware.jobs.token");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    public static CacheKey SettingsAllCacheKey => new("middleware.setting.all");

    /// <summary>
    /// Gets UrlRecord
    /// </summary>
    public static CacheKey UrlRecordBySlugCacheKey => new("middleware.urlrecord.byslug.{0}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity ID
    /// {1} : entity name        
    /// </remarks>
    public static CacheKey UrlRecordCacheKey => new("middleware.urlrecord.{0}-{1}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity name
    /// {1} : store id        
    /// </remarks>
    public static CacheKey UrlRecordByEntityCacheKey => new("middleware.urlrecord.{0}-{1}");

    /// <summary>
    /// Gets Favicon Icon cacheing
    /// </summary>
    public static CacheKey FaviconIconCacheKey => new("middleware.faviconicon");

    /// <summary>
    /// Gets customersrole
    /// </summary>
    public static CacheKey PictureCacheKey => new("middleware.picture-{0}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : system name
    /// {1} : store ID
    /// </remarks>
    public static CacheKey TopicsBySystemNameCacheKey => new("middleware.topic.bysystemname-{0}-{1}");
    /// <summary>
    /// Gets sitemap
    /// </summary>
    public static CacheKey SitemapCacheKey => new("middleware.sitemap.all");

    /// <summary>
    /// Gets customers social media urls
    /// </summary>
    /// <remarks>
    /// {0} : customer Id
    /// </remarks>
    public static CacheKey CustomersSocialMediaUrlsByCustomerIdKey => new("middleware.customer.socialmediaurls.bycustomerid-{0}");

    /// <summary>
    /// Gets offers promotions
    /// </summary>
    public static CacheKey OffersPromotionsKey => new("middleware.offerspromotions.all");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : resource name
    /// </remarks>
    public static CacheKey LocaleStringResourceByNameCacheKey => new("middleware.localestringresource.byname.{0}");

    /// <summary>
    /// Gets Location
    /// </summary>
    public static CacheKey AllLocationsKey => new("middleware.location.all");

    /// <summary>
    /// Gets a key for location hours caching
    /// </summary>
    /// <remarks>
    /// {0} : locationId
    /// {1} : hourstype        
    /// </remarks>
    public static CacheKey LocationHoursCacheKey => new(LocationHoursCacheKeyPattern + "{0}-{1}");

    public static string LocationHoursCacheKeyPattern => "middleware.locationhours.";

    /// <summary>
    /// Get all state province
    /// </summary>
    public static CacheKey AllStateProvincesKey => new("middleware.StateProvinces.all");

    /// <summary>
    /// Get all vendor
    /// </summary>
    public static CacheKey AllVendorKey => new("middleware.vendor.all");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>        
    /// </remarks>
    public static CacheKey LocaleStringResourceCacheKey => new("middleware.localestringresource.{0}");

    /// <summary>
    /// Gets customers social media urls
    /// </summary>
    /// <remarks>
    /// {0} : banner id
    /// </remarks>
    public static CacheKey BannerByBannerIdKey => new("middleware.banners.byid-{0}");

    /// <summary>
    /// Gets customers social media urls
    /// </summary>
    /// <remarks>
    /// {0} : location Id
    /// </remarks>
    public static CacheKey LocationBannerByLocationIdKey => new("middleware.banners.bylocationid-{0}");

    public static string BannersKeyPattern => "middleware.banners.";

    /// <summary>
    /// Gets a key for current store 
    /// </summary>
    public static CacheKey CurrentStoreCacheKey => new("middleware.current.store-{0}");

    /// <summary>
    /// Gets Hello bar
    /// </summary>
    public static CacheKey AllHellobarKey => new("middleware.Hellobar.all");

    /// <summary>
    /// Get all Advertisement
    /// </summary>
    public static CacheKey AllAdvertisementKey => new("middleware.Advertisement.all");

    /// <summary>
    /// Get all blog post
    /// </summary>
    public static CacheKey BlogPostsKey => new("middleware.BlogPosts.all.{0}-{1}-{2}");

    /// <summary>
    /// Gets the cache key template used to store and retrieve blog posts by author identifier and paging information.
    /// </summary>    
    public static CacheKey BlogPostsByAuthorIdCacheKey => new("middleware.BlogPosts.byauthorid.{0}-{1}");

    /// <summary>
    /// Gets the cache key pattern used to retrieve a blog post by its unique identifier.
    /// </summary>
    public static CacheKey BlogPostByIdCacheKey => new("middleware.BlogPosts.BlogPostId.{0}-{1}");

    /// <summary>
    /// Gets the cache key format string used to store and retrieve the latest blog posts.
    /// </summary>
    public static CacheKey AllLatestBlogsCacheKey => new("middleware.BlogPosts.LatestBlogs.{0}-{1}-{2}");

    /// <summary>
    /// Gets the cache key format string used to retrieve or store a blog post in the FM Blog cache by its blog post ID.
    /// </summary>
    public static CacheKey FMBlogByBlogPostIdCacheKey => new("middleware.BlogPosts.FMBlog.{0}");


    /// <summary>
    /// Gets the cache key used to retrieve the author information for a blog post by its unique identifier.
    /// </summary>    
    public static CacheKey FMBlogPostAuthorByBlogPostIdCacheKey => new("middleware.BlogPosts.FMBlogPostAuthor.{0}");

    public static string BlogPostsKeyPattern => "middleware.BlogPosts.";

    /// <summary>
    /// Gets the cache key template used for storing and retrieving generic attribute data.
    /// </summary>
    public static CacheKey GenericAttributeCacheKey => new("middleware.genericattribute.{0}-{1}");

    public static string GenericAttributeKeyPattern => "middleware.genericattribute.";

    public static CacheKey FMCustomerByCustomerIdCacheKey => new("middleware.fmcustomer.bycustomerId.{0}");

    public static string FMCustomerCacheKeyPattern => "middleware.fmcustomer.";

    public static CacheKey CorporateGalleryCacheKey => new("middleware.corporategallery.all");

    public static CacheKey CorporateGalleryPictureByCorporateGalleryIdCacheKey => new("middleware.corporategallery.picturebycorporategalleryid.{0}");
    public static string CorporateGalleryPictureCacheKeyPattern => "middleware.corporategallery.";

}