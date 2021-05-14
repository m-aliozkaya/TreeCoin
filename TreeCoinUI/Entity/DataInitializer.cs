using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Entity
{
    public class DataInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            Quantity quantity = new Quantity()
            {
                Type = "litre"
            };

            Product product = new Product()
            {
                Name = "Elma",
                QuantityId = 1
            };

            context.Quantities.Add(quantity);

            context.SaveChanges();

            context.Products.Add(product);

            context.SaveChanges();

            base.Seed(context); 
        }
    }
}