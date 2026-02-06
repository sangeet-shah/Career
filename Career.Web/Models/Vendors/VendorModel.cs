namespace Career.Web.Models.Vendors;

public record VendorModel
{
    public bool IsCorporate { get; set; }

    public int CorporatePictureId { get; set; }

    public string CorporatePictureUrl { get; set; }

    public string CorporatePictureAltText { get; set; }

    public string CorporatePictureTitle { get; set; }

    public string CorporateShortDescription { get; set; }
}
