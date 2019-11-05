using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class Product
    {
        public int Id { get; set; }




        [Display(Name = "Select Product Catagory")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }


        [Display(Name = "Product Id")]
        public string ProductId { get; set; }



        [Required(ErrorMessage = "Product Name Must Be Filled!")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }



        [Required(ErrorMessage = "Arrival Date Must Be Filled!")]
        [Display(Name = "Arrival Date")]
        public DateTime ArrivalDate { get; set; }






        public string ProductFile { get; set; }
        public string ProductPath { get; set; }

       
        
        [Required(ErrorMessage = "Product Weight Must Be Filled!")]
        [Display(Name = "Product Weight")]
        public string Weight { get; set; }



        [Required(ErrorMessage = "Product Price Must Be Filled!")]
        [Display(Name = "Product Price")]
        public Decimal Price { get; set; }

       
        [Display(Name = "Product Discount(%)")]
        public Decimal Discount { get; set; }

        
       

       
        [Required(ErrorMessage = "Product Description Must Be Filled!")]
        [Display(Name = "Product Description")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Product Quantity Must Be Filled!")]
        [Display(Name = "Product Quantity")]
        public int Quantity { get; set; }

        public virtual List<Order> Orders { get; set; }

    }
}