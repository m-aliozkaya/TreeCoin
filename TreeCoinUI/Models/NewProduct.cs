using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Models
{
    public class NewProduct
    {
        public int ProductId { get; set; }
        public int QuantityId { get; set; }
        public int SupplierId { get; set; }
        public double Price { get; set; }
        public int QuantityValue { get; set; }
        public string Name { get; set; }
        public bool IsApproved { get; set; }
        public string Image { get; set; }
    }
}