using Career.Data.Domains.Common;
using Career.Data.Domains.Directory;
using Career.Data.Domains.Locations;
using Career.Data.Domains.PhysicalStores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Locations;

/// <summary>
/// Store management service interface
/// </summary>
public interface ILocationService
{
    Task<IList<Address>> GetAddressedByIdsAsync(int[] addressIds);

    Task<IList<StateProvince>> GetStateProvinceByIdsAsync(int[] stateProvinceIds);
   
    /// <summary>
    /// Gets all states
    /// <param name="threeLetterIsoCode">threeLetterIsoCode</param>
    /// <param name="countryId">countryId</param>
    /// </summary>
    Task<IList<StateProvince>> GetAllStatesAsync(string threeLetterIsoCode = null, int countryId = 0);

    /// <summary>
    /// Gets active locations
    /// <param name="websiteId">websiteId</param>
    /// </summary>
    Task<IList<Location>> GetLocationsAsync(int websiteId = 0);

    /// <summary>
    /// Gets state
    /// <param name="abbreviation">abbreviation</param>
    /// </summary>
    Task<StateProvince> GetStateByAbbreviationAsync(string abbreviation);

    /// <summary>
    /// Get location
    /// </summary>
    /// <param name="locationId">locationId</param>
    /// <returns>Location</returns>
    Task<Location> GetLocationByLocationIdAsync(string locationId);

    /// <summary>
    /// is store is open or closed
    /// </summary>
    /// <param name="locationid">locationid</param>
    /// <returns>result</returns>
    Task<bool> IsLocationStoreOpenAsync(int locationid);

    /// <summary>
    /// Get location hours
    /// </summary>
    /// <param name="locationid">locationid</param
    /// <param name="type">type</param>
    /// <returns>StoreHours</returns>
    Task<string> GetLocationHoursStringByLocationIdAsync(int locationid, int type);

    /// <summary>
    /// Gets all states/provinces
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>States</returns>
    Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false);

    /// <summary>
    ///  Get Place Id by Store Name
    /// </summary>
    /// <param name="storeManagement">storeManagement</param>
    /// <returns>place_id</returns>
    Task<GoogleResponseCache> GetLocationDetailAsync(Location location);

    /// <summary>
    /// Get location
    /// </summary>
    /// <param name="ukgGuidId">ukgGuidId</param>
    /// <returns>Location</returns>
    Task<Location> GetLocationByUKGGuidIdAsync(string ukgGuidId);

    /// <summary>
    /// Gets address
    /// <param name="addressId">addressId</param>
    /// </summary>
    Task<Address> GetAddressByAddressIdAsync(int addressId);

    /// <summary>
    /// Gets state
    /// <param name="stateId">stateId</param>
    /// </summary>
    Task<StateProvince> GetStateByStateIdAsync(int stateId);
}
