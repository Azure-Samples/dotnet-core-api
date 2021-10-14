using System;

namespace RundooApi.Models
{
    public class Transaction
    {
        public string id { get; set; }
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }
        public string LocationId { get; set; }
        public string ProductId { get; set; }
        public uint Quantity { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}