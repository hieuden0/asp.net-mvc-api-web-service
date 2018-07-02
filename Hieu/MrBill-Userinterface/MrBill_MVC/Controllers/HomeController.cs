using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MrBill_MVC.Filters;
using MrBill_MVC.Models;
using MrBill_MVC.UserService;
using System.ServiceModel;
using MrBill_MVC.TransactionService;
using MrBill_MVC.MrBillService;
using MrBill_MVC.MrBillUserServices;
using MrBill_MVC.MrBillTransactionServices;
using MrBill_MVC.MrBillServices;
using MrBill_MVC.Utilities;
using WebMatrix.WebData;
using UserAuthenticationToken = MrBill_MVC.UserService.AuthenticationToken;
using TransAuthenticationToken = MrBill_MVC.TransactionService.AuthenticationToken;
using MrBillAuthenticationToken = MrBill_MVC.MrBillService.AuthenticationToken;

namespace MrBill_MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(UserModel model)
        {

            return View();
        }

        public ActionResult About(UserModel model)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null) Session["userName"] = membershipUser.UserName;
            model.FirstName = (string)Session["userName"];
            return View();
        }

        public ActionResult Contact(UserModel model)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null) Session["userName"] = membershipUser.UserName;
            model.FirstName = (string)Session["userName"];
            return View();

        }

        public ActionResult Faq()
        {
            return View();
        }


        public ActionResult TemporaryDataOldService()
        {
            var oldUserService = new UserServicePortTypeClient();
            ((BasicHttpBinding)oldUserService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var authUser = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

            var oldTransactionService = new TransactionServicePortTypeClient();
            ((BasicHttpBinding)oldTransactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var authTrans = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

            var oldService = new MrBillServicePortTypeClient();
            ((BasicHttpBinding)oldService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var auth = new MrBillAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            var users = oldService.getMrBillUsers(auth);

            var newUserService = new MrBillUserServicesClient();
            var newTransService = new MrBillTransactionServicesClient();
            var tempToken = MrBillUtility.GenerateAuthenticationToken("admin@mrbill.se");
            var suppList = newUserService.GetAllSupplierInfoList(tempToken, "admin@mrbill.se", true);
            foreach (var item in users)
            {
                //Create user
                var userDTO = new UserDTO();
                var user = oldUserService.getUser(authUser, item.userId.ToString());
                userDTO.Address = string.IsNullOrEmpty(user.address.address) ?
                                            user.address.address2 : user.address.address;
                userDTO.City = user.address.city;
                userDTO.CompanyId = 1;
                userDTO.Country = user.address.country;
                userDTO.FirstName = user.firstName;
                userDTO.Password = user.password;
                userDTO.Phone = string.IsNullOrEmpty(user.businessPhoneNumber) ? user.businessPhoneNumber
                                            : user.homePhoneNumber;
                userDTO.PostCode = user.address.postalCode;
                userDTO.Status = user.idSpecified ? 1 : 0;
                userDTO.Username = user.username;
                userDTO.UserRoleId = 3;

                int createStatus = newUserService.CreateUser(userDTO);
                if (createStatus == 1 && !WebSecurity.UserExists(userDTO.Username))
                {
                    WebSecurity.CreateAccount(userDTO.Username, userDTO.Password);
                }
                var authToken = MrBillUtility.GenerateAuthenticationToken(user.username);
                var userId = newUserService.GetUserIDByUsername(authToken, user.username);
                //Create records in UserSupplier
                if (item.transactionSuppliers != null)
                {
                    foreach (var uSuppItem in item.transactionSuppliers)
                    {
                        var userSupplierInfoDTO = new MrBillTransactionServices.UserSupplierInfoDTO();

                        userSupplierInfoDTO.Password = uSuppItem.supplierPassword;
                        userSupplierInfoDTO.UserId = userId;
                        userSupplierInfoDTO.Username = uSuppItem.supplierUsername;
                        if (uSuppItem.name.ToUpper() == "SAS")
                            userSupplierInfoDTO.SupplierId = 1;
                        else if (uSuppItem.name.ToUpper() == "HOTELS")
                            userSupplierInfoDTO.SupplierId = 2;
                        else if (uSuppItem.name.ToUpper() == "EASYPARK")
                            userSupplierInfoDTO.SupplierId = 3;
                        else if (uSuppItem.name.ToUpper() == "CABONLINE")
                            userSupplierInfoDTO.SupplierId = 4;
                        else if (uSuppItem.name.ToUpper() == "NORWEIGAN")
                            userSupplierInfoDTO.SupplierId = 5;
                        else
                            userSupplierInfoDTO.SupplierId = 6; //My database is wrong. Please update this id to 6

                        newTransService.SaveTransactionSuppliers(authToken, user.username, userSupplierInfoDTO);
                    }
                }

                //Create records in Transaction
                foreach (var trans in item.transactions)
                {
                    var transDTO = new TransactionDTO();
                    transDTO.ReceiptLink = trans.fileLocation;
                    var price = trans.price ?? 0;
                    var vat1 = trans.vat1 ?? 0;
                    var localAmount = trans.localAmount ?? 0;
                    transDTO.Price = (float)price;
                    transDTO.CardNumber = trans.creditCardNumber;
                    transDTO.Destination = trans.destination;
                    transDTO.Vat1 = (float)vat1;
                    transDTO.BookingDate = trans.expenseDate;
                    transDTO.AddedDate = trans.addedDate == null || trans.addedDate == DateTime.MinValue
                                                ? DateTime.Now : trans.addedDate;
                    transDTO.PriceUserCurrency = (float)localAmount;
                    transDTO.AirDepTime1 = trans.date1;
                    transDTO.AirDepTime2 = trans.date2;
                    transDTO.Country = trans.country;
                    transDTO.Units = trans.numberOfUnits;
                    transDTO.BookingRef = trans.bookingReference;
                    transDTO.PriceCurrency = trans.currency;

                    foreach (var i in suppList)
                    {
                        if (i.SupplierName == trans.transactionSupplier)
                            transDTO.SupplierID = i.SupplierId;
                    }
                    if (transDTO.SupplierID == 0)
                        transDTO.SupplierID = 6;
                    //these parameters are just template
                    transDTO.CategoryID = 1;
                    transDTO.Status = 1;
                    transDTO.ExportStatus = 1;
                    transDTO.PaymentID = 4;
                    //end temp
                    transDTO.UserID = userId;
                    transDTO.Product = trans.product;
                    transDTO.CardHolder = trans.creditCardOwner;
                    transDTO.Traveller = trans.travelerName;

                    //newTransService.CreateNewTransaction(authToken, user.username, transDTO);
                }
            }

            return View();
        }

        public ActionResult AdminCreateAccountOldDatabase()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminCreateAccountOldDatabase(string username, string password)
        {
            //if(!WebSecurity.UserExists(username))
            //{
                WebSecurity.CreateAccount(username, password);
            //}
            //else
            //{
            //    var oldUserService = new UserServicePortTypeClient();
            //    ((BasicHttpBinding)oldUserService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            //    var authUser = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

            //    var oldUser = oldUserService.getUserByUsername(authUser, username);
            //    WebSecurity.ChangePassword(username, oldUser.password, password);
            //}
            return View();
        }
    }
}
