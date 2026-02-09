using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Directory;
using Middleware.Web.Domains.Locations;
using Middleware.Web.Domains.PhysicalStores;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Helpers;
using Middleware.Web.Services.Logs;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Dapper;
using Newtonsoft.Json;
using System.Globalization;
using static Middleware.Web.Domains.PhysicalStores.GoogleResponseCache;
using Middleware.Web.Data;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Directory;
using Middleware.Web.Domains.Locations;
using Middleware.Web.Domains.PhysicalStores;

namespace Middleware.Web.Services.Locations;

public class LocationService : ILocationService
{
    private const string LocationTable = "FM_Location";
    private const string StateProvinceTable = "StateProvince";
    private const string LocationHourTable = "FM_LocationHour";
    private const string CountryTable = "Country";
    private const string AddressTable = "Address";

    #region Fields

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IHttpClientService _httpClientService;
    private readonly ILogService _logService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;
    private readonly IDateTimeHelper _dateTimeHelper;

    #endregion

    #region Ctor

    public LocationService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager,
        IHttpClientService httpClientService,
        ILogService logService,
        ISettingService settingService,
        IStoreService storeService,
        IDateTimeHelper dateTimeHelper)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
        _httpClientService = httpClientService;
        _logService = logService;
        _settingService = settingService;
        _storeService = storeService;
        _dateTimeHelper = dateTimeHelper;
    }

    #endregion

    #region Utilities

    private async Task<IList<Location>> GetAllCachedLocation(bool enabled = true)
    {
        var physicalStore = await _staticCacheManager.GetAsync(CacheKeys.AllLocationsKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT * FROM [{LocationTable}]";
            var list = (await conn.QueryAsync<Location>(sql)).AsList();
            return list;
        });

        if (enabled)
            physicalStore = physicalStore.Where(x => x.Published).ToList();

        return physicalStore;
    }

    private async Task<IList<StateProvince>> GetAllCachedStateProvinces()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllStateProvincesKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT * FROM [{StateProvinceTable}] WHERE Published = 1";
            var list = (await conn.QueryAsync<StateProvince>(sql)).AsList();
            return list;
        });
    }

    private string ConvertToTime(TimeSpan? time)
    {
        DateTime dateTime = DateTime.Today.Add(time.Value);
        return dateTime.ToString("h:mmtt", CultureInfo.InvariantCulture).ToUpper();
    }

    private async Task<IList<LocationHour>> GetLocationHoursByLocationIdAsync(int locationid, int hourTypeId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.LocationHoursCacheKey, locationid, hourTypeId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT * FROM [{LocationHourTable}] WHERE LocationId = @LocationId AND HourTypeId = @HourTypeId";
            var list = (await conn.QueryAsync<LocationHour>(sql, new { LocationId = locationid, HourTypeId = hourTypeId })).AsList();
            return list;
        });
    }

    #endregion

    #region Methods

    public async Task<IList<Address>> GetAddressedByIdsAsync(int[] ids)
    {
        if (ids == null || ids.Length == 0)
            return new List<Address>();
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{AddressTable}] WHERE Id IN @Ids";
        var list = (await conn.QueryAsync<Address>(sql, new { Ids = ids })).AsList();
        return list.OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
    }

    public async Task<IList<StateProvince>> GetStateProvinceByIdsAsync(int[] ids)
    {
        if (ids == null || ids.Length == 0)
            return new List<StateProvince>();
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{StateProvinceTable}] WHERE Id IN @Ids";
        var list = (await conn.QueryAsync<StateProvince>(sql, new { Ids = ids })).AsList();
        return list.OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
    }

    public async Task<IList<StateProvince>> GetAllStatesAsync(string threeLetterIsoCode = null, int countryId = 0)
    {
        var query = await GetAllCachedStateProvinces();

        if (!string.IsNullOrEmpty(threeLetterIsoCode) || countryId != 0)
        {
            HashSet<int> countryIds;
            using (var conn = _db.CreateNop())
            {
                if (countryId > 0)
                {
                    var c = await conn.QueryFirstOrDefaultAsync<Country>($"SELECT Id FROM [{CountryTable}] WHERE Id = @Id", new { Id = countryId });
                    countryIds = c != null ? new HashSet<int> { c.Id } : new HashSet<int>();
                }
                else if (!string.IsNullOrEmpty(threeLetterIsoCode))
                {
                    var list = (await conn.QueryAsync<Country>($"SELECT Id FROM [{CountryTable}] WHERE LOWER(ThreeLetterIsoCode) = LOWER(@Code)", new { Code = threeLetterIsoCode })).AsList();
                    countryIds = list.Select(x => x.Id).ToHashSet();
                }
                else
                    countryIds = (await conn.QueryAsync<int>($"SELECT Id FROM [{CountryTable}]")).ToHashSet();

                query = query.Where(q => countryIds.Contains(q.CountryId)).ToList();
            }
        }

        return query.ToList();
    }

    public async Task<IList<Location>> GetLocationsAsync(int websiteId = 0)
    {
        var query = await GetAllCachedLocation();
        if (websiteId > 0)
            query = query.Where(x => x.WebsiteId == websiteId).ToList();
        return query;
    }

    public async Task<StateProvince> GetStateByAbbreviationAsync(string abbreviation)
    {
        if (string.IsNullOrEmpty(abbreviation))
            return null;
        return (await GetAllCachedStateProvinces()).FirstOrDefault(x => string.Equals(x.Abbreviation, abbreviation, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Location> GetLocationByLocationIdAsync(string locationId)
    {
        if (string.IsNullOrEmpty(locationId))
            return null;
        if (!int.TryParse(locationId, out var id))
            return null;
        return (await GetAllCachedLocation()).FirstOrDefault(x => x.LocationId == id);
    }

    public async Task<bool> IsLocationStoreOpenAsync(int locationid)
    {
        var locationhours = await GetLocationHoursByLocationIdAsync(locationid, (int)HourTypeEnum.Store);
        var currentDate = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
        var currentDay = (int)currentDate.DayOfWeek;
        var currentTime = currentDate.TimeOfDay;

            foreach (var locationhour in locationhours)
            {
                var openingHour = locationhour.OpeningHour;
                var closingHour = locationhour.ClosingHour;

                if (locationhour.DayId == currentDay && openingHour.HasValue && closingHour.HasValue)
                {
                    if (currentTime >= openingHour && currentTime <= closingHour)
                        return true;
                }
            }

        return false;
    }

    public async Task<string> GetLocationHoursStringByLocationIdAsync(int locationid, int type)
    {
        var locationhours = await GetLocationHoursByLocationIdAsync(locationid, type);
        var hoursstring = string.Empty;
            foreach (var locationhour in locationhours)
            {
                var openingHour = locationhour.OpeningHour;
                var closingHour = locationhour.ClosingHour;

                if (openingHour.HasValue)
                    hoursstring += Enum.GetName(typeof(DayOfWeek), locationhour.Day) + ": " + ConvertToTime(openingHour) + " - " + ConvertToTime(closingHour) + "<br />";
                else
                    hoursstring += Enum.GetName(typeof(DayOfWeek), locationhour.Day) + ": Closed" + "<br />";
            }

        return hoursstring;
    }

    public async Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false)
    {
        var query = (await GetAllCachedStateProvinces())
            .Where(sp => showHidden || sp.Published)
            .OrderBy(sp => sp.CountryId).ThenBy(sp => sp.DisplayOrder).ThenBy(sp => sp.Name)
            .ToList();
        return query;
    }

    public async Task<GoogleResponseCache> GetLocationDetailAsync(Location location)
    {
        try
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.PhysicalStoreGoogleResponseCacheKey, location.Id);

            var cachedResponse = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                if (location == null)
                    return null;

                var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
                var credentialsSettings = await _settingService.LoadSettingAsync<FMCommonSettings>(storeId);

                string placeId = string.Empty;
                var response = await _httpClientService.GetAsync(requestUri: "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + location.Name + "&key=" + credentialsSettings.StoreLocatorGeocodingAPIKey + "&location=" + location.Latitude + "," + location.Longitude, requestHeaders: null);
                if (string.IsNullOrEmpty(response.responseResult))
                    return null;

                var googleAddress = JsonConvert.DeserializeObject<GoogleAddress>(response.responseResult);
                if (googleAddress != null && googleAddress.Results != null &&
                    googleAddress.Results.Count != 0 && googleAddress.Status.ToLower() == "ok")
                    placeId = googleAddress.Results.Select(x => x.place_id).FirstOrDefault();

                if (googleAddress != null && !string.IsNullOrEmpty(googleAddress.error_message))
                {
                    _logService.Error("GetPlaceIdByPhysicalStore: Google Map API Error\n" + googleAddress.error_message);
                    return null;
                }

                if (string.IsNullOrEmpty(placeId))
                    return null;

                response = await _httpClientService.GetAsync(requestUri: "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeId + "&key=" + credentialsSettings.StoreLocatorGeocodingAPIKey, requestHeaders: null);
                if (string.IsNullOrEmpty(response.responseResult))
                    return null;

                var rootObject = JsonConvert.DeserializeObject<RootObject>(response.responseResult);
                if (rootObject == null || rootObject.result == null || rootObject.status.ToLower() != "ok")
                    return null;
                if (rootObject != null && !string.IsNullOrEmpty(rootObject.error_message))
                {
                    _logService.Error("GetStoreDetailByPlaceId: Google Map API Error\n" + rootObject.error_message + ":\n status" + googleAddress.Status);
                    return null;
                }

                var googleResponseCache = new GoogleResponseCache
                {
                    PhysicalStoreId = location.Id,
                    Vicinity = rootObject.result.vicinity,
                    Rating = rootObject.result.rating,
                    TotalGoogleRating = rootObject.result.user_ratings_total,
                    GooglePhysicalStoreName = rootObject.result.name,
                    StoreDetail = rootObject.result.StoreDetail,
                    Place_id = rootObject.result.place_id,
                };
                if (rootObject.result.reviews != null)
                    googleResponseCache.PhysicalStoreReviews = rootObject.result.reviews.Select(s => new PhysicalStoreReview
                    {
                        Author_name = s.author_name,
                        Author_url = s.author_url,
                        Language = s.language,
                        Profile_photo_url = s.profile_photo_url,
                        Rating = s.rating,
                        Relative_time_description = s.relative_time_description,
                        Text = s.text,
                        Time = s.time
                    }).ToList();

                return googleResponseCache;
            });

            return cachedResponse;
        }
        catch (Exception ex)
        {
            _logService.Error(ex.Message, ex);
        }

        return null;
    }

    public async Task<Location> GetLocationByUKGGuidIdAsync(string ukgGuidId)
    {
        if (string.IsNullOrEmpty(ukgGuidId))
            return null;
        var locations = await GetAllCachedLocation(enabled: false);
        return locations.FirstOrDefault(x => x.UKGGuid != null && x.UKGGuid.Trim().Equals(ukgGuidId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Address> GetAddressByAddressIdAsync(int addressId)
    {
        if (addressId <= 0)
            return null;
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{AddressTable}] WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Address>(sql, new { Id = addressId });
    }

    public async Task<StateProvince> GetStateByStateIdAsync(int stateId)
    {
        if (stateId <= 0)
            return null;
        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{StateProvinceTable}] WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<StateProvince>(sql, new { Id = stateId });
    }

    #endregion
}
