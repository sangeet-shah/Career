using Middleware.Web.Data;
using Middleware.Web.Domains.LandingPages;
using Dapper;
using Middleware.Web.Data;
using System.Threading.Tasks;

namespace Middleware.Web.Services.RegistrationPage;

public class LandingPageClosedService : ILandingPageClosedService
{
    private const string LandingPageClosedTable = "FM_LandingPageClosed";

    private readonly DbConnectionFactory _db;

    public LandingPageClosedService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<LandingPageClosed> GetClosedFromByLandingPageIdAsync(int landingPageId)
    {
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{LandingPageClosedTable}] WHERE LandingPageId = @LandingPageId";
        return await conn.QueryFirstOrDefaultAsync<LandingPageClosed>(sql, new { LandingPageId = landingPageId });
    }
}
