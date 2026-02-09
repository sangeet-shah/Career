namespace Middleware.Web.Models.Blogs;

public record BlogPostModel
{
    public int Id { get; set; }
    
    public int LanguageId { get; set; }
    
    public string Title { get; set; }
    
    public string Body { get; set; }
    
    public string BodyOverview { get; set; }
    
    public bool AllowComments { get; set; }
    
    public string Tags { get; set; }
    
    public DateTime? StartDateUtc { get; set; }

    public DateTime? EndDateUtc { get; set; }

    public string MetaKeywords { get; set; }

    public string MetaDescription { get; set; }

    public string MetaTitle { get; set; }

    public  bool LimitedToStores { get; set; }

    public DateTime? UpdatedDateUtc { get; set; }

    public string BlogAuthorPosition { get; set; }

    public int? BlogCoverPictureId { get; set; }

    public int? FeatureArticlePictureId { get; set; }

    public int? BlogBoxPictureId { get; set; }

    public string BlogBoxPicturePictureURL { get; set; }

    public string SEOPictureUrl1 { get; set; }

    public string SEOPictureUrl2 { get; set; }

    public string SEOPictureUrl3 { get; set; }

    public int? BlogShopByCategoryPictureId { get; set; }

    public int? BlogRecommendedPictureId { get; set; }

    public int? BlogSidebarPictureId { get; set; }

    public int MostViewCount { get; set; }

    public int? FMUSAPictureId { get; set; }

    public string FMUSAPictureURL { get; set; }       

    public string SeName { get; set; }

    public bool FMUSABio { get; set; }
}
