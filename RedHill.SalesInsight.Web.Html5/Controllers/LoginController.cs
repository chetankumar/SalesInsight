using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL.Utilities;
using RedHill.SalesInsight.Logger;
using RedHill.SalesInsight.Web.Html5.Helpers;
using RedHill.SalesInsight.Web.Html5.Models;
using RedHill.SalesInsight.Web.Html5.Utils;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mail;
using System.Web.Mvc;
using System.Web.Security;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class LoginController : Controller
    {

        //
        // GET: /Login/

        public ActionResult Index(string redirectUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return new RedirectResult("/Home/Pipeline");
            if (redirectUrl != null)
                ViewBag.RedirectUrl = redirectUrl;
            else
                ViewBag.RedirectUrl = "";
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(LoginView loginView)
        {
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["SalesInsightConnectionString"].ConnectionString);
            connection.Open();

            if (ModelState.IsValid)
            {
                String username = loginView.Username;
                String password = loginView.Password;
                SIUser user = SIDAL.ValidateUser(username, password);

                if (user == null)
                {
                    ViewBag.AuthenticationMessage = "Authentication failed for these credentials";
                    return View("Index");
                }
                else
                {
                    var member = SIDAL.FindUserByName(username);
                    var currentuser = SIDAL.FindByUserId(member.UserId);
                    var sus = SIDAL.FindSuperUserSettings();
                    if (currentuser.LastPasswordChangedDate <= DateTime.Now.AddDays(-sus.MaximumPasswordAge))
                    {
                        TempData["Error"] = "Your password has expired. Please create a new password.";
                        string token = PasswordUtils.GeneratePasswordResetToken(username, 2 * 24 * 60);
                        SIDAL.SaveUserPasswordToken(username, token);
                        return RedirectToAction("ResetPassword", new { @token = token, @u = member.UserId });
                    }
                    FormsAuthentication.SetAuthCookie(user.Username.ToString(), true);
                    if (!string.IsNullOrEmpty(loginView.RedirectUrl))
                    {
                        string url = loginView.RedirectUrl;
                        Regex regx = new Regex(@"http[^\s]+");
                        var isLinkExist = regx.IsMatch(url);
                        //System.Text.RegularExpressions.Regex.Replace(loginView.RedirectUrl, @"http[^\s]+", "");
                        if (isLinkExist)
                        {
                            url = "/Home";
                        }
                        return new RedirectResult(url);
                    }
                    else
                        return new RedirectResult("/Home");
                }
            }
            else
            {
                ViewBag.AuthenticationMessage = "Please fill the required fields.";
                return View("Index");
            }
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult ResetPassword(string token, Guid? u = null)
        {
            ChangePasswordView model = new ChangePasswordView();

            model.UserId = u.GetValueOrDefault();
            model.Token = token;
            string user = Membership.GetUser(u).UserName;
            var canUseToken = SIDAL.ValidatePasswordToken(user, token);
            if (canUseToken)
            {
                return View(model);
            }
            else
            {
                ViewBag.Message = "Your password reset link has been expired. Please Contact your administrator to generate new password link.";
                return View("Index");
            }
        }

        [HttpPost]
        public JsonResult ResetPassword(string oldPassword, string newPassword, string confirmPassword, Guid? u = null, string token = null)
        {
            List<string> statusList = new List<string>();
            var username = "";
            var userId = u.GetValueOrDefault();
            var m = Membership.GetUser(userId);
            username = m.UserName;
            var password = SIDAL.GetUserPassword(userId);

            try
            {
                if (token != null)
                {
                    SIDAL.UpdateUserPasswordToken(username, token);
                }
                if (!String.IsNullOrEmpty(oldPassword))
                {
                    if (!String.IsNullOrEmpty(newPassword) && !String.IsNullOrEmpty(confirmPassword))
                    {
                        if (newPassword.Equals(confirmPassword))
                        {
                            if (password == oldPassword)
                            {

                                // Function to fetch the super User Settings
                                var dbObj = SIDAL.FindSuperUserSettings();
                                if (dbObj != null)
                                {
                                    int minLength = dbObj.MinimumLength;
                                    bool RequireOneCaps = dbObj.RequireOneCaps;
                                    bool RequireOneLower = dbObj.RequireOneLower;
                                    bool RequireOneDigit = dbObj.RequireOneDigit;
                                    bool RequireSpecialChar = dbObj.RequireSpecialChar;
                                    int PasswordHistoryLimit = dbObj.PasswordHistoryLimit;
                                    int MaximumPasswordAge = dbObj.MaximumPasswordAge;

                                    //Access Utill Class for password Validation
                                    PasswordUtility passValidation = new PasswordUtility();

                                    statusList = passValidation.ValidatePassword(newPassword, minLength, RequireOneCaps, RequireOneLower, RequireOneDigit, RequireSpecialChar, PasswordHistoryLimit, MaximumPasswordAge);

                                    if (statusList.Count() == 0)
                                    {
                                        bool canUse = SIDAL.CanUsePassword(username, newPassword);
                                        if (canUse)
                                        {

                                            SIDAL.ResetPassword(username, newPassword);

                                            try
                                            {
                                                //Send Password Changed Email
                                                MailUtils.SendMail(ConfigurationHelper.Company.Name, new string[] { m.Email }, "Password update notification", "Hi! <br/>Your password has been updated. <br/> If you haven't changed it please contact the System Administrator.");
                                            }
                                            catch { }

                                            FormsAuthentication.SignOut();
                                            SIDAL.SaveUpdatePasswordHistory(username, newPassword, MaximumPasswordAge);
                                        }
                                        else
                                        {
                                            if (PasswordHistoryLimit > 1)
                                            {
                                                statusList.Add("You can not reuse your last " + PasswordHistoryLimit + " " + " passwords. Please enter a diffrent password.");
                                            }
                                            else
                                            {
                                                statusList.Add("You can not reuse your last password. Please enter a diffrent password.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { statusList }, JsonRequestBehavior.AllowGet);
                                    }   // Returns List of error and display on the pop up menu..
                                }
                            }
                            else
                            {
                                statusList.Add("Current password is incorrect. try again.");
                                return Json(new { statusList }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            statusList.Add("New password does not match the confirm password.");
                            return Json(new { statusList }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        statusList.Add("Please Enter new Password.");
                        return Json(new { statusList }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    statusList.Add("Password can not be Empty.");
                    return Json(new { statusList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                statusList.Add("Password can not be Empty.");
            }
            return Json(new { statusList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ChangePassword(string username, string token, int newUser = 0)
        {
            var canUseToken = SIDAL.ValidatePasswordToken(username, token);
            if (canUseToken)
            {
                ChangePasswordView model = new ChangePasswordView(username, token, newUser);
                return View("ChangePassword", model);
            }
            else
            {
                ViewBag.Error = "Your password link has been expired.Please generate a new  link to change your password.";
                return View("ForgotPassword");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string username)
        {
            var u = SIDAL.FindUserByName(username);
            Guid userid;
            string email = string.Empty;
            string userName = string.Empty;
            if (u != null)
            {
                userid = u.UserId;
                userName = u.UserName;
                var membership = SIDAL.FindByUserId(userid);
                email = membership.Email;
            }
            if (u == null)
            {
                var s = SIDAL.FindByEmail(username);
                if (s != null)
                {
                    var user = SIDAL.FindUserByEmail(username);
                    email = s.Email;
                    userName = user.UserName;
                    userid = user.UserId;
                }

                if (s == null)
                {
                    ViewBag.Error = "No user was found with that username or email";
                    return View("ForgotPassword");
                }
            }
            try
            {

                string path = "~/Mailers/ResetPassword.html";
                path = Server.MapPath(path);

                string token = PasswordUtils.GeneratePasswordResetToken(userName, 7 * 24 * 60);
                SIDAL.SaveUserPasswordToken(userName, token);
                string link = GetRootUrl() + string.Format("/Login/ChangePassword?token={0}&username={1}", token, userName);

                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("@Datetime", DateTime.Now.ToString());
                values.Add("@UserName", userName);
                values.Add("@ResetTokenLink", link);

                string mailerBody = MailerUtils.FillTemplate(path, values);
                try
                {
                    //Send Password Changed Email
                    MailUtils.SendMail(ConfigurationHelper.Company.Name, new string[] { email }, "Password Reset", mailerBody);
                }
                catch { }
                //SendMail(new string[] { email }, "Password Reset", mailerBody, null, null, true);

                //string emailToShow = email.Substring(0, 2).PadRight(10, '*') + email.Substring(email.Length - 4);

                ViewBag.Message = String.Format("Reset password instructions has been sent to your registered email.");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "Failed to send the Reset Password link. Please contact support.";
            }
            return View("ForgotPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword(ChangePasswordView model)
        {
            List<string> statusList = new List<string>();
            var user = SIDAL.FindUserByName(model.UserName);
            if (user == null)
            {
                ViewBag.Message = "The user is not on the system. Please generate the reset password link again";
                return View(model);
            }
            //aspnet_User profile = SIDAL.FindByUserId(user.UserId);
            string userName = user.UserName;
            var m = SIDAL.FindByUserId(user.UserId);
            try
            {
                SIDAL.UpdateUserPasswordToken(model.UserName, model.Token);
                if (!String.IsNullOrEmpty(model.NewPassword) && !String.IsNullOrEmpty(model.ConfirmPassword))
                {
                    if (model.NewPassword.Equals(model.ConfirmPassword))
                    {
                        // Function to fetch the super User Settings

                        var dbObj = SIDAL.FindSuperUserSettings();
                        if (dbObj != null)
                        {
                            int minLength = dbObj.MinimumLength;
                            bool RequireOneCaps = dbObj.RequireOneCaps;
                            bool RequireOneLower = dbObj.RequireOneLower;
                            bool RequireOneDigit = dbObj.RequireOneDigit;
                            bool RequireSpecialChar = dbObj.RequireSpecialChar;
                            int PasswordHistoryLimit = dbObj.PasswordHistoryLimit;
                            int MaximumPasswordAge = dbObj.MaximumPasswordAge;

                            // Access Utility Class for password Validation
                            PasswordUtility passValidation = new PasswordUtility();

                            statusList = passValidation.ValidatePassword(model.NewPassword, minLength, RequireOneCaps, RequireOneLower, RequireOneDigit, RequireSpecialChar, PasswordHistoryLimit, MaximumPasswordAge);

                            if (statusList.Count() == 0)
                            {
                                bool canUse = SIDAL.CanUsePassword(userName, model.NewPassword);
                                if (canUse)
                                {

                                    SIDAL.ChangePassword(model.UserName, model.NewPassword, m.Email);

                                    SIDAL.SaveUpdatePasswordHistory(userName, model.NewPassword, MaximumPasswordAge);

                                    try
                                    {
                                        //Send Password Changed Email
                                        var companyName = SIDAL.GetCompanyNameByUserId(user.UserId);
                                        MailUtils.SendMail(companyName, new string[] { m.Email }, "Password update notification", "Hi! <br/>Your password has been updated. <br/> If you haven't changed it please contact the System Administrator.");
                                    }
                                    catch { }
                                }
                                else
                                {
                                    if (PasswordHistoryLimit > 1)
                                    {
                                        statusList.Add("You can not reuse your last " + PasswordHistoryLimit + " " + " passwords. Please enter a diffrent password.");
                                    }
                                    else
                                    {
                                        statusList.Add("You can not reuse your last password. Please enter a diffrent password.");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        statusList.Add("New password does not match with confirm password.");
                    }
                }
                else
                {
                    statusList.Add("Please enter the new password.");
                }
            }
            catch
            {
                statusList.Add("Something went wrong while changing the password. Please inform your administrator.");
            }
            if (statusList.Count() > 0)
            {
                ViewBag.PasswordErrors = statusList;
                return View(model);
            }
            return RedirectToAction("Index");
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
            if (when < DateTime.UtcNow.AddHours(48))
            {
                result.Errors.Add(TokenValidationStatus.Expired);
            }

            return result;
        }

        public string GetRootUrl()
        {
            return Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
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

        public ActionResult ImportAllEntities()
        {
            FileLogger logger = new FileLogger();
            AUJSIntegration.Data.SyncManager syncManager = new AUJSIntegration.Data.SyncManager(SIDAL.DefaultCompany().CompanyId, logger);
            bool success = false;
            try
            {
                syncManager.Import();
                success = true;
            }
            catch (Exception ex)
            {
                //Log exception here
                logger.LogError(ex.Message + " - " + ex.StackTrace);
            }
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
    }
}
