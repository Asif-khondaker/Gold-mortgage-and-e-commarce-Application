using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GoldMortgageApp.Models
{
    public class MortgageItemsController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();

        // GET: MortgageItems
        public ActionResult Index()
        {
            var mortgageItems = db.MortgageItems.Include(m => m.Customer).Where(a=>a.Status==true);
            return View(mortgageItems.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(DateTime From, DateTime To)
        {

            
            List<MortgageItem> deposites = db.MortgageItems.Where(a => a.IssueDate >= From && a.IssueDate <= To).Where(a=>a.Status==true).Include(d => d.Customer).ToList();
            List<MortgageItem> deposites2 = db.MortgageItems.Where(a => a.IssueDate.Day == To.Day && a.IssueDate.Month == To.Month && a.IssueDate.Year == To.Year).Where(a => a.Status == true).Include(d => d.Customer).ToList();

            foreach(MortgageItem M in deposites2)
            {
                MortgageItem Moo = deposites.FirstOrDefault(a => a.Id == M.Id);

                if(Moo ==null
                    )
                {
                    deposites.Add(M);
                }
                
            }


            double Amount = 0;
            if (deposites.Count > 0)
            {
                foreach (MortgageItem D in deposites)
                {
                    Amount = Amount + D.Loan;
                }
            }

            MortgageItem deposite = new MortgageItem();
            deposite.ItemQuantity = "Total";
            deposite.Loan = Amount;


            deposites.Add(deposite);

            return View(deposites.ToList());
        }

        // GET: MortgageItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MortgageItem mortgageItem = db.MortgageItems.Find(id);
            if (mortgageItem == null)
            {
                return HttpNotFound();
            }
            return View(mortgageItem);
        }

        // GET: MortgageItems/Create
        public ActionResult Create(int  Id)
        {
            Customer C = db.Customers.Find(Id);
            ViewBag.Customer = C.FullName;
            MortgageItem MR = new MortgageItem();
            MR.CustomerId = Id;
            return View(MR);
        }

        // POST: MortgageItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IssueDate,MortgageItemD,ItemQuantity,Price,Loan,InterestRate,InterestRatePerMonth,MaturityOfThisLoan,CustomerId")] MortgageItem mortgageItem, HttpPostedFileBase file)
        {
            mortgageItem.Status = true;
            try
            {
                if (file.ContentLength > 0)
                {
                    mortgageItem.File = DateTime.Now.Date.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Date.Millisecond.ToString() + Path.GetFileName(file.FileName);
                    mortgageItem.FilePath = Path.Combine(Server.MapPath("~/Image"), mortgageItem.File);
                    file.SaveAs(mortgageItem.FilePath);

                }
            }

            catch
            {
                ViewBag.Message = " File Upload Failed!";
                return View(mortgageItem);
            }

            if (ModelState.IsValid)
            {
                db.MortgageItems.Add(mortgageItem);
                db.SaveChanges();
                return RedirectToAction("Details", "Customers", new { Id = mortgageItem.CustomerId });
            }

           
            return RedirectToAction("Details","Customers",new {Id=mortgageItem.CustomerId});
        }

        // GET: MortgageItems/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MortgageItem mortgageItem = db.MortgageItems.Find(id);
            if (mortgageItem == null)
            {
                return HttpNotFound();
            }
            return View(mortgageItem);
        }

        // POST: MortgageItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IssueDate,MortgageItemD,MortgageItemFile,MortgageItemPath,ItemQuantity,Price,Loan,InterestRate,InterestRatePerMonth,MaturityOfThisLoan,File,FilePath,CustomerId")] MortgageItem mortgageItem, HttpPostedFileBase file)
        {
            mortgageItem.Status = true;
            if (file != null)
            {
                try
                {
                    if (file.ContentLength > 0)
                    {
                        mortgageItem.File = DateTime.Now.Date.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Date.Millisecond.ToString() + Path.GetFileName(file.FileName);
                        mortgageItem.FilePath = Path.Combine(Server.MapPath("~/Image"), mortgageItem.File);
                        file.SaveAs(mortgageItem.FilePath);

                    }
                }

                catch
                {
                    ViewBag.Message = " File Upload Failed!";
                    return View(mortgageItem);
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(mortgageItem).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Successfully Updated!";
                return View(mortgageItem);
            }
           
            return View(mortgageItem);
        }

        // GET: MortgageItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MortgageItem mortgageItem = db.MortgageItems.Find(id);
            if (mortgageItem == null)
            {
                return HttpNotFound();
            }
            return View(mortgageItem);
        }

        // POST: MortgageItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MortgageItem mortgageItem = db.MortgageItems.Find(id);
            mortgageItem.Status = false;
            mortgageItem.MaturityOfThisLoan = DateTime.Now;
            db.Entry(mortgageItem).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Customers", new { Id = mortgageItem.CustomerId });
        }

        public JsonResult GetMaturityDate(double InterestRatePerMonth, double Price, double Loan)
        {

           // MortgageItem MI = db.MortgageItems.Find(Id);
            Price = Price - Loan;

            int month = Convert.ToInt32(Price / InterestRatePerMonth);
            if (Price % InterestRatePerMonth > 0)
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
            if(MonthNow>12)
            {
                YearNow = YearNow + 1;
                MonthNow = MonthNow - 12;
            }
          

            string DateNow = YearNow + "-" + MonthNow.ToString("00") + "-" + DayNow.ToString("00");

            return Json(DateNow, JsonRequestBehavior.AllowGet);

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
