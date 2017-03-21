using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.OData.Query;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Glyde.NoSql.AzureTables
{
    public class AzureTablesDataStore
    {
        private readonly CloudTableClient _tableClient;
        private readonly ConcurrentDictionary<string, CloudTable> _tableMap = new ConcurrentDictionary<string, CloudTable>();

        public AzureTablesDataStore()
        {
            _tableClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient();

        }

        public async Task Add<T>(T entity)
        {            
            var table = GetTable<T>();

            var tableEntity = new EntityWrapper<T>(entity);

            var op = TableOperation.Insert(tableEntity);
            await table.ExecuteAsync(op);
        }

        private CloudTable GetTable<T>()
        {
            var tableName = EntityMetadata<T>.Table;
            return _tableMap.GetOrAdd(tableName, s =>
            {
                var t = _tableClient.GetTableReference(s);
                var result = t.CreateIfNotExistsAsync().Result;
                return t;
            });
        }

        public async Task<IEnumerable<T>> GetAll<T>()
        {

            var query = new TableQuery<EntityWrapper<T>>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, EntityMetadata<T>.PartitionKey));

            var table = GetTable<T>();

            var result = await table.ExecuteQuerySegmentedAsync(query,
                new TableContinuationToken());

            return result.Select(x => x.GetEntity()).ToList();
        }
    }

    public class UnsupportedPropertyTypeException : Exception
    {
    }

    public class AmbiguousKeyException : Exception
    {
    }
}