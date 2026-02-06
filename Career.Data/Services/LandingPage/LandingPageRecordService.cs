using Career.Data.Data;
using Career.Data.Domains.LandingPages;
using Career.Data.Services.Settings;
using Career.Data.Services.Stores;
using LinqToDB;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.LandingPages;

/// <summary>
/// Contest page log service interface
/// </summary>
public class LandingPageRecordService : ILandingPageRecordService
{
    #region Fields

    private readonly IRepository<LandingPageRecord> _landingPageRecordRepository;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public LandingPageRecordService(IRepository<LandingPageRecord> landingPageRecordRepository,
        ISettingService settingService,
        IStoreService storeService)
    {
        _landingPageRecordRepository = landingPageRecordRepository;
        _settingService = settingService;
        _storeService = storeService;
    }

    #endregion

    #region Methods 

    /// <summary>
    /// Insert landing page record
    /// </summary>
    /// <param name="landingPageRecord">landingPageRecord</param>
    public async Task InsertLandingPageRecordAsync(LandingPageRecord landingPageRecord)
    {
        if (landingPageRecord == null)
            throw new ArgumentNullException("contestPageLog");

        await _landingPageRecordRepository.InsertAsync(landingPageRecord);
    }

    /// <summary>
    /// Get all landing page records count by landing page id
    /// </summary>
    /// <param name="landingPageId">landing page identifier</param>
    /// <returns>contest page logs count</returns>
    public async Task<int> GetLandingPageRecordsCountBylandingPageIdAsync(int landingPageId)
    {
        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var query = await (from cpl in _landingPageRecordRepository.Table
                           where cpl.LandingPageId == landingPageId && cpl.StoreId == storeId
                           select cpl).CountAsync();

        return query;
    }

    #endregion
}
