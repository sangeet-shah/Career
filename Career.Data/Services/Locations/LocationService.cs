using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.Directory;
using Career.Data.Domains.Locations;
using Career.Data.Domains.PhysicalStores;
using Career.Data.Extensions;
using Career.Data.Services.Common;
using Career.Data.Services.Helpers;
using Career.Data.Services.Logs;
using Career.Data.Services.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Career.Data.Domains.PhysicalStores.GoogleResponseCache;

namespace Career.Data.Services.Locations;

/// <summary>
/// Store management service interface
/// </summary>
public class LocationService : ILocationService
{
    #region Fields

    private readonly IRepository<Location> _locationRepository;
    private readonly IRepository<StateProvince> _stateProvincerepository;
    private readonly IRepository<LocationHour> _locationHoursRepository;
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<Address> _addressRepository;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IHttpClientService _httpClientService;
    private readonly ILogService _logService;
    private readonly ISettingService _settingService;
    private readonly IDateTimeHelper _dateTimeHelper;

    #endregion

    #region Ctor

    public LocationService(IRepository<StateProvince> stateProvincerepository,
        IRepository<Country> countryRepository,
        IStaticCacheManager staticCacheManager,
        IHttpClientService httpClientService,
        ILogService logService,
        ISettingService settingService,
        IRepository<Location> locationRepository,
        IRepository<LocationHour> locationHoursRepository,
        IRepository<Address> addressRepository,
        IDateTimeHelper dateTimeHelper)
    {
        _stateProvincerepository = stateProvincerepository;
        _countryRepository = countryRepository;
        _staticCacheManager = staticCacheManager;
        _httpClientService = httpClientService;
        _logService = logService;
        _settingService = settingService;
        _locationRepository = locationRepository;
        _locationHoursRepository = locationHoursRepository;
        _addressRepository = addressRepository;
        _dateTimeHelper = dateTimeHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get all cached physical stores
    /// </summary>
    /// <returns>PhysicalStores</returns>
    private async Task<IList<Location>> GetAllCachedLocation(bool enabled = true)
    {
        var physicalStore = await _staticCacheManager.GetAsync(CacheKeys.AllLocationsKey, async () =>
        {
            return await (from p in _locationRepository.Table
                          select p).ToListAsync();
        });

        if (enabled)
            physicalStore = physicalStore.Where(x => x.Published).ToList();

        return physicalStore;
    }

    /// <summary>
    /// Get all cached StateProvince
    /// </summary>
    /// <returns>StateProvinces</returns>
    private async Task<IList<StateProvince>> GetAllCachedStateProvinces()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllStateProvincesKey, async () =>
        {
            return await (from p in _stateProvincerepository.Table
                          where p.Published
                          select p).ToListAsync();
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
            return await _locationHoursRepository.Table.Where(x => x.LocationId == locationid && x.HourTypeId == hourTypeId).ToListAsync();
        });
    }

    #endregion

    #region Methods

    public async Task<IList<Address>> GetAddressedByIdsAsync(int[] ids)
    {
        return await _addressRepository.GetByIdsAsync(ids,cache => default);       
    }
    public async Task<IList<StateProvince>> GetStateProvinceByIdsAsync(int[] ids)
    {
        return await _stateProvincerepository.GetByIdsAsync(ids, cache => default);       
    }

    /// <summary>
    /// Gets all states
    /// <param name="threeLetterIsoCode">threeLetterIsoCode</param>
    /// <param name="countryId">countryId</param>
    /// </summary>
    public async Task<IList<StateProvince>> GetAllStatesAsync(string threeLetterIsoCode = null, int countryId = 0)
    {
        var query = await GetAllCachedStateProvinces();

        if (!string.IsNullOrEmpty(threeLetterIsoCode))
            query = (from q in query
                     join c in _countryRepository.Table on q.CountryId equals c.Id
                     where c.ThreeLetterIsoCode.ToLower() == threeLetterIsoCode.ToLower()
                     select q).ToList();

        if (countryId != 0)
            query = (from q in query
                     join c in _countryRepository.Table on q.CountryId equals c.Id
                     where c.Id == countryId
                     select q).ToList();

        return query.ToList();
    }

    /// <summary>
    /// Gets active locations
    /// <param name="websiteId">websiteId</param>
    /// </summary>
    public async Task<IList<Location>> GetLocationsAsync(int websiteId = 0)
    {
        var query = await GetAllCachedLocation();
        if (websiteId > 0)
            query = query.Where(x => x.WebsiteId == websiteId).ToList();        

        return query;
    }

    /// <summary>
    /// Gets state
    /// <param name="abbreviation">abbreviation</param>
    /// </summary>
    public async Task<StateProvince> GetStateByAbbreviationAsync(string abbreviation)
    {
        if (string.IsNullOrEmpty(abbreviation))
            return null;

        return (await GetAllCachedStateProvinces()).Where(x => x.Abbreviation.ToLower() == abbreviation.ToLower()).FirstOrDefault();
    }

    /// <summary>
    /// Get location
    /// </summary>
    /// <param name="locationId">locationId</param>
    /// <returns>Location</returns>
    public async Task<Location> GetLocationByLocationIdAsync(string locationId)
    {
        if (string.IsNullOrEmpty(locationId))
            return null;

        return (await GetAllCachedLocation()).Where(x => x.LocationId == Convert.ToInt32(locationId)).FirstOrDefault();
    }

    #region Location hours

    /// <summary>
    /// is store is open or closed
    /// </summary>
    /// <param name="locationid">locationid</param>
    /// <returns>result</returns>
    public async Task<bool> IsLocationStoreOpenAsync(int locationid)
    {        
        var locationhours= await GetLocationHoursByLocationIdAsync(locationid, (int)HourTypeEnum.Store);
        var currentDate = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
        var currentDay = (int)currentDate.DayOfWeek;
        var currentTime = currentDate.TimeOfDay;

        foreach (var locationhour in locationhours)
        {
            TimeSpan? openingHour = null;
            if (!string.IsNullOrWhiteSpace(locationhour.OpeningHour) && TimeSpan.TryParse(locationhour.OpeningHour, out var parsedOpening))
                openingHour = parsedOpening;

            TimeSpan? closingHour = null;
            if (!string.IsNullOrWhiteSpace(locationhour.ClosingHour) && TimeSpan.TryParse(locationhour.ClosingHour, out var parsedClosing))
                closingHour = parsedClosing;

            if (locationhour.DayId == currentDay && openingHour.HasValue && closingHour.HasValue)
            {
                if (currentTime >= openingHour && currentTime <= closingHour)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get location hours
    /// </summary>
    /// <param name="locationid">locationid</param
    /// <param name="type">type</param>
    /// <returns>StoreHours</returns>
    public async Task<string> GetLocationHoursStringByLocationIdAsync(int locationid, int type)
    {        
        var locationhours = await GetLocationHoursByLocationIdAsync(locationid, type);
        var hoursstring = string.Empty;
        foreach (var locationhour in locationhours)
        {
            TimeSpan? openingHour = null;
            if (!string.IsNullOrWhiteSpace(locationhour.OpeningHour) && TimeSpan.TryParse(locationhour.OpeningHour, out var parsedOpening))
                openingHour = parsedOpening;

            TimeSpan? closingHour = null;
            if (!string.IsNullOrWhiteSpace(locationhour.ClosingHour) && TimeSpan.TryParse(locationhour.ClosingHour, out var parsedClosing))
                closingHour = parsedClosing;

            if (openingHour.HasValue)
                hoursstring += Enum.GetName(typeof(DayOfWeek), locationhour.Day) + ": " + ConvertToTime(openingHour) + " - " + ConvertToTime(closingHour) + "<br />";
            else
                hoursstring += Enum.GetName(typeof(DayOfWeek), locationhour.Day) + ": Closed" + "<br />";
        }

        return hoursstring;
    }

    #endregion

    /// <summary>
    /// Gets all states/provinces
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>States</returns>
    public async Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false)
    {
        var query = from sp in await GetAllCachedStateProvinces()
                    orderby sp.CountryId, sp.DisplayOrder, sp.Name
                    where showHidden || sp.Published
                    select sp;
        return query.ToList();
    }

    /// <summary>
    ///  Get Place Id by Store Name
    /// </summary>
    /// <param name="storeManagement">storeManagement</param>
    /// <returns>place_id</returns>
    public async Task<GoogleResponseCache> GetLocationDetailAsync(Location location)
    {
        try
        {
            //cache result between HTTP requests
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.PhysicalStoreGoogleResponseCacheKey, location.Id);

            //get current password usage time
            var cachedResponse = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                if (location == null)
                    return null;

                var credentialsSettings = await _settingService.LoadSettingAsync<FMCommonSettings>();

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
                else if (rootObject != null && !string.IsNullOrEmpty(rootObject.error_message))
                {
                    _logService.Error("GetStoreDetailByPlaceId: Google Map API Error\n" + rootObject.error_message + ":\n status" + googleAddress.Status);
                    return null;
                }

                // cast to cached
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

    /// <summary>
    /// Get location
    /// </summary>
    /// <param name="ukgGuidId">ukgGuidId</param>
    /// <returns>Location</returns>
    public async Task<Location> GetLocationByUKGGuidIdAsync(string ukgGuidId)
    {
        if (string.IsNullOrEmpty(ukgGuidId))
            return null;

        var locations = await GetAllCachedLocation(enabled: false);
        return locations.FirstOrDefault(x => x.UKGGuid != null && x.UKGGuid.Trim().Equals(ukgGuidId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets address
    /// <param name="addressId">addressId</param>
    /// </summary>
    public async Task<Address> GetAddressByAddressIdAsync(int addressId)
    {
        if (addressId <= 0)
            return null;

        return await _addressRepository.GetByIdAsync(addressId, cache => default);
    }

    /// <summary>
    /// Gets state
    /// <param name="stateId">stateId</param>
    /// </summary>
    public async Task<StateProvince> GetStateByStateIdAsync(int stateId)
    {
        if (stateId <= 0)
            return null;

        return await _stateProvincerepository.GetByIdAsync(stateId, cache => default);
    }

    #endregion
}
