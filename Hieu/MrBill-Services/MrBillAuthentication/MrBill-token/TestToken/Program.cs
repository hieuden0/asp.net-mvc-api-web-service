using System;
using MrBillAuthDecode;
using MrBillAuthEncode;

namespace TestToken
{
    class Program
    {
        private static void Main(string[] args)
        {
            //example 
            string message = DateTime.UtcNow.AddMinutes(5).Ticks.ToString();
            string username = "pierre@mrbill.se";
            string token = MrBillEncode.GetAuthenticationToken(message, username);
            //string token = "mIiGKFMnu9PhPzk95gWAe1beIny55gdo";
            string decodeMessage = MrBillDecode.MrBillAuthDecode(token, username);
            try
            {
                DateTime checkToken = new DateTime(long.Parse(decodeMessage));
                if (checkToken.CompareTo(DateTime.UtcNow) > 0)
                {
                    string succesfull = "token is valid";
                }
            }
            catch (Exception exception)
            {
                string tokenInValud = "Token is invalid";
            }

           
        }
    }
}
