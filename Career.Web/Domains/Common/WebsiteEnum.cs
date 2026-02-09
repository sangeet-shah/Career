using System.ComponentModel.DataAnnotations;

namespace Career.Web.Domains.Common;

public enum WebsiteEnum
{
    [Display(Name = "Unclaimed Freight Furniture")]
    UCF = 1,

    [Display(Name = "The Furniture Mart")]
    FM = 2,

    [Display(Name = "Ashley")]
    ASH = 3,

    [Display(Name = "Carpet One")]
    CO = 4,

    [Display(Name = "Furniture Mart USA")]
    FMUSA = 5,

    [Display(Name = "Others")]
    Others = 6,

    [Display(Name = "Furniture Superstore")]
    FSS = 7,

    [Display(Name = "Becker Furniture")]
    BFM = 8,

    [Display(Name = "Bille Arthur")]
    BADS = 9
}
