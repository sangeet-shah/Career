using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.CorporateManagement;
using Middleware.Web.Domains.Locations;
using Middleware.Web.Models.StoreManagement;
using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Locations;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using System.Text.RegularExpressions;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class PhysicalStoreController : ControllerBase
{
    #region Fields

    private readonly IPictureService _pictureService;
    private readonly ILocationService _locationService;
    private readonly ICommonService _commonService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;
    private readonly IBannerManagementService _bannerManagementService;

    #endregion

    #region Ctor

    public PhysicalStoreController(
        IPictureService pictureService,
        ILocationService locationService,
        ICommonService commonService,
        ISettingService settingService,
        IStoreService storeService,
        IBannerManagementService bannerManagementService)
    {
        _pictureService = pictureService;
        _locationService = locationService;
        _commonService = commonService;
        _settingService = settingService;
        _storeService = storeService;
        _bannerManagementService = bannerManagementService;
    }

    #endregion

    #region Method

    [HttpGet("{location}")]
    public async Task<IActionResult> PhysicalStoreDetail(string location)
    {
        if (string.IsNullOrEmpty(location) || !Regex.IsMatch(location, @"^\d+$"))
            return NotFound();

        var locationDetail = await _locationService.GetLocationByLocationIdAsync(location);
        if (locationDetail == null || !locationDetail.Published || locationDetail.WebsiteId != (int)WebsiteEnum.FMUSA)
            return NotFound();

        var model = new LocationDetailModel();
        model.StoreOpenCloseMessage = await _locationService.IsLocationStoreOpenAsync(locationDetail.LocationId) ? "We're Open" : "We're Closed";
        model.StoreHoursFromDb = await _locationService.GetLocationHoursStringByLocationIdAsync(locationDetail.LocationId, (int)HourTypeEnum.Store);
        model.PickupHoursFromDb = await _locationService.GetLocationHoursStringByLocationIdAsync(locationDetail.LocationId, (int)HourTypeEnum.Pickup);

        model.StorePictureURL = await _pictureService.GetPictureUrlAsync(locationDetail.PictureId);

        var banner = await _bannerManagementService.GetBannerByLocationIdAsync(locationDetail.LocationId);
        if (banner != null)
        {
            var picture = await _pictureService.GetPictureByIdAsync(banner.PictureId);
            model.PhysicalStoreBannerModel.ImageUrl = await _pictureService.GetPictureUrlAsync(banner.PictureId);
            model.PhysicalStoreBannerModel.Alt = picture.AltAttribute ?? banner.Title;
            model.PhysicalStoreBannerModel.Title = banner.Title;
            model.PhysicalStoreBannerModel.Url = banner.Url;
        }

        var address = await _locationService.GetAddressByAddressIdAsync(locationDetail.AddressId);
        model.StoreAddresFromDb = address?.Address1 ?? string.Empty;
        model.StoreName = locationDetail.Name;
        model.TourVideoLink = locationDetail.TourVideoUrl;
        model.Longitude = locationDetail.Longitude;
        model.Latitude = locationDetail.Latitude;
        model.StoreId = locationDetail.Id;
        model.LocationId = locationDetail.LocationId;
        model.GoogleUrlFromDb = locationDetail.GoogleUrl;
        model.FacebookUrlFromDb = locationDetail.FacebookUrl;
        model.TwitterUrlFromDb = locationDetail.TwitterUrl;
        model.LinkedInUrlFromDb = locationDetail.LinkedInUrl;
        model.PinterestUrlFromDb = locationDetail.PinterestUrl;
        model.InstagramUrlFromDb = locationDetail.InstagramUrl;
        model.YouTubeUrlFromDb = locationDetail.YouTubeUrl;
        model.StoreAddress = address?.Address1 ?? string.Empty;
        model.City = address.City;
        model.State = address.StateProvinceId.HasValue ? (await _locationService.GetStateByStateIdAsync(address.StateProvinceId.Value))?.Name : string.Empty;
        model.MetaDescription = locationDetail.MetaDescription;
        model.Zipcode = address.ZipPostalCode;
        model.Description = locationDetail.Description;
        model.PhoneNumber = address.PhoneNumber;

        model.StoreInfoModel.Description = locationDetail.Description;
        model.MetaDescription = locationDetail.MetaDescription;
        model.MetaTitle = locationDetail.MetaTitle;

        var seoPictureId1Url = await _pictureService.GetPictureUrlAsync(locationDetail.SEOPictureId1, showDefaultPicture: false);
        if (!string.IsNullOrEmpty(seoPictureId1Url))
            model.SchemaPictureUrls.Add(seoPictureId1Url);

        var seoPictureId2Url = await _pictureService.GetPictureUrlAsync(locationDetail.SEOPictureId2, showDefaultPicture: false);
        if (!string.IsNullOrEmpty(seoPictureId2Url))
            model.SchemaPictureUrls.Add(seoPictureId2Url);

        var seoPictureId3Url = await _pictureService.GetPictureUrlAsync(locationDetail.SEOPictureId3, showDefaultPicture: false);
        if (!string.IsNullOrEmpty(seoPictureId3Url))
            model.SchemaPictureUrls.Add(seoPictureId3Url);

        var googleResponseCache = await _locationService.GetLocationDetailAsync(locationDetail);
        if (googleResponseCache != null)
        {
            model.Vicinity = googleResponseCache.Vicinity;
            model.Rating = googleResponseCache.Rating;
            model.StoreDetail = googleResponseCache.StoreDetail;
            model.Place_id = googleResponseCache.Place_id;
            model.TotalGoogleRating = googleResponseCache.TotalGoogleRating;
            model.GooglePhysicalStoreName = googleResponseCache.GooglePhysicalStoreName;
            model.PhysicalStoreReviews = googleResponseCache.PhysicalStoreReviews.Select(s => new Middleware.Web.Domains.PhysicalStores.GoogleResponseCache.PhysicalStoreReview
            {
                Author_name = s.Author_name,
                Author_url = s.Author_url,
                Language = s.Language,
                Profile_photo_url = s.Profile_photo_url,
                Rating = s.Rating,
                Relative_time_description = s.Relative_time_description,
                Text = s.Text,
                Time = s.Time
            }).ToList();
        }

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>(storeId);
        if (corporateManagementSettings != null)
        {
            model.LocationFaceBookIconUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LocationFaceBookIconId, showDefaultPicture: false);
            model.LocationGoogleIconUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LocationGoogleIconId, showDefaultPicture: false);
            model.LocationTwitterIconUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LocationTwitterIconId, showDefaultPicture: false);
            model.LocationPinterestIconUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LocationPinterestIconId, showDefaultPicture: false);
            model.LocationLinkedInIconUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LocationLinkedInIconId, showDefaultPicture: false);
            model.LocationInstagramIconUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LocationInstagramIconId, showDefaultPicture: false);
            model.LocationYouTubeIconUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.LocationYouTubeIconId, showDefaultPicture: false);
        }

        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var physicalStores = await _locationService.GetLocationsAsync();
        var addressIds = physicalStores.Select(x => x.AddressId).Distinct().ToArray();

        var addresses = await _locationService.GetAddressedByIdsAsync(addressIds);

        var stateProvinceIds = addresses
               .Where(a => a.StateProvinceId.HasValue)
               .Select(a => a.StateProvinceId.Value)
               .Distinct()
               .ToArray();
        var states = await _locationService.GetStateProvinceByIdsAsync(stateProvinceIds);

        var addressDict = addresses.ToDictionary(a => a.Id);
        var stateDict = states.ToDictionary(s => s.Id);

        var enrichedStores = new List<(Location Store, string StateAbbreviation, string City)>();
        foreach (var store in physicalStores)
        {
            if (store.AddressId > 0 && addressDict.TryGetValue(store.AddressId, out var address))
            {
                var stateAbbr = string.Empty;
                if (address.StateProvinceId.HasValue && stateDict.TryGetValue(address.StateProvinceId.Value, out var state))
                    stateAbbr = state.Abbreviation;

                enrichedStores.Add((store, stateAbbr, address.City));
            }
        }
        var groupedByState = enrichedStores.ToLookup(x => x.StateAbbreviation);

        var locationList = new List<LocationStateListModel>();
        foreach (var stateGroup in groupedByState)
        {
            var stateName = (await _locationService.GetStateByAbbreviationAsync(stateGroup.Key))?.Name ?? string.Empty;

            var cityGroups = stateGroup.ToLookup(x => x.City);
            var cityList = cityGroups.Select(cityGroup => new LocationStateListModel.CityModel
            {
                City = cityGroup.Key,
                PhysicalStoreListModel = cityGroup.Select(s => new LocationStateListModel.CityModel.LocationModel
                {
                    Id = s.Store.LocationId,
                    WebsiteId = s.Store.WebsiteId,
                    StoreUrl = !string.IsNullOrEmpty(s.Store.StoreUrl) ? s.Store.StoreUrl : "#",
                    Name = s.Store.FMUSAName
                }).ToList()
            }).ToList();

            locationList.Add(new LocationStateListModel
            {
                StateName = stateName,
                State = stateGroup.Key,
                CityListModel = cityList
            });
        }

        var model = new LocationModel
        {
            LocationList = locationList
        };

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>(storeId);

        var picture = await _pictureService.GetPictureByIdAsync(corporateManagementSettings.MapImageId);
        model.LocationMapImageUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.MapImageId);
        model.LocationMapImageAltText = picture?.AltAttribute ?? string.Empty;
        model.LocationMapImageTitle = picture?.TitleAttribute ?? string.Empty;

        return Ok(model);
    }

    #endregion
}
