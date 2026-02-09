namespace Middleware.Web.Models.StoreManagement;

public record LocationModel
{
    public LocationModel()
    {
			LocationList = new List<LocationStateListModel>();
    }

    public string LocationMapImageUrl { get; set; }

    public string LocationMapImageAltText { get; set; }

    public string LocationMapImageTitle { get; set; }

    public IList<LocationStateListModel> LocationList { get; set; }

    public bool IsMobile { get; set; }
}
