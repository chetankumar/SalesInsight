using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Utilities;
using RedHill.SalesInsight.Web.Html5.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class SupportRequestView
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    _id = Guid.NewGuid();
                }
                return _id;
            }
        }
        public string RequestId { get; set; }
        public Guid UserId { get; set; }
        public string BrowserSpecs { get; set; }
        public string ScreenResolution { get; set; }
        public int Category { get; set; }
        public string UrgencyLevel { get; set; }
        public string Phone { get; set; }
        public string ContactMedium { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }

        public List<string> Attachments { get; set; }
        public List<string> AttachmentPaths { get; set; }

        public void Save()
        {
            var currentContext = HttpContext.Current;

            var browser = currentContext.Request.Browser;
            //Save Support Request
            string browserDetails = string.Format("{0} {1}", browser.Browser, browser.Version);
            this.BrowserSpecs = string.IsNullOrWhiteSpace(browserDetails) ? currentContext.Request.UserAgent : browserDetails;
            string directory = Path.Combine(HttpContext.Current.Server.MapPath("~/Temp/"), "support_request");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var entity = this.ToEntity();
            //Save attachments
            var attachments = new List<SupportRequestAttachment>();
            //this.Attachments = new List<string>();
            this.AttachmentPaths = new List<string>();
            SupportRequestAttachment sra = null;
            var utils = new AwsUtils();
            if (this.Attachments != null)
            {
                foreach (var attachment in this.Attachments)
                {
                    sra = new SupportRequestAttachment();

                    string key = AwsUtils.GenerateKey("companies", this.CompanyId, "support_request", "attachments", attachment);
                    var s3Url = utils.GetPreSignedURL(key, DateTime.Now.AddMinutes(2 * 24 * 60));
                    string filePath = Path.Combine(directory, attachment);

                    utils.UploadFile(filePath, key, null);

                    this.AttachmentPaths.Add(filePath);

                    sra.Id = Guid.NewGuid();
                    sra.AttachmentLink = s3Url;
                    sra.SupportRequest = entity;
                    sra.CreatedAt = DateTime.Now;

                    attachments.Add(sra);
                }
            }

            SIDAL.AddSupportRequest(entity, attachments);

            var user = SIDAL.GetUser(this.UserId);

            if (user != null)
            {
                this.Name = user.Name;
            }

            SupportRequest request = SIDAL.GetSupportRequest(entity.Id);

            //Email Support Request
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("@RequestId", request.RequestId);
            values.Add("@UrgencyLevel", request.UrgencyLevel);
            values.Add("@Category", request.SupportCategory.Name);
            values.Add("@Description", request.Description);
            values.Add("@CreatedAt", request.CreatedAt.GetValueOrDefault().ToString("MM/dd/yyyy hh:mm tt"));
            values.Add("@BrowserSpecs", request.BrowserSpecs);
            values.Add("@ScreenResolution", request.ScreenResolution);
            values.Add("@Name", (this.Name == null ? (request.aspnet_User != null ? request.aspnet_User.UserName : "") : this.Name));
            values.Add("@Email", request.Email);
            values.Add("@Phone", request.Phone);
            values.Add("@ContactMedium", request.ContactMedium);

            string subject = string.Format("{0} - {1} [{2}]", this.CompanyName, request.SupportCategory.Name, request.RequestId);
            string body = MailerUtils.FillTemplate(currentContext.Server.MapPath("~/Mailers/SupportRequest.html"), values);

            string[] recipients = SIDAL.GetSupportRecipients(request.CategoryId.GetValueOrDefault()).ToArray();

            MailUtils.SendMail(this.CompanyName, recipients, subject, body, this.AttachmentPaths, null, null, null);

            string userSubject = string.Format("{0} - {1} [{2}]", this.CompanyName, "Confirmation for Support Request", request.RequestId);

            string userMessageBody = "Thank you for submitting your support request. A representative will contact you within 24 hours.<br/><br/>" + body;

            //Send copy of email to user
            MailUtils.SendMail(this.CompanyName, new string[] { this.Email }, userSubject, userMessageBody, this.AttachmentPaths, null, DeleteAttachments, null);
        }

        private bool DeleteAttachments()
        {
            //TODO: Write code to delete attachments
            return false;
        }

        public SupportRequest ToEntity()
        {
            var sRequest = new SupportRequest();
            sRequest.Id = this.Id;
            sRequest.UserId = this.UserId != Guid.Empty ? this.UserId : (Guid?)null;
            sRequest.BrowserSpecs = this.BrowserSpecs;
            sRequest.CategoryId = this.Category;
            sRequest.ScreenResolution = this.ScreenResolution;
            sRequest.Phone = this.Phone;
            sRequest.Email = this.Email;
            sRequest.UrgencyLevel = this.UrgencyLevel;
            sRequest.ContactMedium = this.ContactMedium;
            sRequest.Description = this.Description;
            sRequest.CreatedAt = DateTime.Now;

            return sRequest;
        }
    }
}