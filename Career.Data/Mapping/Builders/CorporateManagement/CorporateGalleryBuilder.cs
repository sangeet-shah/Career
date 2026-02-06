using Career.Data.Domains.CorporateManagement;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.CorporateManagement;

public class CorporateGalleryBuilder : EntityBuilder<CorporateGallery>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CorporateGallery), nameof(CorporateGallery.Title))).AsString(400).NotNullable();
    }

    #endregion
}
