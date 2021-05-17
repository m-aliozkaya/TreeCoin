using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
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
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> UserManager;
        private RoleManager<ApplicationRole> RoleManager;

        IdentityDataContext _context = new IdentityDataContext();

        public AccountController()
        {
            var userStore = new UserStore<ApplicationUser>(new IdentityDataContext());
            UserManager = new UserManager<ApplicationUser>(userStore);

            var roleStore = new RoleStore<ApplicationRole>(new IdentityDataContext());
            RoleManager = new RoleManager<ApplicationRole>(roleStore);
        }

        public ActionResult Dukkanim()
        {
            var userId = User.Identity.GetUserId();
            var bakiye = _context.Users.Find(userId).Money;
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();

            var model = _context.SupplierProducts.Join(_context.Products, sp => sp.ProductId, p => p.Id, (sp, p) => new Dukkanim(){
            ProductId = p.Id, Image = p.Image, IsApproved = p.IsApproved, QuantityValue = sp.QuantityValue, ProductName = p.Name, Price = sp.Price, SupplierId = sp.SupplierId
            }).Where(sp => sp.SupplierId == supplier.Id).ToList();

            return View(model);
        }

        public ActionResult Cuzdanim()
        {
            var userId = User.Identity.GetUserId();
            var bakiye = _context.Users.Find(userId).Money;
            var customer = _context.Customers.Where(c => c.UserId == userId).FirstOrDefault();

            var finans = _context.FinanceHistories.Where(f => f.CustomerId == customer.Id).OrderByDescending(f => f.Date);

            ViewBag.CustomerId = customer.Id;

            Cuzdanim model;

            if (finans.Count() != 0 )
            {
                model = new Cuzdanim() { Bakiye = bakiye, FinanceHistory = finans.ToList() };
            } else
            {
                model = new Cuzdanim() { Bakiye = bakiye };
            }


            return View(model);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LoadMoney(int Id, string submit)
        {
            int money;

            switch (submit)
            {
                case "50 TL":
                    money = 50;
                    break;
                case "100 TL":
                    money = 100;
                    break; 
                case "200 TL":
                    money = 200;
                    break;
                default:
                    throw new Exception();
            }

            var moneyConfirm = new MoneyConfirm() { Money = money, CustomerId = Id };

            _context.MoneyConfirms.Add(moneyConfirm);
            _context.SaveChanges();
            
            return RedirectToAction("Cuzdanim");
        }

        public ActionResult Register()
        {
            var roles = RoleManager.Roles.Where(r => r.Name != "admin").ToList();
            ViewBag.RoleId = new SelectList(roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                //Kayıt işlemleri

                var user = new ApplicationUser();
                user.Name = model.Name;
                user.SurName = model.SurName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Adress = model.Adress;
                user.PhoneNumber = model.PhoneNumber;
                user.Tc = model.Tc;

                var result = UserManager.Create(user, model.Password);
                var role = RoleManager.FindById(model.RoleId).Name;

                if (result.Succeeded)
                {              
                       UserManager.AddToRole(user.Id, role);
                    
                    return RedirectToAction("Login");

                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı  oluşturulamadı.");
                }

            }
            var roles = RoleManager.Roles.Where(r => r.Name != "admin").ToList();
            ViewBag.RoleId = new SelectList(roles, "Id", "Name", model.RoleId);
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                //Login işlemleri
                var user = UserManager.Find(model.UserName, model.Password);

                if (user != null)
                {
                    // varolan kullanıcıyı sisteme dahil et.
                    // ApplicationCookie oluşturup sisteme bırak.

                    var authManager = HttpContext.GetOwinContext().Authentication;
                    var identityclaims = UserManager.CreateIdentity(user, "ApplicationCookie");
                    var authProperties = new AuthenticationProperties();
                    authProperties.IsPersistent = model.RememberMe;
                    authManager.SignIn(authProperties, identityclaims);

                    if (!String.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya Şifre Yanlış.");
                }
            }

            return View();
        }


        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}