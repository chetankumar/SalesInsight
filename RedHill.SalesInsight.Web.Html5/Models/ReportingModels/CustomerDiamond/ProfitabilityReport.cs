using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.Web.Html5.Helpers;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProfitabilityReport
    {
        public DiamondReport DiamondParent { get; set; }
        public SalesDashboard DashboardParent { get; set; }
        public List<ProductivityStat> Stats { get; set; }
        public List<CustomerProfitability> Profitabilities { get; set; }
        public MetricIndicatorAllowance Allowances { get; set; }
        public List<MarketSegmentStat> MarketSegmentStats { get; set; }
        public List<DistrictVolumeStat> DistrictVolumeStats { get; set; }

        public decimal ActualPrice { get; set; }
        public decimal ActualMaterial { get; set; }
        public decimal ActualSpread { get; set; }
        public decimal ActualVariable { get; set; }
        public decimal ActualFixed { get; set; }
        public decimal ActualProfit { get; set; }

        public decimal BudgetPrice { get; set; }
        public decimal BudgetMaterial { get; set; }
        public decimal BudgetSpread { get; set; }
        public decimal BudgetVariable { get; set; }
        public decimal BudgetFixed { get; set; }
        public decimal BudgetProfit { get; set; }

        public decimal VariancePrice { get; set; }
        public decimal VarianceMaterial { get; set; }
        public decimal VarianceSpread { get; set; }
        public decimal VarianceVariable { get; set; }
        public decimal VarianceFixed { get; set; }
        public decimal VarianceProfit { get; set; }
        public List<MetricIndicatorAllowance> metricAllowanceList { get; set; }

        public IndicatorModel PriceIndicator
        {
            get
            {
                MetricIndicatorAllowance allowance = SIDAL.GetMetricIndicator("Price",this.metricAllowanceList);
                if (allowance != null)
                    return new IndicatorModel(this.ActualPrice, this.BudgetPrice, allowance.Ok, allowance.Caution, allowance.LessIsBetter.GetValueOrDefault());
                else
                    return null;
            }
        }
        public IndicatorModel MaterialIndicator
        {
            get
            {
                MetricIndicatorAllowance allowance = SIDAL.GetMetricIndicator("Material", this.metricAllowanceList);
                if (allowance != null)
                    return new IndicatorModel(this.ActualMaterial, this.BudgetMaterial, allowance.Ok, allowance.Caution, allowance.LessIsBetter.GetValueOrDefault());
                else
                    return null;
            }
        }
        public IndicatorModel SpreadIndicator
        {
            get
            {
                MetricIndicatorAllowance allowance = SIDAL.GetMetricIndicator("Spread", this.metricAllowanceList);
                if (allowance != null)
                    return new IndicatorModel(this.ActualSpread, this.BudgetSpread, allowance.Ok, allowance.Caution, allowance.LessIsBetter.GetValueOrDefault());
                else
                    return null;
            }
        }
        public IndicatorModel VariableIndicator
        {
            get
            {
                MetricIndicatorAllowance allowance = SIDAL.GetMetricIndicator("Variable", this.metricAllowanceList);
                if (allowance != null)
                    return new IndicatorModel(this.ActualVariable, this.BudgetVariable, allowance.Ok, allowance.Caution, allowance.LessIsBetter.GetValueOrDefault());
                else
                    return null;
            }
        }
        public IndicatorModel FixedIndicator
        {
            get
            {
                MetricIndicatorAllowance allowance = SIDAL.GetMetricIndicator("Fixed", this.metricAllowanceList);
                if (allowance != null)
                    return new IndicatorModel(this.ActualFixed, this.BudgetFixed, allowance.Ok, allowance.Caution, allowance.LessIsBetter.GetValueOrDefault());
                else
                    return null;
            }
        }
        public IndicatorModel ProfitIndicator
        {
            get
            {
                MetricIndicatorAllowance allowance = SIDAL.GetMetricIndicator("Profit", this.metricAllowanceList);
                if (allowance != null)
                    return new IndicatorModel(this.ActualProfit, this.BudgetProfit, allowance.Ok, allowance.Caution, allowance.LessIsBetter.GetValueOrDefault());
                else
                    return null;
            }
        }

        public ProfitabilityReport(DiamondReport parent,List<Plant> plantList = null)
        {
            this.DiamondParent = parent;
            this.metricAllowanceList = SIDAL.GetAllMetricIndicator();
            
            LoadCustomerProfitabilities();
            LoadCustomerBudgets(plantList);
        }
        public ProfitabilityReport(SalesDashboard parent, List<Plant> plantList = null)
        {
            this.DashboardParent = parent;
            this.metricAllowanceList = SIDAL.GetAllMetricIndicator();
            LoadDistrictProfitabilities();
            LoadDistrictBudgets(plantList);
        }

        private void LoadDistrictProfitabilities()
        {
            this.Profitabilities = SIDAL.GetDistrictProfitabilityReport(this.DashboardParent.SelectedDistricts, this.DashboardParent.StartDate, this.DashboardParent.EndDate);
        }
        private void LoadDistrictBudgets(List<Plant> plantList = null)
        {
            this.Stats = new List<ProductivityStat>();
            var districtProductivityList = SIDAL.GetDistrictProductivityReport(this.DashboardParent.SelectedDistricts, this.DashboardParent.StartDate, this.DashboardParent.EndDate);
            foreach (CustomerProductivity prod in districtProductivityList)
            {
                this.Stats.Add(new ProductivityStat(prod, plantList));
            }

            this.DistrictVolumeStats = this.Stats.GroupBy(x => x.DistrictId).Select(x => new DistrictVolumeStat
            {
                DistrictId = x.Key,
                DistrictVolume = x.Sum(y => y.Quantity)
            }).ToList();

            this.MarketSegmentStats = this.Profitabilities.GroupBy(x => x.SegmentId).Select(x => new MarketSegmentStat(x.Key, x.Sum(y => y.Revenue - y.MaterialCost), this.Stats.Where(y => y.SegmentId == x.Key).Sum(y => y.Quantity), this.DistrictVolumeStats)).ToList();

            decimal TotalQuantity = Convert.ToDecimal(this.Stats.Sum(x => x.Quantity));

            if (TotalQuantity > 0)
            {
                ActualPrice = this.Profitabilities.Sum(x => x.Revenue) / TotalQuantity;
                ActualMaterial = this.Profitabilities.Sum(x => x.MaterialCost) / TotalQuantity;
                ActualSpread = ActualPrice - ActualMaterial;
                ActualVariable = this.Stats.Sum(x => x.VariableCost) / TotalQuantity;
                ActualFixed = this.Stats.Sum(x => x.FixedCost) / TotalQuantity;
                ActualProfit = ActualSpread - ActualVariable - ActualFixed;

                BudgetPrice = this.Stats.Sum(x => x.BudgetPrice * Convert.ToDecimal(x.Quantity)) / TotalQuantity;
                BudgetMaterial = this.Stats.Sum(x => x.BudgetMaterial * Convert.ToDecimal(x.Quantity)) / TotalQuantity;
                BudgetSpread = this.Stats.Sum(x => x.BudgetSpread * Convert.ToDecimal(x.Quantity)) / TotalQuantity;
                BudgetVariable = Convert.ToDecimal(this.Stats.Sum(x => x.BudgetVariable * x.Quantity)) / TotalQuantity;
                BudgetFixed = Convert.ToDecimal(this.Stats.Sum(x => x.BudgetFixed * x.Quantity)) / TotalQuantity;
                BudgetProfit = this.Stats.Sum(x => x.BudgetProfit * Convert.ToDecimal(x.Quantity)) / TotalQuantity;

                VariancePrice = this.ActualPrice - this.BudgetPrice;
                VarianceMaterial = this.ActualMaterial - this.BudgetMaterial;
                VarianceSpread = this.ActualSpread - this.BudgetSpread;
                VarianceVariable = this.ActualVariable - this.BudgetVariable;
                VarianceFixed = this.ActualFixed - this.BudgetFixed;
                VarianceProfit = this.ActualProfit - this.BudgetProfit;
            }
        }

        private void LoadCustomerProfitabilities()
        {
            this.Profitabilities = SIDAL.GetCustomerProfitabilityReport(this.DiamondParent.CustomerIds, this.DiamondParent.StartDate, this.DiamondParent.EndDate);
        }
        private void LoadCustomerBudgets(List<Plant> plantList = null)
        {
            this.Stats = new List<ProductivityStat>();
            var customerProductivityList = SIDAL.GetCustomerProductivityReport(this.DiamondParent.CustomerIds, this.DiamondParent.StartDate, this.DiamondParent.EndDate);
            foreach (CustomerProductivity prod in customerProductivityList)
            {
                this.Stats.Add(new ProductivityStat(prod, plantList));
            }

            this.DistrictVolumeStats = this.Stats.Select(x => new DistrictVolumeStat
            {
                DistrictId = x.DistrictId,
                DistrictVolume = x.Quantity,
                SegmentId = x.SegmentId
            }).ToList();

            this.MarketSegmentStats = this.Profitabilities.GroupBy(x => x.SegmentId).Select(x => new MarketSegmentStat(x.Key, x.Sum(y => y.Revenue - y.MaterialCost), this.Stats.Where(y => y.SegmentId == x.Key).Sum(y => y.Quantity), this.DistrictVolumeStats)).ToList();

            decimal TotalQuantity = Convert.ToDecimal(this.Stats.Sum(x => x.Quantity));

            if (TotalQuantity > 0)
            {
                ActualPrice = this.Profitabilities.Sum(x => x.Revenue) / TotalQuantity;
                ActualMaterial = this.Profitabilities.Sum(x => x.MaterialCost) / TotalQuantity;
                ActualSpread = ActualPrice - ActualMaterial;
                ActualVariable = this.Stats.Sum(x => x.VariableCost) / TotalQuantity;
                ActualFixed = this.Stats.Sum(x => x.FixedCost) / TotalQuantity;
                ActualProfit = ActualSpread - ActualVariable - ActualFixed;

                BudgetPrice = this.Stats.Sum(x => x.BudgetPrice * Convert.ToDecimal(x.Quantity)) / TotalQuantity;
                BudgetMaterial = this.Stats.Sum(x => x.BudgetMaterial * Convert.ToDecimal(x.Quantity)) / TotalQuantity;
                BudgetSpread = this.Stats.Sum(x => x.BudgetSpread * Convert.ToDecimal(x.Quantity)) / TotalQuantity;
                BudgetVariable = Convert.ToDecimal(this.Stats.Sum(x => x.BudgetVariable * x.Quantity)) / TotalQuantity;
                BudgetFixed = Convert.ToDecimal(this.Stats.Sum(x => x.BudgetFixed * x.Quantity)) / TotalQuantity;
                BudgetProfit = this.Stats.Sum(x => x.BudgetProfit * Convert.ToDecimal(x.Quantity)) / TotalQuantity;

                VariancePrice = this.ActualPrice - this.BudgetPrice;
                VarianceMaterial = this.ActualMaterial - this.BudgetMaterial;
                VarianceSpread = this.ActualSpread - this.BudgetSpread;
                VarianceVariable = this.ActualVariable - this.BudgetVariable;
                VarianceFixed = this.ActualFixed - this.BudgetFixed;
                VarianceProfit = this.ActualProfit - this.BudgetProfit;
            }
        }

        public string ChartData
        {
            get
            {
                JArray array = new JArray();
                JArray objLabels = new JArray();
                JArray objSpreads = new JArray();
                JArray objTargets = new JArray();
                List<string> segments = this.MarketSegmentStats.Select(x => x.MarketSegmentName).ToList();
                foreach (MarketSegmentStat stat in this.MarketSegmentStats.OrderByDescending(x => x.Quantity))
                {
                    if (stat.Quantity > 0)
                    {
                        objLabels.Add(stat.MarketSegmentName + "<br/>" + stat.Quantity.ToString("N0") + " " + ConfigurationHelper.Company.DeliveryQtyUomPlural);
                        objSpreads.Add(stat.Spread / Convert.ToDecimal(stat.Quantity));
                        objTargets.Add(stat.Target);
                    }
                }
                array.Add(objLabels);
                array.Add(objSpreads);
                array.Add(objTargets);
                return array.ToString();
            }
        }
    }
}
