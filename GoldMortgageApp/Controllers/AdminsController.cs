using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GoldMortgageApp.Models;

namespace GoldMortgageApp.Controllers
{
    public class AdminsController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();

        // GET: Admins

        public ActionResult OrderList()
        {
            List<Buyer> Buyers = db.Buyers.OrderByDescending(a => a.Date).Where(a => a.PaymentStatus == true && a.IsDelivered == false).ToList();
            if (Session["Messsage"]!=null)
            {
                ViewBag.Message = Session["Messsage"].ToString();
                Session["Messsage"] = null;
            }
            return View(Buyers.ToList());
        }

        public ActionResult ProductList(int Id)
        {

            List<Order> Orders = db.Orders.Include(a=>a.Product).Where(a=>a.BuyerId==Id).ToList();
            return View(Orders.ToList());
        }

        public ActionResult Delivered(int Id)
        {
            Buyer buyer = db.Buyers.Find(Id);
            buyer.IsDelivered = true;

            db.Entry(buyer).State = EntityState.Modified;
            db.SaveChanges();
            Session["Messsage"] = "Order added to the delivary list! ";
            return RedirectToAction("OrderList");
        }

        public ActionResult DeliveredProductList()
        {
            List<Buyer> Buyers = db.Buyers.OrderByDescending(a => a.Date).Where(a => a.PaymentStatus == true && a.IsDelivered == true).ToList();
            return View(Buyers.ToList());
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Password)
        {
            var admins = db.Admins.ToList();
            var adminCheck = admins.Find(a => a.UserName == UserName && a.Password == Password);

            if (adminCheck != null)
            {
                
                Session["Admin"] = adminCheck.UserName;
                Session["Id"] = adminCheck.Id;
                Session["Count"] = null;
                Session["products"] = null;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.LogInError = "Either Name or PassWord Not Match!";
                return View();

            }

        }

        public ActionResult LogOut()
        {
            Session["Admin"] = null;
            Session["Id"] = null;
            Session["User"] = null;
            Session["Customer"] =null;
            Session["Name"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ChangePassword()
        {
            ChangePassword admin = new ChangePassword();
            admin.Id = Convert.ToInt32(Session["Id"]);
            admin.UserName = Session["Admin"].ToString();

            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "Id,UserName,OldPassword,NewPassword,ConfirmPassword")] ChangePassword admin)
        {


            if (ModelState.IsValid)
            {
                var adminlist = db.Admins.ToList();
                var adminCheck = adminlist.Find(a => a.Id == admin.Id);
                if (adminCheck.Password == admin.OldPassword)
                {
                    adminCheck.Password = admin.NewPassword;
                    UpdateAdmin(adminCheck);
                }
                else
                {
                    ViewBag.error = "Password not Match!";
                }



            }
            return View();

        }

        private ActionResult UpdateAdmin(Admin adminCheck)
        {
            db.Entry(adminCheck).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.messagepaschange = "Password has Been Changed!";
            return RedirectToAction("ChangePassword");
        }

        // GET: Admins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Password,Name")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(admin);
        }

        // GET: Admins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Password,Name")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Admin admin = db.Admins.Find(id);
            db.Admins.Remove(admin);
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
