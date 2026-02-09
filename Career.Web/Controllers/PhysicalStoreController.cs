using Career.Web.Domains.Common;
using Career.Web.Domains.Locations;
using Career.Web.Infrastructure;
using Career.Web.Models.Api;
using Career.Web.Models.StoreManagement;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class PhysicalStoreController : BaseController
{
    #region Fields

    private readonly IApiClient _apiClient;
    private readonly IUserAgentHelper _userAgentHelper;

    #endregion

    #region Ctor

    public PhysicalStoreController(
        IApiClient apiClient,
        IUserAgentHelper userAgentHelper)
    {
        _apiClient = apiClient;
        _userAgentHelper = userAgentHelper;
    }

    #endregion

    #region Method

    public async Task<IActionResult> PhysicalStoreDetail(string location)
    {
        if (string.IsNullOrEmpty(location) || !Regex.IsMatch(location, @"^\d+$"))
            return InvokeHttp404();

        // get the store detail
        var locationDetail = await _apiClient.GetAsync<LocationDto>("api/Location/GetByLocationId", new { locationId = location });
        if (locationDetail == null || !locationDetail.Published || locationDetail.WebsiteId != (int)WebsiteEnum.FMUSA)
            return InvokeHttp404();

        var model = new LocationDetailModel();

        // Get store hours and open/close status
        var isOpenRes = await _apiClient.GetAsync<bool?>("api/Location/IsLocationStoreOpen", new { locationid = locationDetail.LocationId });
        model.StoreOpenCloseMessage = (isOpenRes == true) ? "We're Open" : "We're Closed";

        var storeHoursRes = await _apiClient.GetStringAsync("api/Location/GetLocationHoursString", new { locationid = locationDetail.LocationId, type = (int)HourTypeEnum.Store });
        model.StoreHoursFromDb = storeHoursRes;

        var pickupHoursRes = await _apiClient.GetStringAsync("api/Location/GetLocationHoursString", new { locationid = locationDetail.LocationId, type = (int)HourTypeEnum.Pickup });
        model.PickupHoursFromDb = pickupHoursRes;

        // picture only need to display for FM theme only
        var pictureUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = locationDetail.PictureId, showDefaultPicture = true });
        model.StorePictureURL = pictureUrlRes?.Url;

        // get active banner
        var banner = await _apiClient.GetAsync<BannerDto>("api/BannerManagement/GetBannerByLocationId", new { locationId = locationDetail.LocationId });
        if (banner != null)
        {
            var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = banner.PictureId });
            var bannerUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = banner.PictureId, showDefaultPicture = true });
            model.PhysicalStoreBannerModel.ImageUrl = bannerUrlRes?.Url;
            model.PhysicalStoreBannerModel.Alt = picture?.AltAttribute ?? banner.Title;
            model.PhysicalStoreBannerModel.Title = banner.Title;
            model.PhysicalStoreBannerModel.Url = banner.Url;
        }

        var address = await _apiClient.GetAsync<AddressDto>("api/Location/GetAddressByAddressId", new { addressId = locationDetail.AddressId });
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
        model.City = address?.City;
        model.State = string.Empty;
        if (address?.StateProvinceId.HasValue == true)
        {
            var state = await _apiClient.GetAsync<StateProvinceDto>("api/Location/GetStateByStateId", new { stateId = address.StateProvinceId.Value });
            model.State = state?.Name ?? string.Empty;
        }
        model.MetaDescription = locationDetail.MetaDescription;
        model.Zipcode = address?.ZipPostalCode;
        model.Description = locationDetail.Description;
        model.PhoneNumber = address?.PhoneNumber;

        //StoreInfo
        model.StoreInfoModel.Description = locationDetail.Description;
        model.MetaDescription = locationDetail.MetaDescription;
        model.MetaTitle = locationDetail.MetaTitle;

        // SEO pictures
        var seoPictureId1UrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = locationDetail.SEOPictureId1, showDefaultPicture = false });
        if (!string.IsNullOrEmpty(seoPictureId1UrlRes?.Url))
            model.SchemaPictureUrls.Add(seoPictureId1UrlRes.Url);

        var seoPictureId2UrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = locationDetail.SEOPictureId2, showDefaultPicture = false });
        if (!string.IsNullOrEmpty(seoPictureId2UrlRes?.Url))
            model.SchemaPictureUrls.Add(seoPictureId2UrlRes.Url);

        var seoPictureId3UrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = locationDetail.SEOPictureId3, showDefaultPicture = false });
        if (!string.IsNullOrEmpty(seoPictureId3UrlRes?.Url))
            model.SchemaPictureUrls.Add(seoPictureId3UrlRes.Url);

        // google response
        var googleResponseCache = await _apiClient.PostAsync<object, GoogleResponseCacheDto>("api/Location/GetLocationDetail", new { LocationId = location });
        if (googleResponseCache != null)
        {
            model.Vicinity = googleResponseCache.Vicinity;
            model.Rating = googleResponseCache.Rating;
            model.StoreDetail = googleResponseCache.StoreDetail;
            model.Place_id = googleResponseCache.Place_id;
            model.TotalGoogleRating = googleResponseCache.TotalGoogleRating;
            model.GooglePhysicalStoreName = googleResponseCache.GooglePhysicalStoreName;
            model.PhysicalStoreReviews = googleResponseCache.PhysicalStoreReviews;
        }

        var corporateManagementSettings = await _apiClient.GetAsync<FMCorporateManagementSettingsDto>("api/Setting/GetFMCorporateManagementSettings", new { storeId = 0 });
        if (corporateManagementSettings != null)
        {
            var fbIconUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.LocationFaceBookIconId, showDefaultPicture = false });
            model.LocationFaceBookIconUrl = fbIconUrlRes?.Url;

            var googleIconUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.LocationGoogleIconId, showDefaultPicture = false });
            model.LocationGoogleIconUrl = googleIconUrlRes?.Url;

            var twitterIconUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.LocationTwitterIconId, showDefaultPicture = false });
            model.LocationTwitterIconUrl = twitterIconUrlRes?.Url;

            var pinterestIconUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.LocationPinterestIconId, showDefaultPicture = false });
            model.LocationPinterestIconUrl = pinterestIconUrlRes?.Url;

            var linkedinIconUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.LocationLinkedInIconId, showDefaultPicture = false });
            model.LocationLinkedInIconUrl = linkedinIconUrlRes?.Url;

            var instagramIconUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.LocationInstagramIconId, showDefaultPicture = false });
            model.LocationInstagramIconUrl = instagramIconUrlRes?.Url;

            var youtubeIconUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.LocationYouTubeIconId, showDefaultPicture = false });
            model.LocationYouTubeIconUrl = youtubeIconUrlRes?.Url;
        }

        return View(model);
    }

    public async Task<IActionResult> List()
    {
        var physicalStores = await _apiClient.GetAsync<LocationDto[]>("api/Location/GetLocations", new { websiteId = 0 });
        var addressIds = physicalStores.Select(x => x.AddressId).Distinct().ToArray();

        var addresses = await _apiClient.PostAsync<int[], AddressDto[]>("api/Location/GetAddressedByIds", addressIds);

        var stateProvinceIds = addresses
               .Where(a => a.StateProvinceId.HasValue)
               .Select(a => a.StateProvinceId.Value)
               .Distinct()
               .ToArray();
        var states = await _apiClient.PostAsync<int[], StateProvinceDto[]>("api/Location/GetStateProvinceByIds", stateProvinceIds);

        var addressDict = addresses.ToDictionary(a => a.Id);
        var stateDict = states.ToDictionary(s => s.Id);

        var enrichedStores = new List<(LocationDto Store, string StateAbbreviation, string City)>();
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
            var state = await _apiClient.GetAsync<StateProvinceDto>("api/Location/GetStateByAbbreviation", new { abbreviation = stateGroup.Key });
            var stateName = state?.Name ?? string.Empty;

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


        // Prepare map image
        var model = new LocationModel
        {
            LocationList = locationList,
            IsMobile = _userAgentHelper.IsMobileDevice()
        };

        var corporateManagementSettings = await _apiClient.GetAsync<FMCorporateManagementSettingsDto>("api/Setting/GetFMCorporateManagementSettings", new { storeId = 0 });

        var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = corporateManagementSettings.MapImageId });
        var mapImageUrlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId = corporateManagementSettings.MapImageId, showDefaultPicture = true });
        model.LocationMapImageUrl = mapImageUrlRes?.Url;
        model.LocationMapImageAltText = picture?.AltAttribute ?? string.Empty;
        model.LocationMapImageTitle = picture?.TitleAttribute ?? string.Empty;

        return View(model);
    }

    #endregion
}
