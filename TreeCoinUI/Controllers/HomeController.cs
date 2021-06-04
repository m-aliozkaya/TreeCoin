using Microsoft.AspNet.Identity;
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
        IdentityDataContext _context = new IdentityDataContext();

        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(_context.Products.Where(product => product.IsApproved == true).ToList());
        }

        public ActionResult Buy(int id)
        {
            ViewBag.Success = null;
            var buy = new Buy() { ProductId = id };
            return View(buy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(Buy model)
        {
            var user = _context.Users.Find(User.Identity.GetUserId());
            var supplierProducts = _context.SupplierProducts.Where(p => p.ProductId == model.ProductId).OrderBy(sp => sp.Price);
            var productName = _context.Products.Find(model.ProductId).Name;

            var wantedQuantity = model.Quantity;
            var customerMoney = user.Money;
            var purchasedQuantity = 0;
            double amount = 0;

            foreach (var item in supplierProducts)
            {
                var supplier = _context.Users.Find(_context.Suppliers.Find(item.SupplierId).UserId);

                while (wantedQuantity > 0 && item.QuantityValue > 0)
                {
                    if ((customerMoney - item.Price) <= 0)
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

            user.Money -= amount;
            _context.SaveChanges();
            Console.WriteLine($"{purchasedQuantity} adet ürün {amount} TL' den alındı");


            ViewBag.Success = true;
            ViewBag.Message = $"{purchasedQuantity} adet ürün {amount} TL' den alındı";
            return View(model);
        }
    }
}