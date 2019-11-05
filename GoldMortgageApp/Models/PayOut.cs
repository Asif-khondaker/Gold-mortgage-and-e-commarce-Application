using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class PayOut
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Previous Due Must Be Filled!")]
        [Display(Name="Previous Due(Loan+Interest)")]
        public double PreviousLone { get; set; }
         [Required(ErrorMessage = "Payout Amount Must Be Filled!")]
        [Display(Name = "Payout Amount")]
        public double Payout { get; set; }

         
         
         public double Discount { get; set; }

        
        [Required(ErrorMessage = "Current Due Must Be Filled!")]
        [Display(Name = "Current Due")]
        public double Due { get; set; }



         [Required(ErrorMessage = "Interest Rate Must Be Filled!")]
         [Display(Name = "Interest Rate(Monthly)")]
         public double InterestRate { get; set; }

         [Display(Name = "Interest Amount(Per Month)")]
         public double InterestRatePerMonth { get; set; }





        [Required(ErrorMessage = "Maturity Of This Loan Must Be Filled!")]
        [Display(Name = "Maturity Of This Loan")]

        public DateTime  MaturityOfThisLoan { get; set; }
        [Display(Name = "Mortgage Item(Description)")]
        public string MortgageItemD { get; set; }
         [Required(ErrorMessage = "Item Quantity Must Be Filled!")]
         [Display(Name = "Item Quantity")]






        


        public string ItemQuantity { get; set; }
         [Required(ErrorMessage = "Mortgage Item Price Must Be Filled!")]
         [Display(Name = "Approximiate Price")]
         public double Price { get; set; }

    }
}