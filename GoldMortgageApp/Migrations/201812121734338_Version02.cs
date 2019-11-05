namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Version02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        MobileNo = c.String(nullable: false),
                        Email = c.String(),
                        Address = c.String(nullable: false),
                        UserId = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Deposites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepositeId = c.String(),
                        TransectionId = c.String(),
                        Type = c.String(),
                        Amount = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                        MortgageItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MortgageItems", t => t.MortgageItemId, cascadeDelete: true)
                .Index(t => t.MortgageItemId);
            
            CreateTable(
                "dbo.MortgageItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false),
                        MortgageItemD = c.String(nullable: false),
                        MortgageItemFile = c.String(),
                        MortgageItemPath = c.String(),
                        ItemQuantity = c.String(nullable: false),
                        Price = c.String(nullable: false),
                        Loan = c.Double(nullable: false),
                        InterestRate = c.Double(nullable: false),
                        InterestRatePerMonth = c.Double(nullable: false),
                        MaturityOfThisLoan = c.DateTime(nullable: false),
                        File = c.String(),
                        FilePath = c.String(),
                        CustomerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            AddColumn("dbo.Admins", "Customer_Id", c => c.Int());
            CreateIndex("dbo.Admins", "Customer_Id");
            AddForeignKey("dbo.Admins", "Customer_Id", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deposites", "MortgageItemId", "dbo.MortgageItems");
            DropForeignKey("dbo.MortgageItems", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Admins", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.MortgageItems", new[] { "CustomerId" });
            DropIndex("dbo.Deposites", new[] { "MortgageItemId" });
            DropIndex("dbo.Admins", new[] { "Customer_Id" });
            DropColumn("dbo.Admins", "Customer_Id");
            DropTable("dbo.MortgageItems");
            DropTable("dbo.Deposites");
            DropTable("dbo.Customers");
        }
    }
}
