using Middleware.Web.Models.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Middleware.Web.Models.Career;

public record CareerSearchModel : BaseSearchModel
{
    public CareerSearchModel()
    {
        SelectedStates = new List<string>();
        SelectedCities = new List<string>();
        SelectedJobCategories = new List<string>();
        AvailableStates = new List<SelectListItem>();
        AvailableCities = new List<SelectListItem>();
        AvailableJobCategories = new List<SelectListItem>();
        CarrerList = new List<CareerDetailModel>();
        Parameters = new Dictionary<string, string>();

        // set default page size
        PageSize = 10;
    }

    public IList<CareerDetailModel> CarrerList { get; set; }
    
    public string? State { get; set; }
    public IList<string> SelectedStates { get; set; }
    public IList<SelectListItem> AvailableStates { get; set; }

    public string? City { get; set; }
    public IList<string> SelectedCities { get; set; }
    public List<SelectListItem> AvailableCities { get; set; }

    public string? JobCategory { get; set; }
    public IList<string> SelectedJobCategories { get; set; }
    public List<SelectListItem> AvailableJobCategories { get; set; }

    public string? Store { get; set; }

    public bool IsMobile { get; set; }

    public int TopImage1 { get; set; }

    public string? TopImage1Url { get; set; }

    public string? TopImage1AltText { get; set; }

    public string? TopImage1Title { get; set; }

    public int TopImage2 { get; set; }

    public string? TopImage2Url { get; set; }

    public string? TopImage2AltText { get; set; }

    public string? TopImage2Title { get; set; }

    public int TopImage3 { get; set; }

    public string? TopImage3Url { get; set; }

    public string? TopImage3AltText { get; set; }

    public string? TopImage3Title { get; set; }

    public string? TopDescription { get; set; }

    public int Image { get; set; }

    public string? ImageUrl { get; set; }

    public string? AltText { get; set; }

    public int MobileImage { get; set; }

    public string? MobileImageUrl { get; set; }

    public string? MobileAltText { get; set; }        

    public new int Page { get; set; }

    public Dictionary<string, string> Parameters { get; set; }

    public string? CurrentPageUrl { get; set; }
}
