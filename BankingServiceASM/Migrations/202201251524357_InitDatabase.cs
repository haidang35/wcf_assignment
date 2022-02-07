namespace BankingServiceASM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        AccountNumber = c.String(nullable: false, maxLength: 128),
                        AccountCode = c.String(),
                        Password = c.String(),
                        PinCode = c.String(),
                        FullName = c.String(),
                        IdentityNumber = c.String(),
                        Status = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        Balance = c.Double(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AccountNumber);
            
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                    {
                        TransactionId = c.String(nullable: false, maxLength: 128),
                        OrderCode = c.String(),
                        SenderAccountNumber = c.String(maxLength: 128),
                        ReceiverAccountNumber = c.String(maxLength: 128),
                        Amount = c.Double(nullable: false),
                        Message = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Accounts", t => t.ReceiverAccountNumber)
                .ForeignKey("dbo.Accounts", t => t.SenderAccountNumber)
                .Index(t => t.SenderAccountNumber)
                .Index(t => t.ReceiverAccountNumber);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionHistories", "SenderAccountNumber", "dbo.Accounts");
            DropForeignKey("dbo.TransactionHistories", "ReceiverAccountNumber", "dbo.Accounts");
            DropIndex("dbo.TransactionHistories", new[] { "ReceiverAccountNumber" });
            DropIndex("dbo.TransactionHistories", new[] { "SenderAccountNumber" });
            DropTable("dbo.TransactionHistories");
            DropTable("dbo.Accounts");
        }
    }
}
