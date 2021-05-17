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
    [Authorize(Roles = "supplier")]
    public class ProductController : Controller
    {
        IdentityDataContext _context = new IdentityDataContext();

        public ActionResult YeniUrun()
        {
            ViewBag.QuantityId = new SelectList(_context.Quantities, "Id", "Type");
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult YeniUrun(NewProduct model)
        {

            var userId = User.Identity.GetUserId();
            var bakiye = _context.Users.Find(userId).Money;
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    Image = model.Image,
                    Name = model.Name,
                    QuantityId = model.QuantityId
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                var supplierProduct = new SupplierProduct()
                {
                    ProductId = product.Id,
                    Price = model.Price,
                    QuantityValue = model.QuantityValue,
                    SupplierId = supplier.Id
                };

                _context.SupplierProducts.Add(supplierProduct);
                _context.SaveChanges();

                return RedirectToAction("Dukkanim", "Account");
            }

            ViewBag.QuantityId = new SelectList(_context.Quantities, "Id", "Type", model.QuantityId);
            return View(model);
        }        
        
        public ActionResult UrunEkle()
        {
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UrunEkle(Urun model)
        {
            var userId = User.Identity.GetUserId();
            var bakiye = _context.Users.Find(userId).Money;
            var supplier = _context.Suppliers.Where(c => c.UserId == userId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var supplierProduct = new SupplierProduct()
                {
                    ProductId = model.ProductId,
                    Price = model.Price,
                    QuantityValue = model.QuantityValue,
                    SupplierId = supplier.Id
                };

                var urun = _context.SupplierProducts.Where(sp => sp.ProductId == supplierProduct.ProductId && sp.SupplierId == supplierProduct.SupplierId).FirstOrDefault();

                if (urun == null)
                {
                    _context.SupplierProducts.Add(supplierProduct);
                }
                else
                {
                    urun.QuantityValue += supplierProduct.QuantityValue;

                    if (supplierProduct.Price != 0)
                    {
                        urun.Price = supplierProduct.Price;
                    }

                }
                _context.SaveChanges();

                return RedirectToAction("Dukkanim", "Account");
            }

            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", model.ProductId);
            return View(model);
        }

    }
}