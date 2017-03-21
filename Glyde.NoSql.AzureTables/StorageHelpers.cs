using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Glyde.NoSql.AzureTables
{
    public static class StorageHelpers
    {
        public static string GeneratePartitionKey<T>()
        {
            return typeof(T).Name;
        }
        public static string GeneratePartitionKey(object dao)
        {
            return dao.GetType().Name;
        }

        public static string BuildRowKey(object dao)
        {
            var keyPropertyInfo = dao.GetType().GetTypeInfo().DeclaredProperties
                .Select(p => new
                {
                    Property = p,
                    Attribute = p.GetCustomAttribute(typeof(KeyAttribute))
                })
                .First(x => x.Attribute != null);

            return keyPropertyInfo.Property.GetValue(dao).ToString();
        }
    }
}