using System;

namespace RundooApi.Models
{
    public class ProductInfo
    {
        public string SupplierId { get; set; }
        public string LocationId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}