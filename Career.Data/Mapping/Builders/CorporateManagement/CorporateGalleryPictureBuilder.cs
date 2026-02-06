using Career.Data.Domains.CorporateManagement;
using Career.Data.Extensions;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Career.Data.Mapping.Builders.CorporateManagement;

public class CorporateGalleryPictureBuilder : EntityBuilder<CorporateGalleryPicture>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CorporateGalleryPicture), nameof(CorporateGalleryPicture.CorporateGalleryId)))
            .AsInt32().ForeignKey<CorporateGallery>();
    }

    #endregion
}
