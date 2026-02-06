using Career.Data.Domains;
using LinqToDB.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Data;

/// <summary>
/// Represents a data provider
/// </summary>
public partial interface IDataProvider
{
    #region Methods

    /// <summary>
    /// Get a support database parameter object (used by stored procedures)
    /// </summary>
    /// <returns>Parameter</returns>
    DbParameter GetParameter();

    /// <summary>
    /// Insert a new entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity
    /// </returns>
    Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Performs bulk insert entities operation
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Collection of Entities</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

    /// <summary>
    /// Updates record in table, using values from entity parameter. 
    /// Record to update identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Updates records in table, using values from entity parameter. 
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

    /// <summary>
    /// Deletes record in table. Record to delete identified
    /// by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Performs delete records in a table
    /// </summary>
    /// <param name="entities">Entities for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task BulkDeleteEntitiesAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

    /// <summary>
    /// Truncates database table
    /// </summary>
    /// <param name="resetIdentity">Performs reset identity column</param>
    Task TruncateAsync<TEntity>(bool resetIdentity = false) where TEntity : BaseEntity;

    /// <summary>
    /// Returns queryable source for specified mapping class for current connection,
    /// mapped to database table or view.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Queryable source</returns>
    IQueryable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity;

    /// Executes command asynchronously and returns number of affected records
    /// </summary>
    /// <param name="sql">Command text</param>
    /// <param name="dataParameters">Command parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of records, affected by command execution.
    /// </returns>
    Task<int> ExecuteNonQueryAsync(string sql, params DataParameter[] dataParameters);

    /// <summary>
    /// Executes command using System.Data.CommandType.StoredProcedure command type and
    /// returns results as collection of values of specified type
    /// </summary>
    /// <typeparam name="T">Result record type</typeparam>
    /// <param name="procedureName">Procedure name</param>
    /// <param name="parameters">Command parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the returns collection of query result records
    /// </returns>
    Task<IList<T>> QueryProcAsync<T>(string procedureName, params DataParameter[] parameters);

    #endregion

    #region Properties

    /// <summary>
    /// Name of database provider
    /// </summary>
    string ConfigurationName { get; }

    /// <summary>
    /// Gets a value indicating whether this data provider supports backup
    /// </summary>
    bool BackupSupported { get; }

    /// <summary>
    /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
    /// </summary>
    int SupportedLengthOfBinaryHash { get; }

    #endregion
}
