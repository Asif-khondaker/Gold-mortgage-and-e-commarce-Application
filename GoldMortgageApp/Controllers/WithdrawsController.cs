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
    public class WithdrawsController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();

        // GET: Withdraws
        public ActionResult Index()
        {
            var withdraws = db.Withdraws.Include(w => w.BankAccount);
            return View(withdraws.ToList());
        }

        // GET: Withdraws/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Withdraw withdraw = db.Withdraws.Find(id);
            if (withdraw == null)
            {
                return HttpNotFound();
            }
            return View(withdraw);
        }

        // GET: Withdraws/Create
        public ActionResult Create()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "CardNumber");
            return View();
        }

        // POST: Withdraws/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DepositeId,TransectionId,Amount,Date,BankAccountId")] Withdraw withdraw)
        {
            if (ModelState.IsValid)
            {
                db.Withdraws.Add(withdraw);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "CardNumber", withdraw.BankAccountId);
            return View(withdraw);
        }

        // GET: Withdraws/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Withdraw withdraw = db.Withdraws.Find(id);
            if (withdraw == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "CardNumber", withdraw.BankAccountId);
            return View(withdraw);
        }

        // POST: Withdraws/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DepositeId,TransectionId,Amount,Date,BankAccountId")] Withdraw withdraw)
        {
            if (ModelState.IsValid)
            {
                db.Entry(withdraw).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "CardNumber", withdraw.BankAccountId);
            return View(withdraw);
        }

        // GET: Withdraws/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Withdraw withdraw = db.Withdraws.Find(id);
            if (withdraw == null)
            {
                return HttpNotFound();
            }
            return View(withdraw);
        }

        // POST: Withdraws/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Withdraw withdraw = db.Withdraws.Find(id);
            db.Withdraws.Remove(withdraw);
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
