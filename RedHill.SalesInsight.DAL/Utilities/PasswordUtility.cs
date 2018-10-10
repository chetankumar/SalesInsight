using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public class PasswordUtility
    {
        public List<string> Errors { get; set; }

        public char[] UpperCase = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public char[] LowerCase = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public char[] DigitContains = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public char[] SpecialTypeCharacters = new char[] { ';', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '{', '}', '[', ']', '+', '-', '|', '\'', '"', '/', '\\', ':', '<', '>', '~', '`' };

        public List<string> ValidatePassword(string password, int minLength, bool requiredCaps, bool requireLower, bool requireDigit, bool requireSplChar, int passwordHisLim, int maxPasswordAge)
        {
            bool UpperCaseStatus = false;
            bool LowerCaseStatus = false;
            bool DigitContainStatus = false;
            bool SpecialCharacterStatus = false;
            bool minLengthStatus = false;

            List<string> errors = new List<string>();
            List<string> success = new List<string>();

            if (requiredCaps == true)
            {
                for (int i = 0; i < password.Length; i++)
                {
                    for (int j = 0; j < UpperCase.Length; j++)
                    {
                        if (UpperCase.Contains(password[i]))
                        {
                            UpperCaseStatus = true;
                            break;
                        }
                        else
                            continue;
                    }
                }

            }
            if (requireLower == true)
            {
                for (int i = 0; i < password.Length; i++)
                {
                    for (int j = 0; j < LowerCase.Length; j++)
                    {
                        if (LowerCase.Contains(password[i]))
                        {
                            LowerCaseStatus = true;
                            break;
                        }
                        else
                            continue;
                    }
                }
            }
            if (requireDigit == true)
            {
                for (int i = 0; i < password.Length; i++)
                {
                    for (int j = 0; j < DigitContains.Length; j++)
                    {
                        if (DigitContains.Contains(password[i]))
                        {
                            DigitContainStatus = true;
                            break;
                        }
                        else
                            continue;
                    }
                }
            }
            if (requireSplChar == true)
            {
                for (int i = 0; i < password.Length; i++)
                {
                    for (int j = 0; j < SpecialTypeCharacters.Length; j++)
                    {
                        if (SpecialTypeCharacters.Contains(password[i]))
                        {
                            SpecialCharacterStatus = true;
                            break;
                        }
                        else
                            continue;
                    }
                }
            }
            if (minLength > 0)
            {
                if (password.Length >= minLength)
                {
                    minLengthStatus = true;
                }
            }


            if (requireSplChar == true && SpecialCharacterStatus == false)
            {
                errors.Add("Password must contain at least one Special Character");
            }
            if (requiredCaps == true && UpperCaseStatus == false)
            {
                errors.Add("Password must contain at least one upper case letter");
            }
            if (requireLower == true && LowerCaseStatus == false)
            {
                errors.Add("Password must contain at least one lower case letter");
            }
            if (requireDigit == true && DigitContainStatus == false)
            {
                errors.Add("Password must contain one digit from 0-9");
            }
            if (minLength > 0 && minLengthStatus == false)
            {
                errors.Add("Password must be at least " + minLength + " characters in length");
            }

            if (errors != null)
                return errors;
            else
            {
                success.Add("Reset Password Success.");
                return success;
            }


        }

        public static string OneWayHash(string password)
        {
            string secretKey = password;
            string salt = "redhill_advisors_1234567@#@!!";
            SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] preHash = System.Text.Encoding.UTF32.GetBytes(secretKey + salt);
            byte[] hash = sha.ComputeHash(preHash);
            string hashedPassword = System.Convert.ToBase64String(hash, 0, 15);
            return hashedPassword;
        }
    }
}
