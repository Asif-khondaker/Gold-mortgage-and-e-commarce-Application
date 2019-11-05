using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class Buyer
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Name Must Be Filled")]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid  Number")]
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }



        [Display(Name = "Select Your Location")]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }


        [Required(ErrorMessage = "Buyer Address Must Be Filled!")]
        [Display(Name = "Buyer Address")]
        public string Description { get; set; }

        public bool PaymentStatus { get; set; }

        public bool IsDelivered { get; set; }

        public Decimal Payment { get; set; }

        public string PaymentId { get; set; }

        public DateTime Date { get; set; }

        public virtual List<Order> Orders { get; set; }
    }


}