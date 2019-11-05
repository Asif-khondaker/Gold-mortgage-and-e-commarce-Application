using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class ViewCart
    {
        public int Id { get; set; }

        [Display(Name = "Product Id")]
        public string ProductId { get; set; }

        [Display(Name = "Product Name")]
        public string Name { get; set; }
        
        
        [Display(Name = "Product Image")]
        public string ProductFile { get; set; }
       
        
        [Display(Name = "Product Weight")]
        public string Weight { get; set; }

       
        [Display(Name = "Product Quantity")]
        public int Quantity { get; set; }

        
        [Display(Name = "Old Price")]
        public Decimal Price { get; set; }


        [Display(Name = "Product Discount(%)")]
        public Decimal Discount { get; set; }

        [Display(Name = "Payable")]
        public Decimal NPrice { get; set; }


        [Display(Name = "Product Available?")]
        public string Available { get; set; }


       
    }
}