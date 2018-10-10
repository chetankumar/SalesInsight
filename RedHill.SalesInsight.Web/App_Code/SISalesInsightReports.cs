using System.Web;

public class SISalesInsightReports
{
    //---------------------------------
    // Methods
    //---------------------------------

    #region public static string GetReportsUrl(HttpRequest request)

    public static string GetReportsUrl(HttpRequest request)
    {
        return string.Format
        (
            "{0}Reports",
            request.ApplicationPath.Length <= 1 ? "/" : request.ApplicationPath + "/"
        );
    }

    #endregion
}