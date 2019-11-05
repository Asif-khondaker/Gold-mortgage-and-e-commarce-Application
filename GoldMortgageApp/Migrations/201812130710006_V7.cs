namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Deposites", "MortgageItemId", "dbo.MortgageItems");
            DropIndex("dbo.Deposites", new[] { "MortgageItemId" });
            AddColumn("dbo.Deposites", "CustomerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Deposites", "CustomerId");
            AddForeignKey("dbo.Deposites", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
            DropColumn("dbo.Deposites", "MortgageItemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Deposites", "MortgageItemId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Deposites", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Deposites", new[] { "CustomerId" });
            DropColumn("dbo.Deposites", "CustomerId");
            CreateIndex("dbo.Deposites", "MortgageItemId");
            AddForeignKey("dbo.Deposites", "MortgageItemId", "dbo.MortgageItems", "Id", cascadeDelete: true);
        }
    }
}
