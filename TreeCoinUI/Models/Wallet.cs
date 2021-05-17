using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TreeCoinUI.Entity;
using TreeCoinUI.Identity;

namespace TreeCoinUI.Models
{
    public class Wallet
    {
        public double Money { get; set; }
        public FinanceHistory FinanceHistory { get; set; }

    }
}