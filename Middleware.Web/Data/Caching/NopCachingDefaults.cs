namespace Middleware.Web.Data.Caching;

/// <summary>
/// Represents default values related to caching
/// </summary>
public static partial class NopCachingDefaults
{
    /// <summary>
    /// Gets the default cache time in minutes
    /// </summary>
    public static int CacheTime => 1440; //24hours caching time

    /// <summary>
    /// Gets the key used to store the protection key list to Redis (used with the PersistDataProtectionKeysToRedis option enabled)
    /// </summary>
    public static string RedisDataProtectionKey => "Nop.DataProtectionKeys";    

    /// <summary>
    /// Gets or sets the short term cache time in minutes
    /// </summary>
    public static int ShortTermCacheTime { get; private set; } = 30;
}
