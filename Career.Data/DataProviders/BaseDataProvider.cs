using Career.Data.Domains;
using Career.Data.Mapping;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Tools;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.DataProviders;

public abstract class BaseDataProvider
{
    #region Utils

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected abstract DbConnection GetInternalDbConnection(string connectionString);

    /// <summary>
    /// Creates the database connection
    /// </summary>
    protected  DataConnection CreateDataConnection()
    {
        return CreateDataConnection(LinqToDbDataProvider);
    }

    /// <summary>
    /// Creates the database connection
    /// </summary>
    /// <param name="dataProvider">Data provider</param>
    /// <returns>Database connection</returns>
    protected  DataConnection CreateDataConnection(IDataProvider dataProvider)
    {
        ArgumentNullException.ThrowIfNull(dataProvider);

        var dataConnection = new DataConnection(dataProvider, CreateDbConnection(), CareerMappingSchema.GetMappingSchema(ConfigurationName, LinqToDbDataProvider))
        {
            CommandTimeout = 0
        };

        return dataConnection;
    }

    /// <summary>
    /// Creates a connection to a database
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected  DbConnection CreateDbConnection(string connectionString = null)
    {
        return GetInternalDbConnection(!string.IsNullOrEmpty(connectionString) ? connectionString : GetCurrentConnectionString());
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns queryable source for specified mapping class for current connection,
    /// mapped to database table or view.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Queryable source</returns>
    public  IQueryable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity
    {
        var options = new DataOptions()
            .UseConnectionString(LinqToDbDataProvider, GetCurrentConnectionString())
            .UseMappingSchema(CareerMappingSchema.GetMappingSchema(ConfigurationName, LinqToDbDataProvider));

        return new DataContext(options)
        {
            CommandTimeout = 0
        }
        .GetTable<TEntity>();
    }

    /// <summary>
    /// Inserts record into table. Returns inserted entity with identity
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the inserted entity
    /// </returns>
    public  async Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection();
        entity.Id = await dataContext.InsertWithInt32IdentityAsync(entity);
        return entity;
    }

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
        await dataContext.BulkCopyAsync(new BulkCopyOptions(), entities.RetrieveIdentity(dataContext));
    }

    /// <summary>
    /// Updates record in table, using values from entity parameter.
    /// Record to update identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection();
        await dataContext.UpdateAsync(entity);
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
        //we don't use the Merge API on this level, because this API not support all databases.
        //you may see all supported databases by the following link: https://linq2db.github.io/articles/sql/merge/Merge-API.html#supported-databases
        foreach (var entity in entities)
            await UpdateEntityAsync(entity);
    }

    /// <summary>
    /// Deletes record in table. Record to delete identified
    /// by match on primary key value from obj value.
    /// </summary>
    /// <param name="entity">Entity for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection();
        await dataContext.DeleteAsync(entity);
    }

    /// <summary>
    /// Performs delete records in a table
    /// </summary>
    /// <param name="entities">Entities for delete operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public  async Task BulkDeleteEntitiesAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection();
        if (entities.All(entity => entity.Id == 0))
        {
            foreach (var entity in entities)
                await dataContext.DeleteAsync(entity);
        }
        else
        {
            await dataContext.GetTable<TEntity>()
               .Where(e => e.Id.In(entities.Select(x => x.Id)))
               .DeleteAsync();
        }
    }

    /// <summary>
    /// Truncates database table
    /// </summary>
    /// <param name="resetIdentity">Performs reset identity column</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public  async Task TruncateAsync<TEntity>(bool resetIdentity = false) where TEntity : BaseEntity
    {
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);
        await dataContext.GetTable<TEntity>().TruncateAsync(resetIdentity);
    }

    /// <summary>
    /// Executes command asynchronously and returns number of affected records
    /// </summary>
    /// <param name="sql">Command text</param>
    /// <param name="dataParameters">Command parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of records, affected by command execution.
    /// </returns>
    public  async Task<int> ExecuteNonQueryAsync(string sql, params DataParameter[] dataParameters)
    {
        using var dataConnection = CreateDataConnection(LinqToDbDataProvider);
        var command = new CommandInfo(dataConnection, sql, dataParameters);

        return await command.ExecuteAsync();
    }

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
    public  Task<IList<T>> QueryProcAsync<T>(string procedureName, params DataParameter[] parameters)
    {
        using var dataConnection = CreateDataConnection(LinqToDbDataProvider);
        var command = new CommandInfo(dataConnection, procedureName, parameters);

        var rez = command.QueryProc<T>()?.ToList();
        return Task.FromResult<IList<T>>(rez ?? new List<T>());
    }

    #endregion

    #region Properties

    /// <summary>
    /// Linq2Db data provider
    /// </summary>
    protected abstract IDataProvider LinqToDbDataProvider { get; }

    /// <summary>
    /// Database connection string
    /// </summary>
    protected static string GetCurrentConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        return builder.Build().GetConnectionString("DbConnection");
    }

    /// <summary>
    /// Name of database provider
    /// </summary>
    public string ConfigurationName => LinqToDbDataProvider.Name;

    #endregion
}
