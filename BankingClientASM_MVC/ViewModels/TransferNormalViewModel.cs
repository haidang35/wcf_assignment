using BankingClientASM_MVC.BankingService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankingClientASM_MVC.ViewModels
{
    public class TransferNormalViewModel
    {
        [Required]
        [Display(Name = "Receiver Account Number")]
        public string ReceiverAccountNumber { get; set; }
        [Required]
        [Display(Name = "Choose Account Pay Fees")]
        public AccountPayFee AccountPayFees { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Message { get; set; }
    }
}