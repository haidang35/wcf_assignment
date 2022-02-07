using BankingServiceASM.Data;
using BankingServiceASM.Dtos;
using BankingServiceASM.Entities;
using BankingServiceASM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BankingServiceASM
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BankingAuthService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BankingAuthService.svc or BankingAuthService.svc.cs at the Solution Explorer and start debugging.
    public class BankingAuthService : IBankingAuthService
    {
        private BankingDbContext db = new BankingDbContext();
        public AccountDto RegisterAccount(AccountRegisterDto account)
        {
            var accountExisted = db.Accounts.Where(a => a.Username == account.Username).FirstOrDefault();
            if (accountExisted == null)
            {
                Random random = new Random();
                var accountNumber = random.Next();
                var newAccount = new Account
                {
                    AccountNumber = accountNumber.ToString(),
                    AccountCode = Guid.NewGuid().ToString(),
                    Username = account.Username,
                    Password = Hashing.Hash(account.Password),
                    PinCode = random.Next(100000, 999999).ToString(),
                    IdentityNumber = account.IdentityNumber,
                    FullName = account.FullName,
                    PhoneNumber = account.PhoneNumber,
                    Email = account.Email,
                    Address = account.Address,
                    Balance = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = AccountStatus.Active,
                };
                db.Accounts.Add(newAccount);
                db.SaveChanges();
                return newAccount.ToAccountDto();
            }
            return null;
        }
    }
}
