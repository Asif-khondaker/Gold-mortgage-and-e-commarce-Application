using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoldMortgageApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Must Be Filled")]
        public string FullName { get; set; }

        [Remote("IsMobileNoExist", "Customers", AdditionalFields = "Id",
               ErrorMessage = "MobileNo already exists")]
        [Required(ErrorMessage = "Mobile Number Must Be Filled")]
        public string MobileNo { get; set; }

        [Display(Name="Email(Not Required)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Adress Must Be Filled!")]
        public string Address { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public bool Status { get; set; }

        public virtual List<MortgageItem> MortgageItems { get; set; }

        public virtual List<Deposite> Deposites { get; set; }




    }
}