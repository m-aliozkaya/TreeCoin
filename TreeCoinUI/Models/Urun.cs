using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Models
{
    public class Urun
    {
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }
        public int QuantityValue { get; set; }
    }
}