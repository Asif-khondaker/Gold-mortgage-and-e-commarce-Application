using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class ViewFinace
    {
        public int Serial { get; set; }

        public string Description { get; set; }

        public double Loan { get; set; }

        public double Interest { get; set; }

        public double Deposite { get; set; }

        public double Due { get; set; }
    }
}