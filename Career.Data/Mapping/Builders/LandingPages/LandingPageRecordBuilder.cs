using Career.Data.Domains.Directory;
using Career.Data.Domains.LandingPages;
using Career.Data.Domains.Locations;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System.Data;

namespace Career.Data.Mapping.Builders.LandingPages;
public class LandingPageRecordBuilder : EntityBuilder<LandingPageRecord>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(LandingPageRecord), nameof(LandingPageRecord.LandingPageId)))
            .AsInt32().Nullable().ForeignKey<LandingPage>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(LandingPageRecord), nameof(LandingPageRecord.LocationId)))
            .AsInt32().Nullable().ForeignKey<Location>(primaryColumnName: NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.LocationId))).OnDelete(Rule.SetNull)
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(LandingPageRecord), nameof(LandingPageRecord.StateProvinceId))).AsInt32()
            .Nullable().ForeignKey<StateProvince>().OnDelete(Rule.SetNull);
    }

    #endregion
}