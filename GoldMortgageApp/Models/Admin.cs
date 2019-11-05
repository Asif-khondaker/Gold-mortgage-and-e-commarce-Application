using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoldMortgageApp.Models
{
    public class Admin
    {
        public int Id { get; set; }

       
        [Required(ErrorMessage = "UserName Must Be Filled")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password Must Be Filled")]

        public string Password { get; set; }
        [Required(ErrorMessage = "Name Must Be Filled")]
        public string Name { get; set; }


    }
}