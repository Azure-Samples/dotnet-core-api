using Azure;
using Azure.Data.Tables;
using System;

namespace RundooApi.Models
{
    public class CustomerEntry : ITableEntry
    {
        public static string TableEntityKeyFormat
        {
            get
            {
                return "Supplier_{0}_Customer";
            }
        }

        private string supplierId;
        private string customerId;

        public CustomerEntry(
            string supplierId,
            string customerId)
        {
            this.supplierId = supplierId;
            this.customerId = customerId;
        }

        public TableEntity ConvertToTableEntity()
        {
            var entity = new TableEntity();
            entity.PartitionKey = string.Format(TableEntityKeyFormat, supplierId);
            entity.RowKey = customerId;

            return entity;
        }
    }
}