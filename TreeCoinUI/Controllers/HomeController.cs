using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TreeCoinUI.Entity;
using TreeCoinUI.Identity;
using TreeCoinUI.Models;

namespace TreeCoinUI.Controllers
{
    [Authorize(Roles = "customer, admin")]
    public class HomeController : Controller
    {

        private UserManager<ApplicationUser> UserManager;
        private RoleManager<ApplicationRole> RoleManager;
        IdentityDataContext _context = new IdentityDataContext();

        public HomeController()
        {
            var userStore = new UserStore<ApplicationUser>(new IdentityDataContext());
            UserManager = new UserManager<ApplicationUser>(userStore);

            var roleStore = new RoleStore<ApplicationRole>(new IdentityDataContext());
            RoleManager = new RoleManager<ApplicationRole>(roleStore);
        }

        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(_context.Products.Where(product => product.IsApproved == true).ToList());
        }

        public ActionResult Buy(int id)
        {
            ViewBag.Success = null;
            ViewBag.Product = _context.Products.Find(id);
            var buy = new Buy() { ProductId = id };
            return View(buy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(Buy model)
        {
            var user = _context.Users.Find(User.Identity.GetUserId());
            var supplierProducts = _context.SupplierProducts.Where(p => p.ProductId == model.ProductId).OrderBy(sp => sp.Price);

            var wantedQuantity = model.Quantity;
            var customerMoney = user.Money;
            var purchasedQuantity = 0;
            double amount = 0;

            foreach (var item in supplierProducts)
            {
                var supplier = _context.Users.Find(_context.Suppliers.Find(item.SupplierId).UserId);

                while (wantedQuantity > 0 && item.QuantityValue > 0)
                {
                    if ((customerMoney - (item.Price + (item.Price * 1/100) ) <= 0))
                    {
                        break;
                    }

                    wantedQuantity--;
                    purchasedQuantity++;
                    item.QuantityValue--;

                    supplier.Money += item.Price;
                    amount += item.Price;
                    customerMoney -= item.Price;
                }
            }

            double commission = GetCommission(user, amount);
            
            ViewBag.Product = _context.Products.Find(model.ProductId);
            ViewBag.Success = true;
            ViewBag.Message = $"{purchasedQuantity} adet ürün {amount} TL' den alındı. İşlem için {commission} TL ücret kesildi.";
            return View(model);
        }

        public double GetCommission(ApplicationUser customer, double amount)
        {
            double commission = amount * 1 / 100;
            var accountant = _context.Users.Where(u => u.Name == "System").FirstOrDefault();
            customer.Money -= amount + commission;
            accountant.Money += commission;
            _context.SaveChanges();
            return commission;
        }
    }
}