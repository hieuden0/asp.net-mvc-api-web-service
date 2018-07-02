using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MrBill_MVC.Models;
using WebMatrix.WebData;

namespace MrBill_MVC.Controllers
{
    [Authorize]
    [RoutePrefix("api/user")]
    public class LoginWebServiceController : ApiController
    {
        //http://localhost:39764/api/LoginWebService
        //{
        //  "username": "pierre@mrbill.se",
        //  "password": "Rapide10"
        //}

        // POST api/User/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public HttpResponseMessage LoginUser(LoginModel model)
        {
            // Invoke the "token" OWIN service to perform the login: /api/token
            // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                var seccessMessage = new HttpResponseMessage()
                {
                    Content = new StringContent("Successfully logged in")
                };
                return seccessMessage;
            }
            else
            {
                var errorMessage = new HttpResponseMessage()
                {
                    Content = new StringContent("error logging in")
                };
                return errorMessage;
            }
        }



    }
}
