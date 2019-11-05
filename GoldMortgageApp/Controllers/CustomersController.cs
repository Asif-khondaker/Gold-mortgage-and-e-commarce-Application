using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GoldMortgageApp.Models;
using GoldMortgageApp.com.onnorokomsms.api1;



namespace GoldMortgageApp.Controllers
{
    public class CustomersController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();
        public ActionResult RedListedCustomer()
        {
            DateTime Date=DateTime.Now;
            int Month=Date.Month+1;


            List<MortgageItem> MIs = db.MortgageItems.Include(a => a.Customer).Where(a => a.Status == true && a.MaturityOfThisLoan >= DateTime.Now).Where(a => a.MaturityOfThisLoan.Month >= Date.Month && a.MaturityOfThisLoan.Month <= Month).Where(a => a.MaturityOfThisLoan.Year == Date.Year).ToList();
              if(Date.Month==12)
            {
                Month=1;
                MIs = db.MortgageItems.Include(a => a.Customer).Where(a => a.Status == true && a.MaturityOfThisLoan >= DateTime.Now).Where(a => a.MaturityOfThisLoan.Month == Date.Month || a.MaturityOfThisLoan.Month == Month).Where(a => a.MaturityOfThisLoan.Year == Date.Year|| a.MaturityOfThisLoan.Year==Date.Year+1).ToList();
  
            }
            return View(MIs.ToList());
        }

        public ActionResult Payout(double Due,int Id)
        {
            MortgageItem MI = db.MortgageItems.Find(Id);
            Customer C=db.Customers.Find(MI.CustomerId);

            ViewBag.Name = C.FullName;
            ViewBag.MortgageItem = MI.Id;
            ViewBag.MortgageDescription = MI.ItemQuantity;

            PayOut payout = new PayOut();

            payout.PreviousLone = Due;
            payout.MaturityOfThisLoan = MI.MaturityOfThisLoan;
            payout.Id = Id;
            payout.ItemQuantity = MI.ItemQuantity;
            payout.Price = MI.Price;
            payout.InterestRate = MI.InterestRate;
            payout.InterestRatePerMonth = MI.InterestRatePerMonth;

            return View(payout);
        }
         [HttpPost]
         [ValidateAntiForgeryToken]
        public ActionResult Payout(int Id, double PreviousLone, double Payout, double Discount, double Due, double InterestRate, double InterestRatePerMonth, DateTime MaturityOfThisLoan,string MortgageItemD, string ItemQuantity, double Price)
        {
           
            PayOut payOut=new PayOut();
             payOut.Id=Id;
             payOut.ItemQuantity=ItemQuantity;
             payOut.PreviousLone=PreviousLone;
             payOut.Payout=Payout;
             payOut.Due=Due;
             payOut.MaturityOfThisLoan=MaturityOfThisLoan;
             payOut.Price = Price;
             payOut.Discount = Discount;
             payOut.InterestRate = InterestRate;
             payOut.InterestRatePerMonth = InterestRatePerMonth;
             payOut.MortgageItemD = MortgageItemD;

             MortgageItem mortgageItem = db.MortgageItems.Find(payOut.Id);
             MortgageItem MINew = db.MortgageItems.Find(payOut.Id);
             MortgageItem MIDelete = db.MortgageItems.Find(payOut.Id);
             DateTime MIDeleteMaturityOfThisLoan = DateTime.Now;
             MIDelete.MaturityOfThisLoan = MIDeleteMaturityOfThisLoan;
            MIDelete.Status = false;

            db.Entry(MIDelete).State = EntityState.Modified;
            db.SaveChanges();

            
            
             
            Customer C = db.Customers.Find(MINew.CustomerId);

            ViewBag.Name = C.FullName;
            ViewBag.MortgageItem = MINew.Id;
            ViewBag.MortgageDescription = MINew.ItemQuantity;

            

             if(Due>0)
             {
                 mortgageItem.ItemQuantity = payOut.ItemQuantity;
                 mortgageItem.IssueDate = DateTime.Now;
                 mortgageItem.Price = payOut.Price;
                 mortgageItem.MaturityOfThisLoan = payOut.MaturityOfThisLoan;
                 mortgageItem.Loan = payOut.Due;
                 mortgageItem.Status = true;
                 mortgageItem.MortgageItemD = MortgageItemD;
                 mortgageItem.InterestRate = InterestRate;
                 mortgageItem.InterestRatePerMonth = InterestRatePerMonth;
                

                 db.MortgageItems.Add(mortgageItem);
                 db.SaveChanges();

             }


             //Deposite Add

             Deposite deposite = new Deposite();

             deposite.Amount = Payout;
             deposite.CustomerId = mortgageItem.CustomerId;
             deposite.Date = MIDeleteMaturityOfThisLoan;
             deposite.DepositeId = "N/A";
             deposite.TransectionId = "N/A";
             deposite.Type = "Manual";
             db.Deposites.Add(deposite);
             db.SaveChanges();
             if(Discount>0)
             {
                 Deposite deposite2 = new Deposite();

                 deposite2.Amount = Discount;
                 deposite2.CustomerId = mortgageItem.CustomerId;
                 deposite2.Date = MIDeleteMaturityOfThisLoan;
                 deposite2.DepositeId = 1 + DateTime.Now.ToString() + mortgageItem.CustomerId.ToString();
                 deposite2.TransectionId = 1 + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                 deposite2.Type = "Discount";
                 db.Deposites.Add(deposite2);
                 db.SaveChanges();
                 

             }
            


           
            ViewBag.Message = "Successfully Payed Out!";


            return View(payOut);
        }
        // GET: Customers
        public ActionResult CustomerList()
        {
            return View(db.Customers.Where(a=>a.Status==true).ToList());
        }

        public ActionResult CustomerInfo()
        {
            List<Customer> CS = new List<Customer>();
            return View(CS.ToList());
        }
        [HttpPost]
        public ActionResult CustomerInfo(string mobileOrUserId)
        {
            
            List<Customer> Customers = db.Customers.Where(a => a.MobileNo == mobileOrUserId || a.UserId == mobileOrUserId).Where(a => a.Status ==true).ToList();
           
            List<ViewFinace> VFS = new List<ViewFinace>();

            List<MortgageItem> MRSPast = db.MortgageItems.Where(a=>a.Customer.MobileNo==mobileOrUserId||a.Customer.UserId==mobileOrUserId).Where(a=>a.Status==false||a.MaturityOfThisLoan<DateTime.Now).OrderBy(a=>a.IssueDate).ToList();



            List<MortgageItem> MRS = db.MortgageItems.Where(a => a.Customer.MobileNo == mobileOrUserId || a.Customer.UserId == mobileOrUserId).Where(a => a.Status == true && a.MaturityOfThisLoan >= DateTime.Now).OrderBy(a => a.IssueDate).ToList();
            List<Deposite> Dps=db.Deposites.Where(a=>a.Customer.MobileNo==mobileOrUserId||a.Customer.UserId==mobileOrUserId).ToList();


            int count=1;
            double amount=0;
            double Deposite=0;
            double Interest = 0;
            DateTime DT = new DateTime(1990,1,1);

           
           

            if(MRSPast.Count>0)
            {
                 foreach(var M in MRSPast)
            {
                ViewFinace VF = new ViewFinace();
                VF.Serial = count;
                count = count + 1;
                VF.Loan = M.Loan;
                amount = amount + VF.Loan;
                DT = M.MaturityOfThisLoan;
             
                
                     
                //Interest Calculation


              //  int DayCount = (M.MaturityOfThisLoan.Year - M.IssueDate.Year) * 365 + (M.MaturityOfThisLoan.Month - M.IssueDate.Month) * 30 + M.MaturityOfThisLoan.Day - M.IssueDate.Day;
                int DayCount = Convert.ToInt32((M.MaturityOfThisLoan - M.IssueDate).TotalDays);

                //if(M.MaturityOfThisLoan.Day<=M.IssueDate.Month)
                //{
                //    DayCount = DayCount - 1;
                //}
                 double DailyI=M.InterestRate/30;

                VF.Interest = M.Loan * (DailyI /100) * DayCount;
                Interest = Interest + VF.Interest;
                VF.Description = "Mortgage Item: "+M.Id;
                VFS.Add(VF);

            }

            }

            
                //Deposite Calculation
                
                

                foreach(var Dp in Dps)
                {
                 if(Dp.Date<=DT)
                 {
                     ViewFinace VF3 = new ViewFinace();
                     VF3.Serial = count;
                     count = count + 1;
                     VF3.Description = "Date: "+Dp.Date+", Type="+Dp.Type;
                     VF3.Deposite = Dp.Amount;
                     Deposite = Deposite + VF3.Deposite;
                     VFS.Add(VF3);
                     
                 }
                
                
                }

                //ViewFinace VF4 = new ViewFinace();
                //VF4.Serial = count;
                //count = count + 1;
                //VF4.Description = "Total for Past Mortgages";
                //VF4.Deposite = Deposite;
                //VF4.Loan = amount;
                //VF4.Interest = Interest;
                //VF4.Due = Interest + amount - Deposite;
                //VFS.Add(VF4);

                ViewFinace VF5 = new ViewFinace();
                VF5.Description = "------------";
                VFS.Add(VF5);



            // Current Mortgage

               
                amount = 0;
                Deposite = 0;
                Interest = 0;
               double Price=0;
               int MortgageId=0;
                
                if (MRS.Count > 0)
                {
                    foreach (var M in MRS)
                    {
                        ViewFinace VF = new ViewFinace();
                        VF.Serial = count;
                        count = count + 1;
                        VF.Loan = M.Loan;
                        amount = amount + VF.Loan;
                        //Interest Calculation
                        MortgageId = M.Id;

                       // int DayCount = (DateTime.Now.Year - M.IssueDate.Year) * 12 + DateTime.Now.Month - M.IssueDate.Month;
                        int DayCount = Convert.ToInt32((DateTime.Now - M.IssueDate).TotalDays);
                        //if (DateTime.Now.Day <= M.IssueDate.Month)
                        //{
                        //    DayCount = DayCount - 1;
                        //}
                        double MonthlyI = M.InterestRate / 30;

                        VF.Interest = M.Loan * (MonthlyI / 100) * DayCount;
                        Interest = Interest + VF.Interest;
                        VF.Description = "Mortgage Item: " + M.Id;
                        VF.Due = Interest + amount - Deposite;
                        Price=M.Price;
                        VFS.Add(VF);

                    }

                }


                //Deposite Calculation



                foreach (var Dp in Dps)
                {
                    if (Dp.Date > DT)
                    {
                        ViewFinace VF3 = new ViewFinace();
                        VF3.Serial = count;
                        count = count + 1;
                        VF3.Description = "Date: " + Dp.Date + ", Type=" + Dp.Type;
                        VF3.Deposite = Dp.Amount;
                        Deposite = Deposite + VF3.Deposite;
                        VF3.Due = Interest + amount - Deposite;
                        if(VF3.Due>Price)
                        {
                            VF3.Description = VF3.Description + "(Negative)";
                        }
                        VFS.Add(VF3);

                    }


                }
                ViewBag.Due = amount + Interest - Deposite;
                ViewBag.Id = MortgageId;
                ViewBag.VewFinance = VFS.ToList();
               return View(Customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            List<MortgageItem> MRs = db.MortgageItems.Where(a => a.CustomerId == customer.Id).Where(a => a.Status == true).ToList();

            ViewBag.MortgageItem = MRs;
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FullName,MobileNo,Email,Address")] Customer customer)
        {
            customer.Status = true;
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();

                customer.UserId = "M" + DateTime.Now.Year.ToString() + customer.Id;
                customer.Password = DateTime.Now.Millisecond.ToString() + customer.Id;
                //Send Sms
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                //Send Sms To The Customer
                SendOneToOneSingleSms(customer);
                return RedirectToAction("CustomerList");
            }

            return View(customer);
        }
        private void SendOneToOneSingleSms(Customer RT)
        {

            try
            {
                var sms = new SendSms();
                string returnValue = sms.OneToOne("01676688381", "65c5f73cff",
                   RT.MobileNo, "Your Mortgage Account Has Been Created! User Id:" + RT.UserId + " Password:" + RT.Password + ".  Mortgage System, Chittagong ", "1", "", "");
            }
            catch (Exception e)
            {


            }


        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FullName,MobileNo,Email,Address,UserId,Password")] Customer customer)
        {
            customer.Status = true;
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CustomerList");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            customer.Status = false;
            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("CustomerList");
        }
        public JsonResult IsMobileNoExist(string MobileNo, int? Id)
        {
            var validateName = db.Customers.FirstOrDefault
                                (x => x.MobileNo == MobileNo && x.Id != Id && x.Status==true);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMaturityDate(double InterestRatePerMonth, double Price, double Amount)
        {



            Price = Price - Amount;
            //MortgageItem MI = db.MortgageItems.Find(Id);

            int month = Convert.ToInt32(Price / InterestRatePerMonth);
            if (Price % InterestRatePerMonth > 0)
            {
                month=month+1;
            }

            DateTime Date = DateTime.Now;

            int RealYear = Convert.ToInt32(month / 12);
            int RealMonth = month % 12;

            int YearNow = Date.Year;
            int MonthNow = Date.Month;
            int DayNow = Date.Day;

            YearNow = YearNow + RealYear;
            MonthNow =  MonthNow+RealMonth;
            if (MonthNow > 12)
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
