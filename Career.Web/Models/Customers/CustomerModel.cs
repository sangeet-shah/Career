using Career.Web.Models.Blogs;
using System.Collections.Generic;

namespace Career.Web.Models.Customers;

public record CustomerModel
{
    public CustomerModel()
    {
        BlogPostList = new List<BlogPostModel>();
    }

    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int AvatarId { get; set; }

    public string AvatarUrl { get; set; }

    public string AvatarAuthorPosition { get; set; }

    public string Biography { get; set; }

    public int DisplayOrder { get; set; }

    public bool FMUSABio { get; set; }

    public int? BioImageId { get; set; }

    public string BioImageUrl { get; set; }

    public IList<BlogPostModel> BlogPostList { get; set; }

    public string SocialMediaURLs { get; set; }
}