using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedHill.SalesInsight.Web.Html5.Models;
using RedHill.SalesInsight.DAL;
using Newtonsoft.Json.Linq;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class BackLogSection
    {
        public SalesDashboard Parent { get; set; }
        public List<BackLogStat> Stats { get; set; }
        public BackLogSection(SalesDashboard parent)
        {
            this.Parent = parent;
            LoadStats();
        }

        public void LoadStats()
        {
            DateTime tmp = Parent.StartDate;
            this.Stats = new List<BackLogStat>();
            while (tmp <= Parent.StartDate.AddMonths(5))
            {
                var plantStats = new List<BackLogPlantStat>();
                List<ProjectProjection> projections = SIDAL.GetPlantProjections(Parent.SelectedDistricts, tmp, tmp.AddMonths(1)).Where(x=>x.PlantId != null).ToList();
                if (projections.Count > 0)
                {
                    plantStats.AddRange(
                        projections.GroupBy(x => x.PlantId.GetValueOrDefault()).Select(x => new BackLogPlantStat(tmp, x.Key, (int)x.Sum(y => y.Projection.GetValueOrDefault())))
                    );
                }

                if (plantStats.Count > 0)
                {
                    var budgets = SIDAL.GetPlantBudgets(this.Parent.SelectedDistricts, tmp, tmp.AddMonths(1).AddDays(-1));
                    double budget = 0;
                    if (budgets != null && budgets.Count() > 0)
                    {
                        budget = budgets.Sum(x => x.Budget.GetValueOrDefault());
                    }
                    this.Stats.Add(new BackLogStat(tmp, plantStats.Sum(x => x.TotalProjected), budget));
                }
                else
                {
                    this.Stats.Add(new BackLogStat(tmp, 0,0));
                }
                tmp = tmp.AddMonths(1);
            }
        }

        public int TotalProjected
        {
            get
            {
                if (this.Stats != null && this.Stats.Count > 0)
                {
                    return this.Stats.Sum(x => x.TotalProjected);
                }
                return 0;
            }
        }
        public double TotalBudget
        {
            get
            {
                if (this.Stats != null && this.Stats.Count > 0)
                {
                    return this.Stats.Sum(x => x.Budget);
                }
                return 0;
            }
        }
        public double TotalVariance
        {
            get
            {
                return this.TotalProjected - this.TotalBudget;
            }
        }

        public string ChartData
        {
            get
            {
                JArray array = new JArray();
                JArray objLabels = new JArray();
                JArray objActuals = new JArray();
                JArray objTargets = new JArray();
                DateTime tmpDate = Parent.StartDate;
                while (tmpDate <= Parent.StartDate.AddMonths(5))
                {
                    objLabels.Add(tmpDate.ToString("MMM, yyyy"));
                    var stat = Stats.Where(x => x.MonthDate == tmpDate).FirstOrDefault();
                    if (stat == null)
                    {
                        objActuals.Add(0);
                        objTargets.Add(0);
                    }
                    else
                    {
                        objActuals.Add(stat.TotalProjected);
                        objTargets.Add(stat.Budget);
                    }
                    tmpDate = tmpDate.AddMonths(1);
                }
                array.Add(objLabels);
                array.Add(objActuals);
                array.Add(objTargets);
                return array.ToString();
            }
        }
    }
}
