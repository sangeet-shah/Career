using Middleware.Web.Data;
using Middleware.Web.Domains.LandingPages;
using Dapper;
using Middleware.Web.Data;
using System.Threading.Tasks;

namespace Middleware.Web.Services.SummerJams;

public class SummerJamService : ISummerJamService
{
    private const string SummerJamTable = "FM_SummerJam";

    private readonly DbConnectionFactory _db;

    public SummerJamService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task InsertSummerJamAsync(SummerJam summerJam)
    {
        using var conn = _db.CreateNop();
        var sql = $@"INSERT INTO [{SummerJamTable}] (FirstName, LastName, Email, Address1, Address2, City, StateProvinceId, ZipCode, Phone, DOB, StoreId, CreatedDateUtc)
VALUES (@FirstName, @LastName, @Email, @Address1, @Address2, @City, @StateProvinceId, @ZipCode, @Phone, @DOB, @StoreId, @CreatedDateUtc);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
        summerJam.Id = await conn.ExecuteScalarAsync<int>(sql, summerJam);
    }
}
