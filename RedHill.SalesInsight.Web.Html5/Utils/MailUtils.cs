using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Utils
{
    public class MailUtils
    {
        public static bool SendMail(string fromName, string[] recipients, string subject, string message, List<string> attachments = null, string[] ccRecipients = null, Func<bool> onSuccess = null, Func<bool> onFailure = null)
        {
            try
            {
                RestClient client = new RestClient();
                client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                client.Authenticator =
                        new HttpBasicAuthenticator("api",
                                                   "key-d3164a3582602b12f0c3878fcd0a2c05");
                RestRequest request = new RestRequest();
                request.AlwaysMultipartFormData = true;
                request.AddParameter("domain",
                                     "readymixinsight.com", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";

                if (!string.IsNullOrWhiteSpace(fromName))
                    request.AddParameter("from", fromName + " <postmaster@readymixinsight.com>");
                else
                    request.AddParameter("from", "Ready Mix Insight <postmaster@readymixinsight.com>");
                // request.AddParameter("from", companyName.Name + "<postmaster@readymixinsight.com>");
                foreach (string r in recipients)
                    request.AddParameter("to", r);

                if (ccRecipients != null)
                {
                    foreach (string cc in ccRecipients)
                        request.AddParameter("cc", cc);
                }

                request.AddParameter("subject", subject);
                request.AddParameter("html", message);


                request.Method = Method.POST;
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                        request.AddFile("attachment", attachment);
                }

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK && attachments != null)
                {
                    if (onSuccess != null)
                        return onSuccess();
                }
                else
                {
                    if (onFailure != null)
                        return onFailure();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}