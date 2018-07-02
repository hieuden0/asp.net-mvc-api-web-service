using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using System.Web.Security;
using MrBill_MVC.Models;
using MrBill_MVC.MrBillServices;
using MrBill_MVC.MrBillTransactionServices;
using MrBill_MVC.MrBillUserServices;
using Novacode;
using AuthenticationToken = MrBill_MVC.UserService.AuthenticationToken;
using Transaction = MrBill_MVC.TransactionService.Transaction;
using MrBill_MVC.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace MrBill_MVC.Controllers
{
    public class PrintController : Controller
    {
        // GET: Print
        public ActionResult PrintOutlay(string id, UserModel model)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            //userName = "StoraHovdingen";
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var user = new UserDTO();

            try
            {
                user = userServiceClient.GetUserByUsername(authToken, userName);
            }

            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "N?got gick fel, v?nligen f?rs?k igen senare";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = user.Username;
            ViewBag.Name = user.FirstName + " " + user.LastName;
            ViewBag.Adress = user.Address;
            ViewBag.City = user.City;
            ViewBag.Zip = user.PostCode;
            ViewBag.Country = user.Country;

            var companyName = user.CompanyInfo == null ? "" : user.CompanyInfo.CompanyName;
            ViewBag.Company = companyName;
            ViewBag.Url = Request.Headers["host"];
            var userId = user.UserId;

            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();
            string[] monthList = { id.Replace('_', ' ') };
            var reportDate = DateTime.Parse(monthList[0], new CultureInfo("sv-SE"));
            ViewBag.ReportMonth = reportDate.Month;
            ViewBag.ReportYear = reportDate.Year;
            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade f?r ditt konto";
            }

            ViewBag.transList = transList;
            ViewBag.printDate = DateTime.Now;
            return View();
        }

        [HttpPost]
        public ActionResult PrintOutlay(UserModel model)
        {
            return View(model);
        }
        [WordDocument]
        public ActionResult PrintDocment(string id)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            //userName = "StoraHovdingen";
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var user = new UserDTO();

            try
            {
                user = userServiceClient.GetUserByUsername(authToken, userName);
            }

            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Name = user.FirstName + " " + user.LastName;
            ViewBag.Adress = user.Address;
            ViewBag.City = user.City;
            ViewBag.Zip = user.PostCode;
            ViewBag.Country = user.Country;
            var companyName = user.CompanyInfo == null ? "" : user.CompanyInfo.CompanyName;
            ViewBag.Company = companyName;
            ViewBag.Url = Request.Headers["host"];
            var userId = user.UserId;

            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();
            string[] monthList = { id.Replace('_', ' ') };
            var month = DateTime.Parse(monthList[0], new CultureInfo("sv-SE")).Month;
            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade f?r ditt konto";
            }

            ViewBag.transList = transList;
            ViewBag.printDate = DateTime.Now;
            ViewBag.WordDocumentFilename = "MrBill " + monthList[0];


            //ZipFile zip = new ZipFile("MyNewZip.zip");

            //foreach (var trans in userTransactions.Where(e => e.date1.Month.Equals(month)))
            //{	
            //	zip.AddDirectory(trans.fileLocation ,null); // AddDirectory recurses subdirectories
            //}

            //zip.Save(); 
            return View("PrintOutlay");

        }


        public ActionResult PrintExpense(string id, UserModel model)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            //userName = "StoraHovdingen";
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var user = new UserDTO();

            try
            {
                user = userServiceClient.GetUserByUsername(authToken, userName);
            }

            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "N?got gick fel, v?nligen f?rs?k igen senare";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Name = user.FirstName + " " + user.LastName;
            ViewBag.Adress = user.Address;
            ViewBag.City = user.City;
            ViewBag.Zip = user.PostCode;
            ViewBag.Country = user.Country;
            var companyName = user.CompanyInfo == null ? "" : user.CompanyInfo.CompanyName;
            ViewBag.Company = companyName;
            ViewBag.Url = Request.Headers["host"];
            var userId = user.UserId;

            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();
            string[] monthList = { id.Replace('_', ' ') };
            var month = DateTime.Parse(monthList[0], new CultureInfo("sv-SE")).Month;
            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade f?r ditt konto";
            }

            ViewBag.transList = transList;
            ViewBag.printDate = DateTime.Now;
            return View();
        }


        [HttpPost]
        public ActionResult PrintExpense(UserModel model)
        {
            return View(model);
        }
        [WordDocument]
        public ActionResult PrintDocmentOutlay(string id)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            //userName = "StoraHovdingen";
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var user = new UserDTO();

            try
            {
                user = userServiceClient.GetUserByUsername(authToken, userName);
            }

            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Name = user.FirstName + " " + user.LastName;
            ViewBag.Adress = user.Address;
            ViewBag.City = user.City;
            ViewBag.Zip = user.PostCode;
            ViewBag.Country = user.Country;

            var companyName = user.CompanyInfo == null ? "" : user.CompanyInfo.CompanyName;
            ViewBag.Company = companyName;
            ViewBag.Url = Request.Headers["host"];
            var userId = user.UserId;

            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();
            string[] monthList = { id.Replace('_', ' ') };
            var month = DateTime.Parse(monthList[0], new CultureInfo("sv-SE")).Month;
            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade f?r ditt konto";
            }

            ViewBag.transList = transList;
            ViewBag.printDate = DateTime.Now;
            ViewBag.WordDocumentFilename = "MrBill " + monthList[0];
            return View("PrintExpense");
        }


        public class WordDocumentAttribute : ActionFilterAttribute
        {
            public string DefaultFilename { get; set; }

            public override void OnActionExecuted(ActionExecutedContext filterContext)
            {
                var result = filterContext.Result as ViewResult;

                if (result != null)
                    result.MasterName = "~/Views/Shared/_LayoutWord.cshtml";

                filterContext.Controller.ViewBag.WordDocumentMode = false;

                base.OnActionExecuted(filterContext);
            }

            public override void OnResultExecuted(ResultExecutedContext filterContext)
            {
                var filename = filterContext.Controller.ViewBag.WordDocumentFilename;
                filename = filename ?? DefaultFilename ?? "Document";

                filterContext.HttpContext.Response.AppendHeader("Content-Disposition", string.Format("filename={0}.doc", filename));
                filterContext.HttpContext.Response.ContentType = "application/msword";

                base.OnResultExecuted(filterContext);
            }
        }

        public bool SendEmailReport(string username, int month, int year, int sortType)
        {
            var client = new MrBillServicesClient();
            //var result = client.SendEmailReport(username, new string[] { "bchieu@tma.com.vn" }, month, year);
            var result = client.SendEmailReport(username, new string[] { username }, month, year, sortType);
            return result;
        }

        public string pdfReview(string username, int month, int year, int sortType)
        {
            var client = new MrBillServicesClient();
            var result = client.pdfReview(username, month, year, sortType);

            var count = 0;
       
            while (true && count < 10)
            {
                count++;
                if (string.IsNullOrEmpty(result))
                {
                    result = client.pdfReview(username, month, year,sortType);
                }

                var filePath = AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings["pdfPath"] + result;

                if (System.IO.File.Exists(filePath))
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            if (!(count < 10)) return "";

            return result;
        }

        public FileResult DownloadReport(string username, int month, int year, int sortType)
        {
            var client = new MrBillServicesClient();
            var a = client.testConnection();
            var result = client.DownloadReport(username, new string[] { "kmquan@tma.com.vn" }, month, year,sortType);

            return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, username + " - Transaction " + month + "-" + year + ".pdf");
        }
    }
}