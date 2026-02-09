using System.Collections.Generic;

namespace Middleware.Web.Domains.PhysicalStores;

public class GoogleResponseCache
{
    public GoogleResponseCache()
    {
        PhysicalStoreReviews = new List<PhysicalStoreReview>();
    }

    public int PhysicalStoreId { get; set; }

    public string Vicinity { get; set; }

    public double? Rating { get; set; }

    public double? TotalGoogleRating { get; set; }

    public string StoreDetail { get; set; }

    public string Place_id { get; set; }

    public IList<PhysicalStoreReview> PhysicalStoreReviews { get; set; }

    public string GooglePhysicalStoreName { get; set; }


    public class PhysicalStoreReview
    {
        public string Author_name { get; set; }
        public string Author_url { get; set; }
        public string Language { get; set; }
        public string Profile_photo_url { get; set; }
        public int Rating { get; set; }
        public string Relative_time_description { get; set; }
        public string Text { get; set; }
        public int Time { get; set; }
    }
}
