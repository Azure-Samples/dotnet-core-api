using Azure;
using Azure.Data.Tables;
using System;

namespace RundooApi.Models
{
    public class LocationEntry : ITableEntry
    {
        public static string TableEntityKeyFormat
        {
            get
            {
                return "Supplier_{0}_Location";
            }
        }

        private string supplierId;
        private string locationId;

        public LocationEntry(
            string supplierId,
            string locationId)
        {
            this.supplierId = supplierId;
            this.locationId = locationId;
        }

        public TableEntity ConvertToTableEntity()
        {
            var entity = new TableEntity();
            entity.PartitionKey = string.Format(TableEntityKeyFormat, supplierId);
            entity.RowKey = locationId;

            return entity;
        }
    }
}