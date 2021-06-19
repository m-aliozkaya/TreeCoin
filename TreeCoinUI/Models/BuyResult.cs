using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TreeCoinUI.Identity;

namespace TreeCoinUI.Models
{
    public class BuyResult
    {
        public int CustomerId { get; set; }
        public int PurchasedQuantity { get; set; }
        public double Amount { get; set; }
    }
}