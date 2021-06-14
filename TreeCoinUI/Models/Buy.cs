using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using TreeCoinUI.Entity;

namespace TreeCoinUI.Models
{
    public class Buy
    {
        [DisplayName("Alınacak Miktar")]
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}