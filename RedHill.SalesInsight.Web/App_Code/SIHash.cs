using System;
using System.Security.Cryptography;
using System.Text;

namespace RedHill.SalesInsight.Web.App_Code
{
    public class SIHash
    {
        #region public static string ComputeSHA256Hash(string text, Encoding encoding)

        public static string ComputeSHA256Hash(string text, Encoding encoding)
        {
            // Create the hash
            SHA256Managed hash = new SHA256Managed();

            // Get the has bytes
            byte[] hased = hash.ComputeHash(encoding.GetBytes(text));

            // Get the base64 of that
            return Convert.ToBase64String(hased);
        }

        #endregion

        #region public static string GenerateSalt()

        public static string GenerateSalt()
        {
            // Create the provider to create the salt
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // Create the buffer
            byte[] saltBuffer = new byte[16];

            // Get the bytes
            rng.GetBytes(saltBuffer);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(saltBuffer);
        }

        #endregion
    }
}