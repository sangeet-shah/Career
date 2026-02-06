using Career.Data.Data;
using Career.Data.Domains.RegistrationPage;
using Dapper;
using Middleware.Web.Data;
using System.Threading.Tasks;

namespace Middleware.Web.Services.RegistrationPage;

public class RegistrationPageFieldsService : IRegistrationPageFieldsService
{
    private const string RegistrationPageFieldsTable = "RegistrationPageFields";

    private readonly DbConnectionFactory _db;

    public RegistrationPageFieldsService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<RegistrationPageFields> GetRegistrationPageFieldsByContestIdAsync(int contestId)
    {
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{RegistrationPageFieldsTable}] WHERE ContestId = @ContestId";
        return await conn.QueryFirstOrDefaultAsync<RegistrationPageFields>(sql, new { ContestId = contestId });
    }
}
