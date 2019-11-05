using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Must Be Filled")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Delivary Charge Must Be Filled!")]
        [Display(Name = "Delivary Charge")]
        public Decimal Charge { get; set; }

        public virtual List<Buyer> Buyers { get; set; }
    }
}