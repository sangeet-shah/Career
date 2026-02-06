using Career.Data.Domains;

namespace Career.Data.Data.Caching;

/// <summary>
/// Represents default values related to caching entities for hub
/// </summary>
public static partial class CareerEntityCacheDefaults<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Gets an entity type name used in cache keys
    /// </summary>
    public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

    /// <summary>
    /// Gets a key for caching entity by identifier
    /// </summary>
    /// <remarks>
    /// {0} : entity id
    /// </remarks>
    public static CacheKey ByIdCacheKey => new($"Career.{EntityTypeName}.byid.{{0}}", ByIdPrefix, Prefix);

    /// <summary>
    /// Gets a key for caching entities by identifiers
    /// </summary>
    /// <remarks>
    /// {0} : entity ids
    /// </remarks>
    public static CacheKey ByIdsCacheKey => new($"Career.{EntityTypeName}.byids.{{0}}", ByIdsPrefix, Prefix);

    /// <summary>
    /// Gets a key for caching all entities
    /// </summary>
    public static CacheKey AllCacheKey => new($"Career.{EntityTypeName}.all.", AllPrefix, Prefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string Prefix => $"Career.{EntityTypeName}.";

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string ByIdPrefix => $"Career.{EntityTypeName}.byid.";

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string ByIdsPrefix => $"Career.{EntityTypeName}.byids.";

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string AllPrefix => $"Career.{EntityTypeName}.all.";
}
