using System;

namespace Career.Data.Domains.Common;

public class HelloBars : BaseEntity
{
    /// <summary>
    /// Gets or sets the hello bar name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the hello bar content
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the marquee enabled of the hello bar
    /// </summary>
    public bool MarqueeEnabled { get; set; }

    /// <summary>
    /// Gets or sets the marquee direction id of the hello bar
    /// </summary>
    public int MarqueeDirectionId { get; set; }

    /// <summary>
    /// Gets or sets the background color of the hello bar
    /// </summary>
    public string BGColorRgb { get; set; }

    /// <summary>
    /// Gets or sets the start date
    /// </summary>
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the end date
    /// </summary>
    public DateTime? EndDateUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the hello bar is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is subject to ACL
    /// </summary>
    public bool SubjectToAcl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the device selection Id
    /// </summary>
    public int DeviceSelectionId { get; set; }

    /// <summary>
    /// Gets or sets value indicating whether the hello bar link display as a button
    /// </summary>
    public bool DisplaylinkButton { get; set; }

    /// <summary>
    /// Gets or sets value indicating enable outerspace 
    /// </summary>
    public bool EnableOuterSpace { get; set; }

    /// <summary>
    /// Gets or sets value of outerspace left
    /// </summary>
    public int OuterSpaceLeft { get; set; }

    /// <summary>
    /// Gets or sets value of outerspace Right
    /// </summary>
    public int OuterSpaceRight { get; set; }

    /// <summary>
    /// Gets or sets value of outerspace Top
    /// </summary>
    public int OuterSpaceTop { get; set; }

    /// <summary>
    /// Gets or sets value of outerspace Bottom
    /// </summary>
    public int OuterSpaceBottom { get; set; }

    /// <summary>
    /// Gets or sets value indicating enable innerspace 
    /// </summary>
    public bool EnableInnerSpace { get; set; }

    /// <summary>
    /// Gets or sets value of innerspace left
    /// </summary>
    public int InnerSpaceLeft { get; set; }

    /// <summary>
    /// Gets or sets value of innerspace Right
    /// </summary>
    public int InnerSpaceRight { get; set; }

    /// <summary>
    /// Gets or sets value of innerspace Top
    /// </summary>
    public int InnerSpaceTop { get; set; }

    /// <summary>
    /// Gets or sets value of innerspace Bottom
    /// </summary>
    public int InnerSpaceBottom { get; set; }

    /// <summary>
    /// Gets or sets value of display full width
    /// </summary>
    public bool DisplayFullWidth { get; set; }

    /// <summary>
    /// Gets or sets value of height
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets value of custom backgorund color
    /// </summary>
    public bool CustomBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the hello bar mobile content
    /// </summary>
    public string MobileContent { get; set; }

    /// <summary>
    /// Gets or sets the hello bar custom url
    /// </summary>
    public string CustomUrl { get; set; }

    /// <summary>
    /// Gets or sets the hello bar popup disclaimer
    /// </summary>
    public bool PopupDisclaimer { get; set; }

    /// <summary>
    /// Gets or sets the hello bar popup disclaimer title
    /// </summary>
    public string PopupDisclaimerTitle { get; set; }

    /// <summary>
    /// Gets or sets the hello bar popup disclaimer content
    /// </summary>
    public string PopupDisclaimerContent { get; set; }
}