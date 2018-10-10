using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Web.Html5.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;


namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationProfile
    {
        public long QuoteId { get; set; }
        public long CompanyId { get; set; }
        public long QuoteRefNumber
        {
            get
            {
                return QuoteId;
            }
        }

        public DateTime CreationDate { get; set; }
        public Guid CreatedBy { get; set; }

        public DateTime ApprovalDate { get; set; }
        public string PushedBy { get; set; }
        public DateTime? LastPushedAt { get; set; }
        public Guid ApprovedBy { get; set; }
        public string ApproverName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string CustomerJobRef { get; set; }
        public string BidDate { get; set; }
        public string StartDate { get; set; }
        public string ProjectToJob { get; set; }
        public string DistanceToJob { get; set; }
        public string DeliveryInstructions { get; set; }
        public string CustomerDispatchId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }

        public string TaxCode { get; set; }
        public string TaxExceptReason { get; set; }

        public int PlantId { get; set; }
        public int AggregatePlantId { get; set; }
        public int BlockPlantId { get; set; }
        public string PlantName { get; set; }
        public string PlantDispatchId { get; set; }
        public DateTime? PricingMonth { get; set; }

        public int DistrictId { get; set; }
        public string DistrictName { get; set; }

        public DateTime? QuoteDate { get; set; }
        public DateTime? AcceptanceExpiration { get; set; }
        public DateTime? QuoteExpiration { get; set; }

        public string Status { get; set; }
        public bool EnableEdit { get; set; }
        public bool AddAttachments { get; set; }
        public bool Awarded { get; set; }
        public bool Active { get; set; }
        public string MarketSegment { get; set; }
        public string SalesStaffName { get; set; }
        public SalesStaff SalesStaff { get; set; }
        public int ToJob { get; set; }
        public int WaitOnJob { get; set; }
        public int Unload { get; set; }
        public int Wash { get; set; }
        public int Return { get; set; }

        public double TotalVolume { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal SackPrice { get; set; }
        public decimal AverageSellingPrice { get; set; }
        public double AverageLoad { get; set; }
        public double AverageUnLoad { get; set; }

        public decimal QuoteSpread { get; set; }
        public decimal QuoteContribution { get; set; }
        public decimal QuoteProfit { get; set; }
        public double QuoteCydHr { get; set; }

        public decimal DistrictSpread { get; set; }
        public decimal DistrictContribution { get; set; }
        public decimal DistrictProfit { get; set; }
        public double DistrictCydHr { get; set; }

        public decimal VarianceSpread { get { return QuoteSpread - DistrictSpread; } }
        public decimal VarianceContribution { get { return QuoteContribution - DistrictContribution; } }
        public decimal VarianceProfit { get { return QuoteProfit - DistrictProfit; } }
        public double VarianceCydHr { get { return QuoteCydHr - DistrictCydHr; } }
        public long? BackupPlantId { get; set; }
        // Quote Approval Attributess

        public string[] ApprovalRecipients { get; set; }
        public string[] CCRecipients { get; set; }
        public string ApprovalSubject { get; set; }
        public string ApprovalText { get; set; }
        public Guid CurrentUserId { get; set; }
        public SelectList QuoteApprovalRecipients
        {
            get
            {
                List<SIUser> quotationRequestManagers = SIDAL.GetQuoteApprovalManagers(this.QuoteId, true);
                return new SelectList(quotationRequestManagers, "UserId", "Username");
            }
        }

        // Project Entry Form Notification Attributess

        public string[] NotificationRecipients { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationText { get; set; }
        public List<SIUser> UserForDistrict
        {
            get
            {
                return SIDAL.GetUserForDistrict(this.DistrictId);
            }
        }

        public SelectList AllNotificationRecipients
        {
            get
            {
                List<SIUser> notificationRecipients = this.UserForDistrict.Where(x => x.Active == true).ToList();
                return new SelectList(notificationRecipients, "UserId", "Username");
            }
        }

        public SelectList AllEnableEditRecipients
        {
            get
            {
                List<SIUser> enableEditRecipients = SIDAL.GetQuoteApprovalUser(this.QuoteId).Where(x => x.Active == true).ToList();
                return new SelectList(enableEditRecipients, "UserId", "Username");
            }
        }
        public SelectList AllNotificationEmailRecipients
        {
            get
            {
                List<SIUser> notificationRecipients = this.UserForDistrict.Where(x => x.Active == true).ToList();
                return new SelectList(notificationRecipients, "Email", "Username");
            }
        }

        // Comments / Approval Acceptance Notification Attributess

        public string[] CommentRecipients { get; set; }
        public string CommentSubject { get; set; }
        public string CommentText { get; set; }
        public string[] ApprovalNotificationRecipients { get; set; }
        public string ApprovalNotificationSubject { get; set; }
        public string ApprovalNotificationText { get; set; }
        public bool NotifyUser { get; set; }

        //Enable Edit Quote attributes
        public string[] EnableEditRecipients { get; set; }
        public string EnableEditSubject { get; set; }
        public string EnableEditText { get; set; }
        public bool UserIsApprover { get; set; }
        public bool IsUserQuotationLimit { get; set; }
        public bool EnableWithoutNotification { get; set; }
        // Send Email to Customers
        public string EmailRecipients { get; set; }
        public string EmailSubject { get; set; }
        public string EmailText { get; set; }

        public bool AggregateEnabled { get; set; }
        public bool BlockEnabled { get; set; }
        public bool ConcreteEnabled { get; set; }

        public QuotationProfile(long id)
        {
            this.QuoteId = id;
            this.NotifyUser = true;
        }

        public QuotationProfile()
        {
            this.CompanyId = SIDAL.DefaultCompany().CompanyId;
            this.NotifyUser = true;
        }

        public QuotationProfile(Quotation q)
        {
            this.QuoteId = q.Id;
            this.NotifyUser = true;
            Load(q);
        }

        public void Load(Quotation q = null)
        {
            if (q == null)
            {
                q = SIDAL.FindQuotation(QuoteId);
            }
            this.QuoteId = q.Id;
            this.QuoteDate = q.QuoteDate;
            this.QuoteExpiration = q.QuoteExpirationDate;
            this.AcceptanceExpiration = q.AcceptanceExpirationDate;
            this.Status = q.Status;
            this.EnableEdit = q.EnableEdit.GetValueOrDefault();
            this.Awarded = q.Awarded.GetValueOrDefault(false);
            this.Active = q.Active.GetValueOrDefault(false);
            if (q.TaxCodeId != null)
            {
                TaxCode tc = SIDAL.FindTaxCode(q.TaxCodeId.Value);
                this.TaxCode = tc.Code + " - " + tc.Description;
            }
            this.TaxExceptReason = q.TaxExemptReason;

            this.CreatedBy = q.UserId;
            this.CreationDate = q.CreatedOn.GetValueOrDefault();
            this.CustomerId = q.CustomerId.GetValueOrDefault();

            if (q.Customer != null)
            {
                this.CustomerDispatchId = q.Customer.DispatchId;
                this.CustomerNumber = q.Customer.CustomerNumber;
                this.CustomerName = q.Customer.Name;
            }

            if (q.CustomerContactId != null)
            {
                CustomerContact cc = SIDAL.GetQuotationContact(q.Id);
                if (cc != null)
                {
                    this.EmailRecipients = cc.Email;
                }
            }

            this.ProjectId = q.ProjectId.GetValueOrDefault();

            if (q.Project != null)
            {
                this.CustomerJobRef = q.Project.CustomerRefName;
                this.BidDate = q.Project.BidDate == null ? "" : q.Project.BidDate.Value.ToString("M/d/yyyy");
                this.StartDate = q.Project.StartDate == null ? "" : q.Project.StartDate.Value.ToString("M/d/yyyy");
                this.ProjectToJob = q.Project.ToJobMinutes == null ? "" : q.Project.ToJobMinutes.Value.ToString("N0");
                this.DistanceToJob = q.Project.DistanceToJob == null ? "" : q.Project.DistanceToJob.Value.ToString("N0");
                this.DeliveryInstructions = q.Project.DeliveryInstructions;
                if (q.Project.MarketSegmentId != null)
                {
                    this.MarketSegment = SIDAL.GetMarketSegment(q.Project.MarketSegmentId).Name;
                }
                this.ProjectName = q.Project.Name;
                try
                {
                    this.SalesStaff = SIDAL.GetSalesStaff(q.SalesStaffId.GetValueOrDefault()).SalesStaff;
                    this.SalesStaffName = SalesStaff.Name;
                }
                catch (NullReferenceException) { }
            }

            this.PlantId = q.PlantId.GetValueOrDefault();
            this.AggregatePlantId = q.AggregatePlantId.GetValueOrDefault();
            this.BlockPlantId = q.BlockPlantId.GetValueOrDefault();
            this.BackupPlantId = q.Project.BackupPlantId;

            this.PricingMonth = q.PricingMonth != null ? q.PricingMonth : DateTime.Today;

            var purchaseConcrete = (q?.Customer?.PurchaseConcrete).GetValueOrDefault();
            var purchaseAggregate = (q?.Customer?.PurchaseAggregate).GetValueOrDefault();
            var purchaseBlock = (q?.Customer?.PurchaseBlock).GetValueOrDefault();


            this.ConcreteEnabled = purchaseConcrete & q.ConcreteEnabled.GetValueOrDefault();
            this.AggregateEnabled = ConfigurationHelper.AggregateEnabled & purchaseAggregate & q.AggregateEnabled.GetValueOrDefault();
            this.BlockEnabled = ConfigurationHelper.BlockEnabled & purchaseBlock & q.BlockEnabled.GetValueOrDefault();

            this.PlantName = SIDAL.GetFormattedPlantNameForQuote(q, this.ConcreteEnabled, this.AggregateEnabled, this.BlockEnabled);
            if (q.Plant != null)
            {
                this.PlantDispatchId = q.Plant.DispatchId;
                this.ToJob = q.Project.ToJobMinutes.GetValueOrDefault();
                this.WaitOnJob = q.Project.WaitOnJob.GetValueOrDefault();
                this.Unload = (int)q.AvgUnload.GetValueOrDefault(0);
                this.Wash = q.Project.WashMinutes.GetValueOrDefault();
                this.Return = q.Project.ReturnMinutes.GetValueOrDefault();

                District d = SIDAL.GetDistrict(q.Plant.DistrictId);
                if (d != null)
                {
                    this.DistrictId = d.DistrictId;
                    this.ApprovalText = d.QuoteApprovalRequestText;
                    this.ApprovalSubject = "Quotation Approval Request - Quote " + this.QuoteRefNumber + " (" + this.ProjectName + ")";
                    this.NotificationSubject = "Project Entry Form - Quote " + this.QuoteRefNumber + " (" + this.ProjectName + ")";
                    this.NotificationText = d.ProjectEntryFormNotification;
                    this.NotificationRecipients = SIDAL.GetProjectEntryRecipients(d.DistrictId).Select(x => x.UserId.ToString()).ToArray();
                    this.EmailSubject = "Quotation for " + this.ProjectName + " (" + this.QuoteRefNumber + ")";
                    if (this.SalesStaff != null)
                        this.EmailText = d.EmailQuoteToCustomer + string.Format((string.IsNullOrEmpty(d.EmailQuoteToCustomer) ? "" : "{0}{0}") + (string.IsNullOrEmpty(this.SalesStaffName) ? "" : this.SalesStaffName + "{0}") + (string.IsNullOrEmpty(SalesStaff.Email) ? "" : SalesStaff.Email + "{0}") + (string.IsNullOrEmpty(SalesStaff.Phone) ? "" : SalesStaff.Phone), Environment.NewLine);
                    else
                        this.EmailText = d.EmailQuoteToCustomer;
                    DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(q.Project.MarketSegmentId.GetValueOrDefault(), d.DistrictId);
                    if (dms != null)
                    {
                        this.DistrictContribution = dms.ContMarg.GetValueOrDefault();
                        this.DistrictSpread = dms.Spread.GetValueOrDefault();
                        this.DistrictProfit = dms.Profit.GetValueOrDefault();
                        this.DistrictCydHr = dms.CydHr.GetValueOrDefault();
                    }
                }
            }

            this.CommentSubject = "Quotation Comments - Quote " + this.QuoteRefNumber + " (" + this.ProjectName + ")";
            this.ApprovalNotificationSubject = "Quotation Approved - Quote " + this.QuoteRefNumber + " (" + this.ProjectName + ")";
            this.ApprovalNotificationRecipients = new string[] { this.CreatedBy.ToString() };
            this.CommentRecipients = new string[] { this.CreatedBy.ToString() };
            this.IsUserQuotationLimit = SIDAL.CheckUserQuoteApprovalLimit(this.CurrentUserId, this.QuoteId);
            this.EnableEditRecipients = new string[] { q.ApprovedBy.ToString() };
            this.EnableEditSubject = "Approved Quote Edit " + (this.IsUserQuotationLimit ? "Notification" : "Request") + " - Quote " + this.QuoteRefNumber + " (" + this.ProjectName + ")";
            this.EnableEditText = "Please allow editing of this previously approved quote by clicking the [Enable Editing] button on Edit Quotation page.";
            this.ApproverName = SIDAL.GetUserUserName(q.ApprovedBy.GetValueOrDefault());
            if (this.IsUserQuotationLimit)
            {
                var approverForEmailText = "";
                if (q.ApprovedBy != null)
                {
                    approverForEmailText = SIDAL.GetCompanyUser(q.ApprovedBy.GetValueOrDefault()).Name;
                }
                if (string.IsNullOrEmpty(approverForEmailText))
                {
                    approverForEmailText = this.ApproverName;
                }
                this.EnableEditText = "Editing has been re-enabled for this previously approved quote by " + approverForEmailText;
            }
            if (this.CurrentUserId != null)
                this.UserIsApprover = q.ApprovedBy == CurrentUserId ? true : false;
            if (this.UserIsApprover)
                this.EnableWithoutNotification = this.UserIsApprover;
            this.ApprovedBy = q.ApprovedBy.GetValueOrDefault();
            this.TotalVolume = q.TotalVolume.GetValueOrDefault(0);
            this.TotalRevenue = q.TotalRevenue.GetValueOrDefault(0);
            this.TotalProfit = q.TotalProfit.GetValueOrDefault(0);
            this.SackPrice = q.FskBasePrice.GetValueOrDefault(0);
            this.AverageSellingPrice = q.AvgSellingPrice.GetValueOrDefault(0);
            this.AverageLoad = q.AvgLoad.GetValueOrDefault(0);
            this.AverageUnLoad = q.AvgUnload.GetValueOrDefault(0);
            this.QuoteSpread = q.Spread.GetValueOrDefault(0);
            this.QuoteContribution = q.Contribution.GetValueOrDefault(0);
            this.QuoteProfit = q.Profit.GetValueOrDefault(0);
            this.QuoteCydHr = q.CYDHr.GetValueOrDefault(0);
            this.PushedBy = q.PushedBy;
            this.LastPushedAt = q.LastPushedAt;
        }

        public string Description
        {
            get
            {
                return "Quote #" + this.QuoteId + " : " + this.CustomerName;
            }
        }

        public SIUser User
        {
            get
            {
                try
                {
                    return SIDAL.GetUser(CreatedBy);
                }
                catch (Exception ex)
                {
                    return new SIUser();
                }
            }
        }
    }
}
