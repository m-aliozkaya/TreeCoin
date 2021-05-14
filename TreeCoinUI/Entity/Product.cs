using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public int QuantityId { get; set; }
        public string Name { get; set; }
        public Quantity Quantity { get; set; }
    }
}