using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Models
{
    public class SalesHistory
    {
        public int CustomerId { get; set; }
        public string ProductName { get; set; }
        public DateTime Date { get; set; }
        public int QuantityValue { get; set; }
        public double Price { get; set; }
    }
}