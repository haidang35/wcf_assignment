using BankingClientASM_MVC.Helpers;
using BankingClientASM_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BankingClientASM_MVC.Controllers
{
    public class BankingController : Controller
    {
        private BankingService.BankingServiceClient bankingClient = new BankingService.BankingServiceClient();
        private BankingAuthService.BankingAuthServiceClient bankingAuthClient = new BankingAuthService.BankingAuthServiceClient();

        [Authorize]
        public ActionResult Dashboard()
        {
            var account = Session["accountCurrent"];
            return View(account);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Transfer()
        {
            try
            {
                var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                if(accountCurrent != null && isValidAuthBankingService())
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToAction("Login");
        }

      
        [HttpPost]
        [Authorize]
        public ActionResult Transfer(TransferViewModel transferInfo)
        {
            if (ModelState.IsValid)
            {
                if(isValidAuthBankingService())
                {
                    try
                    {
                        var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                        if (accountCurrent != null)
                        {
                            var transfer = new BankingService.AccountTransactionDto()
                            {
                                ReceiverAccountCode = "d092dab2-f981-42f8-9bdb-d4f777301b4c",
                                Password = "123456",
                                SenderAccountNumber = accountCurrent.AccountNumber,
                                OrderCode = transferInfo.OrderCode,
                                SenderPinCode = "",
                                AccountPayFees = transferInfo.AccountPayFees,
                                Amount = transferInfo.Amount,
                                Message = transferInfo.Message,
                            };
                            TempData["lastTransferInfo"] = transfer;
                            var accountLoginInfo = Session["accountLoginInfo"] as AccountLoginViewModel;
                            if (accountLoginInfo != null)
                            {
                                bankingClient.SendConfirmationPinCode(new BankingService.AccountLoginDto { 
                                    Username = accountLoginInfo.Username,
                                    Password = accountLoginInfo.Password
                                });
                                return RedirectToAction("ConfirmationPinCode");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
            return View(transferInfo);
        }
        
        [HttpGet]
        public ActionResult ConfirmationPinCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmationPinCode(PinCodeViewModel pin)
        {
           if(ModelState.IsValid)
            {
                var transferInfo = TempData["lastTransferInfo"] as BankingService.AccountTransactionDto;
                if(transferInfo != null && isValidAuthBankingService())
                {
                    transferInfo.SenderPinCode = pin.PinCode;
                    var transferResult = bankingClient.RequireTransaction(transferInfo);
                    if(transferResult != null)
                    {
                        return View("TransferSuccess", transferResult);
                    }
                }
            }
            return View(pin);
        }


        [HttpGet]
        [Authorize]
        public ActionResult TransactionHistory(DateTime? fromDateParam, DateTime? toDateParam)
        {
            DateTime fromDate = fromDateParam ?? DateTime.Today;
            DateTime toDate = toDateParam ?? DateTime.Now.AddDays(1);
            try
            {
                var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                var accountLoginInfo = Session["accountLoginInfo"] as AccountLoginViewModel;
                if(accountCurrent != null && isValidAuthBankingService() && accountLoginInfo != null)
                {
                    var accountTransaction = new BankingService.AccountGetTransactionHistoryDto
                    {
                        Username = accountLoginInfo.Username,
                        Password = accountLoginInfo.Password,
                        FromDate = fromDate,
                        ToDate = toDate,
                    };
                    Debug.WriteLine("Hello world");
                    var transactionHistory =  bankingClient.ShowTransactionHistory(accountTransaction);
                    return View(transactionHistory);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(AccountLoginViewModel account, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bankingClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                    bankingClient.ClientCredentials.UserName.UserName = account.Username;
                    bankingClient.ClientCredentials.UserName.Password = account.Password;
                    var accountLogin = bankingClient.ShowAccountInformation(new BankingService.AccountLoginDto {
                       Username = account.Username,
                       Password = account.Password
                    });


                    if (accountLogin != null)
                    {
                        Session.Add("accountLoginInfo", account);
                        int timeout = 2;
                        var ticket = new FormsAuthenticationTicket(account.Username, true, timeout);
                        string encrypt = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypt);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session.Add("accountCurrent", accountLogin);
                        if(Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        return RedirectToAction("Dashboard", "Banking");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return View(account);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(AccountRegisterViewModal newAccount)
        {
            if(ModelState.IsValid)
            {
                var account = bankingAuthClient.RegisterAccount(new BankingAuthService.AccountRegisterDto()
                {
                    Username = newAccount.Username,
                    Password = newAccount.Password,
                    FullName = newAccount.FullName,
                    Email = newAccount.Email,
                    PhoneNumber = newAccount.PhoneNumber,
                    IdentityNumber = newAccount.IdentityNumber,
                    Address = newAccount.Address,
                }    
                );
                if (account != null)
                {
                    TempData["Message"] = new XMessage(XMessageType.Success, "Register new account successfull");
                }else
                {
                    TempData["Message"] = new XMessage(XMessageType.Failed, "Register new accoount failed !");
                }
            }
            return View(newAccount);
        }

        public bool isValidAuthBankingService()
        {
            try
            {
                var accountLoginInfo = Session["accountLoginInfo"] as AccountLoginViewModel;
                Debug.WriteLine("account", accountLoginInfo.Username);
                if (accountLoginInfo != null)
                {
                    bankingClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                    bankingClient.ClientCredentials.UserName.UserName = accountLoginInfo.Username;
                    bankingClient.ClientCredentials.UserName.Password = accountLoginInfo.Password;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}