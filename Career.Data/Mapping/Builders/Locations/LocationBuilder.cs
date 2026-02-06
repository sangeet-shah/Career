using Career.Data.Domains.Locations;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.Locations;

public class LocationBuilder : EntityBuilder<Location>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.LocationId))).AsInt32().NotNullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.Name))).AsString(400).NotNullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.AliasName))).AsString(400).Nullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.FMUSAName))).AsString(400).Nullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.Latitude))).AsString(400).NotNullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.Longitude))).AsString(400).NotNullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.StoreManager))).AsString(400).Nullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.AssistantManager))).AsString(400).Nullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.OfficeSupervisor))).AsString(400).Nullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.WarehouseSupervisor))).AsString(400).Nullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.VerifiedByEmail))).AsString(400).Nullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(Location), nameof(Location.UKGGuid))).AsString(400).Nullable();
    }

    #endregion
}