using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MrBillAuthDecode
{
    public class MrBillDecode
    {
        public static string MrBillAuthDecode(string token, string keyCode)
        {
            if (string.IsNullOrEmpty(keyCode) || string.IsNullOrEmpty(token)) return null;
            keyCode = keyCode.Trim();
            token = token.Trim();
            byte[] results;
            //step 1:Get key byte
            if (keyCode.Length < 16)
            {
                keyCode += new string('0', 16 - keyCode.Length);

            }
            keyCode = keyCode.Substring(0, 12) + "<#7a";
            byte[] tdesKey = Encoding.UTF8.GetBytes(keyCode);
            //step 2: Config 
            TripleDESCryptoServiceProvider tdesAlgorithm = new TripleDESCryptoServiceProvider
            {
                Key = tdesKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            //step 3: dencode
            try
            {
                ICryptoTransform decryptor = tdesAlgorithm.CreateDecryptor();
                byte[] dataToDecrypt = Convert.FromBase64String(token);
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            catch(Exception exception)
            {
                return exception.Message;
            }
            finally
            {
                tdesAlgorithm.Clear();
            }
            //step 4: return message
            return Encoding.UTF8.GetString(results);
        }
    }
}
