using System;
using System.ServiceModel;
using System.Web;

public class SISalesInsightService
{
    //---------------------------------
    // Methods
    //---------------------------------

    #region public static string GetServiceUrl(HttpRequest request)

    public static string GetServiceUrl(HttpRequest request)
    {
        return string.Format
        (
            "{0}Services/SalesInsight.svc",
            request.ApplicationPath.Length <= 1 ? "/" : request.ApplicationPath + "/"
        );
    }

    #endregion

    /*#region public static SalesInsightClient GetSalesInsightClient(HttpRequest request)

    public static SalesInsightClient GetSalesInsightClient(HttpRequest request)
    {
        // Create the binding
        string relativePath = GetServiceUrl(request);

        // Create the service path
        string completePath = string.Format
        (
            "{0}://{1}:{2}{3}",
            request.Url.Scheme,
            request.Url.Host,
            request.Url.Port,
            relativePath
        );

        SILog.Write(SILogType.Info, "WEB", completePath);

        // Create the binding
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.MaxReceivedMessageSize = int.MaxValue;
        binding.MaxBufferSize = int.MaxValue;

        // Create the service
        return new SalesInsightClient(binding, new EndpointAddress(completePath));
    }

    #endregion*/

    #region public static SalesInsightClient GetSalesInsightClient(HttpRequest request)

    

    #endregion

    #region public static Uri GetRealRequestUri(HttpRequest request)

    public static Uri GetRealRequestUri(HttpRequest request)
    {
        if(string.IsNullOrEmpty(request.Headers["Host"]))
        {
            return request.Url;
        }

        UriBuilder ub       = new UriBuilder(request.Url);
        string[] realHost   = request.Headers["Host"].Split(':');
        string host         = realHost[0];

        ub.Host = host;

        string portString = realHost.Length > 1 ? realHost[1] : "";
        int port;

        if(int.TryParse(portString, out port))
        {
            ub.Port = port;
        }
        
        return ub.Uri;
    }

    #endregion

}
