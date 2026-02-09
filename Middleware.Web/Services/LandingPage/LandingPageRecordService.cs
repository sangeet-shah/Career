using Middleware.Web.Data;
using Middleware.Web.Domains.LandingPages;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Dapper;
using System;
using System.Threading.Tasks;
using Middleware.Web.Data;

namespace Middleware.Web.Services.LandingPages;

public class LandingPageRecordService : ILandingPageRecordService
{
    private const string LandingPageRecordTable = "FM_LandingPageRecord";

    private readonly DbConnectionFactory _db;
    private readonly IStoreService _storeService;

    public LandingPageRecordService(DbConnectionFactory db,
        ISettingService settingService,
        IStoreService storeService)
    {
        _db = db;
        _storeService = storeService;
    }

    public async Task InsertLandingPageRecordAsync(LandingPageRecord landingPageRecord)
    {
        if (landingPageRecord == null)
            throw new ArgumentNullException(nameof(landingPageRecord));

        using var conn = _db.CreateNop();
        var sql = $@"INSERT INTO [{LandingPageRecordTable}] (LandingPageId, FirstName, LastName, City, StateProvinceId, StoreId, ZipCode, Email, DOB, Gender, PhoneNumber, Address, InstagramHandle, TwitterHandle, EventSubscribed, EmailSubscribed, SMSSubscribed, LocationId, CreatedOnUtc)
VALUES (@LandingPageId, @FirstName, @LastName, @City, @StateProvinceId, @StoreId, @ZipCode, @Email, @DOB, @Gender, @PhoneNumber, @Address, @InstagramHandle, @TwitterHandle, @EventSubscribed, @EmailSubscribed, @SMSSubscribed, @LocationId, @CreatedOnUtc);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
        landingPageRecord.Id = await conn.ExecuteScalarAsync<int>(sql, landingPageRecord);
    }

    public async Task<int> GetLandingPageRecordsCountBylandingPageIdAsync(int landingPageId)
    {
        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        using var conn = _db.CreateNop();
        var sql = $"SELECT COUNT(1) FROM [{LandingPageRecordTable}] WHERE LandingPageId = @LandingPageId AND StoreId = @StoreId";
        return await conn.ExecuteScalarAsync<int>(sql, new { LandingPageId = landingPageId, StoreId = storeId });
    }
}
