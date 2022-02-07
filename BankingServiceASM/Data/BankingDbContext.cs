using BankingServiceASM.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankingServiceASM.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext() : base("name=ConnectionString") { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionHistory>()
                        .HasRequired(t => t.Sender)
                        .WithMany(a => a.TransferTransactionHistories)
                        .HasForeignKey(t => t.SenderAccountNumber)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionHistory>()
                        .HasRequired(t => t.Receiver)
                        .WithMany(a => a.ReceiveTransactionHistories)
                        .HasForeignKey(t => t.ReceiverAccountNumber)
                        .WillCascadeOnDelete(false);
        }
    }
}