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
        public bool IsApproved { get; set; }
        public string Image { get; set; }
        public Quantity Quantity { get; set; }
    }
}