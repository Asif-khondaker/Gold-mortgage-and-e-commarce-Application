namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ECommerce04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Quantity");
        }
    }
}
