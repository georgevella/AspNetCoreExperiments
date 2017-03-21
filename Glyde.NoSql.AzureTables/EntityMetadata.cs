using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Glyde.NoSql.AzureTables
{
    internal static class EntityMetadata<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static string Table { get; }

        static EntityMetadata()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            var tableAttribute = typeInfo.GetCustomAttribute<TableAttribute>();
            Table = tableAttribute.Name;

            var keyFields = typeInfo.DeclaredProperties
                .Where(
                    p =>
                        p.Name.Equals("id", StringComparison.OrdinalIgnoreCase) ||
                        p.GetCustomAttribute<KeyAttribute>() != null)
                .ToList();

            if (keyFields.Count != 1)
            {
                throw new AmbiguousKeyException();
            }

            KeyProperty = keyFields.First();

            PartitionKey = typeInfo.Name;

            var props = typeInfo.DeclaredProperties.Where(p => !Equals(p, KeyProperty) && ColumnMetadata.SupportedTypes.Contains(p.PropertyType)).ToList();

            Columns = props.Select(x => new ColumnMetadata(x)).ToList();

            FullTypeName = typeInfo.AssemblyQualifiedName;
        }

        // ReSharper disable once StaticMemberInGenericType
        public static List<ColumnMetadata> Columns { get; set; }

        // ReSharper disable once StaticMemberInGenericType
        public static string PartitionKey { get; set; }

        // ReSharper disable once StaticMemberInGenericType
        public static PropertyInfo KeyProperty { get; set; }
        public static string FullTypeName { get; private set; }

        public static string GetRowKey(T entity)
        {
            return KeyProperty.GetValue(entity).ToString();
        }
    }
}