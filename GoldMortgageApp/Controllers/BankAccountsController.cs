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
    public class BankAccountsController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();



      


        // GET: BankAccounts
        public ActionResult Index()
        {
            return View(db.BankAccounts.ToList());
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CardNumber,SecurityCode,ExpiryDate,CardHolderName,Amount")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CardNumber,SecurityCode,ExpiryDate,CardHolderName,Amount")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }








          public ActionResult Transaction(string PaymentId)
        {

           var buyer=db.Buyers.FirstOrDefault(a=>a.PaymentId==PaymentId);
            if (buyer==null)
            {
                RedirectToAction("Index", "Home");
            }


            BankAccount bankAccount = new BankAccount();
            bankAccount.Amount =Convert.ToDouble(buyer.Payment);
            ViewBag.PaymentId = PaymentId;

            return View(bankAccount);
        }

        // POST: Deposites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction(BankAccount Bank, string PaymentId)
        {
            
            var b = db.BankAccounts.FirstOrDefault(a => a.SecurityCode == Bank.SecurityCode && a.ExpiryDate == Bank.ExpiryDate && a.CardNumber == Bank.CardNumber && a.CardHolderName == Bank.CardHolderName);
            if (b != null)
            {
                if (b.Amount >= Bank.Amount)
                {
                    // Transection(Withdraw) From the bank 
                    BankAccount BA = db.BankAccounts.Find(b.Id);

                    BA.Amount = BA.Amount - Bank.Amount;
                    db.Entry(BA).State = EntityState.Modified;
                    db.SaveChanges();



                    string TID = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();






                    // Deposite Add in the System Database

                    var buy = db.Buyers.FirstOrDefault(a => a.PaymentId == PaymentId);

                    Buyer buyer = db.Buyers.Find(buy.Id);
                    buyer.PaymentStatus = true;
                    db.Entry(buyer).State = EntityState.Modified;
                    db.SaveChanges();
                    

                    //Withdraw Record Save To The Bank DB
                    Withdraw WT = new Withdraw();
                    WT.BankAccountId = BA.Id;
                    WT.DepositeId = buyer.PaymentId;
                    WT.TransectionId = TID;
                    WT.Amount = Bank.Amount;
                    WT.Date = DateTime.Now;

                    db.Withdraws.Add(WT);
                    db.SaveChanges();


                    //Product Reduce in the System Database

                    List<Order> ord = db.Orders.Where(a => a.BuyerId == buyer.Id).ToList();

                    if(ord.Count>0)
                    {
                        foreach(Order o in ord)
                        {
                            Product P = db.Products.Find(o.ProductId);
                            P.Quantity = P.Quantity - o.Quantity;
                            db.Entry(P).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }


                    return RedirectToAction("TransectionSuccessPage", new { Id = WT.Id });
                }
                else
                {
                    ViewBag.Error = "Error: Your Balance is Not Sufficient!";
                    return View(Bank);
                }
            }
            else
            {
                ViewBag.Error = "Error: Not Found Your Card! Try Again";
                return View(Bank);
            }

        }



        public ActionResult TransectionSuccessPage(int Id)
        {


            Withdraw Wt = db.Withdraws.Find(Id);
            BankAccount BA = db.BankAccounts.Find(Wt.BankAccountId);

            ViewBag.Account = BA.CardNumber;
            ViewBag.Name = BA.CardHolderName;

            return View(Wt);
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
