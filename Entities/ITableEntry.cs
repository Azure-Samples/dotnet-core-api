using Azure;
using Azure.Data.Tables;
using System;

namespace RundooApi.Models
{
    public interface ITableEntry
    {
        public TableEntity ConvertToTableEntity();
    }
}