using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class RoleAccessView
    {
        public int RoleId { get; set; }

        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Please specify a name for the role")]
        public string RoleName { get; set; }

        [Display(Name = "Addons Access")]
        public string HasAddonsAccess { get; set; }
        [Display(Name = "Competitors Access")]
        public string HasCompetitorsAccess { get; set; }
        [Display(Name = "Customers Access")]
        public string HasCustomersAccess { get; set; }
        [Display(Name = "Forecast Access")]
        public string HasForecastAccess { get; set; }
        [Display(Name = "GC Access")]
        public string HasGCAccess { get; set; }
        [Display(Name = "Market Segments Access")]
        public string HasMarketSegmentsAccess { get; set; }
        [Display(Name = "Miscellaneous Access")]
        public string HasMiscellaneousAccess { get; set; }
        [Display(Name = "Mix Designs Access")]
        public string HasMixDesignsAccess { get; set; }
        [Display(Name = "Pipeline Access")]
        public string HasPipelineAccess { get; set; }
        [Display(Name = "Quotations Access")]
        public string HasQuotationAccess { get; set; }
        [Display(Name = "Raw Materials Access")]
        public string HasRawMaterialsAccess { get; set; }
        [Display(Name = "Reports Access")]
        public string HasReportsAccess { get; set; }
        [Display(Name = "Report Settings Access")]
        public string HasReportSettingsAccess { get; set; }
        [Display(Name = "Roles Access")]
        public string HasRolesAccess { get; set; }
        [Display(Name = "Sack Pricing Access")]
        public string HasSackPricingAccess { get; set; }
        [Display(Name = "Sales Staff Access")]
        public string HasSalesStaffAccess { get; set; }
        [Display(Name = "Statuses/Reasons Lost Access")]
        public string HasStatusesReasonsLostAccess { get; set; }
        [Display(Name = "Structure Access")]
        public string HasStructureAccess { get; set; }
        [Display(Name = "Targets Access")]
        public string HasTargetsAccess { get; set; }
        [Display(Name = "Users Access")]
        public string HasUsersAccess { get; set; }
        [Display(Name = "Dashboard Access")]
        public string HasDashboardAccess { get; set; }
        [Display(Name = "Goal Analysis Access")]
        public string HasGoalAnalysisAccess { get; set; }
        [Display(Name = "Benchmark Analysis Access")]
        public string HasBenchmarkAnalysisAccess { get; set; }
        [Display(Name = "Trend Analysis Access")]
        public string HasTrendAnalysisAccess { get; set; }
        [Display(Name = "Drill-In Access")]
        public string HasDrillInAccess { get; set; }
        [Display(Name = "Alerts Management Access")]
        public string HasAlertsManagementAccess { get; set; }
        [Display(Name = "Custom Web Data Form Access")]
        public string HasCustomWebDataFormAccess { get; set; }
        [Display(Name = "Edit Non Future Projections")]
        public bool CanEditNonFutureProjections { get; set; }
        [Display(Name = "Edit Actual Volume")]
        public bool CanEditActual { get; set; }
        [Display(Name = "Can Print Quotes")]
        public bool CanPrintQuotes { get; set; }
        [Display(Name = "Can Upload Addon Prices Costs")]
        public bool CanUploadAddonPricesCosts { get; set; }
        [Display(Name = "Can Upload Mixes Formulations")]
        public bool CanUploadMixesFormulations { get; set; }
        [Display(Name = "Can Upload Raw Material Cost")]
        public bool CanUploadRawMaterialCost { get; set; }
        [Display(Name = "Exclude Custom Mixes")]
        public bool ExcludeCustomMix { get; set; }

        [Display(Name = "Enable 5sk Pricing")]
        public bool Enable5skPricing { get; set; }

        [Display(Name = "Require Project Location")]
        public bool RequireProjectLocation { get; set; }

        [Display(Name = "Hide Price")]
        public bool HidePrice { get; set; }

        [Display(Name = "Hide Spread")]
        public bool HideSpread { get; set; }

        [Display(Name = "Hide Contribution")]
        public bool HideContribution { get; set; }

        [Display(Name = "Hide Profit")]
        public bool HideProfit { get; set; }

        [Display(Name = "Min Spread")]
        public decimal? MinSpread { get; set; }

        [Display(Name = "Max Spread")]
        public decimal? MaxSpread { get; set; }

        [Display(Name = "Min Contribution")]
        public decimal? MinContribution { get; set; }

        [Display(Name = "Max Contribution")]
        public decimal? MaxContribution { get; set; }

        [Display(Name = "Min Profit")]
        public decimal? MinProfit { get; set; }

        [Display(Name = "Max Profit")]
        public decimal? MaxProfit { get; set; }

        [Display(Name = "Merge Customers")]
        public bool MergeCustomers { get; set; }

        public bool IsAdmin { get; set; }
        public string FullAccess
        {
            get { return SIRolePermissionLevelConstants.FULL_ACCESS; }
        }
        public string Access
        {
            get { return SIRolePermissionLevelConstants.ACCESS; }
        }
        public string ReadOnly
        {
            get { return SIRolePermissionLevelConstants.READ_ONLY; }
        }
        public string EditOnly
        {
            get { return SIRolePermissionLevelConstants.EDIT_ONLY; }
        }
        public string NoAccess
        {
            get { return SIRolePermissionLevelConstants.NO_ACCESS; }
        }
        public string Admin
        {
            get { return SIRolePermissionLevelConstants.ADMIN; }
        }
        public RoleAccessView()
        {
        }
        public RoleAccessView(SIRoleAccess siRoleAccess)
        {
            this.RoleId = siRoleAccess.RoleId;
            this.RoleName = siRoleAccess.RoleName;
            this.HasAddonsAccess = siRoleAccess.HasAddonsAccess;
            this.HasCompetitorsAccess = siRoleAccess.HasCompetitorsAccess;
            this.HasCustomersAccess = siRoleAccess.HasCustomersAccess;
            this.HasForecastAccess = siRoleAccess.HasForecastAccess;
            this.HasGCAccess = siRoleAccess.HasGCAccess;
            this.HasMarketSegmentsAccess = siRoleAccess.HasMarketSegmentsAccess;
            this.HasMiscellaneousAccess = siRoleAccess.HasMiscellaneousAccess;
            this.HasMixDesignsAccess = siRoleAccess.HasMixDesignsAccess;
            this.HasPipelineAccess = siRoleAccess.HasPipelineAccess;
            this.HasQuotationAccess = siRoleAccess.HasQuotationAccess;
            this.HasRawMaterialsAccess = siRoleAccess.HasRawMaterialsAccess;
            this.HasReportsAccess = siRoleAccess.HasReportsAccess;
            this.HasReportSettingsAccess = siRoleAccess.HasReportSettingsAccess;
            this.HasRolesAccess = siRoleAccess.HasRolesAccess;
            this.HasSackPricingAccess = siRoleAccess.HasSackPricingAccess;
            this.HasSalesStaffAccess = siRoleAccess.HasSalesStaffAccess;
            this.HasStatusesReasonsLostAccess = siRoleAccess.HasStatusesReasonsLostAccess;
            this.HasStructureAccess = siRoleAccess.HasStructureAccess;
            this.HasTargetsAccess = siRoleAccess.HasTargetsAccess;
            this.HasUsersAccess = siRoleAccess.HasUsersAccess;
            this.HasDashboardAccess = siRoleAccess.HasDashboardAccess;
            this.HasGoalAnalysisAccess = siRoleAccess.HasGoalAnalysisAccess;
            this.HasBenchmarkAnalysisAccess = siRoleAccess.HasBenchmarkAnalysisAccess;
            this.HasTrendAnalysisAccess = siRoleAccess.HasTrendAnalysisAccess;
            this.HasDrillInAccess = siRoleAccess.HasDrillInAccess;
            this.HasAlertsManagementAccess = siRoleAccess.HasAlertsManagementAccess;
            this.HasCustomWebDataFormAccess = siRoleAccess.HasCustomWebDataFormAccess;
            this.CanEditNonFutureProjections = siRoleAccess.CanEditNonFutureProjections;
            this.CanEditActual = siRoleAccess.CanEditActuals;
            this.CanPrintQuotes = siRoleAccess.CanPrintQuotes;
            this.CanUploadAddonPricesCosts = siRoleAccess.CanUploadAddonPricesCosts;
            this.CanUploadMixesFormulations = siRoleAccess.CanUploadMixesFormulations;
            this.CanUploadRawMaterialCost = siRoleAccess.CanUploadRawMaterialCost;
            this.ExcludeCustomMix = siRoleAccess.ExcludeCustomMix;
            this.Enable5skPricing = siRoleAccess.Enable5skPricing;
            this.RequireProjectLocation = siRoleAccess.RequireProjectLocation;
            this.IsAdmin = siRoleAccess.IsAdmin;
            this.HidePrice = siRoleAccess.HidePrice;
            this.HideSpread = siRoleAccess.HideSpread;
            this.HideContribution = siRoleAccess.HideContribution;
            this.HideProfit = siRoleAccess.HideProfit;
            this.MinSpread = siRoleAccess.MinSpread;
            this.MaxSpread = siRoleAccess.MaxSpread;
            this.MinContribution = siRoleAccess.MinContribution;
            this.MaxContribution = siRoleAccess.MaxContribution;
            this.MinProfit = siRoleAccess.MinProfit;
            this.MaxProfit = siRoleAccess.MaxProfit;
            this.MergeCustomers = siRoleAccess.MergeCustomers;
        }
        public IEnumerable<SelectListItem> GetPipelineAccess()
        {
            string[] labels = { "Read only", "Edit only", "Full Access" };
            string[] values = { ReadOnly, EditOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[0];
                item.Value = values[0];
                if (this.HasPipelineAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetAddonsAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasAddonsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetCompetitorsAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasCompetitorsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetCustomersAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasCustomersAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetForecastAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasForecastAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetGCAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasGCAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetMarketSegmentsAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasMarketSegmentsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetMiscellaneousAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Edit Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, EditOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasMiscellaneousAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetMixDesignsAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasMixDesignsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetPipelineAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Edit Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, EditOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasPipelineAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetQuotationAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Edit Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, EditOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasQuotationAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetRawMaterialsAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasRawMaterialsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetReportsAccessList()
        {
            string[] labels = { "No Access", "Full Access" };
            string[] values = { NoAccess, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasReportsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetReportSettingsAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasReportSettingsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetRolesAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Edit Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, EditOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasRolesAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetSackPricingAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasSackPricingAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetSalesStaffAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasSalesStaffAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetStatusesReasonsLostAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasStatusesReasonsLostAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetStructureAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasStructureAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetTargetsAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasTargetsAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetUserAccessList()
        {
            string[] labels = { "No Access", "Read Only", "Full Access" };
            string[] values = { NoAccess, ReadOnly, FullAccess };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasUsersAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetDashboardAccessList()
        {
            string[] labels = { "Access", "No Access", "Admin" };
            string[] values = { Access, NoAccess, Admin };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasDashboardAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetGoalAnalysisAccessList()
        {
            string[] labels = { "Access", "No Access", "Admin" };
            string[] values = { Access, NoAccess, Admin };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasGoalAnalysisAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetBenchmarkAnalysisAccessList()
        {
            string[] labels = { "Access", "No Access", "Admin" };
            string[] values = { Access, NoAccess, Admin };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasBenchmarkAnalysisAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetTrendAnalysisAccessList()
        {
            string[] labels = { "Access", "No Access", "Admin" };
            string[] values = { Access, NoAccess, Admin };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasTrendAnalysisAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetDrillInAccessList()
        {
            string[] labels = { "Access", "No Access", "Admin" };
            string[] values = { Access, NoAccess, Admin };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasDrillInAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetAlertsManagementAccessList()
        {
            string[] labels = { "Full Control", "Read Only", "Admin" };
            string[] values = { FullAccess, ReadOnly, Admin };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasAlertsManagementAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public IEnumerable<SelectListItem> GetCustomWebDataFormAccessList()
        {
            string[] labels = { "Full Control", "Read Only" };
            string[] values = { FullAccess, ReadOnly };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.HasCustomWebDataFormAccess == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
    }
}