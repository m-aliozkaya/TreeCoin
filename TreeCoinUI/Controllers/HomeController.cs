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
    public class HomeController : Controller
    {
        IdentityDataContext _context = new IdentityDataContext();
        
        // GET: Home
        public ActionResult Index()
        {
            return View(_context.Products.Where(product => product.IsApproved == true).ToList());
        }

        public ActionResult Buy(int id)
        {
            var buy = new Buy() {ProductId = id };
            return View(buy);
        }       
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(Buy model)
        {
            var user = _context.Users.Find(User.Identity.GetUserId());
            user.Money -= 10;

            _context.SaveChanges();

            return View(model);
        }
    }
}