using BankingServiceASM.Data;
using BankingServiceASM.Dtos;
using BankingServiceASM.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;

namespace BankingServiceASM.App_Code.Authentication
{
    public class CustomValidator : UserNamePasswordValidator
    {
        private BankingDbContext db = new BankingDbContext();

        public override void Validate(string userName, string password)
        {
           if(Login(new AccountLoginDto { Username = userName, Password = password }))
            {
                return;
            }
            throw new SecurityTokenException("Account Invalid");
        }

        public bool Login(AccountLoginDto account)
        {
            var acc = db.Accounts.Where(a => a.Username == account.Username).FirstOrDefault();
            if (acc != null)
            {
                return Hashing.CheckHashing(account.Password, acc.Password);
            }
            return false;
        }
    }
}