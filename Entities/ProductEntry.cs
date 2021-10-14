using Azure;
using Azure.Data.Tables;
using System;

namespace RundooApi.Models
{
    public class ProductEntry : ITableEntry
    {
        public static string TableEntityKeyFormat 
        { 
            get 
            {
                return "Supplier_{0}_Location_{1}"; 
            } 
        }

        private string supplierId;
        private string productId;
        private string locationId;
        private int quantity;

        public ProductEntry(ProductInfo info)
        {
            this.supplierId = info.SupplierId;
            this.productId = info.ProductId;
            this.locationId = info.LocationId;
            this.quantity = info.Quantity;
        }

        public TableEntity ConvertToTableEntity()
        {
            var entity = new TableEntity();
            entity.PartitionKey = string.Format(TableEntityKeyFormat, supplierId, locationId);
            entity.RowKey = productId;
            entity["Quantity"] = quantity;

            return entity;
        }
    }
}