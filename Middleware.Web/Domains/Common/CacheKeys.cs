using Career.Data.Data.Caching;

namespace Career.Data.Domains.Common;

public static class CacheKeys
{
    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string PatternCacheKey => "career.";

    /// <summary>
    /// Gets a key for imagekit
    /// </summary>
    public static CacheKey ImageKitCacheKey => new("career.imagekit");

    /// <summary>
    /// Gets a key for homepage banner
    /// </summary>
    public static CacheKey HomePageBannerCacheKey => new("career.homepagebanners");

    /// <summary>
    /// Gets generic topic
    /// </summary>
    public static CacheKey GenericTopicCacheKey => new("career.generictopic.all");

    /// <summary>
    /// Gets states
    /// </summary>
    public static CacheKey StatesKey => new("career.states.all");

    /// <summary>
    /// Gets cities
    /// </summary>
    public static CacheKey CitiesKey => new("career.cities.all");

    /// <summary>
    /// Gets departments
    /// </summary>
    public static CacheKey DepartmentsKey => new("career.departments.all");

    /// <summary>
    /// Gets career brands
    /// </summary>
    public static CacheKey CareerBrandsKey => new("career.careerbrands.all");

    /// <summary>
    /// Gets customersrole
    /// </summary>
    public static CacheKey CustomersRoleKey => new("career.customersrole.all-{0}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : customer GUID
    /// </remarks>
    public static CacheKey CustomerByGuidCacheKey => new("career.customer.byguid.{0}");

    /// <summary>
    /// Get the key for physical store google response key
    /// </summary>
    public static CacheKey PhysicalStoreGoogleResponseCacheKey => new("career.physicalstore.googleresponse-{0}");

    /// <summary>
    /// Gets a key for jobs
    /// </summary>
    public static string JobsPattern => "career.jobs.";

    /// <summary>
    /// Get the key for jobs
    /// </summary>
    public static CacheKey JobsCacheKey => new("career.jobs");

    /// <summary>
    /// Get the key for jobs token
    /// </summary>
    public static CacheKey JobsAPITokenCacheKey => new("career.jobs.token");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    public static CacheKey SettingsAllCacheKey => new("career.setting.all");

    /// <summary>
    /// Gets UrlRecord
    /// </summary>
    public static CacheKey UrlRecordBySlugCacheKey => new("career.urlrecord.byslug.{0}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity ID
    /// {1} : entity name        
    /// </remarks>
    public static CacheKey UrlRecordCacheKey => new("career.urlrecord.{0}-{1}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity name
    /// {1} : store id        
    /// </remarks>
    public static CacheKey UrlRecordByEntityCacheKey => new("career.urlrecord.{0}-{1}");

    /// <summary>
    /// Gets Favicon Icon cacheing
    /// </summary>
    public static CacheKey FaviconIconCacheKey => new("career.faviconicon");

    /// <summary>
    /// Gets customersrole
    /// </summary>
    public static CacheKey PictureCacheKey => new("career.picture-{0}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : system name
    /// {1} : store ID
    /// </remarks>
    public static CacheKey TopicsBySystemNameCacheKey => new("career.topic.bysystemname-{0}-{1}");
    /// <summary>
    /// Gets sitemap
    /// </summary>
    public static CacheKey SitemapCacheKey => new("career.sitemap.all");

    /// <summary>
    /// Gets customers social media urls
    /// </summary>
    /// <remarks>
    /// {0} : customer Id
    /// </remarks>
    public static CacheKey CustomersSocialMediaUrlsByCustomerIdKey => new("career.customer.socialmediaurls.bycustomerid-{0}");

    /// <summary>
    /// Gets offers promotions
    /// </summary>
    public static CacheKey OffersPromotionsKey => new("career.offerspromotions.all");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : resource name
    /// </remarks>
    public static CacheKey LocaleStringResourceByNameCacheKey => new("career.localestringresource.byname.{0}");

    /// <summary>
    /// Gets Location
    /// </summary>
    public static CacheKey AllLocationsKey => new("career.location.all");

    /// <summary>
    /// Gets a key for location hours caching
    /// </summary>
    /// <remarks>
    /// {0} : locationId
    /// {1} : hourstype        
    /// </remarks>
    public static CacheKey LocationHoursCacheKey => new(LocationHoursCacheKeyPattern + "{0}-{1}");

    public static string LocationHoursCacheKeyPattern => "career.locationhours.";

    /// <summary>
    /// Get all state province
    /// </summary>
    public static CacheKey AllStateProvincesKey => new("career.StateProvinces.all");

    /// <summary>
    /// Get all vendor
    /// </summary>
    public static CacheKey AllVendorKey => new("career.vendor.all");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>        
    /// </remarks>
    public static CacheKey LocaleStringResourceCacheKey => new("career.localestringresource.{0}");

    /// <summary>
    /// Gets customers social media urls
    /// </summary>
    /// <remarks>
    /// {0} : banner id
    /// </remarks>
    public static CacheKey BannerByBannerIdKey => new("career.banners.byid-{0}");

    /// <summary>
    /// Gets customers social media urls
    /// </summary>
    /// <remarks>
    /// {0} : location Id
    /// </remarks>
    public static CacheKey LocationBannerByLocationIdKey => new("career.banners.bylocationid-{0}");

    public static string BannersKeyPattern => "career.banners.";

    /// <summary>
    /// Gets a key for current store 
    /// </summary>
    public static CacheKey CurrentStoreCacheKey => new("career.current.store");

    /// <summary>
    /// Gets Hello bar
    /// </summary>
    public static CacheKey AllHellobarKey => new("career.Hellobar.all");

    /// <summary>
    /// Get all Advertisement
    /// </summary>
    public static CacheKey AllAdvertisementKey => new("career.Advertisement.all");

    /// <summary>
    /// Get all blog post
    /// </summary>
    public static CacheKey BlogPostsKey => new("career.BlogPosts.all.{0}-{1}-{2}");

    /// <summary>
    /// Gets the cache key template used to store and retrieve blog posts by author identifier and paging information.
    /// </summary>    
    public static CacheKey BlogPostsByAuthorIdCacheKey => new("career.BlogPosts.byauthorid.{0}-{1}");

    /// <summary>
    /// Gets the cache key pattern used to retrieve a blog post by its unique identifier.
    /// </summary>
    public static CacheKey BlogPostByIdCacheKey => new("career.BlogPosts.BlogPostId.{0}-{1}");

    /// <summary>
    /// Gets the cache key format string used to store and retrieve the latest blog posts.
    /// </summary>
    public static CacheKey AllLatestBlogsCacheKey => new("career.BlogPosts.LatestBlogs.{0}-{1}-{2}");

    /// <summary>
    /// Gets the cache key format string used to retrieve or store a blog post in the FM Blog cache by its blog post ID.
    /// </summary>
    public static CacheKey FMBlogByBlogPostIdCacheKey => new("career.BlogPosts.FMBlog.{0}");


    /// <summary>
    /// Gets the cache key used to retrieve the author information for a blog post by its unique identifier.
    /// </summary>    
    public static CacheKey FMBlogPostAuthorByBlogPostIdCacheKey => new("career.BlogPosts.FMBlogPostAuthor.{0}");

    public static string BlogPostsKeyPattern => "career.BlogPosts.";

    /// <summary>
    /// Gets the cache key template used for storing and retrieving generic attribute data.
    /// </summary>
    public static CacheKey GenericAttributeCacheKey => new("career.genericattribute.{0}-{1}");

    public static string GenericAttributeKeyPattern => "career.genericattribute.";

    public static CacheKey FMCustomerByCustomerIdCacheKey => new("career.fmcustomer.bycustomerId.{0}");

    public static string FMCustomerCacheKeyPattern => "career.fmcustomer.";

    public static CacheKey CorporateGalleryCacheKey => new("career.corporategallery.all");

    public static CacheKey CorporateGalleryPictureByCorporateGalleryIdCacheKey => new("career.corporategallery.picturebycorporategalleryid.{0}");
    public static string CorporateGalleryPictureCacheKeyPattern => "career.corporategallery.";

}