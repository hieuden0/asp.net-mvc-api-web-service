using System;
using System.Security.Cryptography;
using System.Text;

namespace MrBillAuthEncode
{
    public class MrBillEncode
    {
        public static string GetAuthenticationToken(string message, string keyCode)
        {

            if (string.IsNullOrEmpty(keyCode) || string.IsNullOrEmpty(message)) return null;
            //step 1:Get key byte
            keyCode = keyCode.Trim();
            message = message.Trim();
            MD5CryptoServiceProvider abc = new MD5CryptoServiceProvider();

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

            //step 3: encode
            try
            {
                ICryptoTransform encryptor = tdesAlgorithm.CreateEncryptor();
                byte[] messageByte = Encoding.UTF8.GetBytes(message);
                results = encryptor.TransformFinalBlock(messageByte, 0, messageByte.Length);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            finally
            {
                tdesAlgorithm.Clear();
            }
            //step 4: return base64String token
            return Convert.ToBase64String(results);
        }
    }
}
