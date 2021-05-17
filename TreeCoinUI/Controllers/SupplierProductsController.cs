using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TreeCoinUI.Entity;
using TreeCoinUI.Identity;

namespace TreeCoinUI.Controllers
{
    public class SupplierProductsController : Controller
    {
        private IdentityDataContext db = new IdentityDataContext();

        // GET: SupplierProducts
        public ActionResult Index()
        {
            var supplierProducts = db.SupplierProducts.Include(s => s.Product).Include(s => s.Supplier);
            return View(supplierProducts.ToList());
        }

        // GET: SupplierProducts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierProduct supplierProduct = db.SupplierProducts.Find(id);
            if (supplierProduct == null)
            {
                return HttpNotFound();
            }
            return View(supplierProduct);
        }

        // GET: SupplierProducts/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "UserId");
            return View();
        }

        // POST: SupplierProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SupplierId,ProductId,Price,QuantityValue,IsApproved")] SupplierProduct supplierProduct)
        {
            if (ModelState.IsValid)
            {
                db.SupplierProducts.Add(supplierProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", supplierProduct.ProductId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "UserId", supplierProduct.SupplierId);
            return View(supplierProduct);
        }

        // GET: SupplierProducts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierProduct supplierProduct = db.SupplierProducts.Find(id);
            if (supplierProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", supplierProduct.ProductId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "UserId", supplierProduct.SupplierId);
            return View(supplierProduct);
        }

        // POST: SupplierProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SupplierId,ProductId,Price,QuantityValue,IsApproved")] SupplierProduct supplierProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplierProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", supplierProduct.ProductId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "UserId", supplierProduct.SupplierId);
            return View(supplierProduct);
        }

        // GET: SupplierProducts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierProduct supplierProduct = db.SupplierProducts.Find(id);
            if (supplierProduct == null)
            {
                return HttpNotFound();
            }
            return View(supplierProduct);
        }

        // POST: SupplierProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SupplierProduct supplierProduct = db.SupplierProducts.Find(id);
            db.SupplierProducts.Remove(supplierProduct);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
