using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BankingServiceASM.Dtos
{
    [DataContract]
    public class AccountGetTransactionHistoryDto
    {
        [Required]
        [DataMember]
        public string Username { get; set; }
        [Required]
        [DataMember]
        public string Password { get; set; }
        [Required]
        [DataMember]
        public DateTime FromDate { get; set; }
        [Required]
        [DataMember]
        public DateTime ToDate { get; set; }
    }
}