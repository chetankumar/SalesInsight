using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace RedHill.SalesInsight.SyncIgnitor
{
    public class AppSettings
    {
        public static string SIAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SIAPIUrl"];
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (AppSettings.SIAPIUrl == null || AppSettings.SIAPIUrl.Trim().Length == 0)
                    throw new ApplicationException("Invalid API Url");

                string response = "";
                using (WebClient client = new WebClient())
                {
                    response = client.DownloadString(AppSettings.SIAPIUrl);
                }
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
