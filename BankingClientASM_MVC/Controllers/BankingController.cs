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
        [HttpGet]
        public ActionResult Dashboard()
        {
            var account = Session["accountLoginInfo"] as AccountLoginViewModel;
            if(account != null)
            {
                var myAccountInfo = bankingClient.ShowAccountInformation(new BankingService.AccountLoginDto
                {
                    Username = account.Username,
                    Password = account.Password
                });
                if (myAccountInfo != null)
                {
                    return View(myAccountInfo);
                }
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public ActionResult MyAccountInfo()
        {
            var accountLoginInfo = Session["accountLoginInfo"] as AccountLoginViewModel;
            if(accountLoginInfo != null && isValidAuthBankingService())
            {
                var myAccountInfo = bankingClient.ShowAccountInformation(new BankingService.AccountLoginDto
                {
                    Username = accountLoginInfo.Username,
                    Password = accountLoginInfo.Password
                });
                if (myAccountInfo != null)
                {
                    return View(myAccountInfo);
                }
            }
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public ActionResult TransferNormal()
        {
            try
            {
                var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                if (accountCurrent != null)
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

        [Authorize]
        [HttpPost]
        public ActionResult TransferNormal(TransferNormalViewModel transferInfo)
        {
            if (ModelState.IsValid)
            {
                var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                if (accountCurrent != null)
                {
                    TempData["lastTransferNormal"] = new BankingService.TransferNormalInfoDto
                    {
                        ReceiverAccountNumber = transferInfo.ReceiverAccountNumber,
                        SenderAccountNumber = accountCurrent.AccountNumber,
                        SenderPinCode = "",
                        AccountPayFees = transferInfo.AccountPayFees,
                        Amount = transferInfo.Amount,
                        Message = transferInfo.Message,
                    };
                    var accountLoginInfo = Session["accountLoginInfo"] as AccountLoginViewModel;
                    if (accountLoginInfo != null && isValidAuthBankingService())
                    {
                        bankingClient.SendConfirmationOTP(new BankingService.AccountLoginDto
                        {
                            Username = accountLoginInfo.Username,
                            Password = accountLoginInfo.Password
                        });
                        return View("ConfirmationOTPTransferNormal");
                    }
                }

            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ConfirmationOTPTransferNormal()
        {
            if (TempData["lastTransferNormal"] != null)
            {
                return View();
            }
            return RedirectToAction("TransferNormal");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmationOTPTransferNormal(PinCodeViewModel otp)
        {
            if (ModelState.IsValid)
            {
                var transferInfo = TempData["lastTransferNormal"] as BankingService.TransferNormalInfoDto;
                if (transferInfo != null && isValidAuthBankingService())
                {
                    var transaction = bankingClient.TransferNormal(new BankingService.TransferNormalInfoDto()
                    {
                        ReceiverAccountNumber = transferInfo.ReceiverAccountNumber,
                        SenderAccountNumber = transferInfo.SenderAccountNumber,
                        SenderPinCode = otp.PinCode,
                        AccountPayFees = transferInfo.AccountPayFees,
                        Amount = transferInfo.Amount,
                        Message = transferInfo.Message,
                    });
                    if(transaction != null)
                    {
                        return View("TransferSuccess", transaction);
                    }else
                    {
                        return View("TransferFailed");
                    }
                }
            }
            return View();
        }


        [HttpGet]
        [Authorize]
        public JsonResult CheckAccountTransfer(string accountNumber)
        {
            if (isValidAuthBankingService())
            {
                var accountTransferInfo = bankingClient.CheckAccountInfoTransfer(accountNumber);
                if (accountTransferInfo != null)
                {
                    return this.Json(accountTransferInfo, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult TransferPayment()
        {
            try
            {
                var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                if (accountCurrent != null && isValidAuthBankingService())
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
        [ValidateAntiForgeryToken]

        public ActionResult TransferPayment(TransferPaymentViewModel transferInfo)
        {
            if (ModelState.IsValid)
            {
                if (isValidAuthBankingService())
                {
                    try
                    {
                        var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                        if (accountCurrent != null)
                        {
                            var transfer = new BankingService.AccountTransactionDto()
                            {
                                ReceiverAccountCode = "c219ffa0-de35-4456-b2ed-dfb46882e9a2",
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
                                bankingClient.SendConfirmationOTP(new BankingService.AccountLoginDto
                                {
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
        [ValidateAntiForgeryToken]

        public ActionResult ConfirmationPinCode(PinCodeViewModel pin)
        {
            if (ModelState.IsValid)
            {
                var transferInfo = TempData["lastTransferInfo"] as BankingService.AccountTransactionDto;
                if (transferInfo != null && isValidAuthBankingService())
                {
                    transferInfo.SenderPinCode = pin.PinCode;
                    var transferResult = bankingClient.TransferPaymentOrder(transferInfo);
                    if (transferResult != null)
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
            DateTime fromDate = fromDateParam ?? DateTime.Now.AddDays(-7);
            DateTime toDate = toDateParam ?? DateTime.Now.AddDays(1);
            try
            {
                var accountCurrent = Session["accountCurrent"] as BankingService.AccountDto;
                var accountLoginInfo = Session["accountLoginInfo"] as AccountLoginViewModel;
                if (accountCurrent != null && isValidAuthBankingService() && accountLoginInfo != null)
                {
                    var accountTransaction = new BankingService.AccountGetTransactionHistoryDto
                    {
                        Username = accountLoginInfo.Username,
                        Password = accountLoginInfo.Password,
                        FromDate = fromDate,
                        ToDate = toDate,
                    };
                    Debug.WriteLine("Hello world");
                    var transactionHistory = bankingClient.ShowTransactionHistory(accountTransaction);
                    return View(transactionHistory);
                }
            }
            catch (Exception ex)
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
        [ValidateAntiForgeryToken]

        public ActionResult Login(AccountLoginViewModel account, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   /* bankingClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                    bankingClient.ClientCredentials.UserName.UserName = account.Username;
                    bankingClient.ClientCredentials.UserName.Password = account.Password;*/
                    var accountLogin = bankingClient.ShowAccountInformation(new BankingService.AccountLoginDto
                    {
                        Username = account.Username,
                        Password = account.Password
                    });
                    if (accountLogin != null)
                    {
                        Session.Add("accountLoginInfo", account);
                        int timeout = 10;
                        var ticket = new FormsAuthenticationTicket(account.Username, true, timeout);
                        string encrypt = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypt);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session.Add("accountCurrent", accountLogin);
                        if (Url.IsLocalUrl(ReturnUrl))
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
            if (ModelState.IsValid)
            {
                string phoneNumber = "+84" + newAccount.PhoneNumber.Remove(0, 1);
                var account = bankingAuthClient.RegisterAccount(new BankingAuthService.AccountRegisterDto()
                {
                    Username = newAccount.Username,
                    Password = newAccount.Password,
                    FullName = newAccount.FullName,
                    Email = newAccount.Email,
                    PhoneNumber = phoneNumber,
                    IdentityNumber = newAccount.IdentityNumber,
                    Address = newAccount.Address,
                }
                );
                if (account != null)
                {
                    TempData["Message"] = new XMessage(XMessageType.Success, "Register new account successfull");
                }
                else
                {
                    TempData["Message"] = new XMessage(XMessageType.Failed, "Register new accoount failed !");
                }
            }
            return View(newAccount);
        }

        public bool isValidAuthBankingService()
        {
            /* try
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
             return false;*/
            return true;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}