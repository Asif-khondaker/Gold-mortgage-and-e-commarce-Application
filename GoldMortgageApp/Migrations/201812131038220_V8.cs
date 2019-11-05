namespace GoldMortgageApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Withdraws",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepositeId = c.String(),
                        TransectionId = c.String(),
                        Amount = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                        BankAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId, cascadeDelete: true)
                .Index(t => t.BankAccountId);
            
            AddColumn("dbo.Deposites", "BankAccount_Id", c => c.Int());
            CreateIndex("dbo.Deposites", "BankAccount_Id");
            AddForeignKey("dbo.Deposites", "BankAccount_Id", "dbo.BankAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Withdraws", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.Deposites", "BankAccount_Id", "dbo.BankAccounts");
            DropIndex("dbo.Withdraws", new[] { "BankAccountId" });
            DropIndex("dbo.Deposites", new[] { "BankAccount_Id" });
            DropColumn("dbo.Deposites", "BankAccount_Id");
            DropTable("dbo.Withdraws");
        }
    }
}
