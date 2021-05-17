using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TreeCoinUI.Identity;

namespace TreeCoinUI.Entity
{
    public class Supplier
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}