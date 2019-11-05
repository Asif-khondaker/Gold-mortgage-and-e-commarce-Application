using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class Withdraw
    {
        public int Id { get; set; }



        public string DepositeId { get; set; }

        public string TransectionId { get; set; }




      

        public double Amount { get; set; }

     
        public DateTime Date { get; set; }

        public int BankAccountId { get; set; }

        [ForeignKey("BankAccountId")]
        public virtual BankAccount BankAccount { get; set; }

    }
}