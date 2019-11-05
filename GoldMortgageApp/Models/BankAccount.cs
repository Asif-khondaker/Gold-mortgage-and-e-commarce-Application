using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class BankAccount
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Card Number Must Be Filled!")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Security Code Must Be Filled!")]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }

        [Required(ErrorMessage = "Expiry Date Must Be Filled!")]
        [Display(Name = "Expire Date")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "Card Holdetr Name Must Be Filled!")]
        [Display(Name = "Card Holder Name")]
        public string CardHolderName { get; set; }

        public double Amount { get; set; }
        public virtual List<Deposite> Deposites { get; set; }
    }
}