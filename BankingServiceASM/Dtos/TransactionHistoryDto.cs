using BankingServiceASM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BankingServiceASM.Dtos
{
    [DataContract]
    public class TransactionHistoryDto
    {
        [DataMember]
        public string TransactionId { get; set; }
        [DataMember]
        public string OrderCode { get; set; }
        [DataMember]
        public string SenderAccountNumber { get; set; }
        [DataMember]
        public virtual Account Sender { get; set; }
        [DataMember]
        public string ReceiverAccountNumber { get; set; }
        [DataMember]
        public virtual Account Receiver { get; set; }
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public double Fees { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public TransactionHistoryStatus Status { get; set; }
        [DataMember]
        public DateTime CreatedAt { get; set; }
    }
}