using Azure;
using Azure.Data.Tables;
using RundooApi.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace RundooApi.Services
{
    public class DocDBService
    {
        private Container container;

        public DocDBService(CosmosClient cosmosClient, string databaseId, string containerId)
        {
            this.container = cosmosClient.GetContainer(databaseId, containerId);
        }        

        /// <summary>
        /// Add Transaction items to the container
        /// </summary>
        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            ItemResponse<Transaction> transactionResponse;

            try
            {
                // Read the item to see if it exists.  
                transactionResponse = await container.ReadItemAsync<Transaction>(transaction.id, new PartitionKey(transaction.SupplierId));                
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                transactionResponse = await container.CreateItemAsync<Transaction>(transaction, new PartitionKey(transaction.SupplierId));
            }

            return transactionResponse.Resource;
        }

        /// <summary>
        /// Parse Query Type and submit relevant query
        /// </summary>
        public Task<IEnumerable<Transaction>> GetTransactionAsync(TransactionQueryType queryType, string id)
        {
            switch (queryType)
            {
                case TransactionQueryType.Transaction:
                    return GetTransactionByIdAsync(id);
                case TransactionQueryType.Customer:
                    return GetTransactionByCustomerIdAsync(id);
                case TransactionQueryType.Supplier:
                    return GetTransactionBySupplierIdAsync(id);
                case TransactionQueryType.Location:
                    return GetTransactionByLocationIdAsync(id);
                case TransactionQueryType.Product:
                    return GetTransactionByProductIdAsync(id);                
                default:
                    throw new InvalidQueryTypeException($"Invalid query type of type {queryType} passed in when querying transactions");
            }
        }

        /// <summary>
        /// Get Transaction items by TransactionId
        /// </summary>
        private Task<IEnumerable<Transaction>> GetTransactionByIdAsync(string transactionId)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{transactionId}'";

            return ProcessQueryDefinition(sqlQueryText);
        }

        /// <summary>
        /// Get Transaction items by LocationId
        /// </summary>
        private Task<IEnumerable<Transaction>> GetTransactionByLocationIdAsync(string locationId)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.LocationId = '{locationId}'";

            return ProcessQueryDefinition(sqlQueryText);
        }

        /// <summary>
        /// Get Transaction items by ProductId
        /// </summary>
        private Task<IEnumerable<Transaction>> GetTransactionByProductIdAsync(string productId)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.ProductId = '{productId}'";

            return ProcessQueryDefinition(sqlQueryText);
        }

        /// <summary>
        /// Get Transaction items by CustomerId
        /// </summary>
        private Task<IEnumerable<Transaction>> GetTransactionByCustomerIdAsync(string customerId)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.CustomerId = '{customerId}'";

            return ProcessQueryDefinition(sqlQueryText);
        }

        /// <summary>
        /// Get Transaction items by SupplierId
        /// </summary>
        private Task<IEnumerable<Transaction>> GetTransactionBySupplierIdAsync(string supplierId)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.SupplierId = '{supplierId}'";

            return ProcessQueryDefinition(sqlQueryText);
        }

        private async Task<IEnumerable<Transaction>> ProcessQueryDefinition(string sqlQueryText)
        {
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);

            List<Transaction> transactions = new List<Transaction>();

            var queryResultSetIterator = container.GetItemQueryIterator<Transaction>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Transaction> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Transaction transaction in currentResultSet)
                {
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
    }
}
