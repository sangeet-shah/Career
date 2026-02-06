using Career.Data.Data.Caching;
using Career.Data.Domains;
using Career.Data.Extensions;
using Career.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Career.Data.Data;

/// <summary>
/// Represents the Entity Framework repository
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public partial class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    #region Fields

    private readonly IDataProvider _dataProvider;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion        

    #region Ctor

    public EfRepository(IDataProvider dataProvider,
                        IStaticCacheManager staticCacheManager)
    {
        _dataProvider = dataProvider;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="getAllAsync">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    protected  async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, CacheKey> getCacheKey)
    {
        if (getCacheKey == null)
            return await getAllAsync();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(CareerEntityCacheDefaults<TEntity>.AllCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, getAllAsync);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="getAll">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>Entity entries</returns>
    protected  IList<TEntity> GetEntities(Func<IList<TEntity>> getAll, Func<IStaticCacheManager, CacheKey> getCacheKey)
    {
        if (getCacheKey == null)
            return getAll();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(CareerEntityCacheDefaults<TEntity>.AllCacheKey);

        return _staticCacheManager.Get(cacheKey, getAll);
    }

    #endregion

    #region Methods


    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>Entity</returns>
    public  async Task<TEntity> GetByIdAsync(int id, Func<IStaticCacheManager, CacheKey> getCacheKey = null)
    {
        async Task<TEntity> getEntityAsync()
        {
            return await Table.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        if (getCacheKey == null)
            return await getEntityAsync();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
            ?? _staticCacheManager.PrepareKeyForDefaultCache(CareerEntityCacheDefaults<TEntity>.ByIdCacheKey, id);

        return await _staticCacheManager.GetAsync(cacheKey, getEntityAsync);
    }

    /// <summary>
    /// Get entity entries by identifiers
    /// </summary>
    /// <param name="ids">Entity entry identifiers</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public  async Task<IList<TEntity>> GetByIdsAsync(IList<int> ids, Func<IStaticCacheManager, CacheKey> getCacheKey = null)
    {
        if (!ids?.Any() ?? true)
            return new List<TEntity>();

        async Task<IList<TEntity>> getByIdsAsync()
        {
            var query = Table;

            //get entries
            var entries = await query.Where(entry => ids.Contains(entry.Id)).ToListAsync();

            //sort by passed identifiers
            var sortedEntries = new List<TEntity>();
            foreach (var id in ids)
            {
                var sortedEntry = entries.Find(entry => entry.Id == id);
                if (sortedEntry != null)
                    sortedEntries.Add(sortedEntry);
            }

            return sortedEntries;
        }

        if (getCacheKey == null)
            return await getByIdsAsync();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
            ?? _staticCacheManager.PrepareKeyForDefaultCache(CareerEntityCacheDefaults<TEntity>.ByIdsCacheKey, ids);
        return await _staticCacheManager.GetAsync(cacheKey, getByIdsAsync);
    }

    /// <summary>
    /// Insert entity
    /// </summary>
    /// <param name="entity">Entity</param>
    public  async Task InsertAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dataProvider.InsertEntityAsync(entity);
    }

    /// <summary>
    /// Insert entities
    /// </summary>
    /// <param name="entities">Entities</param>
    public  async Task InsertAsync(IList<TEntity> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _dataProvider.BulkInsertEntitiesAsync(entities);
        transaction.Complete();
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Entity</param>
    public  async Task UpdateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dataProvider.UpdateEntityAsync(entity);
    }

    /// <summary>
    /// Update entities
    /// </summary>
    /// <param name="entities">Entities</param>
    public  async Task UpdateAsync(IList<TEntity> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        if (entities.Count == 0)
            return;

        await _dataProvider.UpdateEntitiesAsync(entities);
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity</param>
    public  async Task DeleteAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dataProvider.DeleteEntityAsync(entity);
    }

    /// <summary>
    /// Delete entities
    /// </summary>
    /// <param name="entities">Entities</param>
    public  async Task DeleteAsync(IList<TEntity> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _dataProvider.BulkDeleteEntitiesAsync(entities);
        transaction.Complete();
    }

    /// <summary>
    /// Truncates database table
    /// </summary>
    /// <param name="resetIdentity">Performs reset identity column</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task TruncateAsync(bool resetIdentity = false)
    {
        await _dataProvider.TruncateAsync<TEntity>(resetIdentity);
    }

    /// <summary>
    /// Get paged list of all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    public  async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
        var query = Table;

        query = func != null ? func(query) : query;

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>Entity entries</returns>
    public  IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<IStaticCacheManager, CacheKey> getCacheKey = null)
    {
        IList<TEntity> getAll()
        {
            var query = Table;
            query = func != null ? func(query) : query;

            return query.ToList();
        }

        return GetEntities(getAll, getCacheKey);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>Entity entries</returns>
    public  async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<IStaticCacheManager, CacheKey> getCacheKey = null)
    {
        async Task<IList<TEntity>> getAllAsync()
        {
            var query = Table;
            query = func != null ? func(query) : query;

            return await query.ToListAsync();
        }

        return await GetEntitiesAsync(getAllAsync, getCacheKey);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a table
    /// </summary>
    public  IQueryable<TEntity> Table => _dataProvider.GetTable<TEntity>();

    #endregion
}
