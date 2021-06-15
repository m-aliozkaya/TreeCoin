using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TreeCoinUI.Entity;

namespace TreeCoinUI.Models
{
    public class Admin
    {
        public List<AdminProductConfirm> Products { get; set; }
        public List<FinanceHistory> FinanceHistories { get; set; }
    }
}