using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Must Be Filled")]
        public string Name { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}