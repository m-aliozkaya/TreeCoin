using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TreeCoinUI.Entity;
using TreeCoinUI.Identity;
using TreeCoinUI.Models;

namespace TreeCoinUI.Controllers
{
    // Admin işlemlerinin olduğu class
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        IdentityDataContext _context = new IdentityDataContext();


        // Onaylanmamış ürünleri ve para isteklerini anasayfada listeler
        public ActionResult Index()
        {
            var products = _context.Products.Where(p => p.IsApproved == false).Join(_context.Quantities, p => p.QuantityId, q => q.Id, (p, q) => new AdminProductConfirm()
            {
                QuantityType = q.Type,
                Image = p.Image,
                Name = p.Name,
                Id = p.Id
            }).ToList();
            
            var financeHistories = _context.FinanceHistories.Where(f => f.FinanceTypeId == 2).Join(_context.MoneyTypes, f => f.MoneyTypeId, m => m.Id, (x, y) => new FinanceHistoryModel{ CustomerId = x.CustomerId, Money = x.Money, MoneyCode = y.Code, Id = x.Id }).ToList();

            var model = new Admin() {Products = products, MoneyConfirms = financeHistories };

            return View(model);
        }


        // Adminin ürünü onayladığı ya da sildiği kısım
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string submit, int Id)
        {
            var product = _context.Products.Find(Id);

            switch (submit)
            {
                case "Sil":
                    _context.Products.Remove(product);
                    break;
                case "Onayla":
                    product.IsApproved = true;
                    break;
                default:
                    throw new Exception();
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // Adminin parayı onayladığı kısım
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmMoney(string submit, int CustomerId, int Id)
        {
            var financeHistory = _context.FinanceHistories.Find(Id);
            var moneyType = _context.MoneyTypes.Find(financeHistory.MoneyTypeId);
            var userId = _context.Customers.Find(CustomerId).UserId;
            var user = _context.Users.Find(userId);

            var url = "https://www.tcmb.gov.tr/kurlar/today.xml";
   


            switch (submit)
            {
                case "Sil":
                    financeHistory.FinanceTypeId = 3;
                    break;
                case "Onayla":
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    var doc = new XmlDocument();
                    doc.Load(url);
                    string usd = doc.SelectSingleNode($"Tarih_Date/Currency [@Kod='{moneyType.Code}']/BanknoteSelling").InnerXml;
                    double money = Convert.ToDouble(usd, provider) * financeHistory.Money;
                    financeHistory.FinanceTypeId = 1;
                    user.Money += money;
                    break;
                default:
                    throw new Exception();
            }

            financeHistory.Date = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}