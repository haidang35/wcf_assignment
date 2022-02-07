using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankingClientASM_MVC.Helpers
{
    public enum XMessageType
    {
        Success = 1,
        Failed = 2,
    }
    public class XMessage
    {
        public XMessageType Type { get; set; }
        public string Message { get; set; }
        
        public XMessage(XMessageType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        public string ShowMessageBox()
        {
           switch(this.Type)
            {
                case XMessageType.Success:
                    return $"<div class='alert alert-success'> {this.Message} <div>";
                    break;
                case XMessageType.Failed:
                    return $"<div class='alert alert-danger'> {this.Message} <div>";
                    break;
            }
            return null;
        }
    }
}