using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TreeCoinUI.Entity;

namespace TreeCoinUI.Controllers
{
    public class HomeController : Controller
    {
        DataContext _context = new DataContext();
        
        // GET: Home
        public ActionResult Index()
        {
            return View(_context.Products.ToList());
        }

        public ActionResult Shop()
        {
            return View();
        }

        public ActionResult Wallet()
        {
            return View();
        }
    }
}