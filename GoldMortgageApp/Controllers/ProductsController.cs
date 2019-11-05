using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GoldMortgageApp.Models;
using System.IO;

namespace GoldMortgageApp.Controllers
{
    public class ProductsController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).OrderByDescending(a=>a.ArrivalDate);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,Name,ArrivalDate,Weight,Price,Discount,Description,Quantity")] Product product , HttpPostedFileBase file)
        {

            try
            {
                if (file.ContentLength > 0)
                {
                    product.ProductFile = DateTime.Now.Date.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Date.Millisecond.ToString() + Path.GetFileName(file.FileName);
                    product.ProductPath = Path.Combine(Server.MapPath("~/Image"), product.ProductFile);
                    file.SaveAs(product.ProductPath);

                }
            }

            catch
            {
                ViewBag.Message = " File Upload Failed!";
                return View(product);
            }

            if (ModelState.IsValid)
            {
                
                
                db.Products.Add(product);
                db.SaveChanges();


                product.ProductId = "P-" + DateTime.Now.Year + product.Id;

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,ProductId,Name,ArrivalDate,ProductFile,ProductPath,Weight,Price,Discount,Description,Quantity")] Product product, HttpPostedFileBase file)
        {
            if (file != null)
            {
                try
                {
                    if (file.ContentLength > 0)
                    {
                        product.ProductFile = DateTime.Now.Date.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Date.Millisecond.ToString() + Path.GetFileName(file.FileName);
                        product.ProductPath = Path.Combine(Server.MapPath("~/Image"), product.ProductFile);
                        file.SaveAs(product.ProductPath);

                    }
                }

                catch
                {
                    ViewBag.Message = " File Upload Failed!";
                    return View(product);
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
