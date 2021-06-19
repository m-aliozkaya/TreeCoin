using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Models
{
    public class FinanceHistoryModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string MoneyCode { get; set; }
        public double Money { get; set; }
    }
}