using Middleware.Web.Data;
using Middleware.Web.Domains.Directory;
using Dapper;
using Middleware.Web.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Directory;

public class StateProvinceService : IStateProvinceService
{
    private const string StateProvinceTable = "StateProvince";

    private readonly DbConnectionFactory _db;

    public StateProvinceService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId)
    {
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{StateProvinceTable}] WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<StateProvince>(sql, new { Id = stateProvinceId });
    }

    public async Task<IList<StateProvince>> GetAllStateProvincesAsync()
    {
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{StateProvinceTable}]";
        var list = (await conn.QueryAsync<StateProvince>(sql)).AsList();
        return list;
    }
}
