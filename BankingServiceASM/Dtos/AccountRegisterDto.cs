using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BankingServiceASM.Dtos
{
    [DataContract]
    public class AccountRegisterDto
    {
        [Required]
        [DataMember]
        public string Username { get; set; }
        [Required]
        [DataMember]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [DataMember]
        public string ConfirmationPassword { get; set; }
        [Required]
        [DataMember]
        public string FullName { get; set; }
        [Required]
        [DataMember]
        public string IdentityNumber { get; set; }
        [Required]
        [DataMember]
        public string PhoneNumber { get; set; }
        [Required]
        [DataMember]
        public string Email { get; set; }
        [Required]
        [DataMember]
        public string Address { get; set; }
    }
}