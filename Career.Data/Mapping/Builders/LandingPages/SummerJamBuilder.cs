using Career.Data.Domains.Directory;
using Career.Data.Domains.LandingPages;
using Career.Data.Domains.Stores;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System.Data;

namespace Career.Data.Mapping.Builders.LandingPages;
public class SummerJamBuilder : EntityBuilder<SummerJam>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(SummerJam), nameof(SummerJam.StoreId))).AsInt32().Nullable()
            .ForeignKey<Store>().OnDelete(Rule.SetNull)
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(SummerJam), nameof(SummerJam.StateProvinceId))).AsInt32().Nullable()
            .ForeignKey<StateProvince>().OnDelete(Rule.SetNull);
    }

    #endregion
}