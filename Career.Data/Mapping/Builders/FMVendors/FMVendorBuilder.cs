using Career.Data.Domains.FMVendors;
using Career.Data.Domains.Vendors;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.FMVendors;

public class FMVendorBuilder : EntityBuilder<FMVendor>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(FMVendor), nameof(FMVendor.VendorId))).AsInt32()
            .PrimaryKey().ForeignKey<Vendor>();
    }

    #endregion
}
