using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class Deposite
    {
        public int Id { get; set; }


      
        public string DepositeId { get; set; }

        public string TransectionId { get; set; }

        public string Type { get; set; }



        [Required(ErrorMessage = "Amount Must Be Filled!")]
        
        public double Amount { get; set; }

        [Required(ErrorMessage = "Date Must Be Filled!")]
        [Display(Name = "Date of Amount Add")]
        public DateTime Date { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

    }
}