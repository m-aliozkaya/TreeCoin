using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            var finans = _context.FinanceHistories
                .Where(f => f.CustomerId == customer.Id)
                .Join(_context.MoneyTypes, f => f.MoneyTypeId, m => m.Id, (f,m) => new {f.FinanceTypeId, f.Date, f.CustomerId, f.Money, f.MoneyTypeId, m.Code, f.Id  })
                .ToList();

            var financeHistories = finans.Select(f => new FinanceHistory
            {
                FinanceTypeId = f.FinanceTypeId,
                CustomerId = f.CustomerId,
                Date = f.Date,
                Money = f.Money,
                Id = f.Id,
                MoneyType = new MoneyType {Code = f.Code },
                MoneyTypeId = f.MoneyTypeId
            }).OrderByDescending(f => f.Date).ToList();


            ViewBag.CustomerId = customer.Id;

            Cuzdanim model;

            if (finans.Count() != 0 )
            {
                model = new Cuzdanim() { Bakiye = bakiye, FinanceHistory = financeHistories };
            } else
            {
                model = new Cuzdanim() { Bakiye = bakiye };
            }


            return View(model);
        }

        public ActionResult SatisGecmisim()
        {
            var userId = User.Identity.GetUserId();
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();

            var model = _context.Orders.Where(o => o.SupplierId == supplier.Id).Join(_context.Products, o => o.ProductId, p => p.Id, (o, p) => new SalesHistory() {Date = o.Date, Price = o.Price, ProductName = p.Name, QuantityValue = o.QuantityValue}).ToList();

            return View(model);
        }

        public ActionResult AlimRaporunuAl()
        {
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);
            var customer = _context.Customers.Where(c => c.UserId == userId).FirstOrDefault();

            List<string> infoList = new List<string>();
            infoList.Add($"Kullanici Adi : {user.UserName}");
            infoList.Add($"Ad : {user.Name}");
            infoList.Add($"Soyad : {user.SurName}");
            infoList.Add($"Email : {user.Email}");
            infoList.Add($"Para : {user.Money}");

            var history = _context.Orders.Where(o => o.CustomerId == customer.Id).Join(_context.Products, o => o.ProductId, p => p.Id, (o, p) => new SalesHistory() { Date = o.Date, Price = o.Price, ProductName = p.Name, QuantityValue = o.QuantityValue }).ToList();
            return RaporAl(history, infoList);
        }

        public ActionResult SatisRaporunuAl()
        {
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();

            List<string> infoList = new List<string>();
            infoList.Add($"Kullanici Adi : {user.UserName}");
            infoList.Add($"Ad : {user.Name}");
            infoList.Add($"Soyad : {user.SurName}");
            infoList.Add($"Email : {user.Email}");
            infoList.Add($"Para : {user.Money} TL");

            var history = _context.Orders.Where(o => o.SupplierId == supplier.Id).Join(_context.Products, o => o.ProductId, p => p.Id, (o, p) => new SalesHistory() { Date = o.Date, Price = o.Price, ProductName = p.Name, QuantityValue = o.QuantityValue }).ToList();
            return RaporAl(history, infoList);
        }

        public ActionResult RaporAl(List<SalesHistory> salesHistories, List<string> infos)
        {
            MemoryStream workStream = new MemoryStream();
            //file name to be created 
            string strPDFFileName = string.Format("TreeCoinRapor" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 10f, 0f);
   
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(4);

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
    
            Paragraph paragraph = new Paragraph();
            paragraph.IndentationLeft = 30;
            paragraph.SpacingBefore = 30;

            foreach (var item in infos)
            {
                Chunk chunk = new Chunk(item);
                chunk.SetBackground(new BaseColor(128, 0, 0));
                chunk.Font = FontFactory.GetFont("TimesNewRoman", 15, BaseColor.ORANGE);
                paragraph.Add(chunk);
                paragraph.Add(Chunk.NEWLINE);
            }

            doc.Add(paragraph);

            Paragraph p = new Paragraph();
            p.IndentationLeft = 30;
            p.SpacingBefore = 20;
            p.Add(CreateSalesHistoryTable(tableLayout, salesHistories));
            doc.Add(p);
            
            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", strPDFFileName);
        }

        protected PdfPTable CreateSalesHistoryTable(PdfPTable tableLayout, List<SalesHistory> salesHistories)
        {
            float[] headers = { 35, 35, 35, 35 };  //Header Widths
            tableLayout.SetWidths(headers);
            tableLayout.HorizontalAlignment = Element.ALIGN_LEFT; //Set the pdf headers
            tableLayout.WidthPercentage = 60;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top

    
            tableLayout.AddCell(new PdfPCell(new Phrase("Alim-Satim Geçmisi", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });

            ////Add header
            AddCellToHeader(tableLayout, "Ürün Ismi");
            AddCellToHeader(tableLayout, "Ürün Miktar");
            AddCellToHeader(tableLayout, "Tutar");
            AddCellToHeader(tableLayout, "Tarih");

            ////Add body
          
            foreach (var item in salesHistories)
            {
                AddCellToBody(tableLayout, item.ProductName);
                AddCellToBody(tableLayout, item.QuantityValue.ToString());
                AddCellToBody(tableLayout, item.Price.ToString());
                AddCellToBody(tableLayout, item.Date.ToString());
            }

            return tableLayout;
        }

        // Method to add single cell to the Header
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.ORANGE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0) });
        }

        // Method to add single cell to the body
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LoadMoney(int Id, string submit, string rateCode, int Quantity = 0)
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
                    money = Quantity;
                    break;
            }

            FinanceHistory financeHistory = new FinanceHistory() { CustomerId = Id, Money = money, Date = DateTime.Now, FinanceTypeId = 2 };
            _context.FinanceHistories.Add(financeHistory);

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

                // Kullanıcı Giriş Yapamadı Hata Ekle
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