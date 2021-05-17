using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Entity
{
    public class MoneyConfirm
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double Money { get; set; }
        public bool IsApproved { get; set; }
        public Customer Customer { get; set; }
    }
}