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
    [Authorize(Roles = "admin")]
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

            var financeHistories = _context.FinanceHistories.Where(f => f.FinanceTypeId == 2).ToList();

            var model = new Admin() { Products = products, FinanceHistories = financeHistories };

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
            var financeHistory = _context.FinanceHistories.Find(Id);
            var userId = _context.Customers.Find(CustomerId).UserId;
            var user = _context.Users.Find(userId);

            switch (submit)
            {
                case "Sil":
                    financeHistory.FinanceTypeId = 3;
                    break;
                case "Onayla":
                     financeHistory.FinanceTypeId = 1;
                    user.Money += financeHistory.Money;
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