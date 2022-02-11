namespace BankingServiceASM.Migrations
{
    using BankingServiceASM.Data;
    using BankingServiceASM.Entities;
    using BankingServiceASM.Helpers;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BankingServiceASM.Data.BankingDbContext>
    {
        private  BankingDbContext db = new BankingDbContext();
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BankingServiceASM.Data.BankingDbContext context)
        {
            db.Database.ExecuteSqlCommand("DELETE FROM TransactionHistories;");
            db.Database.ExecuteSqlCommand("DELETE FROM Accounts;");

            Random random = new Random();
            var accountNumber = random.Next();
            var account1 = db.Accounts.Add(new Entities.Account {
                AccountNumber = "131125555",
                AccountCode = "c219ffa0-de35-4456-b2ed-dfb46882e9a1",
                Username = "userdemo01",
                Password = Hashing.Hash("123456"),
                PinCode = random.Next(100000, 999999).ToString(),
                IdentityNumber = "037000555666",
                FullName = "User Demo 01",
                PhoneNumber = "+84357446532",
                Email = "userdemo01@gmail.com",
                Address = "Hanoi",
                Balance = 5000000,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = AccountStatus.Active,
            });
            var account2 = db.Accounts.Add(new Entities.Account
            {
                AccountNumber = "131126666",
                AccountCode = "c219ffa0-de35-4456-b2ed-dfb46882e9a2",
                Username = "userdemo02",
                Password = Hashing.Hash("123456"),
                PinCode = random.Next(100000, 999999).ToString(),
                IdentityNumber = "037000555667",
                FullName = "User Demo 02",
                PhoneNumber = "+84357446533",
                Email = "userdemo02@gmail.com",
                Address = "Hanoi",
                Balance = 5000000,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = AccountStatus.Active,
            });

            db.TransactionHistories.Add(new TransactionHistory() {
                TransactionId = Guid.NewGuid().ToString(),
                OrderCode = "ORDER123",
                SenderAccountNumber = "131125555",
                ReceiverAccountNumber = "131126666",
                Amount = 100000,
                Fees = 1000,
                Message = "Test",
                Status = Entities.TransactionHistoryStatus.Success,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });

            db.TransactionHistories.Add(new TransactionHistory()
            {
                TransactionId = Guid.NewGuid().ToString(),
                OrderCode = "ORDER456",
                SenderAccountNumber = "131125555",
                ReceiverAccountNumber = "131126666",
                Amount = 100000,
                Fees = 1000,
                Message = "Test",
                Status = Entities.TransactionHistoryStatus.Success,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });

            db.TransactionHistories.Add(new TransactionHistory()
            {
                TransactionId = Guid.NewGuid().ToString(),
                OrderCode = "ORDER789",
                SenderAccountNumber = "131125555",
                ReceiverAccountNumber = "131126666",
                Amount = 100000,
                Fees = 1000,
                Message = "Test",
                Status = Entities.TransactionHistoryStatus.Success,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            db.SaveChanges();
        }
    }
}
