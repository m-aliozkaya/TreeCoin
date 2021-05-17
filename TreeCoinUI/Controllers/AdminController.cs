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
    public class AdminController : Controller
    {
        IdentityDataContext _context = new IdentityDataContext();

        // GET: Admin
        public ActionResult Index()
        {
            var products = _context.Products.Where(p => p.IsApproved == false).Join(_context.Quantities, p => p.QuantityId, q => q.Id, (p, q) => new AdminProductConfirm()
            {
                QuantityType = q.Type,
                Image = p.Image,
                Name = p.Name,       
                Id = p.Id
            }).ToList();

            /*            var products = _context.Products.Where(p => p.IsApproved == false).ToList();
            */
            var moneyConfirms = _context.MoneyConfirms.Where(m => m.IsApproved == false).ToList();

            var model = new Admin() { Products = products, MoneyConfirms = moneyConfirms};

            return View(model);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmMoney(string submit, int CustomerId, int Id)
        {
            var moneyConfirm = _context.MoneyConfirms.Find(Id);
            var financeHistory = new FinanceHistory() { CustomerId = CustomerId, Date = DateTime.Now, Money = moneyConfirm.Money };

            switch (submit)
            {
                case "Sil":
                    financeHistory.FinanceTypeId = 3;
                    _context.FinanceHistories.Add(financeHistory);
                    break;
                case "Onayla":
                     financeHistory.FinanceTypeId = 1;
                    _context.FinanceHistories.Add(financeHistory);
                    break;
                default:
                    throw new Exception();
            }

            var moneyConfirmx = _context.MoneyConfirms.Remove(moneyConfirm);
            Console.WriteLine(moneyConfirmx.ToString());

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}