using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace Glyde.NoSql.AzureTables
{
    internal class ColumnMetadata
    {
        internal static List<Type> SupportedTypes { get; } = new List<Type>()
        {
            typeof(string),
            typeof(byte[]),
            typeof(bool),
            typeof(bool?),
            typeof(System.DateTime),
            typeof(System.DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(double),
            typeof(double?),
            typeof(Guid?),
            typeof(Guid),
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
        };

        public PropertyInfo PropertyInfo { get; }

        public ColumnMetadata(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            PropertyInfo = propertyInfo;
            var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            Column = columnAttribute != null ? columnAttribute.Name : propertyInfo.Name.ToLower();
        }

        public string Column { get; set; }

        public EntityProperty GetColumnEntityProperty(object entity)
        {
            var value = PropertyInfo.GetValue(entity);
            return EntityProperty.CreateEntityPropertyFromObject(value);
        }
    }
}