using Career.Data.Domains.Banner;
using Career.Data.Domains.Locations;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.Banners;
public class BannerLocationMappingBuilder : EntityBuilder<BannerLocationMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(BannerLocationMapping), nameof(BannerLocationMapping.BannerId))).AsInt32()
            .ForeignKey<Banner>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(BannerLocationMapping), nameof(BannerLocationMapping.LocationId))).AsInt32()
            .ForeignKey<Location>(primaryColumnName: NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.LocationId)));
    }

    #endregion
}