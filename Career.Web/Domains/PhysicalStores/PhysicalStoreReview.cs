namespace Career.Web.Domains.PhysicalStores;

/// <summary>
/// Google review DTO (replaced Career.Data.Domains.PhysicalStores for LocationDetailModel).
/// </summary>
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
