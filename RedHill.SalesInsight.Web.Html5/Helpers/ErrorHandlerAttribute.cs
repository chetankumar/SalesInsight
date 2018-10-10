using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using RedHill.SalesInsight.DAL;
using System.Diagnostics;
using System.Web.Http.Filters;

namespace RedHill.SalesInsight.Web.Html5.Helpers
{
    public class ErrorHandlerAttribute : HandleErrorAttribute
    {
        public Company Company { get; set; }
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            //remove the following line to capture all exceptions.  this only lets Security exceptions through
            //if (filterContext.Exception.GetType() != typeof(SecurityException)) return;

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var url = filterContext.HttpContext.Request.Url.ToString();
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            filterContext.Result = new ViewResult
            {
                //name your view whatever you want and place a matching view in /Views/Shared
                ViewName = "NotFound",
                TempData = filterContext.Controller.TempData
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 403;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            this.Company = SIDAL.GetCompany();
            var newline = "<br/>";
            StackTrace st = new StackTrace(filterContext.Exception, true);
            //Get the first stack frame
            StackFrame frame = st.GetFrame(0);

            //Get the file name
            string fileName = frame.GetFileName();

            //Get the method name
            string methodName = frame.GetMethod().Name;

            //Get the line number from the stack frame
            int line = frame.GetFileLineNumber();
            
            //Get the column number
            int col = frame.GetFileColumnNumber();
            var messageBody = newline + "Hi," + newline + "An exception occurred in a Application.Below are the details of the exception" + newline +
                "<b> URL: </b> " + " " + url + newline +
                "<b> Log Created Date: </b> " + " " + DateTime.Now.ToString() + newline +
                "<b> Error Message: </b> " + " " + filterContext.Exception.Message.ToString() + newline + newline +
                "<b> Exception Type: </b> " + " " + filterContext.Exception.GetType().ToString() + newline + newline +
                "<b> Exception Details: </b> " + " " + filterContext.Exception.ToString() + "</b>" + newline + newline + newline +
                "Thanks and Regards";
            var exceptionMailTo = ConfigurationManager.AppSettings["ExceptionMailTo"];
            SendMail(new string[]{ exceptionMailTo },"Exception Occured in "+ this.Company.Name, messageBody, null,null,false);
        }


        public void SendMail(string[] recipients, string subject, string message, string attachmentPath = null, string[] ccRecipients = null, bool? isBodyHtml = false)
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
                request.AddParameter("from", "Ready Mix Insight <postmaster@readymixinsight.com>");
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
                if (attachmentPath != null)
                {
                    //var fileName = Path.GetFileName(attachmentPath);
                    //request.AddParameter("attachment", "@" + fileName);
                    request.AddFile("attachment", attachmentPath);
                }

                var response = client.Execute(request);


                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTP_HOST"]);

                mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_FROM"]);
                foreach (string recipient in recipients)
                {
                    mail.To.Add(recipient);
                }
                if (ccRecipients != null)
                {
                    foreach (string recipient in ccRecipients)
                    {
                        mail.CC.Add(recipient);
                    }
                }
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = isBodyHtml.GetValueOrDefault();

                if (attachmentPath != null)
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);
                }

                SmtpServer.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTP_PORT"]);
                SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTP_USERNAME"], ConfigurationManager.AppSettings["SMTP_PASSWORD"]);
                SmtpServer.EnableSsl = ConfigurationManager.AppSettings["SMTP_TLS"] == "true";

                //SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}