using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BankingServiceASM.Dtos
{
    [DataContract]
    public class TransferNormalInfoDto
    {
        [Required]
        [DataMember]
        public string ReceiverAccountNumber { get; set; }
        [Required]
        [DataMember]
        public string SenderAccountNumber { get; set; }
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