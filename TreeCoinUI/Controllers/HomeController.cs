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


        // Ürünleri listeler.
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(_context.Products.Where(product => product.IsApproved == true).ToList());
        }


        // Ürün alım sayfası
        public ActionResult Buy(int id)
        {
            ViewBag.Success = null;
            ViewBag.Product = _context.Products.Find(id);
            var buy = new Buy() { ProductId = id };
            return View(buy);
        }


        // Ürününün alım işleminin gerçekleştiği method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(Buy model)
        {
            var user = _context.Users.Find(User.Identity.GetUserId());

            // Kullanıcı satın alma işlemini yapar ve bir sonuç döner.
            BuyResult result = BuyFromSupplier(model, user);

            // Eğer satın alma olduysa yani tutar > 0 ' ise komisyon alınır ve bu kullanıcının finans geçmişine eklenir.
            if (result.Amount > 0)
            {
                double commission = GetCommission(user, result.Amount);

                FinanceHistory financeHistory = new FinanceHistory() { CustomerId = result.CustomerId, Money = result.Amount + commission, Date = DateTime.Now, FinanceTypeId = 4, MoneyTypeId = 1 };
                _context.FinanceHistories.Add(financeHistory);

                _context.SaveChanges();

                ViewBag.Success = true;
                ViewBag.Message = $"{result.PurchasedQuantity} adet ürün {result.Amount} TL' den alındı. İşlem için {commission} TL ücret kesildi.";

            }
            // Eğer kullanıcı belirli bir fiyattan almak istiyorsa bu istek listesine eklenir.
            else if (model.Price > 0)
            {
                LimitBuy limitBuy = new LimitBuy {Date = DateTime.Now, Price = model.Price, ProductId = model.ProductId, UserId = user.Id, Quantity = model.Quantity };
                _context.LimitBuys.Add(limitBuy);

                _context.SaveChanges();

                ViewBag.Success = true;
                ViewBag.Message = $" {model.Quantity} tane ürün {model.Price}TL' den alınmak üzere istek listesine eklendi.";
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Para yetmediği için ürün alınamadı.";
            }

            ViewBag.Product = _context.Products.Find(model.ProductId);
            return View();
        }


        // Verilen ürün idsine sahip bir istek listesi varsa o ürünü satın almayı dener. Satın aldığı kadar istek listesinden azaltır.
        // Komisyon alır ve işlem geçmişine ekler.
        public void LimitBuy(int productId)
        {
            var limitBuys = _context.LimitBuys.Where(l => l.ProductId == productId).OrderByDescending(l => l.Price);

            foreach (var item in limitBuys)
            {
                var user = _context.Users.Find(item.UserId);
                var result = BuyFromSupplier(new Buy {ProductId = item.ProductId, Quantity = item.Quantity, Price = item.Price }, user);

                if (result.Amount == 0)
                {
                    break;
                }

                if (result.Amount > 0)
                {
                    double commission = GetCommission(user, result.Amount);

                    FinanceHistory financeHistory = new FinanceHistory() { CustomerId = result.CustomerId, Money = result.Amount + commission, Date = DateTime.Now, FinanceTypeId = 4, MoneyTypeId = 1 };
                    _context.FinanceHistories.Add(financeHistory);
                }

                item.Quantity -= result.PurchasedQuantity;

                if (item.Quantity == 0)
                {
                    _context.LimitBuys.Remove(item);
                }

            }

            _context.SaveChanges();
        }


        // Asıl satın alma işleminin gerçekleştiği kısım, kendisine verilen model ve kullanıcı doğrultusunda ..
        // .. satın alma işlemlerini yönetir.
        public BuyResult BuyFromSupplier(Buy model, ApplicationUser user)
        {
            var customerId = _context.Customers.Where(c => c.UserId == user.Id).FirstOrDefault().Id;
            var supplierProducts = _context.SupplierProducts.Where(p => p.ProductId == model.ProductId).OrderBy(sp => sp.Price);

            var wantedQuantity = model.Quantity;
            var customerMoney = user.Money;
            var purchasedQuantity = 0;
            double amount = 0;

            foreach (var item in supplierProducts)
            {
                if (customerMoney < item.Price || wantedQuantity == 0)
                {
                    break;
                }
                else if ((model.Price > 0 && model.Price < item.Price))
                {
                    break;
                }

                var supplier = _context.Users.Find(_context.Suppliers.Find(item.SupplierId).UserId);
                var startValue = item.QuantityValue;
                double supplierAmount = 0;

                while (wantedQuantity > 0 && item.QuantityValue > 0)
                {
                    if ((customerMoney - (item.Price + (item.Price * 1 / 100)) <= 0))
                    {
                        break;
                    }

                    wantedQuantity--;
                    purchasedQuantity++;
                    item.QuantityValue--;

                    supplier.Money += item.Price;
                    amount += item.Price;
                    supplierAmount += item.Price;
                    customerMoney -= item.Price;
                }

                // Sipariş geçmişi oluşturur.
                Order order = new Order()
                {
                    CustomerId = customerId,
                    Date = DateTime.Now,
                    ProductId = item.ProductId,
                    QuantityValue = startValue - item.QuantityValue,
                    SupplierId = item.SupplierId,
                    Price = supplierAmount
                };

                _context.Orders.Add(order);
            }

            return new BuyResult { Amount = amount, PurchasedQuantity = purchasedQuantity, CustomerId = customerId };
        }
        

        // Verilen müşteri ve miktar doğrultusunda komisyon keser.
        public double GetCommission(ApplicationUser customer, double amount)
        {
            double commission = amount * 1 / 100;
            var accountant = _context.Users.Where(u => u.Name == "System").FirstOrDefault();
            customer.Money -= amount + commission;
            accountant.Money += commission;
            return commission;
        }
    }
}