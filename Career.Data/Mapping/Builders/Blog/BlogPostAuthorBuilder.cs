using Career.Data.Domains.Blogs;
using Career.Data.Domains.Customers;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.Blog;
public class BlogPostAuthorBuilder : EntityBuilder<FMBlogPostAuthor>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
             .WithColumn(NameCompatibilityManager.GetColumnName(typeof(FMBlogPostAuthor), nameof(FMBlogPostAuthor.AuthorId))).AsInt32()
             .PrimaryKey().ForeignKey<Customer>()
             .WithColumn(NameCompatibilityManager.GetColumnName(typeof(FMBlogPostAuthor), nameof(FMBlogPostAuthor.BlogPostId))).AsInt32()
             .PrimaryKey().ForeignKey<BlogPost>();
    }

    #endregion
}
