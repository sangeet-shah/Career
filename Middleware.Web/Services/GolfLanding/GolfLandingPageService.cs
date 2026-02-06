using Career.Data.Data;
using Career.Data.Domains.LandingPages;
using Dapper;
using Middleware.Web.Data;
using System;
using System.Threading.Tasks;

namespace Middleware.Web.Services.GolfLanding;

public class GolfLandingPageService : IGolfLandingPageService
{
    private const string GolfEventLandingPageTable = "FM_GolfEventLandingPage";

    private readonly DbConnectionFactory _db;

    public GolfLandingPageService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task InsertGolfEventLandingPageAsync(GolfEventLandingPage golfLandingPage)
    {
        if (golfLandingPage == null)
            throw new ArgumentNullException(nameof(golfLandingPage));

        using var conn = _db.CreateNop();
        var sql = $@"INSERT INTO [{GolfEventLandingPageTable}] (CompanyName, PhoneNumber, Email, SponsorshipLevelId, Contact1, Contact2, Contact3, Contact4, PictureId, CreatedOnUtc)
VALUES (@CompanyName, @PhoneNumber, @Email, @SponsorshipLevelId, @Contact1, @Contact2, @Contact3, @Contact4, @PictureId, @CreatedOnUtc);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
        golfLandingPage.Id = await conn.ExecuteScalarAsync<int>(sql, golfLandingPage);
    }
}
