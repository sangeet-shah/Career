using Career.Data.DataProviders;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;

namespace Career.Data.Data;

/// <summary>
/// Represents SQL Server data provider
/// </summary>
public partial class CareerDataProvider : BaseDataProvider, IDataProvider
{
    #region Utils

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected override DbConnection GetInternalDbConnection(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        return new SqlConnection(connectionString);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get a support database parameter object (used by stored procedures)
    /// </summary>
    /// <returns>Parameter</returns>
    public  DbParameter GetParameter()
    {
        return new SqlParameter();
    }

    #endregion

    #region Properties

    protected override LinqToDB.DataProvider.IDataProvider LinqToDbDataProvider => SqlServerTools.GetDataProvider(SqlServerVersion.v2012, SqlServerProvider.MicrosoftDataSqlClient);

    /// <summary>
    /// Gets a value indicating whether this data provider supports backup
    /// </summary>
    public  bool BackupSupported => true;

    /// <summary>
    /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
    /// </summary>
    public  int SupportedLengthOfBinaryHash => 8000; //for SQL Server 2008 and above HASHBYTES function has a limit of 8000 characters.

    #endregion
}
