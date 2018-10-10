using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL
{
    public class SIRoleAccess
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string HasAddonsAccess { get; set; }
        public string HasCompetitorsAccess { get; set; }
        public string HasCustomersAccess { get; set; }
        public string HasForecastAccess { get; set; }
        public string HasGCAccess { get; set; }
        public string HasMarketSegmentsAccess { get; set; }
        public string HasMiscellaneousAccess { get; set; }
        public string HasMixDesignsAccess { get; set; }
        public string HasPipelineAccess { get; set; }
        public string HasQuotationAccess { get; set; }
        public string HasRawMaterialsAccess { get; set; }
        public string HasReportsAccess { get; set; }
        public string HasReportSettingsAccess { get; set; }
        public string HasRolesAccess { get; set; }
        public string HasSackPricingAccess { get; set; }
        public string HasSalesStaffAccess { get; set; }
        public string HasStatusesReasonsLostAccess { get; set; }
        public string HasStructureAccess { get; set; }
        public string HasTargetsAccess { get; set; }
        public string HasUsersAccess { get; set; }
        public string HasDashboardAccess { get; set; }
        public string HasGoalAnalysisAccess { get; set; }
        public string HasBenchmarkAnalysisAccess { get; set; }
        public string HasTrendAnalysisAccess { get; set; }
        public string HasDrillInAccess { get; set; }
        public string HasAlertsManagementAccess { get; set; }
        public string HasCustomWebDataFormAccess { get; set; }
        public bool CanEditNonFutureProjections { get; set; }
        public bool CanEditActuals { get; set; }
        public bool CanPrintQuotes { get; set; }
        public bool CanUploadAddonPricesCosts { get; set; }
        public bool CanUploadMixesFormulations { get; set; }
        public bool CanUploadRawMaterialCost { get; set; }
        public bool ExcludeCustomMix { get; set; }
        public bool Enable5skPricing { get; set; }
        public bool RequireProjectLocation { get; set; }
        public bool HidePrice { get; set; }
        public bool HideSpread { get; set; }
        public bool HideContribution { get; set; }
        public bool HideProfit { get; set; }
        public decimal? MinSpread { get; set; }
        public decimal? MaxSpread { get; set; }
        public decimal? MinContribution { get; set; }
        public decimal? MaxContribution { get; set; }
        public decimal? MinProfit { get; set; }
        public decimal? MaxProfit { get; set; }
        public bool MergeCustomers { get; set; }

        public SIRoleAccess()
        {
        }

        public SIRoleAccess(RoleAccess rule)
        {
            RoleId = rule.RoleAccessId;
            RoleName = rule.RoleName;
            HasAddonsAccess = rule.AddonsAccess;
            HasCompetitorsAccess = rule.CompetitorsAccess;
            HasCustomersAccess = rule.CustomersAccess;
            HasForecastAccess = rule.ForecastAccess;
            HasGCAccess = rule.GCAccess;
            HasMarketSegmentsAccess = rule.MarketSegmentsAccess;
            HasMiscellaneousAccess = rule.MiscellaneousAccess;
            HasMixDesignsAccess = rule.MixDesignsAccess;
            HasPipelineAccess = rule.PipelineAccess;
            HasQuotationAccess = rule.QuotationAccess;
            HasRawMaterialsAccess = rule.RawMaterialsAccess;
            HasReportsAccess = rule.ReportsAccess;
            HasReportSettingsAccess = rule.ReportSettingsAccess;
            HasRolesAccess = rule.RolesAccess;
            HasSackPricingAccess = rule.SackPricingAccess;
            HasSalesStaffAccess = rule.SalesStaffAccess;
            HasStatusesReasonsLostAccess = rule.StatusesReasonsLostAccess;
            HasStructureAccess = rule.StructureAccess;
            HasTargetsAccess = rule.TargetsAccess;
            HasUsersAccess = rule.UsersAccess;
            HasDashboardAccess = rule.DashboardAccess;
            HasGoalAnalysisAccess = rule.GoalAnalysisAccess;
            HasBenchmarkAnalysisAccess = rule.BenchmarkAnalysisAccess;
            HasTrendAnalysisAccess = rule.TrendAnalysisAccess;
            HasDrillInAccess = rule.DrillInAccess;
            HasAlertsManagementAccess = rule.AlertsManagementAccess;
            HasCustomWebDataFormAccess = rule.CustomWebDataFormAccess;
            CanEditNonFutureProjections = rule.EditNonFutureProjectionAccess;
            CanEditActuals = rule.EditActual;
            CanPrintQuotes = rule.EnablePrinting.GetValueOrDefault(false);
            CanUploadAddonPricesCosts = rule.EnableUploadAddonPricesCosts.GetValueOrDefault(false);
            CanUploadMixesFormulations = rule.EnableUploadMixesFormulations.GetValueOrDefault(false);
            CanUploadRawMaterialCost = rule.EnableUploadRawMaterialCost.GetValueOrDefault(false);
            ExcludeCustomMix = rule.ExcludeCustomMix.GetValueOrDefault(false);
            Enable5skPricing = rule.Enable5skPricing.GetValueOrDefault(false);
            RequireProjectLocation = rule.RequireProjectLocation.GetValueOrDefault(false);
            HidePrice = rule.HidePrice.GetValueOrDefault(false);
            HideSpread = rule.HideSpread.GetValueOrDefault(false);
            HideContribution = rule.HideContribution.GetValueOrDefault(false);
            HideProfit = rule.HideProfit.GetValueOrDefault(false);
            MinSpread = rule.MinSpread;
            MaxSpread = rule.MaxSpread;
            MinContribution = rule.MinContribution;
            MaxContribution = rule.MaxContribution;
            MinProfit = rule.MinProfit;
            MaxProfit = rule.MaxProfit;
            MergeCustomers = rule.MergeCustomers.GetValueOrDefault();
        }

        public void SetRolesAccess()
        {
            if (IsAdmin)
                this.HasRolesAccess = SIRolePermissionLevelConstants.FULL_ACCESS;
            else
                this.HasRolesAccess = SIRolePermissionLevelConstants.READ_ONLY;
        }

        public bool IsAdmin
        {
            get
            {
                return (this.RoleName == "System Admin");
            }
        }
    }
}
