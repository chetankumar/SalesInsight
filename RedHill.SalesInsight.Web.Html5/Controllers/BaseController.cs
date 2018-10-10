using Fio.Mvc.Helpers;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RestSharp;
using RestSharp.Authenticators;
//using Fio.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public partial class BaseController : Controller
    {
        public string DecryptCookie(string name)
        {
            try
            {
                return EncryptionHelper.Decrypt(Request.Cookies[name].Value);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void EncryptCookie(string name, string value, int expireInDays)
        {
            System.Web.HttpCookie cookie = new System.Web.HttpCookie(name, EncryptionHelper.Encrypt(value));
            //cookie.Secure = true;
            cookie.Expires.AddDays(expireInDays);
            Response.Cookies.Add(cookie);
        }

        protected FileContentResult SendExcel(byte[] fileBytes, string name)
        {
            return File(fileBytes, "application/vnd.ms-excel", name);

            //    Response.ClearContent();
            //    Response.ClearHeaders();
            //    Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
            //    Response.ContentType = "application/vnd.ms-excel";
            //    Response.BinaryWrite(fileBytes);
            //    Response.Flush();
            //    Response.Clear();
        }

        public void SendMail(string[] recipients, string subject, string message, string attachmentPath = null, string[] ccRecipients = null)
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
                var companyName = GetCompany();
                if (!string.IsNullOrWhiteSpace(companyName.Name))
                    request.AddParameter("from", companyName.Name + " <postmaster@readymixinsight.com>");
                else
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


                MailMessage mail = new MailMessage();
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


        public void SendMultipleAttachmentMail(string[] recipients, string subject, string message, List<string> attachmentPath = null, string quotePDFPath = "", string filePath = "", string[] ccRecipients = null)
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

                var companyName = GetCompany();
                if (!string.IsNullOrWhiteSpace(companyName.Name))
                    request.AddParameter("from", companyName.Name + " <postmaster@readymixinsight.com>");
                else
                    request.AddParameter("from", "Ready Mix Insight <postmaster@readymixinsight.com>");

                foreach (string r in recipients)
                    request.AddParameter("to", r);

                if (ccRecipients != null)
                {
                    foreach (string cc in ccRecipients)
                        request.AddParameter("cc", cc);
                }
                if (!string.IsNullOrEmpty(quotePDFPath))
                {
                    request.AddFile("attachment", quotePDFPath);
                }
                request.AddParameter("subject", subject);
                request.AddParameter("text", message);
                request.Method = Method.POST;
                if (attachmentPath.Count != 0)
                {
                    foreach (string tempPath in attachmentPath)
                    {
                        request.AddFile("attachment", tempPath);
                    }
                }
                var response = client.Execute(request);
                if (response.StatusCode.ToString() == "OK" && attachmentPath.Count != 0)
                {
                    Directory.Delete(Server.MapPath("~/tempAttachmentFiles/" + filePath + ""), true);
                }
                //MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTP_HOST"]);

                //mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_FROM"]);
                //foreach (string recipient in recipients)
                //{
                //    mail.To.Add(recipient);
                //}
                //if (ccRecipients != null)
                //{
                //    foreach (string recipient in ccRecipients)
                //    {
                //        mail.CC.Add(recipient);
                //    }
                //}
                //mail.Subject = subject;
                //mail.Body = message;

                //if (attachmentPath != null)
                //{
                //    foreach (string tempPath in attachmentPath)
                //    {
                //        System.Net.Mail.Attachment attachment;
                //        attachment = new System.Net.Mail.Attachment(tempPath);
                //        mail.Attachments.Add(attachment);
                //    }
                //}

                //SmtpServer.Port = Int32.Parse(ConfigurationManager.AppSettings["SMTP_PORT"]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTP_USERNAME"], ConfigurationManager.AppSettings["SMTP_PASSWORD"]);
                //SmtpServer.EnableSsl = ConfigurationManager.AppSettings["SMTP_TLS"] == "true";

                //SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetHostUrl
        {
            get
            {
                return Request.Url.GetLeftPart(UriPartial.Authority);
            }
        }

        public Company GetCompany()
        {
            if (Session["CompanyId"] != null)
            {
                return SIDAL.GetCompany(Int32.Parse(Session["companyId"].ToString()));
            }
            else
            {
                return SIDAL.GetCompany();
                //return SIDAL.GetUser(Membership.GetUser(User.Identity.Name).ProviderUserKey.ToString()).Company;
            }
        }

        public SIUser GetUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return SIDAL.GetUser(Membership.GetUser(User.Identity.Name).ProviderUserKey.ToString());
            }
            else
            {
                return null;
            }
        }

        public SIRoleAccess RoleAccess
        {
            get
            {
                RolePrincipal r = (RolePrincipal)User;
                string[] roles = r.GetRoles();
                if (roles != null && roles.Length > 0)
                {
                    return SIDAL.GetAccessRules(roles[0]);
                }
                else
                {
                    return new SIRoleAccess();
                }
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return new RedirectResult("/");
        }

        //Initialize the Access Rules to be available on the view layer.
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<string> ignoreActions = new List<string>();
            ignoreActions.Add("Logo");
            ignoreActions.Add("Login");
            ignoreActions.Add("SupportAttachments");
            ignoreActions.Add("RemoveSupportAttachment");
            ignoreActions.Add("SupportRequest");

            InitializeSessionData();
            if (!User.Identity.IsAuthenticated)
            {

                if (!ignoreActions.Contains(filterContext.ActionDescriptor.ActionName))
                {
                    //TempData["Url"] = System.Text.RegularExpressions.Regex.Replace(Request.RawUrl, @"http[^\s]+", "");
                    TempData["Url"] = Request.RawUrl;
                    filterContext.Result = new RedirectResult("/Login/Index?redirectUrl=" + TempData["Url"]);
                }
            }
        }

        private void InitializeSessionData()
        {
            if (Session["PipelineOptionalColumns"] == null)
            {
                Session["PipelineOptionalColumns"] = new List<string>();
            }
            if (Session["ForecastOptionalColumns"] == null)
            {
                Session["ForecastOptionalColumns"] = new List<string>();
            }
            if (Session["QuoteOptionalColumns"] == null)
            {
                Session["QuoteOptionalColumns"] = new List<string>();
            }
        }

        public string SaveOptionalColumns(string mode, string[] options)
        {
            if (mode == "pipeline")
            {
                if (options != null)
                    Session["PipelineOptionalColumns"] = options.ToList();
                else
                    Session["PipelineOptionalColumns"] = new List<string>();
            }
            if (mode == "forecast")
            {
                if (options != null)
                    Session["ForecastOptionalColumns"] = options.ToList();
                else
                    Session["ForecastOptionalColumns"] = new List<string>();
            }
            if (mode == "quote")
            {
                if (options != null)
                    Session["QuoteOptionalColumns"] = options.ToList();
                else
                    Session["QuoteOptionalColumns"] = new List<string>();
            }
            return "OK";
        }

        //Initialize the Access Rules to be available on the view layer.
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.AccessRule = RoleAccess;
            if (GetUser() != null)
            {
                Guid userId = GetUser().UserId;
                int unread = 0;
                List<Notification> notifications = SIDAL.GetLatestNotifications(userId, 10, out unread);
                if (notifications == null)
                {
                    notifications = new List<Notification>();
                }
                ViewBag.ShowESIModules = SIDAL.GetCompanySettings().ESIModules.GetValueOrDefault(false);
                ViewBag.UnreadNotifications = notifications;
                ViewBag.UnreadCount = unread;
            }
            else
            {

            }
        }
    }
}