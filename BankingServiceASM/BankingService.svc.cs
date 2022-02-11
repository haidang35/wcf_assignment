using BankingServiceASM.Data;
using BankingServiceASM.Dtos;
using BankingServiceASM.Entities;
using BankingServiceASM.Helpers;
using BankingServiceASM.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BankingServiceASM
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BankingService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BankingService.svc or BankingService.svc.cs at the Solution Explorer and start debugging.
    public class BankingService : IBankingService
    {
        private BankingDbContext db = new BankingDbContext();
        private TwilioSMSService twilioService = new TwilioSMSService();
        public TransactionHistoryDto TransferPaymentOrder(AccountTransactionDto accountTransaction)
        {
            var receiverAccount = db.Accounts.Where(a => a.AccountCode == accountTransaction.ReceiverAccountCode).FirstOrDefault();
            if (receiverAccount != null)
            {
                if (Hashing.CheckHashing(accountTransaction.Password, receiverAccount.Password))
                {
                    var senderAccount = db.Accounts.Where(a => a.AccountNumber == accountTransaction.SenderAccountNumber).FirstOrDefault();
                    if (senderAccount != null)
                    {
                        if (accountTransaction.SenderPinCode == senderAccount.PinCode && accountTransaction.Amount <= senderAccount.Balance)
                        {
                            using (DbContextTransaction transaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    Debug.WriteLine(accountTransaction.AccountPayFees);
                                    switch (accountTransaction.AccountPayFees)
                                    {
                                        case AccountPayFee.ReceiverAccount:
                                            receiverAccount.Balance += accountTransaction.Amount - getFees(accountTransaction.Amount);
                                            senderAccount.Balance -= accountTransaction.Amount;
                                            break;
                                        case AccountPayFee.SenderAccount:
                                            senderAccount.Balance -= accountTransaction.Amount + getFees(accountTransaction.Amount);
                                            receiverAccount.Balance += accountTransaction.Amount;
                                            break;
                                    }
                                    db.Entry(receiverAccount).State = EntityState.Modified;
                                    db.Entry(senderAccount).State = EntityState.Modified;
                                    var transactionHistory = new Entities.TransactionHistory
                                    {
                                        TransactionId = Guid.NewGuid().ToString(),
                                        OrderCode = accountTransaction.OrderCode,
                                        SenderAccountNumber = senderAccount.AccountNumber,
                                        ReceiverAccountNumber = receiverAccount.AccountNumber,
                                        Amount = accountTransaction.Amount,
                                        Fees = getFees(accountTransaction.Amount),
                                        Message = accountTransaction.Message,
                                        Status = Entities.TransactionHistoryStatus.Success,
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                    };
                                    db.TransactionHistories.Add(transactionHistory);
                                    db.SaveChanges();
                                    transaction.Commit();
                                    return transactionHistory.ToTransactionHistoryDto();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                }
                            }
                        }
                    }

                }
            }
            return null;
        }

        public double getFees(double amount)
        {
            double fees = 0;
            if (amount <= 100000 && amount > 0)
            {
                fees = 1000;
            }
            else if (amount > 100000 && amount <= 500000)
            {
                fees = amount * 0.02;
            }
            else if (amount > 500000 && amount <= 1000000)
            {
                fees = amount * 0.015;
            }
            else if (amount > 1000000 && amount <= 5000000)
            {
                fees = amount * 0.01;
            }
            else if (amount > 5000000)
            {
                fees = amount * 0.005;
            }
            return fees;
        }

        public ICollection<TransactionHistoryDto> ShowTransactionHistory(AccountGetTransactionHistoryDto acc)
        {
            List<TransactionHistoryDto> transactionHistoryList = new List<TransactionHistoryDto>();
            if (CheckExistAccount(acc.Username, acc.Password))
            {
                var account = db.Accounts.Where(a => a.Username == acc.Username).FirstOrDefault();
                var transactionHistories = db.TransactionHistories.
                    Where(t => t.ReceiverAccountNumber == account.AccountNumber || t.SenderAccountNumber == account.AccountNumber)
                    .Where(t => t.CreatedAt >= acc.FromDate && t.CreatedAt <= acc.ToDate)
                    .ToList();
                foreach (var transaction in transactionHistories)
                {
                    transactionHistoryList.Add(transaction.ToTransactionHistoryDto());
                }

            }
            return transactionHistoryList;
        }

        public AccountDto ShowAccountInformation(AccountLoginDto account)
        {
            var acc = db.Accounts.Where(a => a.Username == account.Username).FirstOrDefault();
            if (acc != null)
            {
                if (Hashing.CheckHashing(account.Password, acc.Password))
                {
                    return acc.ToAccountDto();
                }
            }
            return null;
        }

        public bool CheckExistAccount(string username, string password)
        {
            var account = db.Accounts.Where(a => a.Username == username).FirstOrDefault();
            if (account != null)
            {
                return Hashing.CheckHashing(password, account.Password);
            }
            return false;
        }

        public bool SendConfirmationOTP(AccountLoginDto account)
        {
            if (CheckExistAccount(account.Username, account.Password))
            {
                var acc = db.Accounts.Where(a => a.Username == account.Username).FirstOrDefault();
                try
                {
                    Random random = new Random();
                    string pinCode = random.Next(100000, 999999).ToString();
                    acc.PinCode = pinCode;
                    db.Entry(acc).State = EntityState.Modified;
                    db.SaveChanges();
                    string message = $"Banking Service notice: Your confirmation pincode is {pinCode}";
                    twilioService.SendSMS(acc.PhoneNumber, message);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return false;
        }

        public AccountInfoTransferDto CheckAccountInfoTransfer(string accountNumber)
        {
            var account = db.Accounts.Where(a => a.AccountNumber == accountNumber).FirstOrDefault();
            if (account != null)
            {
                return new AccountInfoTransferDto()
                {
                    AccountNumber = account.AccountNumber,
                    FullName = account.FullName,
                };
            }
            return null;
        }

        public TransactionHistoryDto TransferNormal(TransferNormalInfoDto transferInfo)
        {
            var receiverAccount = db.Accounts.Where(a => a.AccountNumber == transferInfo.ReceiverAccountNumber).FirstOrDefault();
            if (receiverAccount != null)
            {

                var senderAccount = db.Accounts.Where(a => a.AccountNumber == transferInfo.SenderAccountNumber).FirstOrDefault();
                if (senderAccount != null)
                {
                    if (transferInfo.SenderPinCode == senderAccount.PinCode && transferInfo.Amount <= senderAccount.Balance)
                    {
                        using (DbContextTransaction transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                switch (transferInfo.AccountPayFees)
                                {
                                    case AccountPayFee.ReceiverAccount:
                                        receiverAccount.Balance += transferInfo.Amount - getFees(transferInfo.Amount);
                                        senderAccount.Balance -= transferInfo.Amount;
                                        break;
                                    case AccountPayFee.SenderAccount:
                                        senderAccount.Balance -= transferInfo.Amount + getFees(transferInfo.Amount);
                                        receiverAccount.Balance += transferInfo.Amount;
                                        break;
                                }
                                db.Entry(receiverAccount).State = EntityState.Modified;
                                db.Entry(senderAccount).State = EntityState.Modified;
                                var transactionHistory = new Entities.TransactionHistory
                                {
                                    TransactionId = Guid.NewGuid().ToString(),
                                    SenderAccountNumber = senderAccount.AccountNumber,
                                    ReceiverAccountNumber = receiverAccount.AccountNumber,
                                    Amount = transferInfo.Amount,
                                    Fees = getFees(transferInfo.Amount),
                                    Message = transferInfo.Message,
                                    Status = Entities.TransactionHistoryStatus.Success,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                };
                                db.TransactionHistories.Add(transactionHistory);
                                db.SaveChanges();
                                transaction.Commit();
                                return transactionHistory.ToTransactionHistoryDto();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                            }
                        }
                    }

                }
            }
            return null;
        }
    }
}
