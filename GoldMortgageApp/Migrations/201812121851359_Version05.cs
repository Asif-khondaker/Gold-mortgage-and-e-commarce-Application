namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Version05 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Admins", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.Admins", new[] { "Customer_Id" });
            DropColumn("dbo.Admins", "Customer_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Admins", "Customer_Id", c => c.Int());
            CreateIndex("dbo.Admins", "Customer_Id");
            AddForeignKey("dbo.Admins", "Customer_Id", "dbo.Customers", "Id");
        }
    }
}
