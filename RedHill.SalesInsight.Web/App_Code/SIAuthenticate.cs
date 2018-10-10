using System;
using System.Text;
using System.Web;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.App_Code;

public class SIAuthenticate
{
    #region public static bool AuthenticateByRequest(HttpRequest request)

    public static bool AuthenticateByRequest(HttpRequest request)
    {
        // Get the vars
        return AuthenticateByUserID
        (
            SIQueryString.GetGuid(request, "userid"),
            SIQueryString.GetString(request, "key"),
            SIQueryString.GetString(request, "hash")
        );
    }

    #endregion

    #region public static bool AuthenticateByUserID(Guid userID, string salt, string hash)

    public static bool AuthenticateByUserID(Guid userID, string salt, string hash)
    {
        // Get the password
        string password = SIDAL.GetUserPassword(userID);

        // Authenticate
        return Authenticate(salt, password, hash);
    }

    #endregion

    #region public static bool Authenticate(string salt, string password, string hash)

    public static bool Authenticate(string salt, string password, string hash)
    {
        // Get the key
        string key = string.Format("{0}{1}", salt.Trim(), password.Trim()).ToLower();

        // Compute the hash
        string validateHash = SIHash.ComputeSHA256Hash(key, Encoding.Unicode);

        // Return the match
        return validateHash == hash;
    }

    #endregion
}