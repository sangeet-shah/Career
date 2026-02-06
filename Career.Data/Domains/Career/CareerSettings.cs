using Career.Data.Configuration;

namespace Career.Data.Domains.Career;

public class CareerSettings : ISettings
{
    #region HR Email

    public string HREmailId { get; set; }

    public int EmailId { get; set; }

    #endregion

    #region Career manager

    public int WebImageId { get; set; }

    public string WebImageAlt { get; set; }

    public string WebImageTitle { get; set; }

    public int MobileImageId { get; set; }

    public string MobileImageAlt { get; set; }

    public string MobileImageTitle { get; set; }

    public string TitleDescription { get; set; }

    public string BannerUrl { get; set; }

    public bool Published { get; set; }

    public int TopImage1Id { get; set; }

    public string TopImage1Alt { get; set; }

    public string TopImage1Title { get; set; }

    public int TopImage2Id { get; set; }

    public string TopImage2Alt { get; set; }

    public string TopImage2Title { get; set; }

    public int TopImage3Id { get; set; }

    public string TopImage3Alt { get; set; }

    public string TopImage3Title { get; set; }

    public string TopImageDescription { get; set; }

    #endregion

    #region Thank you page

    public string ThankYouDescription { get; set; }

    #endregion

    #region Application page CMS

    public string ApplicationPageGreeting { get; set; }

    #endregion
}