using Azure;
using Azure.Data.Tables;
using RundooApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi.Services
{
    public class TablesService
    {
        public TablesService(TableClient tableClient)
        {
            _tableClient = tableClient;
        }

        private TableClient _tableClient;

        public string[] EXCLUDE_TABLE_ENTITY_KEYS = { "PartitionKey", "RowKey", "odata.etag", "Timestamp" };

        public IEnumerable<TableEntity> GetAllRows()
        {
            Pageable<TableEntity> entities = _tableClient.Query<TableEntity>();

            return entities;
        }

        public Task<Response<TableEntity>> GetTableEntity(string partitionKey, string rowKey)
        {
            return _tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey);
        }

        public IEnumerable<ProductInventory> QueryInventoryByLocation(string supplierId, string locationId)
        {
            return _tableClient.Query<TableEntity>(q => q.PartitionKey == string.Format(ProductEntry.TableEntityKeyFormat, supplierId, locationId))
                .Select(e => new ProductInventory { ProductId = e.RowKey, Quantity = (int)e["Quantity"] }).ToArray();
        }

        public IEnumerable<ProductInventory> QueryProductCountAtLocation(string supplierId, string locationId, string productId)
        {
            return _tableClient.Query<TableEntity>(q => q.PartitionKey == string.Format(ProductEntry.TableEntityKeyFormat, supplierId, locationId) && q.RowKey == productId)
                .Select(e => new ProductInventory { ProductId = e.RowKey, Quantity = (int)e["Quantity"] }).ToArray();
        }

        public IEnumerable<string> QuerySupplierCustomerInfo(string supplierId)
        {
            return _tableClient.Query<TableEntity>(q => q.PartitionKey == string.Format(CustomerEntry.TableEntityKeyFormat,supplierId)).Select(e => e.RowKey).ToArray();
        }

        public IEnumerable<string> QuerySupplierLocationInfo(string supplierId)
        {
            return _tableClient.Query<TableEntity>(q => q.PartitionKey == string.Format(LocationEntry.TableEntityKeyFormat, supplierId)).Select(e => e.RowKey).ToArray();
        }

        public Task<Response> InsertTableEntity(TableEntity entity)
        {
            return _tableClient.AddEntityAsync(entity);
        }


        public Task<Response> UpsertTableEntity(TableEntity entity)
        {
            return _tableClient.UpsertEntityAsync(entity);
        }

        public Task<Response> RemoveEntity(string partitionKey, string rowKey)
        {
            return _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}
