using Career.Data.Domains;
using Career.Data.Extensions;
using Career.Data.Infrastructure;
using Career.Data.Migrations;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Expressions;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Career.Data.Mapping;

public static class CareerMappingSchema
{
    #region Fields

    private static ConcurrentDictionary<Type, CareerEntityDescriptor> EntityDescriptors { get; } = new();

    #endregion

    /// <summary>
    /// Returns mapped entity descriptor
    /// </summary>
    /// <param name="entityType">Type of entity</param>
    /// <returns>Mapped entity descriptor</returns>
    public static CareerEntityDescriptor GetEntityDescriptor(Type entityType)
    {
        if (!typeof(BaseEntity).IsAssignableFrom(entityType))
            return null;

        var customAttribute = entityType.GetCustomAttributes(true).FirstOrDefault() as TableAttribute;

        return EntityDescriptors.GetOrAdd(entityType, t =>
        {
            var tableName = NameCompatibilityManager.GetTableName(t);
            var expression = new CreateTableExpression { TableName = tableName, SchemaName = customAttribute != null ? customAttribute.Schema : null };
            var builder = new CreateTableExpressionBuilder(expression, new NullMigrationContext());
            builder.RetrieveTableExpressions(t);

            return new CareerEntityDescriptor
            {
                EntityName = tableName,
                SchemaName = builder.Expression.SchemaName,
                Fields = builder.Expression.Columns.Select(column => new CareerEntityFieldDescriptor
                {
                    Name = column.Name,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsNullable = column.IsNullable,
                    Size = column.Size,
                    Precision = column.Precision,
                    IsIdentity = column.IsIdentity,
                    Type = column.Type ?? System.Data.DbType.String
                }).ToList()
            };
        });
    }

    /// <summary>
    /// Get or create mapping schema with specified configuration name
    /// </summary>
    public static MappingSchema GetMappingSchema(string configurationName, IDataProvider mappings)
    {
        if (Singleton<MappingSchema>.Instance is null)
        {
            Singleton<MappingSchema>.Instance = new MappingSchema(configurationName, mappings.MappingSchema);
            Singleton<MappingSchema>.Instance.AddMetadataReader(new FluentMigratorMetadataReader());
        }

        return Singleton<MappingSchema>.Instance;
    }
}
