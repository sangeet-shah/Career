using System;

namespace Career.Web.Models.Hellobar;

public record HelloBarModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public string BGColorRgb { get; set; }
    public int Height { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string CustomURL { get; set; }
    public bool IsDefault { get; set; }
    public bool PopupDisclaimer { get; set; }
    public string DisclaimerTitle { get; set; }
    public string Disclaimer { get; set; }
    public string MobileContent { get; set; }
}