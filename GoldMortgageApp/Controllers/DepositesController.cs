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
    public class DepositesController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();

        // GET: Deposites
        public ActionResult Index()
        {
            var deposites = db.Deposites.Include(d => d.Customer).Where(a => a.Type != "Discount").OrderByDescending(a => a.Date);
            return View(deposites.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(DateTime From, DateTime To)
        {
            List<Deposite> deposites = db.Deposites.Where(a => a.Date >= From && a.Date <= To).Where(a => a.Type != "Discount").Include(d => d.Customer).ToList();
            List<Deposite> deposites2 = db.Deposites.Where(a => a.Date.Day == To.Day && a.Date.Month == To.Month && a.Date.Year == To.Year).Where(a => a.Type != "Discount").Include(d => d.Customer).ToList();

            foreach (Deposite M in deposites2)
            {
                deposites.Add(M);
            }
            
            double Amount = 0;
            if(deposites.Count>0)
            {
                foreach (Deposite D in deposites)
                {
                    Amount = Amount + D.Amount;
                }
            }

            Deposite deposite = new Deposite();
            deposite.Type = "Total";
            deposite.Amount = Amount;


            deposites.Add(deposite);

            return View(deposites.ToList());
        }

        // GET: Deposites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposite deposite = db.Deposites.Find(id);
            if (deposite == null)
            {
                return HttpNotFound();
            }
            return View(deposite);
        }

        // GET: Deposites/Create
        public ActionResult Create(int Id)
        {
            Deposite deposite = new Deposite();
            deposite.CustomerId = Id;
            return View(deposite);
        }

        // POST: Deposites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Amount,Date,CustomerId")] Deposite deposite)
        {
            deposite.Type = "Manual Transection";
            deposite.DepositeId = "N/A";
            deposite.TransectionId = "N/A";
            deposite.Date=DateTime.Now;
            
            if (ModelState.IsValid)
            {
                db.Deposites.Add(deposite);
                db.SaveChanges();
                ViewBag.Message = "Successfully Deposited!";
                UpdateMaturity(deposite.CustomerId);
                return View(deposite);
            }

           
            return View(deposite);
        }

        private void UpdateMaturity(int p)

        {

            Customer customer = db.Customers.Find(p);
            string mobileOrUserId = customer.MobileNo;
           
           
            
            List<MortgageItem> MRS = db.MortgageItems.Where(a => a.Customer.MobileNo == mobileOrUserId || a.Customer.UserId == mobileOrUserId).Where(a => a.Status == true && a.MaturityOfThisLoan >= DateTime.Now).OrderBy(a => a.IssueDate).ToList();
            List<Deposite> Dps = db.Deposites.Where(a => a.Customer.MobileNo == mobileOrUserId || a.Customer.UserId == mobileOrUserId).ToList();


      
            double amount = 0;
            double Deposite = 0;
            double Interest = 0;
            double Price = 0;
            int MID = 0;
            double InterestRate = 0;

            DateTime DT=DateTime.Now;
   

            if (MRS.Count > 0)
            {
                foreach (var M in MRS)
                {
                    
                    amount = amount + M.Loan;
                    
                  
                    int DayCount = Convert.ToInt32((DateTime.Now - M.IssueDate).TotalDays);
                    
                    double MonthlyI = M.InterestRate / 30;
                    DT = M.IssueDate;
                    InterestRate = M.InterestRatePerMonth;
                    MID = M.Id;
                    
                    Interest = Interest + M.Loan * (MonthlyI / 100) * DayCount;
                  
                    Price = M.Price;
               

                }

            }


            //Deposite Calculation



            foreach (var Dp in Dps)
            {
                if (Dp.Date >= DT)
                {
                   
                
                    Deposite = Deposite + Dp.Amount; ;
                   

                }


            }
           double Due =Price-(amount + Interest - Deposite);




           int month = Convert.ToInt32(Due / InterestRate);
           if (Due % InterestRate > 0)
           {
               month = month + 1;
           }

           DateTime Date = DateTime.Now;

           int RealYear = Convert.ToInt32(month / 12);
           int RealMonth = month % 12;

           int YearNow = Date.Year;
           int MonthNow = Date.Month;
           int DayNow = Date.Day;

           YearNow = YearNow + RealYear;
           MonthNow = MonthNow + RealMonth;

           if (MonthNow > 12)
           {
               YearNow = YearNow + 1;
               MonthNow = MonthNow - 12;
           }

           DateTime Maturity = new DateTime(YearNow, MonthNow, DayNow);


           MortgageItem MI = db.MortgageItems.Find(MID);

           MI.MaturityOfThisLoan = Maturity;

           db.Entry(MI).State = EntityState.Modified;
           db.SaveChanges();
           
        }
        public ActionResult TransectionSuccessPage(int Id)
        {


            Deposite Deposite = db.Deposites.Find(Id);

            return View(Deposite);
        }
        public ActionResult BankDeposite()
        {
            if(Session["Customer"]==null)
            {
                RedirectToAction("Index", "Home");
            }
            int Id = Convert.ToInt32(Session["Customer"]);
            ViewBag.CustomerId = Id;

            return View();
        }

        // POST: Deposites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankDeposite(BankAccount Bank)
        {
            if(Session["Customer"]==null)
            {
                return RedirectToAction("Index", "Home");
            }
            var b = db.BankAccounts.FirstOrDefault(a => a.SecurityCode == Bank.SecurityCode && a.ExpiryDate == Bank.ExpiryDate && a.CardNumber == Bank.CardNumber && a.CardHolderName == Bank.CardHolderName);
            if(b!=null)
            {
                if(b.Amount>=Bank.Amount)
                {
                    // Transection(Withdraw) From the bank 
                    BankAccount BA = db.BankAccounts.Find(b.Id);

                    BA.Amount = BA.Amount - Bank.Amount;
                    db.Entry(BA).State = EntityState.Modified;
                    db.SaveChanges();



                   string TID= DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                   
                    
                    



                    // Deposite Add in the System Database
                    Deposite deposite=new Deposite();
                    deposite.CustomerId = Convert.ToInt32(Session["Customer"]);
                    deposite.DepositeId=DateTime.Now.ToString()+deposite.CustomerId;
                    deposite.TransectionId = TID;
                    deposite.Amount = Bank.Amount;
                    deposite.Date = DateTime.Now;
                    deposite.Type = "Bank";
                    db.Deposites.Add(deposite);
                    db.SaveChanges();
                    UpdateMaturity(deposite.CustomerId);

                    //Withdraw Record Save To The Bank DB
                    Withdraw WT = new Withdraw();
                    WT.BankAccountId = BA.Id;
                    WT.DepositeId = DateTime.Now.Year.ToString() + deposite.CustomerId;
                    WT.TransectionId = TID;
                    WT.Amount = Bank.Amount;
                    WT.Date = DateTime.Now;

                    db.Withdraws.Add(WT);
                    db.SaveChanges();



                    return RedirectToAction("TransectionSuccessPage", new { Id=deposite.Id});
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

        // GET: Deposites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposite deposite = db.Deposites.Find(id);
            if (deposite == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "FullName", deposite.CustomerId);
            return View(deposite);
        }

        // POST: Deposites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DepositeId,TransectionId,Type,Amount,Date,CustomerId")] Deposite deposite)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deposite).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Customers", new { Id = deposite.CustomerId });
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "FullName", deposite.CustomerId);
            return View(deposite);
        }

        // GET: Deposites/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposite deposite = db.Deposites.Find(id);
            if (deposite == null)
            {
                return HttpNotFound();
            }
            return View(deposite);
        }

        // POST: Deposites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Deposite deposite = db.Deposites.Find(id);
            db.Deposites.Remove(deposite);
            db.SaveChanges();
            return RedirectToAction("Index", "Customers", new { Id = deposite.CustomerId });
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
