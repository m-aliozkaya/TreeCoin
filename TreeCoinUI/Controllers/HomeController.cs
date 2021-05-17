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
            var buy = new Buy() {ProductId = id };
            return View(buy);
        }       
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(Buy model)
        {
            var user = _context.Users.Find(User.Identity.GetUserId());
            var supplierProducts = _context.SupplierProducts.Where(p => p.ProductId == model.ProductId).OrderBy(sp => sp.Price);
            var money = user.Money;
            var wantedQuantity = model.Quantity;
            var productName = _context.Products.Find(model.ProductId).Name;

            int toplamAlinan = 0;
            double toplamTutar = 0;

            Dictionary<int, double> idMoney = new Dictionary<int, double>();

            foreach (var item in supplierProducts)
            {
                if (item.QuantityValue <= wantedQuantity && (item.Price * item.QuantityValue) <= money)
                {
                    money -= item.Price * item.QuantityValue;
                    wantedQuantity -= item.QuantityValue;
                    idMoney.Add(item.SupplierId, item.Price * item.QuantityValue);
                    item.QuantityValue = 0;
                    toplamAlinan += item.QuantityValue;
                    toplamTutar += item.Price * item.QuantityValue;
                }
                else
                {
                    int neKadar = (int) Math.Floor(money / item.Price);

                    if (wantedQuantity <= neKadar)
                    {
                        item.QuantityValue -= wantedQuantity;
                        money -= neKadar * item.Price;
                        toplamTutar += neKadar * item.Price;
                        toplamAlinan += neKadar;
                    }
                    else
                    {
                        item.QuantityValue -= neKadar;
                        money -= neKadar * item.Price;
                        toplamTutar += neKadar * item.Price;
                        toplamAlinan += neKadar;
                    }

                    idMoney.Add(item.SupplierId, neKadar * item.Price);
                }

            }

            user.Money = money;

            _context.SaveChanges();

            foreach (var item in idMoney)
            {
                var userId = _context.Suppliers.Find(item.Key).UserId;
                var supplier = _context.Users.Find(userId);
                supplier.Money += item.Value;
            }

            _context.SaveChanges();

            ViewBag.Success = true;
            ViewBag.Message = $"{toplamAlinan} tane {productName} {toplamTutar} TL'den alındı ";
            return View(model);
        }
    }
}