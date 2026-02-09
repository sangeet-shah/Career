using Middleware.Web.Data.Caching;
using Middleware.Web.Domains;
using Middleware.Web.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Middleware.Web.Data;

/// <summary>
/// Represents an entity repository
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    #region Methods

    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>Entity</returns>
    Task<TEntity> GetByIdAsync(int id, Func<IStaticCacheManager, CacheKey> getCacheKey = null);

    /// <summary>
    /// Get entity entries by identifiers
    /// </summary>
    /// <param name="ids">Entity entry identifiers</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    Task<IList<TEntity>> GetByIdsAsync(IList<int> ids, Func<IStaticCacheManager, CacheKey> getCacheKey = null);

    /// <summary>
    /// Insert entity
    /// </summary>
    /// <param name="entity">Entity</param>
    Task InsertAsync(TEntity entity);

    /// <summary>
    /// Insert entities
    /// </summary>
    /// <param name="entities">Entities</param>
    Task InsertAsync(IList<TEntity> entities);

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Entity</param>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Update entities
    /// </summary>
    /// <param name="entities">Entities</param>
    Task UpdateAsync(IList<TEntity> entities);

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity</param>
    Task DeleteAsync(TEntity entity);

    /// <summary>
    /// Delete entities
    /// </summary>
    /// <param name="entities">Entities</param>
    Task DeleteAsync(IList<TEntity> entities);

    /// <summary>
    /// Truncates database table
    /// </summary>
    /// <param name="resetIdentity">Performs reset identity column</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task TruncateAsync(bool resetIdentity = false);

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>Entity entries</returns>
    IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<IStaticCacheManager, CacheKey> getCacheKey = null);

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<IStaticCacheManager, CacheKey> getCacheKey = null);

    #endregion

    #region Properties

    /// <summary>
    /// Gets a table
    /// </summary>
    IQueryable<TEntity> Table { get; }

    #endregion
}
