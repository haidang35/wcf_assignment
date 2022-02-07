using BankingServiceASM.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BankingServiceASM.Entities
{
    public enum TransactionHistoryStatus
    {
        Success = 1,
        Failed = 2,
    }

    public class TransactionHistory
    {
        [Key]
        public string TransactionId { get; set; }
        public string OrderCode { get; set; }
        public string SenderAccountNumber { get; set; }
        public virtual Account Sender { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public virtual Account Receiver { get; set; }
        public double Amount { get; set; }
        public double Fees { get; set; }
        public string Message { get; set; }
        public TransactionHistoryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TransactionHistoryDto ToTransactionHistoryDto()
        {
            return new TransactionHistoryDto
            {
                TransactionId = this.TransactionId,
                OrderCode = this.OrderCode,
                SenderAccountNumber = this.SenderAccountNumber,
                ReceiverAccountNumber = this.ReceiverAccountNumber,
                Amount = this.Amount,
                Fees = this.Fees,
                Message = this.Message,
                Status = this.Status,
                CreatedAt = this.CreatedAt,
            };
        }
    }
}