namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Version04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Status");
        }
    }
}
