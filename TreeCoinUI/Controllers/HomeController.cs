using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TreeCoinUI.Entity;
using TreeCoinUI.Identity;

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
    }
}