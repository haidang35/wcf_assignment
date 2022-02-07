using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BankingServiceASM.Services
{
    public class TwilioSMSService
    {
        private string accountSid = System.Configuration.ConfigurationManager.AppSettings["TWILIO_ACCOUNT_SID"];
        private string authToken = System.Configuration.ConfigurationManager.AppSettings["TWILIO_AUTH_TOKEN"];
        private string fromPhone = System.Configuration.ConfigurationManager.AppSettings["TWILIO_FROM_PHONE"];
        public void SendSMS(string toPhoneNumber, string body)
        {
            TwilioClient.Init(accountSid, authToken);
            var message = MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber(fromPhone),
                to: new Twilio.Types.PhoneNumber(toPhoneNumber)
            );

            Debug.WriteLine(message.Sid);
        }
    }
}