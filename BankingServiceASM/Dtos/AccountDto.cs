using BankingServiceASM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BankingServiceASM.Dtos
{
    [DataContract]
    public class AccountDto
    {
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string AccountCode { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string IdentityNumber { get; set; }
        [DataMember]
        public AccountStatus Status { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public double Balance { get; set; }
        [DataMember]
        public string PinCode { get; set; }
        [DataMember]
        public virtual ICollection<TransactionHistory> ReceiveTransactionHistories { get; set; }
        [DataMember]
        public virtual ICollection<TransactionHistory> TransferTransactionHistories { get; set; }
    }
}