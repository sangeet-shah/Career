using System;

namespace Middleware.Web.Domains.Banner;

public class Banner : BaseEntity
{
    public int BannerDisplayTargetId { get; set; }

    public string Title { get; set; }

    public string Title2 { get; set; }

    public int PictureId { get; set; }

    public int MobilePictureId { get; set; }

    public string Description { get; set; }

    public string Url { get; set; }

    public string UrlTitle { get; set; }

    public DateTime? StartDateUtc { get; set; }

    public DateTime? EndDateUtc { get; set; }

    public bool SubjectToAcl { get; set; }

    public int DisplayOrder { get; set; }

    public bool Published { get; set; }

    public bool PopupEnabled { get; set; }

    public string PopupTitle { get; set; }

    public string PopupContent { get; set; }

    public string LeftBtnText { get; set; }

    public string LeftBtnBgColorCode { get; set; }

    public string LeftBtnTextColorCode { get; set; }

    public string RightBtnText { get; set; }

    public string RightBtnBgColorCode { get; set; }

    public string RightBtnTextColorCode { get; set; }

    public decimal WidthPercentage { get; set; }
}