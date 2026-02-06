using System;

namespace Career.Data.Domains.Media;

public class Banner : BaseEntity
{
    #region properties

    /// <summary>
    /// Gets or sets the enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the display section
    /// </summary>
    public int DisplaySection { get; set; }

    /// <summary>
    /// Gets or sets the Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the FontSize
    /// </summary>
    public int FontSize { get; set; }

    /// <summary>
    /// Gets or sets the picture identifier
    /// </summary>
    public int PictureId { get; set; }

    /// <summary>
    /// Gets or sets the description 
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the alternate text 
    /// </summary>
    public string AlternateText { get; set; }

    /// <summary>
    /// Gets or sets the url title 
    /// </summary>
    public string UrlTitle { get; set; }

    /// <summary>
    /// Gets or sets the url
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the category id(s)
    /// </summary>
    public string CategoryIds { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer id(s)
    /// </summary>
    public string ManufacturerIds { get; set; }

    /// <summary>
    /// Gets or sets the promotional tag id(s)
    /// </summary>
    public string PromotionalTagIds { get; set; }

    /// <summary>
    /// Gets or sets the position
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is subject to ACL
    /// </summary>
    public bool SubjectToAcl { get; set; }

    /// <summary>
    /// Gets or sets MobilePictureId
    /// </summary>
    public int MobilePictureId { get; set; }

    /// <summary>
    /// Gets or sets klaviyo list id
    /// </summary>
    public string KlaviyoListId { get; set; }

    /// <summary>
    /// Gets or sets klaviyo list field name
    /// </summary>
    public string KlaviyoListFieldName { get; set; }

    /// <summary>
    /// Gets or sets klaviyo list field value
    /// </summary>
    public string KlaviyoListFieldValue { get; set; }

    /// <summary>
    /// Gets or sets feature check box
    /// </summary>
    public bool FeaturedCheckbox { get; set; }

    /// <summary>
    /// Gets or sets checkbox text
    /// </summary>
    public string CheckboxText { get; set; }

    /// <summary>
    /// Gets or sets clicked border box color
    /// </summary>
    public string ClickedBorderBoxColor { get; set; }

    /// <summary>
    /// Gets or sets webpage identification comma seperated
    /// </summary>
    public string WebPageIds { get; set; }

    /// <summary>
    /// Gets or sets the Title2
    /// </summary>
    public string Title2 { get; set; }

    /// <summary>
    /// Gets or sets WidthPercentage
    /// </summary>
    public decimal WidthPercentage { get; set; }

    #endregion        
}
