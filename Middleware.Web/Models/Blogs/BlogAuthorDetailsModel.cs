namespace Middleware.Web.Models.Blogs;

public record BlogAuthorDetailsModel
{
    public int AuthorId { get; set; }

    public string AuthorFirstName { get; set; }

    public string AuthorMiddleName { get; set; }

    public string AuthorLastName { get; set; }

    public bool FMUSABio { get; set; }

    public string AuthorPictureUrl { get; set; }

    public string AuthorPosition { get; set; }

    public string AuthorUrl { get; set; }

    public bool AuthorType { get; set; }
}
