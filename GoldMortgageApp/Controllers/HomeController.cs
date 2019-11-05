using GoldMortgageApp.com.onnorokomsms.api1;
using GoldMortgageApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GoldMortgageApp.Controllers
{
    public class HomeController : Controller
    {
        private MortgageSystemDBContext db = new MortgageSystemDBContext();

        public ActionResult Index()
        {
            List<Product> products = db.Products.Where(a => a.Quantity > 0).OrderByDescending(a => a.ArrivalDate).ToList();
            return View(products.ToList());
        }
        
        public ActionResult Products()
        {
            List<Category> Categories = db.Categories.ToList();
            Category C = new Category();

            C.Id = 0;
            C.Name = "--Select--";
            Categories.Add(C);
            ViewBag.CategoryId = new SelectList(Categories.OrderBy(a=>a.Id), "Id", "Name");
            List<Product> products = db.Products.Where(a => a.Quantity > 0).OrderByDescending(a => a.ArrivalDate).ToList();
            return View(products.ToList());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Products(int CategoryId)
        {

            List<Product> products = db.Products.Where(a => a.Quantity > 0 && a.CategoryId==CategoryId).OrderByDescending(a => a.ArrivalDate).ToList();
          
            List<Category> Categories = db.Categories.ToList();
            Category C = new Category();

            C.Id = 0;
            C.Name = "--Select--";
            Categories.Add(C);
            ViewBag.CategoryId = new SelectList(Categories.OrderBy(a => a.Id), "Id", "Name");
            
            return View(products.ToList());
        }
        public ActionResult CustomerInfo()
        {
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "Name");
            return View();
        }

        public ActionResult Login()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Buyer buyer)
        {
            var Customer = db.Buyers.FirstOrDefault(a => a.PaymentId == buyer.PaymentId && a.MobileNo == buyer.MobileNo);
            if (Customer == null)
            {
                ViewBag.Message = "PaymentId or MobileNo Not Match!";
                return View(buyer);
            }
            else
            {
               return RedirectToAction("OrderStatus",new {Id=Customer.Id});
            }
           
        }

        public ActionResult OrderStatus(int Id)
        {

            List<ViewCart> viewCarts = new List<ViewCart>();
            Buyer buyer = db.Buyers.Find(Id);

            List<Order> Orders = db.Orders.Where(a=>a.BuyerId==Id).ToList();
            if(Orders.Count !=0)
            {
                foreach (Order P in Orders)
                {
                    Product product = db.Products.Find(P.ProductId);
                    ViewCart VC = new ViewCart();

                    VC.Id = product.Id;
                    VC.ProductId = product.ProductId;
                    VC.Name = product.Name;
                    VC.Quantity = P.Quantity;
                    VC.ProductFile = product.ProductFile;
                    VC.Price = product.Price;
                    VC.Weight = product.Weight;
                    VC.Discount = product.Discount;

                   
                     if (((product.Quantity - P.Quantity) >= 0)|| buyer.PaymentStatus==true)
                        {
                            

                            VC.Available = "Yes";
                            decimal discount = P.Quantity * product.Price * (product.Discount / 100);
                            VC.NPrice = product.Price * P.Quantity - discount;
                        }
                        else
                        {
                            Order O = db.Orders.Find(P.Id);

                            db.Orders.Remove(O);
                            db.SaveChanges();

                            VC.Available = "No";
                            VC.NPrice = 0;
                        }
                    

                    viewCarts.Add(VC);

                }





                double TotalPayable = 0;


                foreach (ViewCart V in viewCarts)
                {
                    TotalPayable = TotalPayable + Convert.ToDouble(V.NPrice);
                }

                double ShippingCost =Convert.ToDouble(buyer.Location.Charge);
                TotalPayable = ShippingCost+TotalPayable;

              
                buyer.Payment =Convert.ToDecimal(TotalPayable);
                db.Entry(buyer).State = EntityState.Modified;
                db.SaveChanges();
                if(buyer.PaymentStatus==true)
                {
                    ViewBag.PaymentStatus = "Ok";
                }
                else
                {
                    ViewBag.PaymentStatus = "Not Ok";
                }
                ViewBag.PaymentId = buyer.PaymentId;
                ViewBag.TotalPayable = TotalPayable;
                ViewBag.ShippingCost = ShippingCost + "TK.(" + buyer.Location.Name+")";

                


            }

            return View(viewCarts.ToList());
            
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerInfo(Buyer buyer)
        {


            if (ModelState.IsValid)
            {

                buyer.Date = DateTime.Now;
                buyer.IsDelivered = false;
                buyer.PaymentStatus = false;

                db.Buyers.Add(buyer);
                db.SaveChanges();



                //Product Add
                if (Session["products"] != null)
                {

                    List<Order> orders = new List<Order>();
                    List<Product> products = (List<Product>)Session["products"];

                    foreach (Product P in products)
                    {
                        Product product = db.Products.Find(P.Id);

                        if ((product.Quantity - P.Quantity) >= 0)
                        {
                            Order ord = new Order();

                            ord.BuyerId = buyer.Id;
                            ord.ProductId = P.Id;
                            ord.Quantity = P.Quantity;

                            db.Orders.Add(ord);
                            db.SaveChanges();

                        }





                    }
                }

                //Buyer Update
                buyer.PaymentId = "GM-" + DateTime.Now.Year + buyer.Id;
                db.Entry(buyer).State = EntityState.Modified;
                db.SaveChanges();
                Session["products"] = null;
                @Session["Count"] = null;
                //Send Sms
                try
                {
                    var sms = new SendSms();
                    string returnValue = sms.OneToOne("01839507129", "88e288845f",
                    buyer.MobileNo, "To Complete Your Order Please Pay through The following Link: XYZ/OrderStatus ! Payment Id:" + buyer.PaymentId +".  Mortgage System, Chittagong ","Text", "", "");
                    ViewBag.Message = "Message has been sent to your mobile no.";
                }
                catch (Exception e)
                {


                }
                ViewBag.LocationId = new SelectList(db.Locations, "Id", "Name");
                return View();
            }
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "Name");
           
            return View(buyer);
        }

        public ActionResult ShowCart()
        {
            List<ViewCart> viewCarts = new List<ViewCart>();
            if (Session["products"] != null)
            {
                List<Product> products = (List<Product>)Session["products"];

                foreach(Product P in products)
                {
                    Product product = db.Products.Find(P.Id);
                    ViewCart VC = new ViewCart();

                    VC.Id = product.Id;
                    VC.ProductId = product.ProductId;
                    VC.Name = product.Name;
                    VC.Quantity = P.Quantity;
                    VC.ProductFile = product.ProductFile;
                    VC.Price = product.Price;
                    VC.Weight = product.Weight;
                    VC.Discount = product.Discount;

                    var vC=viewCarts.FirstOrDefault(a=>a.Id==P.Id);

                    if(vC == null)
                    {
                        if ((product.Quantity - P.Quantity) >= 0)
                        {
                            VC.Available = "Yes";
                            decimal discount = P.Quantity * product.Price * (product.Discount / 100);
                            VC.NPrice = product.Price * P.Quantity - discount;
                        }
                        else
                        {
                            VC.Available = "No";
                            VC.NPrice = 0;
                        }
                    }
                    else
                    {
                        if ((product.Quantity - P.Quantity-vC.Quantity) >= 0)
                        {
                            VC.Available = "Yes";
                            decimal discount = P.Quantity * product.Price * (product.Discount / 100);
                            VC.NPrice = product.Price * P.Quantity - discount;
                        }
                        else
                        {
                            VC.Available = "No";
                            VC.NPrice = 0;
                        }
                    }
                    

                    viewCarts.Add(VC);

                }

                
               
            }

            double TotalPayable=0;
           

            foreach(ViewCart V in viewCarts)
            {
                TotalPayable = TotalPayable +Convert.ToDouble(V.NPrice);
            }
            ViewBag.TotalPayable = TotalPayable;

            return View(viewCarts.ToList());
        }
        public ActionResult DeleteFromCart(int Id)
        {
            
                List<Product> products = (List<Product>)Session["products"];
                List<Product> productsnew = new List<Product>();

            foreach(Product P in products)
            {
                if(P.Id!=Id)
                {
                    productsnew.Add(P);

                }
                else
                {
                    Session["Count"] = Convert.ToInt32(Session["Count"]) - 1;
                }
            }

            Session["products"] = productsnew;

            return RedirectToAction("ShowCart");
        }

        public ActionResult Details(int Id)
        {
            Product product = db.Products.Find(Id);
            ViewBag.Quantity = product.Quantity;
            product.Quantity = 1;
            
            return View(product);
        }
        public ActionResult ChangeQuantity(int Id)
        {
            Product product = db.Products.Find(Id);
            ViewBag.Quantity = product.Quantity;
            product.Quantity = 1;

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeQuantity(int Id, int Quantity)
        {

            //Product Add

            if (Session["products"] != null)
            {
                List<Product> products = (List<Product>)Session["products"];

              
                    for (int i = 0; i <= products.Count - 1; i++)
                    {
                        if (products[i].Id == Id)
                        {
                            products[i].Quantity =  Quantity;
                        }
                    }
                
            


                Session["products"] = products;
            }

        else
            {
                List<Product> products = new List<Product>();

                Product p = new Product();
                p.Id = Id;
                p.Quantity = Quantity;

                products.Add(p);
                Session["products"] = products;


            }
            
          
            



            return RedirectToAction("ShowCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCart(int Id, int Quantity)
        {

            //Product Add

            if(Session["products"]!=null)
            {
                List<Product> products =(List<Product>) Session["products"];

                var proSearch = products.FirstOrDefault(a => a.Id == Id);

                if(proSearch !=null)
                {
                        for( int i=0 ;i<=products.Count-1;i++ )
                        {
                            if(products[i].Id==Id)
                            {
                                products[i].Quantity = products[i].Quantity + Quantity;
                                Session["Count"] = Convert.ToInt32(Session["Count"]) - 1;

                            }
                        }
                }
                else
                {
                    Product p = new Product();
                    p.Id = Id;
                    p.Quantity = Quantity;

                    products.Add(p);
                }


                
                Session["products"] = products;
            }
            else
            {
                List<Product> products = new List<Product>();

                Product p = new Product();
                p.Id = Id;
                p.Quantity = Quantity;

                products.Add(p);
                Session["products"] = products;


            }
            
            //cart Number

            if (Session["Count"]!=null)
            {
                Session["Count"] = Convert.ToInt32(Session["Count"]) + 1;
            }
            else
            {
                Session["Count"] = 1;
            }




            return RedirectToAction("Details", new { Id = Id });
        }
        public ActionResult UserLogin()
        {
            return View();
        }
        public ActionResult PersonalInfo()
        {
            if (Session["Customer"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            int id = Convert.ToInt32(Session["Customer"]);

           
            Customer customer = db.Customers.Find(id);
           string mobileOrUserId = customer.MobileNo;
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
                    if (Dp.Date >= DT)
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
                ViewBag.HaveToPay = amount + Interest;
                ViewBag.Paid = Deposite;


                ViewBag.Id = MortgageId;
                ViewBag.VewFinance = VFS.ToList();
               return View(Customers.ToList());         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(string UserId, string Password)
        {
            var Customer = db.Customers.FirstOrDefault(a => a.UserId == UserId && a.Password == Password);
            if(Customer==null)
            {
                ViewBag.Message = "Password or UserId Not Match!";
            }
            else
            {
                Session["Admin"] = null;
                Session["Id"] = null;
                Session["Customer"] = null;
                Session["Name"] = null;

                Session["Customer"] = Customer.Id;
                Session["Name"] = Customer.FullName;


                Session["Count"] = null;
                Session["products"] = null;

            }
           return RedirectToAction("Index", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult DepositeDetails()
        {
            if(Session["Customer"]==null)
            {
                return RedirectToAction("Index");
            }

            int Id =(int)Session["Customer"];

            List<MortgageItem> MIS = db.MortgageItems.Where(a => a.Id == Id).ToList();
            List<Deposite> deposites = new List<Deposite>(); 

            if(MIS.Count>0)
            {
                int Mid = MIS[MIS.Count - 1].Id;
                DateTime  Date=MIS[MIS.Count - 1].IssueDate;
                deposites = db.Deposites.Include(d => d.Customer).Where(a => a.Type != "Discount").OrderByDescending(a => a.Date).Where(a=>a.CustomerId==Id).Where(a=>a.Date>=Date).ToList();
            }

            
            return View(deposites.ToList());
        }


        public ActionResult MortgageDetails()
        {
            if (Session["Customer"] == null)
            {
                return RedirectToAction("Index");
            }

            int Id = (int)Session["Customer"];

            List<MortgageItem> MIS = db.MortgageItems.Where(a => a.CustomerId == Id).ToList();
            MortgageItem M = new MortgageItem();

            if (MIS.Count > 0)
            {
              
               M = MIS[MIS.Count - 1];
               
            }


            return View(M);
        }

    }
}