using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Middleware.Web.Domains.PhysicalStores;

public class GoogleAddress
{
    public GoogleAddress()
    {
        Results = new List<Result>();
    }
    public IList<Result> Results { get; set; }
    public string Status { get; set; }
    public string error_message { get; set; }
}
public class AddressComponent
{
    public AddressComponent()
    {
        types = new List<string>();
    }
    public string long_name { get; set; }
    public string short_name { get; set; }
    public IList<string> types { get; set; }
}

public class Northeast
{
    public double lat { get; set; }
    public double lng { get; set; }
}

public class Southwest
{
    public double lat { get; set; }
    public double lng { get; set; }
}

public class Bounds
{
    public Bounds()
    {
        this.northeast = new Northeast();
        this.southwest = new Southwest();
    }
    public Northeast northeast { get; set; }
    public Southwest southwest { get; set; }
}

//public class Location
//{
//    public double Lat { get; set; }
//    public double Lng { get; set; }
//}

public class Geometry
{
    public Geometry()
    {
        this.bounds = new Bounds();
        this.viewport = new Bounds();
    }
    public Bounds bounds { get; set; }
    public string location_type { get; set; }
    public Bounds viewport { get; set; }
}

public class Result
{
    public Result()
    {
        address_components = new List<AddressComponent>();
        types = new List<string>();
        StoreInfoModel = new StoreInfoModels();
        geometry = new Geometry();
        opening_hours = new OpeningHours();
        photos = new List<Photo>();
        reviews = new List<Review>();
        StoreInfoModel = new StoreInfoModels();
        JobModelList = new List<JobModels>();
    }
    public IList<AddressComponent> address_components { get; set; }
    public string formatted_address { get; set; }
    public Geometry geometry { get; set; }
    public string place_id { get; set; }
    public IList<string> types { get; set; }

    // storeinfo with rating propeties
    public string StoreAddresFromDb { get; set; }
    public string StoreHoursFromDb { get; set; }
    public string PickupHoursFromDb { get; set; }
    public string GoogleUrlFromDb { get; set; }
    public string FacebookUrlFromDb { get; set; }
    public string TwitterUrlFromDb { get; set; }
    public string PinterestUrlFromDb { get; set; }
    public string LinkedInUrlFromDb { get; set; }
    public string icon { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public OpeningHours opening_hours { get; set; }
    public string reference { get; set; }
    public string scope { get; set; }
    public string vicinity { get; set; }
    public List<Photo> photos { get; set; }
    public double? rating { get; set; }
    public string adr_address { get; set; }
    public string formatted_phone_number { get; set; }
    public string international_phone_number { get; set; }
    public List<Review> reviews { get; set; }
    public string url { get; set; }
    public int utc_offset { get; set; }
    public string website { get; set; }
    public string StorePictureURL { get; set; }
    public string StoreBannerPictureURL { get; set; }
    public StoreInfoModels StoreInfoModel { get; set; }
    public List<JobModels> JobModelList { get; set; }
    public string StoreName { get; set; }
    public string TourVideoLink { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public int StoreId { get; set; }
    public int LocationId { get; set; }
    public bool DisplayReview { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaKeywords { get; set; }
    public string Zipcode { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string StoreDetail { get; set; }
    public double? user_ratings_total { get; set; }
}

public class Review
{
    public string author_name { get; set; }
    public string author_url { get; set; }
    public string language { get; set; }
    public string profile_photo_url { get; set; }
    public int rating { get; set; }
    public string relative_time_description { get; set; }
    public string text { get; set; }
    public int time { get; set; }
}

public class OpeningHours
{
    public OpeningHours()
    {
        this.weekday_text = new List<object>();
    }
    public bool open_now { get; set; }
    public List<GooglePlaceApiPeriod> periods { get; set; }
    public List<object> weekday_text { get; set; }
}

public class GooglePlaceApiPeriod
{
    public GooglePlaceApiOpenClose close { get; set; }
    public GooglePlaceApiOpenClose open { get; set; }
}

public class GooglePlaceApiOpenClose
{
    public int day { get; set; }
    public string time { get; set; }
}

public class Photo
{
    public Photo()
    {
        this.html_attributions = new List<string>();
    }
    public int height { get; set; }
    public List<string> html_attributions { get; set; }
    public string photo_reference { get; set; }
    public int width { get; set; }
}
public class RootObject
{
    public RootObject()
    {
        this.html_attributions = new List<object>();
        this.result = new Result();
    }
    public List<object> html_attributions { get; set; }
    public Result result { get; set; }
    public string status { get; set; }
    public string error_message { get; set; }
}

public class StoreInfoModels
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int PictureId { get; set; }
    public string RedirectionURL { get; set; }
    public int StoreId { get; set; }
    public string PictureURL { get; set; }
}

public class JobModels
{
    public JobModels()
    {
        Stores = new List<SelectListItem>();
        Departments = new List<SelectListItem>();
    }
    public int JobId { get; set; }
    public string JobTitle { get; set; }
    public string JobSummaryTitle1 { get; set; }
    public string JobSummaryContent1 { get; set; }
    public string JobSummaryTitle2 { get; set; }
    public string JobSummaryContent2 { get; set; }
    public string JobSummaryTitle3 { get; set; }
    public string JobSummaryContent3 { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool Published { get; set; }
    public IList<SelectListItem> Stores { get; set; }
    public string StoreName { get; set; }
    public int DepartmentId { get; set; }
    public IList<SelectListItem> Departments { get; set; }
    public string DepartmentName { get; set; }
    public bool MarkAsNew { get; set; }
    public string Location { get; set; }
    public bool IsFeatured { get; set; }
    public int PictureId { get; set; }
    public string PictureUrl { get; set; }
}
