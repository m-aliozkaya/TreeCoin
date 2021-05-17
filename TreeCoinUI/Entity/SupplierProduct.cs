using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Entity
{
    public class SupplierProduct
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }
        public int QuantityValue { get; set; }
        public Supplier Supplier { get; set; }
        public Product Product { get; set; }
    }
}