namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ECommerce02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Buyers", "IsDelivered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Buyers", "IsDelivered");
        }
    }
}
