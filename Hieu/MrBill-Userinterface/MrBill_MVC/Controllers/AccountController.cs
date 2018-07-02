using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Transactions;

using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using MrBill_MVC.Models;
//using MrBill_MVC.UserService;
using WebGrease.Configuration;
using WebMatrix.WebData;
using MrBill_MVC.MrBillUserServices;
using MrBill_MVC.Utilities;

namespace MrBill_MVC.Controllers
{

    [Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //CustomConfig config = new CustomConfig();


        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            return View();
        }

        // POST: Account/LostPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user;
                //using (var context = new UsersContext())
                //{
                //    var foundUserName = (from u in context.UserProfiles
                //                         where u.UserName == model.Email
                //                         select u.UserName).FirstOrDefault();
                //    if (foundUserName != null)
                //    {
                //        user = Membership.GetUser(foundUserName.ToString());
                //    }
                //    else
                //    {
                //        user = null;
                //    }
                //}
                var authToken = MrBillUtility.GenerateAuthenticationToken(model.Email);

                var userService = new MrBillUserServicesClient();
                ((BasicHttpBinding)userService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
                var foundUser = userService.GetUserByUsername(authToken, model.Email);
                if (foundUser != null)
                {
                    user = Membership.GetUser(foundUser.Username);
                }
                else
                {
                    user = null;
                }

                if (user != null)
                {
                    // Generae password token that will be used in the email link to authenticate user
                    var token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                    // Generate the html link sent via email
                    string resetLink = "<a href='"
                       + Url.Action("ResetPassword", "Account", new { rt = token }, "http")
                       + "'>Återställ lösenordet</a>";

                    // Email stuff
                    string subject = "Återställ ditt lösenord hos MrBill";
                    string body = "Din länk: " + resetLink;
                    string from = "noreply@mrbill.se";

                    MailMessage message = new MailMessage(from, model.Email);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    var client = new SmtpClient();
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = System.Configuration.ConfigurationManager.AppSettings["smtpHost"];

                    // Attempt to send the email
                    try
                    {
                        client.Send(message);
                        ViewBag.Message = "Ett mail har skickats med en återställningslänk";
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Kunde inte skicka email: " + e.Message);
                    }
                }
                else // Email not found
                {
                    /* Note: You may not want to provide the following information
                    * since it gives an intruder information as to whether a
                    * certain email address is registered with this website or not.
                    * If you're really concerned about privacy, you may want to
                    * forward to the same "Success" page regardless whether an
                    * user was found or not. This is only for illustration purposes.
                    */
                    ModelState.AddModelError("", "Ingen användare med den emailen hittad.");
                }
            }

            /* You may want to send the user to a "Success" page upon the successful
            * sending of the reset email link. Right now, if we are 100% successful
            * nothing happens on the page. :P
            */

            return View(model);
        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            if (TempData["SignoutMessage"] != null)
            {
                ViewBag.Message = TempData["SignoutMessage"].ToString();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


      
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl, UserModel userModel)
        {
            #region testcode for create password for account
            try
            {
                if (model.Password == "admincreated")
                {

                    WebSecurity.CreateAccount(model.UserName, "123456");
                }

            }
            catch (Exception exception)
            {

            }
            #endregion
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                if (model.RememberMe)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                }

                Session["userName"] = model.UserName;
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "email eller lösenord är inte korrekt.");
                return View(model);
            }
        }

        // GET: /Account/AdminLogin
        [AllowAnonymous]
        public ActionResult AdminLogin(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(LoginModel model, string returnUrl, UserModel userModel)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password))
            {
                var userServices = new MrBillUserServicesClient();
                var authToken = MrBillUtility.GenerateAuthenticationToken(model.UserName);
                UserDTO user = userServices.GetUserByUsername(authToken, model.UserName);
                if (userServices.IsAdmin(user.UserId))
                {
                    if(userServices.IsSuperAdmin(user.UserId)){
                        Session["userRole"] = (int)MrbillState.MrbillUserRole.MrBillAdmin;
                    }
                    else
                    {
                        Session["userRole"] = (int)MrbillState.MrbillUserRole.OrdinaryUser;
                    }
                    Session["userName"] = model.UserName;
                    return RedirectToAction("AdminManage", "Manager", null);
                }
                else
                {
                    Request.Cookies.Remove("UserId");
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index", "home");
                }
                
            }
            else
            {
                ModelState.AddModelError("", "email eller lösenord är inte korrekt.");
                return View(model);
            }
        }




        [AllowAnonymous]
        public ActionResult Login_extern(string returnUrl)
        {
            if (TempData["SignoutMessage"] != null)
            {
                ViewBag.Message = TempData["SignoutMessage"].ToString();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login_extern(LoginModel model, string returnUrl, UserModel userModel)
        {

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                if (model.RememberMe)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                }

                Session["userName"] = model.UserName;
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "email eller lösenord är inte korrekt.");
                return View(model);
            }


        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string rt)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.ReturnToken = rt;
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool resetResponse = WebSecurity.ResetPassword(model.ReturnToken, model.Password);
                if (resetResponse)
                {
                    ViewBag.Message = "Lösenordet har ändrats";
                }
                else
                {
                    ViewBag.Message = "Något gick fel, vänlig testa igen";
                }
            }
            return View(model);
        }

        //
        // POST: /Account/Login

        //
        // POST: /Account/LogOff



        [HttpPost]
        public ActionResult LogOff()
        {
            Request.Cookies.Remove("UserId");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "home");
        }



        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, FormCollection formCollection)
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

                user.Country = "SE";

                //Not request in cards
                user.Address = string.Empty;
                user.City = string.Empty;
                user.CompanyId = 1;
                user.Phone = "(00) 000-000-000";
                user.PostCode = string.Empty;
                user.Status = 1;
                user.UserRoleId = 3;

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

        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            var ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount != User.Identity.Name) return RedirectToAction("Manage", new { Message = message });
            // Use a transaction to prevent the user from deleting their last login credential
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
            {
                bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                {
                    OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                    scope.Complete();
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Lösenordet har ändrats."
                : message == ManageMessageId.SetPasswordSuccess ? "Lösenordet är ok."
                : message == ManageMessageId.RemoveLoginSuccess ? "Extern login borttagen."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            var hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));

            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");

            if (hasLocalAccount)
            {
                if (!ModelState.IsValid) return View(model);
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                //var userServiceClient = new UserServicePortTypeClient();
                //var auth = new AuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
                var userName = Membership.GetUser().UserName;
                //var userData = userServiceClient.getUserByUsername(auth, userName);
                var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
                var userService = new MrBillUserServicesClient();
                var userData = userService.GetUserByUsername(authToken, userName);


                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    userData.Password = model.ConfirmPassword;
                    userService.UpdateUser("token", userData);

                }

                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }

                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");


            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];

                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }


        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            var externalLogins = new List<ExternalLogin>();
            foreach (var account in accounts)
            {
                var clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }
            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        public ActionResult UpdateAccount()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null) return RedirectToAction("Index", "Home");

            var userName = membershipUser.UserName;
            //userName = "StoraHovdingen";

            //Old user service
            //var userServiceClient = new UserServicePortTypeClient();
            //((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            //var auth = new AuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            //var user = new User();
            //End Old user service

            //New User service
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var user = new UserDTO();
            //End New User service
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userData = userServiceClient.GetUserByUsername(authToken, userName);
            try
            {
                //user = userServiceClient.getUserByUsername(auth, userName);
                user = userServiceClient.GetUserByUsername(authToken, userName);

                var setting = new SettingDTO();
                setting = new MrBillUserServicesClient().GetSetingById(authToken, userName, userData.UserId);
                ViewBag.emailSetting = setting;
            }

            catch (Exception)
            {
                FormsAuthentication.SignOut();
                TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
                return RedirectToAction("Index", "Home");
            }

        
            return View(user);
        }

        //
        // POST: /Account/UpdateAccount

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAccount(UserDTO model)
        {
            //DO THIS

            if (!ModelState.IsValid) return View(model);
            // ChangePassword will throw an exception rather than return false in certain failure scenarios.1
            var userServiceClient = new MrBillUserServicesClient();
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userName = Membership.GetUser().UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userData = userServiceClient.GetUserByUsername(authToken, userName);
            try
            {
                userData.UserId = userData.UserId;
                userData.FirstName = Request.Form["firstName"];
                userData.LastName = Request.Form["lastName"];
                //userData.specialRequest = Request.Form["specialRequest"];
                userData.Address = Request.Form["address"];
                userData.City = Request.Form["city"];
                userData.PostCode = Request.Form["PostCode"];
                userData.Country = Request.Form["country"];

                userServiceClient.UpdateUser(authToken, userData);
            }

            catch (Exception)
            {
                ViewBag.Error = "Något gick fel, vänligen försök igen";
            }
            // If we got this far, something failed, redisplay form
           
            return View(model);
        }

        //
        // POST: /Account/UpdateAccount

        public string AddOrUpdateSetting(string settingValue)
        {
            var userServiceClient = new MrBillUserServicesClient();
            var userName = Membership.GetUser().UserName;
            var authToken = MrBillUtility.GenerateAuthenticationToken(userName);
            var userData = userServiceClient.GetUserByUsername(authToken, userName);
            var client = new MrBillUserServicesClient();
            var settingDto = new SettingDTO();
            settingDto.UserID = userData.UserId;
            settingDto.SettingName = "email_setting";
            settingDto.SettingValue = settingValue;
            var result = client.AddOrUpdateSetting(authToken, userName, settingDto);

            return result;
        }




        [AllowAnonymous]
        public ActionResult UserConditions()
        {

            return View();
        }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("MyBookings", "Travel", null);

        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
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
        #endregion
    }


}


