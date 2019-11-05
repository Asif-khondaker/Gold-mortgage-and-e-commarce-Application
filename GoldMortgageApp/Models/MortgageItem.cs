using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoldMortgageApp.Models
{
    public class MortgageItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Issue Date Must Be Filled!")]
        [Display(Name = "Issue Date")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Mortgage Item Must Be Filled!")]
        [Display(Name = "Mortgage Item(Description)")]
        public string MortgageItemD { get; set; }

        public string MortgageItemFile { get; set; }
        public string MortgageItemPath { get; set; }

        [Required(ErrorMessage = "Mortgage Item Quantity Must Be Filled!")]
        [Display(Name = "Mortgage Item Quantity")]
        public string ItemQuantity { get; set; }

        [Required(ErrorMessage = "Mortgage Item Price Must Be Filled!")]
        [Display(Name = "Approximiate Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Enter Loan Amount!")]
        [Display(Name = "Loan Amount")]
        public double Loan { get; set; }

        [Required(ErrorMessage = "Interest Rate Must Be Filled!")]
        [Display(Name = "Interest Rate(Monthly)")]
        public double InterestRate { get; set; }

        [Display(Name = "Interest Amount(Per Month)")]
        public double InterestRatePerMonth { get; set; }

        [Required(ErrorMessage = "Date Must Be Filled!")]
        [Display(Name = "Maturity Of This Loan")]
        public DateTime MaturityOfThisLoan { get; set; }

        public string File { get; set; }
        public string FilePath { get; set; }

        
       
        public int CustomerId { get; set; }
        public bool Status { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

       

    }
}