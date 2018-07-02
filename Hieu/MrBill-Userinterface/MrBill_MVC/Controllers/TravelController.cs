using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Util;
using System.Web.WebPages;
using MrBill_MVC.Models;
using MrBill_MVC.MrBillServices;
using TransactionSupplier = MrBill_MVC.UserService.TransactionSupplier;
using UserAuthenticationToken = MrBill_MVC.UserService.AuthenticationToken;
using TransAuthenticationToken = MrBill_MVC.TransactionService.AuthenticationToken;
using MrBill_MVC.MrBillUserServices;
using MrBill_MVC.MrBillTransactionServices;
using MrBill_MVC.Utilities;

namespace MrBill_MVC.Controllers
{
    public class TravelController : Controller
    {
        // GET: Travel
        public ActionResult MyBookings()
        {
            var userServiceClient = new MrBillUserServicesClient();
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userData = userServiceClient.GetUserByUsername(authToken, userName);
            var userService = new MrBillUserServicesClient();
            ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var user = new UserDTO();
            var userId = 0;
            try
            {
                userId = userService.GetUserIDByUsername(authToken, userName);
            }
            catch (Exception)
            {
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Login", "Account");
            }
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();

            string[] monthList = { "januari " + DateTime.Now.Year.ToString(),
                                   "februari " + DateTime.Now.Year.ToString(),
                                   "mars " + DateTime.Now.Year.ToString(),
                                   "april " + DateTime.Now.Year.ToString(),
                                   "maj " + DateTime.Now.Year.ToString(),
                                   "juni " + DateTime.Now.Year.ToString(),
                                   "juli " + DateTime.Now.Year.ToString(),
                                   "augusti " + DateTime.Now.Year.ToString(),
                                   "september " + DateTime.Now.Year.ToString(),
                                   "oktober " + DateTime.Now.Year.ToString(),
                                   "november " + DateTime.Now.Year.ToString(),
                                   "december " + DateTime.Now.Year.ToString() };

            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade för ditt konto";
            }
            ViewBag.Year = DateTime.Today.Year;
            ViewBag.transList = transList;
            ViewBag.UserName = userName;
           

            return View();
        }

        [HttpPost]
        public ActionResult MyBookings(UserModel model)
        {
            ViewBag.Year = DateTime.Today.AddYears(-1).ToString();
            return View(model);

        }

        public ActionResult MyBookingsLastYear()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();

            var userId = 0;
            try
            {
                userId = userServiceClient.GetUserIDByUsername(authToken, userName);
            }
            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();
            string[] monthList = { "januari " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "februari " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "mars " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "april " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "maj " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "juni " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "juli " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "augusti " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "september " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "oktober " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "november " + DateTime.Today.AddYears(-1).Year.ToString(),
                                    "december " + DateTime.Today.AddYears(-1).Year.ToString() };
            //string[] monthList = { "january " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "february " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "march " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "april " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "may " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "june " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "july " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "august " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "september " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "october " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "november " + DateTime.Today.AddYears(-1).Year.ToString(),
            //                        "december " + DateTime.Today.AddYears(-1).Year.ToString() };


            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade för ditt konto";
            }
            ViewBag.Year = DateTime.Today.Year;
            ViewBag.transList = transList;

            return View();
        }

        public ActionResult DeletedBookings()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();
            var userId = 0;
            try
            {
                userId = userServiceClient.GetUserIDByUsername(authToken, userName);
            }
            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }

            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();
            string[] monthList = { "januari " + DateTime.Now.Year.ToString(), "februari " + DateTime.Now.Year.ToString(), "mars " + DateTime.Now.Year.ToString(), "april " + DateTime.Now.Year.ToString(), "maj " + DateTime.Now.Year.ToString(), "juni " + DateTime.Now.Year.ToString(), "juli " + DateTime.Now.Year.ToString(), "augusti " + DateTime.Now.Year.ToString(), "september " + DateTime.Now.Year.ToString(), "oktober " + DateTime.Now.Year.ToString(), "november " + DateTime.Now.Year.ToString(), "december " + DateTime.Now.Year.ToString() };
            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade för ditt konto";
            }

            ViewBag.transList = transList;
            return View();
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


        public ActionResult AddBooking(UserModel model)
        {

            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);            
            var userService = new MrBillUserServicesClient();
            ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userSupplierList = userService.GetAllSupplierInfoList(authToken, userName, false);
            ViewBag.SupplierList = userSupplierList;

            return View(model);
        }

        [HttpPost]
        public ActionResult AddBooking(HttpPostedFileBase file)
        {
            var userService = new MrBillUserServicesClient();
            ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var transList = new List<TransactionDTO>();
            var transaction = new TransactionDTO();
            var membershipUser = Membership.GetUser();
            if (membershipUser == null)
                return RedirectToAction("Index", "Home");
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var user = userService.GetUserByUsername(authToken, userName);
            var supplier = Request.Form["supplier"];
            var year = DateTime.Now.Year.ToString();
            var month = Request.Form["ArrivalDate"].AsDateTime().Month;
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/"), fileName);
                string fileuploadDir = Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier);
                if (!Directory.Exists(fileuploadDir))
                {
                    Directory.CreateDirectory(fileuploadDir);
                }
                file.SaveAs(path);
                transaction.ReceiptLink = "/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/" + fileName;
            }
            else
            {
                transaction.ReceiptLink = "";
            }
            var tempPrice = Request.Form["price"];
            transaction.Price = float.Parse(tempPrice);
            transaction.CardNumber = Request.Form["payment"];
            transaction.Destination = Request.Form["country"];
            try
            {
                transaction.Vat1 = float.Parse(Request.Form["vat"]);
            }
            catch (Exception)
            {
                transaction.Vat1 = 0;
            }

            if (Request.Form["created"] != null && Request.Form["created"] != "")
            {
                transaction.BookingDate = Request.Form["created"].AsDateTime();
                transaction.AddedDate = Request.Form["created"].AsDateTime();
            }
            else
            {
                transaction.BookingDate = DateTime.Now;
                transaction.AddedDate = DateTime.Now;
            }

            //transaction.AddedDate = DateTime.Now;

            var localAmmount = Request.Form["localAmount"];

            transaction.PriceUserCurrency = !string.IsNullOrEmpty(localAmmount) ? float.Parse(localAmmount) : 0;

            transaction.AirDepTime1 = Request.Form["ArrivalDate"].AsDateTime();
            transaction.AirDepTime2 = Request.Form["ReturnDate"].AsDateTime();
            transaction.Country = "SE";
            //transaction.transactionOwner = "MrBill";
            //transaction.userType = "REGULAR";
            transaction.Units = 1;
            transaction.BookingRef = Request.Form["reference"];
            transaction.PriceCurrency = Request.Form["currency"];
            transaction.Description = Request.Form["moreInfo"];
            //transaction.ExtraInfo = Request.Form["moreInfo"];
            //transaction.tripType = Request.Form["category"];

            var supplierName = Request.Form["supplier"];
            var createSupplierStatus = transactionService.CreateSupplier(authToken, userName, supplierName);
            var supplierId = transactionService.GetSupplierIDBySupplierName(supplierName);
            transaction.SupplierID = supplierId;

            //these parameters are just template
            transaction.CategoryID = 1;
            transaction.Status = 1;
            transaction.ExportStatus = 1;
            transaction.PaymentID = 4;
            //end temp
            var userId = user.UserId;
            transaction.UserID = userId;

            transaction.Product = Request.Form["product"];
            transaction.CardHolder = Request.Form["CcHolder"];
            transaction.Traveller = Request.Form["traveller"];
            //transaction.transactionSupplier = supplier;
            //transaction.travelerProjectNumber = Request.Form["projectNum"];

            var projectName = Request.Form["projectNum"];
            transaction.ProjectNO = null;
            if (projectName != "")
            {
                int? projectId = null;
                var statusCreateProject = transactionService.CreateProject(authToken, userName, projectName, userId);
                projectId = transactionService.GetProjectID(authToken, userName, projectName, userId);
                transaction.ProjectNO = projectId;
            }
            //transList.Add(transaction);
         
            transactionService.CreateNewTransaction(authToken, userName, transaction);
            ViewBag.SavedText = "Sparad";
    
            return View();
        }

        public ActionResult EditMyBookings(int id, UserModel model)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userId = 0;
            try
            {
                userId = userServiceClient.GetUserIDByUsername(authToken, userName);
            }
            catch (Exception)
            {

                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransaction = transactionService.GetTransactionById(authToken, userName, id);
            var transList = new List<TransactionDTO>
            {
                userTransaction
            };
            ViewBag.transaction = transList;
            ViewBag.ProjectName = null;
            ViewBag.SupplierName = "";
            ViewBag.Url = Request.Headers["host"];
            ViewBag.ReturnStatus = "";
            return View();
        }

        [HttpPost]
        public ActionResult EditMyBookings(UserModel model, HttpPostedFileBase file)
        {
            var userService = new MrBillUserServicesClient();
            ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

            var transList = new List<TransactionDTO>();
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userId = userService.GetUserIDByUsername(authToken, userName);
            var transId = Request.Form["id"];
            var editTransaction = transactionService.GetTransactionById(authToken, userName, int.Parse(transId));
            var supplier = Request.Form["supplier"];
            var year = DateTime.Now.Year.ToString();
            var month = Request.Form["date1"].AsDateTime().Month;
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/"), fileName);

                string fileuploadDir = Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier);
                if (!Directory.Exists(fileuploadDir))
                {
                    Directory.CreateDirectory(fileuploadDir);
                }

                if (!string.IsNullOrEmpty(editTransaction.ReceiptLink))
                {
                    try
                    {
                        System.IO.File.Delete(editTransaction.ReceiptLink);
                    }
                    catch (Exception)
                    {

                    }
                }
                file.SaveAs(path);

                editTransaction.ReceiptLink = "/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/" + fileName;

            }
            if (string.IsNullOrEmpty(editTransaction.ReceiptLink))
            {
                editTransaction.ReceiptLink = "";
            }
            if (Request.Form["created"] != null && Request.Form["created"] != "")
            {
                editTransaction.BookingDate = Request.Form["created"].AsDateTime();
                editTransaction.AddedDate = Request.Form["created"].AsDateTime();
            }
            else
            {
                editTransaction.BookingDate = DateTime.Now;
                editTransaction.AddedDate = DateTime.Now;
            }
            //editTransaction.AddedDate = DateTime.Now;
            editTransaction.Price = float.Parse(Request.Form["price"]);
            editTransaction.CardNumber = Request.Form["payment"];
            editTransaction.Destination = Request.Form["country"];
            editTransaction.AirDepTime1 = Request.Form["date1"].AsDateTime();
            editTransaction.AirDepTime2 = Request.Form["date2"].AsDateTime();

            var localAmmount = Request.Form["localAmount"];
            editTransaction.PriceUserCurrency = !string.IsNullOrEmpty(localAmmount) ? float.Parse(localAmmount) : 0;

            editTransaction.Country = "SE";
            editTransaction.Vat1 = float.Parse(Request.Form["vat"]);
            //editTransaction.transactionOwner = "MrBill";
            //editTransaction.userType = "REGULAR";
            editTransaction.BookingRef = Request.Form["reference"];
            editTransaction.PriceCurrency = Request.Form["currency"];
            editTransaction.Description = Request.Form["moreInfo"];
            //editTransaction.ExtraInfo = Request.Form["moreInfo"];

            var category = Request.Form["category"];
            if (category == null) // Now it is always null because old system is not implemented yet
                editTransaction.CategoryID = 1;

            //var supplierId = int.Parse(Request.Form["supplier"]);
            //editTransaction.SupplierID = supplierId;

            var supplierId = 0;
            var supplierName = Request.Form["supplier"];
            var createSupplierStatus = transactionService.CreateSupplier(authToken, userName, supplierName);
            supplierId = transactionService.GetSupplierIDBySupplierName(supplierName);
            editTransaction.SupplierID = supplierId;
            

            //these parameters are just template
            //editTransaction.CategoryID = 1;
            //editTransaction.Status = 1;
            //editTransaction.ExportStatus = 1;
            //editTransaction.PaymentID = 4;
            //end temp

            editTransaction.Product = Request.Form["product"];
            editTransaction.CardHolder = Request.Form["CcHolder"];
            editTransaction.Traveller = Request.Form["traveller"];

            //Now we don't set anything with 3 table: employee, costcenter and project
            //editTransaction.ProjectNO = null;//Request.Form["projectNum"];
            //
            var projectName = Request.Form["projectNum"];
            editTransaction.ProjectNO = null;
            if (projectName != "")
            {
                int? projectId = null;

                var statusCreateProject = transactionService.CreateProject(authToken, userName, projectName, userId);
                projectId = transactionService.GetProjectID(authToken, userName, projectName, userId);
                editTransaction.ProjectNO = projectId;
            }
            

            editTransaction.TransactionId = int.Parse(Request.Form["TransactionId"]);
            transList.Add(editTransaction);

            var returnStatus = transactionService.UpdateTransaction(authToken, userName, editTransaction,"");
            
            ViewBag.ReturnStatus = returnStatus;
            ViewBag.transaction = transList;
            ViewBag.ProjectName = projectName;
            ViewBag.SupplierName = supplierName;
            ViewBag.Url = Request.Headers["host"];
            
            RedirectToAction("MyBookings", "Travel");
            return View();
        }

        public string CreateMainStrip(string username,int transactionId, string groupName)
        {
            var membershipUser = Membership.GetUser();
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var transactionService = new MrBillTransactionServicesClient();

            var transaction = transactionService.GetTransactionById(authToken, userName, transactionId);
            //var result = client.SendEmailReport(username, new string[] { "bchieu@tma.com.vn" }, month, year);
            var result = transactionService.UpdateTransaction(authToken, userName, transaction, groupName);
            return result;
        }

        public ActionResult ConnectTrip(string username, int transactionId,int groupId, int mainTrip)
        {
            var membershipUser = Membership.GetUser();
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var transactionService = new MrBillTransactionServicesClient();

            var transaction = transactionService.GetTransactionById(authToken, userName, transactionId);
            //var result = client.SendEmailReport(username, new string[] { "bchieu@tma.com.vn" }, month, year);
            var result = transactionService.UpdateTransactionWithMainTrip(authToken, userName, transaction,groupId, mainTrip);
            return Json(new { result = 1 });
        }

        public string RemoveMainTrip(string username, int mainTrip)
        {
            var membershipUser = Membership.GetUser();
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var transactionService = new MrBillTransactionServicesClient();

            var result = transactionService.RemoveMainTrip(authToken, userName, mainTrip);
            return result;
        }

        public string ChangeMainTrip(string username, int mainTrip,int groupIdOfGrouptrans, int transIdOfGroupTrans)
        {
            var membershipUser = Membership.GetUser();
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var transactionService = new MrBillTransactionServicesClient();
            var result = transactionService.ChangeMainTrip(authToken, userName, mainTrip, groupIdOfGrouptrans, transIdOfGroupTrans);
            return result;
        }



        public ActionResult ActivateBooking(int id, UserModel model)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            //userName = "StoraHovdingen7";
            var userServiceClient = new MrBillUserServicesClient();

            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userId = 0;
            try
            {
                userId = userServiceClient.GetUserIDByUsername(authToken, userName);
            }
            catch (Exception)
            {

                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }

            var transactionService = new MrBillTransactionServicesClient();

            var userTransaction = transactionService.GetTransactionById(authToken, userName, id);

            var transList = new List<TransactionDTO>
            {
                userTransaction
            };

            ViewBag.transaction = transList;

            return View(model);

        }

        [HttpPost]
        public ActionResult ActivateBooking(UserModel model)
        {
            var userService = new MrBillUserServicesClient();
            var transactionService = new MrBillTransactionServicesClient();
            var transList = new List<TransactionDTO>();

            var transaction = new TransactionDTO();
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var user = userService.GetUserIDByUsername(authToken, userName);

            var transId = Request.Form["id"];

            var editTransaction = transactionService.GetTransactionById(authToken, userName, int.Parse(transId));

            editTransaction.Vat3 = null;

            transList.Add(editTransaction);

            transactionService.UpdateTransaction(authToken, userName, editTransaction,"");

            ViewBag.transaction = transList;
            return RedirectToAction("DeletedBookings");

        }

        //REMOVE
        public ActionResult RemoveBooking(int id, UserModel model)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

            var userId = 0;
            try
            {
                userId = userServiceClient.GetUserIDByUsername(authToken, userName);
            }
            catch (Exception)
            {

                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }

            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

            var userTransaction = transactionService.GetTransactionById(authToken, userName, id);

            var transList = new List<TransactionDTO>
            {
                userTransaction
            };

            ViewBag.transaction = transList;

            return View(model);

        }

        [HttpPost]
        public ActionResult RemoveBooking(UserModel model)
        {
            var userService = new MrBillUserServicesClient();
            ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

            var transList = new List<TransactionDTO>();
            var transaction = new TransactionDTO();
            var membershipUser = Membership.GetUser();
            if (membershipUser == null)
                return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userId = userService.GetUserIDByUsername(authToken, userName);
            var transId = Request.Form["id"];
            var editTransaction = transactionService.GetTransactionById(authToken, userName, int.Parse(transId));
            editTransaction.Vat3 = 0;
            editTransaction.Status = 0;

            if (editTransaction.TransactionGroupID != null && editTransaction.MainTrip == null)
            {
                int mainTrip = (int)editTransaction.MainTrip;
                transactionService.RemoveMainTrip(authToken, userName, mainTrip);
                editTransaction.TransactionGroupID = null;
            }
            transList.Add(editTransaction);
            transactionService.UpdateTransaction(authToken, userName, editTransaction,"");
            ViewBag.transaction = transList;
            return RedirectToAction("MyBookings");
        }


        public ActionResult TravelAccounts()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");
            var userName = membershipUser.UserName;

            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userService = new MrBillUserServicesClient();
            var companyList = userService.GetAllSupplierInfoList(authToken, userName, true);
            //var companyList = new List<SelectListItem>
            //{
            //    new SelectListItem {Text = "VÄLJ", Value = "VÄLJ"},
            //    new SelectListItem {Text = "SAS", Value = "SAS"},
            //    new SelectListItem {Text = "HOTELS", Value = "HOTELS"},
            //    new SelectListItem {Text = "EASYPARK", Value = "EASYPARK"},
            //    new SelectListItem {Text = "CABONLINE", Value = "CABONLINE"},
            //    new SelectListItem {Text = "NORWEIGAN", Value = "NORWEIGAN"},
            //};
            ViewBag.SupDropDown = companyList;

            return View();

        }

        [HttpPost]
        public ActionResult TravelAccounts(TravelModel model)
        {
            var userName = Membership.GetUser().UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();
            var companyList = userServiceClient.GetAllSupplierInfoList(authToken, userName, true);
            //var companyList = new List<SelectListItem>
            //{
            //    new SelectListItem {Text = "VÄLJ", Value = "VÄLJ"},
            //    new SelectListItem {Text = "SAS", Value = "SAS"},
            //    new SelectListItem {Text = "HOTELS", Value = "HOTELS"},
            //    new SelectListItem {Text = "EASYPARK", Value = "EASYPARK"},
            //    new SelectListItem {Text = "CABONLINE", Value = "CABONLINE"},
            //    new SelectListItem {Text = "NORWEIGAN", Value = "NORWEIGAN"},
            //};

            var selectedItem = Request.Form["SupDropDown"];
            ViewBag.SupDropDown = companyList;

            if (selectedItem == "VÄLJ")
            {
                return View(model);
            }

            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

            var userId = userServiceClient.GetUserIDByUsername(authToken, userName);

            var auth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            var transSup = new MrBillTransactionServices.UserSupplierInfoDTO();
            transSup.Username = model.UserName;
            transSup.Password = model.ConfirmPassword;
            transSup.SupplierId = int.Parse(Request.Form["SupDropDown"]);
            transSup.UserId = userId;

            transactionService.SaveTransactionSuppliers(authToken, userName, transSup);
            var supplierName = "";
            foreach (var item in companyList)
            {
                if (item.SupplierId == int.Parse(Request.Form["SupDropDown"]))
                {
                    supplierName = item.SupplierName;
                }
            }
            ViewBag.Added = supplierName + " tillagt";
            return View(model);
        }

        public ActionResult Manage()
        {
            var username = Membership.GetUser().UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(username);

            //Old service
            //var userServiceClient = new UserServicePortTypeClient();
            //((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            //var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            //var user1 = userServiceClient.getUserByUsername(auth, username);

            //var userSuppList = user1.transactionSuppliers.ToList();
            //End Old service

            //New service
            var userServices = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServices.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            //var token = //waiting for Hien Nguyen
            var user = userServices.GetUserByUsername(authToken, username);
            var userSuppList = user.UserSupplierInfoes;
            //End new service

            //var supplierNames = { "SAS", "HOTELS", "EASYPARK", "CABONLINE", "NORWEIGAN" };
            var supplierInfoes = userServices.GetAllSupplierInfoList(authToken, username, false);
            var suppBigList = new List<MrBillUserServices.UserSupplierInfoDTO>();
            foreach (var sup in supplierInfoes)
            {
                var newUserSupplier = new MrBillUserServices.UserSupplierInfoDTO();
                var newTransSupp = new MrBillUserServices.SupplierInfoDTO();
                newTransSupp.SupplierName = sup.SupplierName;
                newTransSupp.Url = sup.Url;
                newTransSupp.SupplierId = sup.SupplierId;
                newUserSupplier.SupplierInfo = newTransSupp;
                suppBigList.Add(newUserSupplier);
            }




            var dict = suppBigList.ToDictionary(p => p.SupplierInfo.SupplierName);
            if (userSuppList != null)
            {
                foreach (var supp in userSuppList)
                {
                    dict[supp.SupplierInfo.SupplierName] = supp;
                }
            }
            var merged = dict.Values.ToList();

            ////var suppNameList = user.transactionSuppliers.Select(supp => new SelectListItem { Text = supp.name, Value = supp.name }).ToList();

            ViewBag.userSuppliers = merged;

            return View();
        }

        [HttpPost]
        public ActionResult Manage(TravelModel model)
        {
            var userName = Membership.GetUser().UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);

            //New user services
            var userService = new MrBillUserServicesClient();
            ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userModel = userService.GetUserByUsername(authToken, userName);
            //End

            //var userServiceClient = new UserServicePortTypeClient();
            //((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            //var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            //var user = userServiceClient.getUserByUsername(auth, userName);
            var userId = userModel.UserId;

            var userSupplierList = userModel.UserSupplierInfoes.Where(us => us.Username == userName).ToList();

            var suppNameList = userSupplierList.Select(supp => new SelectListItem { Text = supp.SupplierInfo.SupplierName, Value = supp.SupplierInfo.SupplierName });

            ViewBag.SupDropDown = suppNameList;

            //var transactionServiceClient = new TransactionServicePortTypeClient();
            //((BasicHttpBinding)transactionServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            //var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };




            var userTransList = userModel.UserSupplierInfoes;
            var transSupList = new List<MrBillTransactionServices.UserSupplierInfoDTO>();
            var transSup = new MrBillTransactionServices.UserSupplierInfoDTO();

            var supplierInfoes = userService.GetAllSupplierInfoList(authToken, userName, false);
            var suppBigList = new List<MrBillUserServices.UserSupplierInfoDTO>();
            foreach (var sup in supplierInfoes)
            {
                var newUserSupp = new MrBillUserServices.UserSupplierInfoDTO();
                var newTransSupp = new MrBillUserServices.SupplierInfoDTO();
                newTransSupp.SupplierName = sup.SupplierName;
                newTransSupp.SupplierId = sup.SupplierId;
                newTransSupp.SignUpUrl = sup.SignUpUrl;

                newUserSupp.SupplierInfo = newTransSupp;
                suppBigList.Add(newUserSupp);

            }


            var dict = suppBigList.ToDictionary(p => p.SupplierInfo.SupplierName);
            if (userSupplierList != null)
            {
                foreach (var supp in userTransList)
                {
                    dict[supp.SupplierInfo.SupplierName] = supp;
                }
            }
            var merged = dict.Values.ToList();

            foreach (var supplier in merged)
            {
                if (Request.Form["CompanyName"] == supplier.SupplierInfo.SupplierName)
                {
                    if (string.IsNullOrEmpty(Request.Form["UserName"]))
                        continue;

                    supplier.Username = Request.Form["UserName"];
                    var supplierId = Request.Form["SupplierId"];
                    transSup.Username = supplier.Username;
                    transSup.Password = Request.Form["ConfirmPassword"];
                    transSup.SupplierId = int.Parse(supplierId);
                    transSup.UserId = userId;
                    ViewBag.changed = Request.Form["CompanyName"] + " sparad";
                }
            }

            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            transactionService.SaveTransactionSuppliers(authToken, userName, transSup);

            ViewBag.userSuppliers = merged;

            return View(model);
        }

        public ActionResult Content()
        {
            var userServiceClient = new MrBillUserServicesClient();
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");
            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userData = userServiceClient.GetUserByUsername(authToken, userName);
            var userService = new MrBillUserServicesClient();
            ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var user = new UserDTO();
            var userId = 0;
            try
            {
                userId = userService.GetUserIDByUsername(authToken, userName);
            }
            catch (Exception)
            {
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Login", "Account");
            }
            var transactionService = new MrBillTransactionServicesClient();
            ((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userTransactions = transactionService.GetTransactionsForUser(authToken, userName, userId);
            var transList = userTransactions.ToList();

            string[] monthList = { "januari " + DateTime.Now.Year.ToString(),
                                   "februari " + DateTime.Now.Year.ToString(),
                                   "mars " + DateTime.Now.Year.ToString(),
                                   "april " + DateTime.Now.Year.ToString(),
                                   "maj " + DateTime.Now.Year.ToString(),
                                   "juni " + DateTime.Now.Year.ToString(),
                                   "juli " + DateTime.Now.Year.ToString(),
                                   "augusti " + DateTime.Now.Year.ToString(),
                                   "september " + DateTime.Now.Year.ToString(),
                                   "oktober " + DateTime.Now.Year.ToString(),
                                   "november " + DateTime.Now.Year.ToString(),
                                   "december " + DateTime.Now.Year.ToString() };

            ViewBag.monthList = monthList;

            if (transList.Count < 1)
            {
                ViewBag.noBook = "Inga bokningar hittade för ditt konto";
            }
            ViewBag.Year = DateTime.Today.Year;
            ViewBag.transList = transList;
            ViewBag.UserName = userName;
            //transList.Sort()

            return PartialView("ContentPartial");
        }

        public ActionResult Item(TransactionDTO item, List<TransactionDTO> transList, string transGroupListCondition, string date)
        {
            ViewBag.item = item;
            ViewBag.transList = transList;
            ViewBag.date = date;
            ViewBag.transGroupListConditionItem2 = transGroupListCondition.ToString();

            ModelState.Clear();
            return PartialView("_ItemPartial");
        }
    }
}