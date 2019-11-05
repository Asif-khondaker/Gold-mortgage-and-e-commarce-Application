namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrectionMortgagePrice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MortgageItems", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MortgageItems", "Price", c => c.String(nullable: false));
        }
    }
}
