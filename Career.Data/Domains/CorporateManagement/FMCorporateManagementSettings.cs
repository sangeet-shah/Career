using Career.Data.Configuration;

namespace Career.Data.Domains.CorporateManagement;

public class FMCorporateManagementSettings : ISettings
{
    #region Home page

    public string ContentDesktopDescription { get; set; }
    public string ContentMobileDescription { get; set; }
    public int NumberOfNewsArticles { get; set; }

    #endregion

    #region Our story page

    public int WebBannerId { get; set; }
    public int MobileBannerId { get; set; }
    public string Title { get; set; }
    public string FullDescription { get; set; }

    #endregion

    #region Locations

    public int MapImageId { get; set; }
    public int LocationFaceBookIconId { get; set; }
    public int LocationGoogleIconId { get; set; }
    public int LocationTwitterIconId { get; set; }
    public int LocationPinterestIconId { get; set; }
    public int LocationLinkedInIconId { get; set; }
    public int LocationInstagramIconId { get; set; }
    public int LocationYouTubeIconId { get; set; }

    #endregion

    #region Social icons

    public int FacebookIconId { get; set; }
    public string FacebookUrl { get; set; }
    public int YouTubeIconId { get; set; }
    public string YouTubeUrl { get; set; }
    public int TwitterIconId { get; set; }
    public string TwitterUrl { get; set; }
    public int LinkedInIconId { get; set; }
    public string LinkedInUrl { get; set; }

    #endregion
}