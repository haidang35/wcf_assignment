namespace BankingServiceASM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Username", c => c.String());
            AddColumn("dbo.TransactionHistories", "Fees", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionHistories", "Fees");
            DropColumn("dbo.Accounts", "Username");
        }
    }
}
