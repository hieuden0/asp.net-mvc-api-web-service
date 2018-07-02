using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MrBill_MVC.Utilities
{
    public class MrBillUtility
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        public static string RandomString(int size)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, size)
              .Select(s => s[random.Next(s.Length)]).ToArray());

        }
        public static string GenerateAuthenticationToken(string username)
        {
            try
            {
                var disposeTime = DateTime.UtcNow.AddMinutes(5).Ticks.ToString();
                var token = MrBillAuthEncode.MrBillEncode.GetAuthenticationToken(disposeTime, username);
                return token;
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}