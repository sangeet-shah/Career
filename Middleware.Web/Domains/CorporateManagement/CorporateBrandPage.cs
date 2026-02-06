namespace Career.Data.Domains.CorporateManagement;
public class CorporateBrandPage : BaseEntity
{
    public int PictureId { get; set; }

    public string Description { get; set; }

    public string Url { get; set; }

    public int DisplayOrder { get; set; }
}