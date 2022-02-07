using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BankingServiceASM.Dtos
{
    public enum AccountPayFee
    {
        ReceiverAccount = 1,
        SenderAccount = 2,
    }

    [DataContract]
    public class AccountTransactionDto
    {
        [Required]
        [DataMember]
        public string ReceiverAccountCode { get; set; }
        [Required]
        [DataMember]
        public string Password { get; set; }
        [Required]
        [DataMember]
        public string SenderAccountNumber { get; set; }
        [Required]
        [DataMember]
        public string OrderCode { get; set; }
        [Required]
        [DataMember]
        public string SenderPinCode { get; set; }
        [Required]
        [DataMember]
        public AccountPayFee AccountPayFees { get; set; }
        [Required]
        [DataMember]
        public double Amount { get; set; }
        [Required]
        [DataMember]
        public string Message { get; set; }

    }
}