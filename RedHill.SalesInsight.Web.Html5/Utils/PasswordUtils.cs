using Org.BouncyCastle.Asn1.Ocsp;
using RedHill.SalesInsight.DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using RedHill.SalesInsight.Web.Html5.Models;
namespace RedHill.SalesInsight.Web.Html5.Utils
{
    public class PasswordUtils
    {
        public static string GeneratePasswordResetToken(string username, int tokenExpirationInMinutes)
        {
            byte[] _time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            //byte[] _key = Guid.Parse(user.SecurityStamp).ToByteArray();

            byte[] data = new byte[_time.Length + username.Length];

            System.Buffer.BlockCopy(_time, 0, data, 0, _time.Length);

            System.Buffer.BlockCopy(_time, 0, data, 0, _time.Length);

            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, Convert.ToBase64String(data.ToArray()));
                return hash;
            }
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public TokenValidation ValidateToken(string reason, User user, string token)
        {
            var result = new TokenValidation();
            byte[] data = Convert.FromBase64String(token);
            byte[] _time = data.Take(8).ToArray();
            byte[] _key = data.Skip(8).Take(16).ToArray();
            byte[] _reason = data.Skip(24).Take(4).ToArray();
            byte[] _Id = data.Skip(28).ToArray();

            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(_time, 0));
            if (when < DateTime.UtcNow.AddHours(-24))
            {
                result.Errors.Add(TokenValidationStatus.Expired);
            }

            return result;
        }

       
    }
}