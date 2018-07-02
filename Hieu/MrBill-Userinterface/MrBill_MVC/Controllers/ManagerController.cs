using MrBill_MVC.Models;
using MrBill_MVC.MrBillTransactionServices;
using MrBill_MVC.MrBillUserServices;
using MrBill_MVC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace MrBill_MVC.Controllers
{
    public class ManagerController : Controller
    {
        public ActionResult AdminManage()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();
            UserDTO user = new UserDTO();
            try
            {
                user = userServiceClient.GetUserByUsername(authToken, userName);
                if (user.UserRoleId == (int)MrbillState.MrbillUserRole.OrdinaryUser)
                {
                    return RedirectToAction("Index", "Home");
                } 
            }
            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult AllUserManage()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            UserDTO user = new UserDTO();
            try
            {
                user = userServiceClient.GetUserByUsername(authToken, userName);
                if (user.UserRoleId == (int)MrbillState.MrbillUserRole.OrdinaryUser)
                {
                    return RedirectToAction("Index", "Home");
                } 
            }
            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.userList = userServiceClient.GetAllUser(authToken,userName);

            return View();
        }

        public ActionResult AllUserSupplierManage()
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

            ViewBag.userSupplierList = userServiceClient.GetAllUserSupplierInfo(authToken, userName);
            ViewBag.userList = userServiceClient.GetAllUser(authToken, userName);

            return View();
        }

        //
        // GET: /Manager/Register
        [AllowAnonymous]
        public ActionResult RegisterAdmin()
        {
            return View();
        }

        // POST: /Manager/RegisterAdmin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterAdmin(RegisterModel model, FormCollection formCollection)
        {
            if (!ModelState.IsValid) return View(model);
            // Attempt to register the user
            try
            {
                var userServices = new MrBillUserServicesClient();

                var user = new UserDTO();
                user.FirstName = formCollection["FirstName"];
                user.LastName = formCollection["LastName"];
                user.Password = formCollection["Password"];
                user.Username = formCollection["UserName"];
                user.UserRoleId = int.Parse(formCollection["UserRoleId"]);

                user.Country = "SE";

                //Not request in cards
                user.Address = string.Empty;
                user.City = string.Empty;
                user.CompanyId = 1;
                user.Phone = "(00) 000-000-000";
                user.PostCode = string.Empty;
                user.Status = 1;
                

                var result = 0;
                try
                {
                    result = userServices.CreateUser(user);
                }
                catch (Exception e)
                {
                    ViewBag.Error = "Något gick fel. Vänligen kontakta supporten. " + e.Message;
                    return View(model);
                }

                if (result == 1)
                {
                    WebSecurity.CreateAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);

                    return RedirectToAction("Manage", "Travel");
                }
                else
                {
                    ViewBag.Error = "Något gick fel. Vänligen kontakta supporten. " + result;
                    return View(model);
                }
            }

            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //public ActionResult editSupplierInfo(int useIdSupplier,int supplierId, string userNameSupplier, string password)
        public ActionResult editSupplierInfo(int useIdSupplier, int supplierId, string userNameSupplier, string password)
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
            var newUserSupp = new MrBillTransactionServices.UserSupplierInfoDTO();
            newUserSupp.SupplierId = supplierId;
            newUserSupp.UserId = useIdSupplier;
            newUserSupp.Username = userNameSupplier;
            newUserSupp.Password = password;

            var transactionService = new MrBillTransactionServicesClient();
            transactionService.SaveTransactionSuppliers(authToken, userName, newUserSupp);
            return Json(new { result = 1 });
        }

        public ActionResult SupplierInfobyUserId(int userId)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userServiceClient = new MrBillUserServicesClient();

            ViewBag.userId = userId;
            ViewBag.userSupplierList = userServiceClient.GetAllUserSupplierInfo(authToken, userName).Where(t => t.UserId == userId).ToList();
            var abc = userServiceClient.GetAllUserSupplierInfo(authToken, userName).Where(t => t.UserId == userId).ToList();
            return PartialView("_SupplierInfobyUserIdPartial",abc);
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Användarnamnet är redan registrerat";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Emailadressen används redan";

                case MembershipCreateStatus.InvalidPassword:
                    return "Ogiltigt lösenord";

                case MembershipCreateStatus.InvalidEmail:
                    return "Ogiltig Emailadress";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Ogiltigt lösenord";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Error, testa igen";

                case MembershipCreateStatus.InvalidUserName:
                    return "Anvädnarnamnet finns inte";

                case MembershipCreateStatus.ProviderError:
                    return "Något gick fel med verifieringen";

                case MembershipCreateStatus.UserRejected:
                    return "Verifieringen har avbrutits";

                default:
                    return "Något gick fel. Om problemet kvarstår var vänlig kontakta Administratören.";
            }
        }
    }
}