using Career.Data.Data;
using Career.Data.Domains.LandingPages;
using Dapper;
using Middleware.Web.Data;
using System.Threading.Tasks;

namespace Middleware.Web.Services.LandingPages;

public class LandingPageService : ILandingPageService
{
    private const string LandingPageTable = "FM_LandingPage";

    private readonly DbConnectionFactory _db;

    public LandingPageService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<LandingPage> GetlandingPageByIdAsync(int landingPageId)
    {
        if (landingPageId == 0)
            return null;

        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{LandingPageTable}] WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<LandingPage>(sql, new { Id = landingPageId });
    }
}
