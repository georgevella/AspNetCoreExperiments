using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Glyde.NoSql.AzureTables
{
    public class EntityWrapper<T> : ITableEntity
    {
        private T _entity;


        public EntityWrapper(T entity)
        {
            _entity = entity;
            PartitionKey = EntityMetadata<T>.PartitionKey;
            RowKey = EntityMetadata<T>.GetRowKey(entity);
        }

        public EntityWrapper()
        {

        }

        private const string EntityTypeColumn = "glydesys_Type";
        private const string EntityBodyColumn = "glydesys_Body";

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            var type = Type.GetType(properties[EntityTypeColumn].StringValue);
            var body = properties[EntityBodyColumn].BinaryValue;

            using (var ms = new MemoryStream(body))
            using (var streamWriter = new StreamReader(ms))
            using (var writer = new JsonTextReader(streamWriter))
            {
                _entity = DefaultSerializer.Serializer.Deserialize<T>(writer);
            }
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            using (var ms = new MemoryStream())
            using (var streamWriter = new StreamWriter(ms))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                DefaultSerializer.Serializer.Serialize(writer, _entity);
                writer.Flush();
                streamWriter.Flush();

                var props = EntityMetadata<T>.Columns.ToDictionary(x => x.Column,
                    x => x.GetColumnEntityProperty(_entity));

                var result = props.Union(new Dictionary<string, EntityProperty>()
                {
                    {EntityTypeColumn, EntityProperty.CreateEntityPropertyFromObject(EntityMetadata<T>.FullTypeName)},
                    {EntityBodyColumn, EntityProperty.CreateEntityPropertyFromObject(ms.ToArray())}
                }).ToDictionary(x => x.Key, x => x.Value);

                return result;
            }
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        public T GetEntity()
        {
            return _entity;
        }
    }

    internal static class DefaultSerializer
    {
        public static JsonSerializer Serializer = new JsonSerializer();


    }
}