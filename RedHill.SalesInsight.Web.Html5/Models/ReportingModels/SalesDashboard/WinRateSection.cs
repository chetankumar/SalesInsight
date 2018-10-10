using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedHill.SalesInsight.Web.Html5.Models;
using RedHill.SalesInsight.DAL.DataTypes;
using Newtonsoft.Json.Linq;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class WinRateSection
    {
        public SalesDashboard Parent { get; set; }
        public List<WinRateDistrictSegmentStat> DistrictSegmentStats { get; set; }
        public List<WinRateMonthlyStat> MonthlyStats { get; set; }
        public WinRateSection(SalesDashboard parent)
        {
            this.Parent = parent;
            LoadDistrictSegmentStats();
            LoadMonthlyStats();
        }
        private void LoadDistrictSegmentStats()
        {
            this.DistrictSegmentStats = new List<WinRateDistrictSegmentStat>();
            DateTime tmpDate = Parent.StartDate.AddMonths(-12);
            while (tmpDate <= Parent.StartDate)
            {
                var projects = SIDAL.GetSoldProjects(Parent.SelectedDistricts, tmpDate, tmpDate.AddMonths(1).AddDays(-1))
                    .Where(x => x.Plant != null)
                    .Where(x => x.MarketSegmentId != null).ToList();
                if (projects.Count > 0)
                {
                    this.DistrictSegmentStats.AddRange(projects.GroupBy(x => new
                    {
                        x.Plant.DistrictId,
                        x.MarketSegmentId
                    }).Select(x => new WinRateDistrictSegmentStat(
                        x.Key.DistrictId,
                        x.Key.MarketSegmentId.GetValueOrDefault(),
                        x.Where(y => y.ProjectStatus.StatusType == SIStatusType.Sold.Id).Sum(y => y.Volume.GetValueOrDefault()),
                        x.Where(y => y.ProjectStatus.StatusType == SIStatusType.LostBid.Id).Sum(y => y.Volume.GetValueOrDefault()),
                        tmpDate
                    )).ToList());
                }
                tmpDate = tmpDate.AddMonths(1);
            }
        }
        private void LoadMonthlyStats()
        {
            if (this.DistrictSegmentStats.Count > 0)
            {
                this.MonthlyStats = this.DistrictSegmentStats.GroupBy(x => x.MonthStart).Select(x => new WinRateMonthlyStat
                {
                    MonthStart = x.Key,
                    LostVolume = x.Sum(y => y.LostVolume),
                    SoldVolume = x.Sum(y => y.SoldVolume),
                    Target = x.Sum(y => y.Target * (y.TotalVolume)) / x.Sum(y => y.TotalVolume)
                }).ToList();
            }
            else
            {
                this.MonthlyStats = new List<WinRateMonthlyStat>();
            }
        }

        public double Last3MonthAvg
        {
            get
            {
                if (this.MonthlyStats!= null && this.MonthlyStats.Count > 0)
                {
                    var l3month = this.MonthlyStats.Where(x => x.MonthStart >= this.Parent.StartDate.AddMonths(-3)).ToList();
                    int totalVolume = l3month.Sum(x => x.TotalVolume);
                    if (totalVolume > 0)
                    {
                        return l3month.Sum(x => x.SoldVolume) * 100f / totalVolume;
                    }
                    else
                        return 0;
                }
                return 0;
            }
        }
        public double Last3MonthTarget
        {
            get
            {
                if (this.MonthlyStats != null && this.MonthlyStats.Count > 0)
                {
                    var l3month = this.MonthlyStats.Where(x => x.MonthStart >= this.Parent.StartDate.AddMonths(-3));
                    int totalVolume = l3month.Sum(x => x.TotalVolume);
                    if (totalVolume > 0)
                    {
                        return l3month.Sum(x => x.Target.GetValueOrDefault() * x.TotalVolume) / totalVolume;
                    }
                    else
                        return l3month.Average(x=>x.Target).GetValueOrDefault();
                }
                return 0;
            }
        }
        public double Last3MonthVariance
        {
            get
            {
                return Last3MonthAvg - Last3MonthTarget;
            }
        }

        public string ChartData { get {
            JArray array = new JArray();
            JArray objLabels = new JArray();
            JArray objActuals = new JArray();
            JArray objTargets = new JArray();
            DateTime tmpDate = Parent.StartDate.AddMonths(-12);
            while (tmpDate <= Parent.StartDate)
            {
                objLabels.Add(tmpDate.ToString("MMM, yyyy"));
                var stat = MonthlyStats.Where(x => x.MonthStart == tmpDate).FirstOrDefault();
                if (stat == null)
                {
                    objActuals.Add(null);
                    objTargets.Add(null);
                }
                else
                {
                    objActuals.Add(stat.WinRate);
                    objTargets.Add(stat.Target);
                }
                tmpDate = tmpDate.AddMonths(1);
            }
            array.Add(objLabels);
            array.Add(objActuals);
            array.Add(objTargets);
            return array.ToString();
        } }
    }
}
