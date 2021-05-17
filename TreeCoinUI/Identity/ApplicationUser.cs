using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TreeCoinUI.Entity;

namespace TreeCoinUI.Identity
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public double Money { get; set; }
        public string Tc { get; set; }
        public string Adress { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Supplier> Suppliers { get; set; }
        public DateTime Date { get; set; }

    }
}