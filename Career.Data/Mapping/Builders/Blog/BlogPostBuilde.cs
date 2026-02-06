using Career.Data.Domains.Blogs;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.Blog;

public class BlogPostBuilde : EntityBuilder<FMBlogPost>
{
	public override void MapEntity(CreateTableExpressionBuilder table)
	{
		table
		 .WithColumn(NameCompatibilityManager.GetColumnName(typeof(FMBlogPost), nameof(FMBlogPost.BlogPostId))).AsInt32()
		 .PrimaryKey().ForeignKey<BlogPost>();
	}
}
