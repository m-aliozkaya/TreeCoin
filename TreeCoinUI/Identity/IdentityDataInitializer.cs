using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TreeCoinUI.Entity;

namespace TreeCoinUI.Identity
{
    public class IdentityDataInitializer: CreateDatabaseIfNotExists<IdentityDataContext>
    {
        protected override void Seed(IdentityDataContext context)
        {
            Dictionary<string, string> userIds = new Dictionary<string, string>();

            /* Quantities start */
            List<Quantity> quantities = new List<Quantity>
            {
                new Quantity(){Type = "kg"},
                new Quantity(){Type = "litre"}
            };

            foreach (var item in quantities)
            {
                context.Quantities.Add(item);
            }
            /* Quantities end */

            context.SaveChanges();

            /* Products start */
            List<Product> products = new List<Product>()
            {
                new Product(){Name = "Elma", Image = "elma.jpg", QuantityId = 1, IsApproved = true},
                new Product(){Name = "Petrol", Image = "petrol.jpg", QuantityId = 2, IsApproved = false},
                new Product(){Name = "Patates", Image = "patates.jpg", QuantityId = 2, IsApproved = true},
                new Product(){Name = "Pamuk", Image = "pamuk.jpg", QuantityId = 1, IsApproved = true},
                new Product(){Name = "Buğday", Image = "buğday.jpg", QuantityId = 1, IsApproved = false},
                new Product(){Name = "Arpa", Image = "arpa.jpg", QuantityId = 1, IsApproved = true},
                new Product(){Name = "Kömür", Image = "kömür.jpg", QuantityId = 1, IsApproved = true},
                new Product(){Name = "Odun", Image = "odun.jpg", QuantityId = 1, IsApproved = true},
            };

            foreach (var item in products)
            {
                context.Products.Add(item);
            }
            /* Products end */

            /* FinanceTypes start */
            List<FinanceType> financeTypes = new List<FinanceType>
            {
                new FinanceType(){Name = "load"},
                new FinanceType(){Name = "purchase"},
                new FinanceType(){Name = "failed"},
            };

            foreach (var item in financeTypes)
            {
                context.FinanceTypes.Add(item);
            }
            /* FinanceTypes end */

            if (!context.Roles.Any(i => i.Name == "admin"))
            {
                var store = new RoleStore<ApplicationRole>(context);
                var manager = new RoleManager<ApplicationRole>(store);

                var role = new ApplicationRole() { Name = "admin"};

                manager.Create(role);
            }

            if (!context.Roles.Any(i => i.Name == "customer"))
            {
                var store = new RoleStore<ApplicationRole>(context);
                var manager = new RoleManager<ApplicationRole>(store);

                var role = new ApplicationRole() { Name = "customer" };

                manager.Create(role);
            }

            if (!context.Roles.Any(i => i.Name == "supplier"))
            {
                var store = new RoleStore<ApplicationRole>(context);
                var manager = new RoleManager<ApplicationRole>(store);

                var role = new ApplicationRole() { Name = "supplier" };

                manager.Create(role);
            }

            if (!context.Roles.Any(i => i.Name == "accountant"))
            {
                var store = new RoleStore<ApplicationRole>(context);
                var manager = new RoleManager<ApplicationRole>(store);

                var role = new ApplicationRole() { Name = "accountant" };

                manager.Create(role);
            }

            if (!context.Users.Any(i => i.Name == "accountantUser"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

               var user = new ApplicationUser() { Name = "System", UserName = "accountantUser", Money = 0};

                manager.Create(user, "1234567");
                manager.AddToRole(user.Id, "accountant");
            }

            if (!context.Users.Any(i => i.Name == "krai"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var user = new ApplicationUser() { Name = "Muhammed Ali", SurName = "Özkaya", UserName = "krai", Email = "krai@krai.com", Date = DateTime.Now, Money = 50, Tc = "12345678910", PhoneNumber = "5448271400", Adress = "Sinop" };

                manager.Create(user, "1234567");
                manager.AddToRole(user.Id, "admin");                
                userIds.Add("krai", user.Id);
            }

            if (!context.Users.Any(i => i.Name == "bazig"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var user = new ApplicationUser() { Name = "Furkan", SurName = "Abaylı", UserName = "bazig", Email = "bazig@bazig.com", Date = DateTime.Now, Money = 50, Tc = "12345678910", PhoneNumber = "5448271400", Adress = "Samsun" };

                manager.Create(user, "1234567");
                manager.AddToRole(user.Id, "customer");
                userIds.Add("bazig", user.Id);
            }

            if (!context.Users.Any(i => i.Name == "99lp"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var user = new ApplicationUser() { Name = "Alper", SurName = "Kuru", UserName = "99lp", Email = "99lp@99lp.com", Date = DateTime.Now, Money = 50, Tc = "12345678910", PhoneNumber = "5448271400", Adress = "Ordu" };

                manager.Create(user, "1234567");
                manager.AddToRole(user.Id, "customer");
                userIds.Add("99lp", user.Id);
            }           
            
            if (!context.Users.Any(i => i.Name == "ascarris"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var user = new ApplicationUser() { Name = "Muhammed Emin", SurName = "Fırat", UserName = "ascarris", Email = "ascarris@ascarris.com", Date = DateTime.Now, Money = 350, Tc = "12345678910", PhoneNumber = "5448271400", Adress = "Amasya" };

                manager.Create(user, "1234567");
                manager.AddToRole(user.Id, "supplier");
                userIds.Add("ascarris", user.Id);
            }            
            
            if (!context.Users.Any(i => i.Name == "krebs"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var user = new ApplicationUser() { Name = "Şamil", SurName = "Çelebi", UserName = "krebs", Email = "krebs@krebs.com", Date = DateTime.Now, Money = 50, Tc = "12345678910", PhoneNumber = "5448271400", Adress = "Samsun" };

                manager.Create(user, "1234567");
                manager.AddToRole(user.Id, "supplier");
                userIds.Add("krebs", user.Id);
            }

            context.SaveChanges();

            /* Suppliers start */
            List<Supplier> suppliers = new List<Supplier>()
            {
                new Supplier(){UserId = userIds["bazig"]},
                new Supplier(){UserId = userIds["krebs"]}
            };

            foreach (var item in suppliers)
            {
                context.Suppliers.Add(item);
            }
            /* Suppliers end */


            /* Customers start */
            List<Customer> customers = new List<Customer>()
            {
                new Customer(){UserId = userIds["99lp"]},
                new Customer(){UserId = userIds["ascarris"]}
            };

            foreach (var item in customers)
            {
                context.Customers.Add(item);
            }
            /* Customers end */

            context.SaveChanges();

            /* SupplierProducts start */
            List<SupplierProduct> supplierProducts = new List<SupplierProduct>()
            {
                new SupplierProduct(){ProductId = 1, SupplierId = 1, QuantityValue = 50, Price = 5},
                new SupplierProduct(){ProductId = 2, SupplierId = 1, QuantityValue = 100, Price = 20},
                new SupplierProduct(){ProductId = 3, SupplierId = 2, QuantityValue = 10, Price = 10},
                new SupplierProduct(){ProductId = 4, SupplierId = 1, QuantityValue = 300, Price = 7},
                new SupplierProduct(){ProductId = 5, SupplierId = 2, QuantityValue = 40, Price = 10},
                new SupplierProduct(){ProductId = 3, SupplierId = 1, QuantityValue = 60, Price = 8},
                new SupplierProduct(){ProductId = 1, SupplierId = 2, QuantityValue = 70, Price = 6},
            };

            foreach (var item in supplierProducts)
            {
                context.SupplierProducts.Add(item);
            }
            /* SupplierProducts end */

            context.SaveChanges();

            /* SupplierProducts end */

            /* OrderSuppliers star */
            List<Order> orders = new List<Order>()
            {
                new Order(){SupplierId = 1, CustomerId = 1, ProductId = 1, Price = 150, QuantityValue = 30},
                new Order(){SupplierId = 2, CustomerId = 1, ProductId = 1, Price = 120, QuantityValue = 20},
                new Order(){SupplierId = 2, CustomerId = 1, ProductId = 5, Price = 300, QuantityValue = 30},
                new Order(){SupplierId = 1, CustomerId = 2, ProductId = 2, Price = 400, QuantityValue = 20},
                new Order(){SupplierId = 1, CustomerId = 2, ProductId = 1, Price = 80, QuantityValue = 10}
            };

            foreach (var item in orders)
            {
                context.Orders.Add(item);
            }
            /* OrderSuppliers end */

            context.SaveChanges();

            /* MoneyConfirms start */
            List<MoneyConfirm> moneyConfirms = new List<MoneyConfirm>()
            {
                new MoneyConfirm(){IsApproved = false, Money = 50, CustomerId = 1},
                new MoneyConfirm(){IsApproved = false, Money = 60, CustomerId = 1 },
                new MoneyConfirm(){IsApproved = true, Money = 70, CustomerId = 2},
                new MoneyConfirm(){IsApproved = true, Money = 800, CustomerId = 2},
                new MoneyConfirm(){IsApproved = false, Money = 80, CustomerId = 2}
            };

            foreach (var item in moneyConfirms)
            {
                context.MoneyConfirms.Add(item);
            }
            /* MoneyConfirms end */

            /* FinanceHistory start */
            List<FinanceHistory> financeHistories = new List<FinanceHistory>()
            {
                new FinanceHistory(){CustomerId = 1, Money = 270, Date = DateTime.Now, FinanceTypeId = 2},
                new FinanceHistory(){CustomerId = 1, Money = 300, Date = DateTime.Now, FinanceTypeId = 2},
                new FinanceHistory(){CustomerId = 1, Money = 800, Date = DateTime.Now, FinanceTypeId = 1},
                new FinanceHistory(){CustomerId = 2, Money = 70, Date = DateTime.Now, FinanceTypeId = 1},
                new FinanceHistory(){CustomerId = 2, Money = 100, Date = DateTime.Now, FinanceTypeId = 3},
                new FinanceHistory(){CustomerId = 2, Money = 400, Date = DateTime.Now, FinanceTypeId = 1},
                new FinanceHistory(){CustomerId = 2, Money = 80, Date = DateTime.Now, FinanceTypeId = 1}
            };

            foreach (var item in financeHistories)
            {
                context.FinanceHistories.Add(item);
            }
            /* FinanceHistory end */

            context.SaveChanges();

            base.Seed(context);
        }
    }
}