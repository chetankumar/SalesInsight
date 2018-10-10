using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class AgingReport
    {
        public DiamondReport DiamondParent { get; set; }
        public SalesDashboard DashboardParent { get; set; }
        public List<CustomerAging> Agings { get; set; }

        public decimal TotalBalance  { get; set; }
        public decimal TotalCurrent  { get; set; }
        public decimal TotalOver1Mon { get; set; }
        public decimal TotalOver2Mon { get; set; }
        public decimal TotalOver3Mon { get; set; }
        public decimal TotalOver4Mon { get; set; }
        public DateTime?    AsOfDate { get; set; }
        public decimal TotalDSO { get; set; }
        public TargetIndicatorAllowance DSOAllowance {get { return SIDAL.GetTargetIndicator("DSO"); }}
        public double DSOTarget
        {
            get
            {
                if (DSOAllowance != null)
                    return DSOAllowance.Target;
                else
                    return 0;
            }
        }
        public double DSOVariance
        {
            get
            {
                return Convert.ToDouble(TotalDSO) - DSOTarget;
            }
        }
        public IndicatorModel DSOIndicator
        {
            get
            {
                if (this.DSOAllowance != null)
                {
                    return new IndicatorModel(TotalDSO, DSOAllowance.Target, DSOAllowance.Ok, DSOAllowance.Caution, DSOAllowance.LessIsBetter.GetValueOrDefault());
                }
                else
                    return null;
            }
        }
        public double AROver60Per
        {
            get
            {
                if (TotalBalance > 0)
                {
                    return Convert.ToDouble((TotalBalance - TotalCurrent - TotalOver1Mon) / TotalBalance) * 100.00;
                }
                return 0;
            }
        }
        public AgingReport(DiamondReport parent)
        {
            this.DiamondParent = parent;
            LoadCustomerAgings();
            LoadStats();
        }
        public AgingReport(SalesDashboard parent)
        {
            this.DashboardParent = parent;
            LoadDistrictAgings();
            LoadStats();
        }
        private void LoadDistrictAgings()
        {
            if (this.DashboardParent.ProfitabilityReport != null)
            {
                if (this.DashboardParent.ProfitabilityReport.Profitabilities.Count > 0)
                {
                    var customerIds = DashboardParent.ProfitabilityReport.Profitabilities.Select(x => x.CustomerNumber).Distinct().ToArray();
                    this.Agings = SIDAL.GetCustomerAgingReport(customerIds, DashboardParent.StartDate, DashboardParent.EndDate);
                }
            }
        }
        private void LoadCustomerAgings()
        {
            this.Agings = SIDAL.GetCustomerAgingReport(DiamondParent.CustomerIds, DiamondParent.StartDate, DiamondParent.EndDate);
        }
        private void LoadStats()
        {
            if (this.Agings == null || this.Agings.Count == 0)
            {
                return;
            }
            AsOfDate = this.Agings.First().ReportDate;
            TotalBalance = this.Agings.Sum(x => x.Balance);
            TotalCurrent = this.Agings.Sum(x => x.CurrentAmount);
            TotalOver1Mon = this.Agings.Sum(x => x.Over1Month);
            TotalOver2Mon = this.Agings.Sum(x => x.Over2Month);
            TotalOver3Mon = this.Agings.Sum(x => x.Over3Month);
            TotalOver4Mon = this.Agings.Sum(x => x.Over4Month);

            var dsoStats = new List<DSOStat>();
            foreach (var aging in this.Agings)
            {
                dsoStats.Add(new DSOStat(aging.CustomerNumber, aging.DSO, aging.Balance, aging.ReportDate));
            }
            var dsoSixMonRevenueSum = dsoStats.Sum(x => x.SixMonRevenue);
            if (dsoSixMonRevenueSum > 0)
            {
                this.TotalDSO = dsoStats.Sum(x => x.DSO * x.SixMonRevenue) / dsoStats.Sum(x => x.SixMonRevenue);
            }
        }

    }
}
