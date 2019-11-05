namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ECommerce03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Buyers", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Buyers", "Date");
        }
    }
}
