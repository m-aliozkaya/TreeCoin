using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TreeCoinUI.Identity;

namespace TreeCoinUI.Entity
{
    public class LimitBuy
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public DateTime Date { get; set; }
        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}