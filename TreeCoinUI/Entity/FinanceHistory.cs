using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Entity
{
    public class FinanceHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int FinanceTypeId { get; set; }
        public double Money { get; set; }
        public DateTime Date { get; set; }
        public Customer Customer { get; set; }
        public FinanceType FinanceType { get; set; }
    }
}