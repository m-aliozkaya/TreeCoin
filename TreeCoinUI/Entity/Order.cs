using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public int QuantityValue { get; set; }
        public double Price { get; set; }
        public Supplier Supplier { get; set; }
        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}