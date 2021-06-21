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
            // Kullanıcı işlemlerini yönetmek için controller başlayınca nesneleri oluşturuyoruz.
            var userStore = new UserStore<ApplicationUser>(new IdentityDataContext());
            UserManager = new UserManager<ApplicationUser>(userStore);

            // Rol işlemlerini yönetmek için controller başlayınca nesneleri oluşturuyoruz.
            var roleStore = new RoleStore<ApplicationRole>(new IdentityDataContext());
            RoleManager = new RoleManager<ApplicationRole>(roleStore);
        }


        public ActionResult Dukkanim()
        {
            // Kullanıcı bilgileri alınır
            var userId = User.Identity.GetUserId();
            var bakiye = _context.Users.Find(userId).Money;
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();


            // Satıcının ürünleri (resim, miktarı, ismi, fiyatı, onaylanıp onaylanmadığı) veritabanından çekilip view' a gönderiliyor.
            var model = _context.SupplierProducts.Join(_context.Products, sp => sp.ProductId, p => p.Id, (sp, p) => new Dukkanim(){
            ProductId = p.Id, Image = p.Image, IsApproved = p.IsApproved, QuantityValue = sp.QuantityValue, ProductName = p.Name, Price = sp.Price, SupplierId = sp.SupplierId
            }).Where(sp => sp.SupplierId == supplier.Id).ToList();
            return View(model);
        }


        public ActionResult Cuzdanim()
        {
            // Kullanıcı bilgileri alınır
            var userId = User.Identity.GetUserId();
            var bakiye = _context.Users.Find(userId).Money;
            var customer = _context.Customers.Where(c => c.UserId == userId).FirstOrDefault();


            // Müşterinin işlem geçmişini (alımlar, para yüklemeleri) getirir.
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



            // Müşterinin limit fiyattan yaptığı istekleri göstermek için veritabanından çekiyoruz
            var limitBuyQuery = _context.LimitBuys
                .Where(l => l.UserId == userId)
                .Join(_context.Products, l => l.ProductId, p => p.Id, (l, p) => new {l.Price, l.Quantity, l.Date, ProductName = p.Name })
                .ToList();

            var limitBuys = limitBuyQuery.Select(l => new LimitBuy()
            {
                Date = l.Date,
                Quantity = l.Quantity,
                Price = l.Price,
                Product = new Product { Name = l.ProductName}
            }).OrderByDescending(l => l.Date).ToList();


            // View' da kullanılmak istediğim verileri gönderiyoruz (Müşteri Id, bakiye, işlem geçmişi, bekleyen limit istek listesi)
            ViewBag.CustomerId = customer.Id;
            Cuzdanim model = new Cuzdanim() { Bakiye = bakiye, FinanceHistory = financeHistories, limitBuys = limitBuys };    
            return View(model);
        }


        public ActionResult SatisGecmisim()
        {
            // Kullanıcı bilgileri alınır
            var userId = User.Identity.GetUserId();
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();

            // Satıcının satış geçmişi en son satılandan ilk satılana doğru sıralanarak veritabanından çekilir ve view' a gönderilir.
            var model = _context.Orders.Where(o => o.SupplierId == supplier.Id).Join(_context.Products, o => o.ProductId, p => p.Id, (o, p) => new SalesHistory() {Date = o.Date, Price = o.Price, ProductName = p.Name, QuantityValue = o.QuantityValue}).OrderByDescending(o => o.Date).ToList();
            return View(model);
        }


        public ActionResult AlimRaporunuAl()
        {
            return View();
        }


        public ActionResult SatisRaporunuAl()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AlimRaporunuAl(ReportHistoryDates dates)
        {
            // Kullanıcı bilgileri alınır.
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);
            var customer = _context.Customers.Where(c => c.UserId == userId).FirstOrDefault();

            // Pdf' te kullanılmak üzere kullanıcı bilgilerinin olduğu bir map oluşturulur.
            var map = new Dictionary<string, string>()
            {
                {"Kullanici Adi", user.UserName},
                {"Ad", user.Name},
                {"Soyad", user.SurName},
                {"Email", user.Email},
                {"Para", user.Money.ToString()}
            };

            // Kullanıcının verdiği başlangıç ve bitiş tarihi aralığındaki alım geçmişini getirir.
            var history = _context.Orders.Where(o => o.CustomerId == customer.Id && (o.Date >= dates.StartDate && o.Date <= dates.EndDate)).Join(_context.Products, o => o.ProductId, p => p.Id, (o, p) => new SalesHistory() { Date = o.Date, Price = o.Price, ProductName = p.Name, QuantityValue = o.QuantityValue }).ToList();
            
            // RaporAl fonksiyonunu döndürür. Pdf verilen bilgilere göre oluşturulur ve kullanıcıya o veri indirilme olarak verilir.
            return RaporAl(history, map);
        }


        [HttpPost]
        public ActionResult SatisRaporunuAl(ReportHistoryDates dates)
        {
            // Kullanıcı bilgileri alınır.
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();

            // Pdf' te kullanılmak üzere kullanıcı bilgilerinin olduğu bir map oluşturulur.
            var map = new Dictionary<string, string>()
            {
                {"Kullanici Adi", user.UserName},
                {"Ad", user.Name},
                {"Soyad", user.SurName},
                {"Email", user.Email},
                {"Para", user.Money.ToString()}
            };

            // Kullanıcının verdiği başlangıç ve bitiş tarihi aralığındaki alım geçmişini getirir.
            var history = _context.Orders.Where(o => o.SupplierId == supplier.Id && (o.Date >= dates.StartDate && o.Date <= dates.EndDate)).Join(_context.Products, o => o.ProductId, p => p.Id, (o, p) => new SalesHistory() { Date = o.Date, Price = o.Price, ProductName = p.Name, QuantityValue = o.QuantityValue }).ToList();

            // RaporAl fonksiyonunu döndürür. Pdf verilen bilgilere göre oluşturulur ve kullanıcıya o veri indirilme olarak verilir.
            return RaporAl(history, map);
        }

        
        // Verilen listeye ve kullanıcı bilgilerine göre pdf oluşturur.
        public ActionResult RaporAl(List<SalesHistory> salesHistories, Dictionary<string, string> infos)
        {
            MemoryStream workStream = new MemoryStream();
            //file name to be created 
            string strPDFFileName = string.Format("TreeCoinRapor" + ".pdf");
            Document doc = new Document(PageSize.A5.Rotate());
            doc.SetMargins(30f, 30f, 20f, 0f);
   
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(5);

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
    
            Paragraph paragraph = new Paragraph();
            paragraph.SpacingBefore = 30;

            foreach (var item in infos)
            {
                Chunk chunk = new Chunk(item.Key + ": ");
                chunk.setLineHeight(25);
                chunk.Font = FontFactory.GetFont("TimesNewRoman", 15, Font.BOLD, BaseColor.BLACK);
                Chunk chunk2 = new Chunk(item.Value);
                chunk2.setLineHeight(25);
                chunk2.Font = FontFactory.GetFont("TimesNewRoman", 15, BaseColor.BLACK);
                paragraph.Add(chunk);
                paragraph.Add(chunk2);
                paragraph.Add(Chunk.NEWLINE);
            }

            doc.Add(paragraph);

            Paragraph p = new Paragraph();
            p.SpacingBefore = 50;
            p.Add(CreateSalesHistoryTable(tableLayout, salesHistories));
            doc.Add(p);
            
            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", strPDFFileName);
        }

       
        // Satış geçmişi ile ilgili tablo oluşturur. Ve bu tabloyu verilen listeye göre doldurur.
        protected PdfPTable CreateSalesHistoryTable(PdfPTable tableLayout, List<SalesHistory> salesHistories)
        {
            float[] headers = { 40, 20, 20, 20, 40 };  //Header Widths
            tableLayout.SetWidths(headers);
            tableLayout.HorizontalAlignment = Element.ALIGN_LEFT; //Set the pdf headers
            tableLayout.WidthPercentage =   100;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top

    
            tableLayout.AddCell(new PdfPCell(new Phrase("Alim-Satim Geçmisi", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });

            ////Add header
            AddCellToHeader(tableLayout, "Ürün Adi");
            AddCellToHeader(tableLayout, "Birim Fiyat");
            AddCellToHeader(tableLayout, "Ürün Miktar");
            AddCellToHeader(tableLayout, "Toplam Tutar");
            AddCellToHeader(tableLayout, "Tarih");

            ////Add body
          
            foreach (var item in salesHistories)
            {
                double pricePer = item.Price / item.QuantityValue;
                AddCellToBody(tableLayout, item.ProductName);
                AddCellToBody(tableLayout, pricePer.ToString());
                AddCellToBody(tableLayout, item.QuantityValue.ToString());
                AddCellToBody(tableLayout, item.Price.ToString());
                AddCellToBody(tableLayout, item.Date.ToString());
            }

            return tableLayout;
        }

       
        // Pdf' teki tablonun headerina cell ekleyen method
        
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.ORANGE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0) });
        }

        
        // Pdf' teki tablonun headerina cell ekleyen method
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 12, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });
        }


        // Para isteğinde bulunduran method
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

            int moneyType = Convert.ToInt32(rateCode);

            FinanceHistory financeHistory = new FinanceHistory() { CustomerId = Id, Money = money, Date = DateTime.Now, FinanceTypeId = 2, MoneyTypeId = moneyType };
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


        // Hesaptan ıkış yapabilmemizi sağlayan method.
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}