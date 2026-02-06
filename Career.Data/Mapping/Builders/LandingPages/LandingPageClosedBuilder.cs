using Career.Data.Domains.LandingPages;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.LandingPages;
public class LandingPageClosedBuilder : EntityBuilder<LandingPageClosed>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(LandingPageClosed), nameof(LandingPageClosed.LandingPageId)))
            .AsInt32().PrimaryKey().ForeignKey<LandingPage>();
    }

    #endregion
}