namespace Career.Web.Models.Seo;

/// <summary>
/// URL record from API (mirrors middleware response).
/// </summary>
public class UrlRecordResponse
{
    public int Id { get; set; }
    public int EntityId { get; set; }
    public string EntityName { get; set; }
    public string Slug { get; set; }
    public bool IsActive { get; set; }
    public int LanguageId { get; set; }
}
