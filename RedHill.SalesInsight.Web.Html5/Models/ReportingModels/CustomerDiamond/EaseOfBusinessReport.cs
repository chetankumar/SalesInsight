using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedHill.SalesInsight.Web.Html5.Models;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class EaseOfBusinessReport
    {
        public DiamondReport DiamondParent { get; set; }
        public SalesDashboard DashboardParent { get; set; }

        public List<CustomerOrderChange> OrderChanges { get; set; }
        public List<OrderStat> OrderStats { get; set; }

        public double TotalOrders { get; set; }
        public double TotalVolumeOrdered {get;set;}
        public double TotalVolumeGained { get; set; }
        public double TotalVolumeLost { get; set; }
        public int    NumOrderChanges { get; set; }

        public int NumGainChanges { get; set; }
        public int NumLossChanges { get; set; }

        public double VolumeLostPercentage
        {
            get
            {
                if (TotalVolumeOrdered > 0)
                {
                    return TotalVolumeLost * 100.00 / TotalVolumeOrdered;
                }
                return 0;
            }
        }
        public double OrderChangeIncreases
        {
            get
            {
                if (NumOrderChanges > 0)
                    return NumGainChanges  / (TotalOrders);
                else
                    return 0;
            }
        }
        public double OrderChangeDecreases
        {
            get
            {
                if (NumOrderChanges > 0)
                    return NumLossChanges  / (TotalOrders);
                else
                    return 0;
            }
        }
        public double AvgVolumeChangeIncreases
        {
            get
            {
                if (NumGainChanges > 0)
                    return TotalVolumeGained  / (NumGainChanges);
                else
                    return 0;
            }
        }
        public double AvgVolumeChangeDecreases
        {
            get
            {
                if (NumLossChanges > 0)
                    return Math.Abs(TotalVolumeLost) / (NumLossChanges);
                else
                    return 0;
            }
        }

        public TargetIndicatorAllowance CancellationAllowance { get { return SIDAL.GetTargetIndicator("Cancellations"); } }

        public double CancellationTarget
        {
            get
            {
                if (this.CancellationAllowance != null)
                    return this.CancellationAllowance.Target;
                else
                    return 0;
            }
        }
        public double CancellationVariance
        {
            get
            {
                return Convert.ToDouble(VolumeLostPercentage) - CancellationTarget;
            }
        }
        public IndicatorModel CancellationIndicator
        {
            get
            {
                if (this.CancellationAllowance != null)
                    return new IndicatorModel(VolumeLostPercentage, this.CancellationAllowance.Target, this.CancellationAllowance.Ok, this.CancellationAllowance.Caution, this.CancellationAllowance.LessIsBetter.GetValueOrDefault());
                else
                    return null;
            }
        }

        public EaseOfBusinessReport(DiamondReport parent)
        {
            this.DiamondParent = parent;
            LoadOrderChangesForCustomers();
            LoadStats();
        }
        public EaseOfBusinessReport(SalesDashboard parent)
        {
            this.DashboardParent = parent;
            LoadOrderChangesForDistricts();
            LoadStats();
        }

        private void LoadOrderChangesForDistricts()
        {
            if (this.DashboardParent.ProfitabilityReport != null)
            {
                if (this.DashboardParent.ProfitabilityReport.Profitabilities.Count > 0)
                {
                    var customerIds = DashboardParent.ProfitabilityReport.Profitabilities.Select(x => x.CustomerNumber).Distinct().ToArray();
                    this.OrderChanges = SIDAL.GetCustomerOrderChangeReport(customerIds, DashboardParent.StartDate, DashboardParent.EndDate);
                }
            }
        }
        private void LoadOrderChangesForCustomers()
        {
            this.OrderChanges = SIDAL.GetCustomerOrderChangeReport(DiamondParent.CustomerIds, DiamondParent.StartDate, DiamondParent.EndDate);
        }
        private void LoadStats()
        {
            if (this.OrderChanges != null && this.OrderChanges.Count > 0)
            {
                this.OrderStats = this.OrderChanges.GroupBy(x => x.OrderId).Select(x => new OrderStat
                {
                    orderId = x.Key,
                    FirstVolume = x.OrderBy(y => y.ReportDate).FirstOrDefault() != null ? x.OrderBy(y => y.ReportDate).FirstOrDefault().ConcreteProductVolume : 0,
                    LastVolume = x.OrderBy(y => y.ReportDate).LastOrDefault() != null ? x.OrderBy(y => y.ReportDate).LastOrDefault().ConcreteProductVolume : 0,
                    GainedVolume = x.Where(y => y.VolumeChange.GetValueOrDefault() > 0).Sum(y => y.VolumeChange.GetValueOrDefault()),
                    LostVolume = x.Where(y => y.VolumeChange.GetValueOrDefault() < 0).Sum(y => y.VolumeChange.GetValueOrDefault()),
                    NumGainChanges = x.Where(y => y.VolumeChange > 0).Count(),
                    NumLossChanges = x.Where(y => y.VolumeChange < 0).Count()
                }).ToList();

                this.TotalOrders = this.OrderStats.Count();
                this.NumOrderChanges = this.OrderStats.Sum(x => x.NumLossChanges + x.NumGainChanges);
                this.TotalVolumeOrdered = this.OrderStats.Sum(x => x.FirstVolume);
                this.TotalVolumeGained = this.OrderStats.Sum(x => x.GainedVolume);
                this.TotalVolumeLost = Math.Abs(this.OrderStats.Sum(x => x.LostVolume));
                this.NumGainChanges = this.OrderStats.Sum(x => x.NumGainChanges);
                this.NumLossChanges = this.OrderStats.Sum(x => x.NumLossChanges);
            }
        }
    }
}
