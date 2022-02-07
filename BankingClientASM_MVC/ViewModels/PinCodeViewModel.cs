using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankingClientASM_MVC.ViewModels
{
    public class PinCodeViewModel
    {
        [Required]
        [StringLength(6,ErrorMessage = "Pin code has 6 characters")]
        public string PinCode { get; set; }
    }
}