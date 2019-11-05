using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GoldMortgageApp.Models
{
    public class MortgageSystemDBContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Deposite> Deposites { get; set; }
        public DbSet<MortgageItem> MortgageItems { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Withdraw> Withdraws { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Buyer> Buyers { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Order> Orders { get; set; }

        

    }
}