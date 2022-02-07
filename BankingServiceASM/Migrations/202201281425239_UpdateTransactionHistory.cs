namespace BankingServiceASM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionHistory : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TransactionHistories", new[] { "SenderAccountNumber" });
            DropIndex("dbo.TransactionHistories", new[] { "ReceiverAccountNumber" });
            AlterColumn("dbo.TransactionHistories", "SenderAccountNumber", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.TransactionHistories", "ReceiverAccountNumber", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.TransactionHistories", "SenderAccountNumber");
            CreateIndex("dbo.TransactionHistories", "ReceiverAccountNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TransactionHistories", new[] { "ReceiverAccountNumber" });
            DropIndex("dbo.TransactionHistories", new[] { "SenderAccountNumber" });
            AlterColumn("dbo.TransactionHistories", "ReceiverAccountNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.TransactionHistories", "SenderAccountNumber", c => c.String(maxLength: 128));
            CreateIndex("dbo.TransactionHistories", "ReceiverAccountNumber");
            CreateIndex("dbo.TransactionHistories", "SenderAccountNumber");
        }
    }
}
