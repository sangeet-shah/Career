using Career.Data.Configuration;

namespace Career.Data.Domains.CDN;

/// <summary>
/// Represent the cdn settings
/// </summary>
public class NopAdvanceCDNSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the CDN is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the cdn image url
    /// </summary>
    public string CDNImageUrl { get; set; }    
}