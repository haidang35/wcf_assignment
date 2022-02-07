using BankingServiceASM.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankingServiceASM.Entities
{
    public enum AccountStatus
    {
        Active = 1,
        Deactive = 0,
        Pending = 2,
    }
    public class Account
    {
        [Key]
        public string AccountNumber { get; set; }
        public string AccountCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PinCode { get; set; }
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public AccountStatus Status { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public double Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<TransactionHistory> ReceiveTransactionHistories { get; set; }
        public virtual ICollection<TransactionHistory> TransferTransactionHistories { get; set; }

        public AccountDto ToAccountDto()
        {
            return new AccountDto
            {
                AccountNumber = this.AccountNumber,
                AccountCode = this.AccountCode,
                FullName = this.FullName,
                IdentityNumber = this.IdentityNumber,
                Status = this.Status,
                PhoneNumber = this.PhoneNumber,
                Email = this.Email,
                Address = this.Address,
                Balance = this.Balance,
                PinCode = this.PinCode,
            };
        }
    }
}