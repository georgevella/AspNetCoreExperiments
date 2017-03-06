using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Glyde.Bookmarks.LinkManagement.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Glyde.Bookmarks.LinkManagement.Services
{
    public interface IBookmarkStorage
    {
        Task<IEnumerable<BookmarkDao>> GetBookmarks();

        Task AddBookmark(BookmarkDao bookmarkDao);
    }

    class BookmarkStorage : IBookmarkStorage
    {
        private CloudTable _table;

        public BookmarkStorage()
        {
            var tableClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("bookmarks");
            if (table.CreateIfNotExistsAsync().Result)
            {
                
            }

            _table = table;
        }

        public async Task<IEnumerable<BookmarkDao>> GetBookmarks()
        {
            var query = new TableQuery<EntityWrapper>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.Equal, StorageHelpers.GeneratePartitionKey<BookmarkDao>()));
            var result = await _table.ExecuteQuerySegmentedAsync(query,
                new TableContinuationToken());

            return result.Select(x => (BookmarkDao) x.EntityDao).ToList();
        }

        public async Task AddBookmark(BookmarkDao bookmarkDao)
        {
            var tableEntity = new EntityWrapper(bookmarkDao);

            var op = TableOperation.Insert(tableEntity);
            await _table.ExecuteAsync(op);
        }
    }

    public class EntityWrapper : ITableEntity
    {
        public object EntityDao { get; private set; }
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public EntityWrapper(object entityDao)
        {
            EntityDao = entityDao;

            PartitionKey = StorageHelpers.GeneratePartitionKey(entityDao);
            RowKey = StorageHelpers.BuildRowKey(entityDao);
        }

        public EntityWrapper()
        {
            
        }
        
        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            var type = Type.GetType(properties["Type"].StringValue);
            var body = properties["Body"].BinaryValue;

            using (var ms = new MemoryStream(body))
            using (var streamWriter = new StreamReader(ms))
            using (var writer = new JsonTextReader(streamWriter))
            {
                EntityDao = Serializer.Deserialize(writer, type);
            }
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            using (var ms = new MemoryStream())
            using (var streamWriter = new StreamWriter(ms))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                Serializer.Serialize(writer, EntityDao);
                writer.Flush();
                streamWriter.Flush();

                var result = new Dictionary<string, EntityProperty>()
                {
                    {"Type", EntityProperty.CreateEntityPropertyFromObject(EntityDao.GetType().AssemblyQualifiedName)},
                    {"Body", EntityProperty.CreateEntityPropertyFromObject(ms.ToArray())}
                };

                return result;
            }
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }
    }

    public class EntityWriter<T>
    {
        static EntityWriter()
        {
            
        }
    }

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
                    Attribute = p.GetCustomAttribute<KeyAttribute>()
                })
                .First(x => x.Attribute != null);

            return keyPropertyInfo.Property.GetValue(dao).ToString();
        }
    }
}