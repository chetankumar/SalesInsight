using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProductivityReport
    {
        public DiamondReport DiamondParent { get; set; }
        public SalesDashboard DashboardParent { get; set; }

        public List<PlantBudget> Budgets { get; set; }
        public List<CustomerProductivity> Productivities { get; set; }

        public double OutGateActual {get;set;} 
        public double OutGateBudget {get;set;} 
        public double OutGateVariance {get {return OutGateActual - OutGateBudget;}} 
        public MetricIndicatorAllowance OutGateAllowance {get;set;}
        public IndicatorModel OutGateIndicator { get { if (OutGateAllowance == null) return null; else return new IndicatorModel(OutGateActual, OutGateBudget, OutGateAllowance.Ok, OutGateAllowance.Caution, OutGateAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double ToJobActual { get; set; }
        public double ToJobBudget { get; set; }
        public double ToJobVariance { get { return ToJobActual - ToJobBudget; } }
        public MetricIndicatorAllowance ToJobAllowance { get; set; }
        public IndicatorModel ToJobIndicator { get { if (ToJobAllowance == null) return null; else return new IndicatorModel(ToJobActual, ToJobBudget, ToJobAllowance.Ok, ToJobAllowance.Caution, ToJobAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double WaitActual { get; set; }
        public double WaitBudget { get; set; }
        public double WaitVariance { get { return WaitActual - WaitBudget; } }
        public MetricIndicatorAllowance WaitAllowance { get; set; }
        public IndicatorModel WaitIndicator { get { if (WaitAllowance == null) return null; else return new IndicatorModel(WaitActual, WaitBudget, WaitAllowance.Ok, WaitAllowance.Caution, WaitAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double UnloadActual { get; set; }
        public double UnloadBudget { get; set; }
        public double UnloadVariance { get { return UnloadActual - UnloadBudget; } }
        public MetricIndicatorAllowance UnloadAllowance { get; set; }
        public IndicatorModel UnloadIndicator { get { if (UnloadAllowance == null) return null; else return new IndicatorModel(UnloadActual, UnloadBudget, UnloadAllowance.Ok, UnloadAllowance.Caution, UnloadAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double WashActual { get; set; }
        public double WashBudget { get; set; }
        public double WashVariance { get { return WashActual - WashBudget; } }
        public MetricIndicatorAllowance WashAllowance { get; set; }
        public IndicatorModel WashIndicator { get { if (WashAllowance == null) return null; else return new IndicatorModel(WashActual, WashBudget, WashAllowance.Ok, WashAllowance.Caution, WashAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double FromJobActual { get; set; }
        public double FromJobBudget { get; set; }
        public double FromJobVariance { get { return FromJobActual - FromJobBudget; } }
        public MetricIndicatorAllowance FromJobAllowance { get; set; }
        public IndicatorModel FromJobIndicator { get { if (FromJobAllowance == null) return null; else return new IndicatorModel(FromJobActual, FromJobBudget, FromJobAllowance.Ok, FromJobAllowance.Caution, FromJobAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double CydHrActual { get; set; }
        public double CydHrBudget { get; set; }
        public double CydHrVariance { get { return CydHrActual - CydHrBudget; } }
        public MetricIndicatorAllowance CydHrAllowance { get; set; }
        public IndicatorModel CydHrIndicator { get { if (CydHrAllowance == null) return null; else return new IndicatorModel(CydHrActual, CydHrBudget, CydHrAllowance.Ok, CydHrAllowance.Caution, CydHrAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double RoundTripActual { get; set; }
        public double RoundTripBudget { get; set; }
        public double RoundTripVariance { get { return RoundTripActual - RoundTripBudget; } }
        public MetricIndicatorAllowance RoundTripAllowance { get; set; }
        public IndicatorModel RoundTripIndicator { get { if (RoundTripAllowance == null) return null; else return new IndicatorModel(RoundTripActual, RoundTripBudget, RoundTripAllowance.Ok, RoundTripAllowance.Caution, RoundTripAllowance.LessIsBetter.GetValueOrDefault()); } }

        public double AvgLoadActual { get; set; }
        public double AvgLoadBudget { get; set; }
        public double AvgLoadVariance { get { return AvgLoadActual - AvgLoadBudget; } }
        public MetricIndicatorAllowance AvgLoadAllowance { get; set; }
        public IndicatorModel AvgLoadIndicator { get { if (AvgLoadAllowance == null) return null; else return new IndicatorModel(AvgLoadActual, AvgLoadBudget, AvgLoadAllowance.Ok, AvgLoadAllowance.Caution, AvgLoadAllowance.LessIsBetter.GetValueOrDefault()); } }

        public ProductivityReport(DiamondReport parent,List<Plant> plantList = null)
        {
            this.DiamondParent = parent;
            LoadCustomerProductivities();
            LoadBudgets();
            LoadTotalMetrics(plantList);
            LoadAllowances();
        }
        public ProductivityReport(SalesDashboard parent, List<Plant> plantList = null)
        {
            this.DashboardParent = parent;
            LoadDistrictProductivities();
            LoadDistrictBudgets();
            LoadTotalMetrics(plantList);
            LoadAllowances();
        }
        private void LoadDistrictBudgets()
        {
            this.Budgets = new List<PlantBudget>();
            string[] plantCodes = Productivities.Select(x => x.PlantDispatchCode).Distinct().ToArray();
            foreach (string plantCode in plantCodes)
            {
                DateTime tmpDate = DashboardParent.StartDate;
                while (tmpDate <= DashboardParent.EndDate)
                {
                    PlantBudget b = SIDAL.GetPlantBudgets(plantCode, tmpDate);
                    if (b != null)
                    {
                        int count = Productivities.Where(x => x.PlantDispatchCode == plantCode).Where(x => x.ReportDate.Month == tmpDate.Month).Where(x => x.ReportDate.Year == tmpDate.Year).Count();
                        for (int i = 0; i < count; i++)
                        {
                            this.Budgets.Add(b);
                        }
                    }
                    tmpDate = tmpDate.AddMonths(1);
                }
            }
        }
        private void LoadDistrictProductivities()
        {
            this.Productivities = SIDAL.GetDistrictProductivityReport(this.DashboardParent.SelectedDistricts, this.DashboardParent.StartDate, this.DashboardParent.EndDate);
        }
        private void LoadCustomerProductivities()
        {
            this.Productivities = SIDAL.GetCustomerProductivityReport(this.DiamondParent.CustomerIds, this.DiamondParent.StartDate, this.DiamondParent.EndDate);
        }
        private void LoadBudgets()
        {
            this.Budgets = new List<PlantBudget>();
            string[] plantCodes = Productivities.Select(x => x.PlantDispatchCode).Distinct().ToArray();
            foreach (string plantCode in plantCodes)
            {
                DateTime tmpDate = DiamondParent.StartDate;
                while (tmpDate <= DiamondParent.EndDate)
                {
                    PlantBudget b = SIDAL.GetPlantBudgets(plantCode, tmpDate);
                    if (b != null)
                    {
                        int count = Productivities.Where(x => x.PlantDispatchCode == plantCode).Where(x => x.ReportDate.Month == tmpDate.Month).Where(x => x.ReportDate.Year == tmpDate.Year).Count();
                        for (int i = 0; i < count; i++)
                        {
                            this.Budgets.Add(b);
                        }
                    }
                    tmpDate = tmpDate.AddMonths(1);
                }
            }
        }
        private void LoadAllowances()
        {
            var metricIndicatorList = SIDAL.GetAllMetricIndicator();
            this.OutGateAllowance = SIDAL.GetMetricIndicator("OutGate", metricIndicatorList);
            this.ToJobAllowance = SIDAL.GetMetricIndicator("ToJob", metricIndicatorList);
            this.WaitAllowance = SIDAL.GetMetricIndicator("Wait", metricIndicatorList);
            this.UnloadAllowance = SIDAL.GetMetricIndicator("Unload", metricIndicatorList);
            this.WashAllowance = SIDAL.GetMetricIndicator("Wash", metricIndicatorList);
            this.FromJobAllowance = SIDAL.GetMetricIndicator("FromJob", metricIndicatorList);
            this.CydHrAllowance = SIDAL.GetMetricIndicator("CYDHr", metricIndicatorList);
            try
            {
                this.RoundTripAllowance = new MetricIndicatorAllowance();
                this.RoundTripAllowance.Ok = OutGateAllowance.Ok + ToJobAllowance.Ok + WaitAllowance.Ok + UnloadAllowance.Ok + WashAllowance.Ok + FromJobAllowance.Ok;
                this.RoundTripAllowance.Caution = OutGateAllowance.Caution + ToJobAllowance.Caution + WaitAllowance.Caution + UnloadAllowance.Caution + WashAllowance.Caution + FromJobAllowance.Caution;
                this.RoundTripAllowance.LessIsBetter = true;
            }
            catch (Exception ex)
            {
                this.RoundTripAllowance = null;
            }
            this.AvgLoadAllowance = SIDAL.GetMetricIndicator("AvgLoad", metricIndicatorList);
        }
        private void LoadTotalMetrics(List<Plant> plantList = null)
        {
            if (this.Productivities != null && this.Productivities.Count > 0)
            {
                this.OutGateActual = this.Productivities.Average(x => x.Ticketing + x.LoadTemper) ;
                this.ToJobActual = this.Productivities.Average(x => x.ToJob);
                this.WaitActual = this.Productivities.Average(x => x.Wait);
                this.UnloadActual = this.Productivities.Average(x => x.Unload);
                this.WashActual = this.Productivities.Average(x => x.Wash);
                this.FromJobActual = this.Productivities.Average(x => x.FromJob);
                //this.CydHrActual = (this.Productivities.Sum(x => x.Quantity) / (this.Productivities.Sum(x => (x.Ticketing + x.LoadTemper + x.ToJob + x.Wait + x.Unload + x.Wash + x.FromJob) / Convert.ToDouble(SIDAL.GetPlantByDispatchCode(x.PlantDispatchCode)?.Utilization.GetValueOrDefault(1))))) * 60;
                //this.CydHrActual = (this.Productivities.Sum(x => x.Quantity) / (this.Productivities.Sum(x => (x.Ticketing + x.LoadTemper + x.ToJob + x.Wait + x.Unload + x.Wash + x.FromJob) / Convert.ToDouble(SIDAL.GetPlantUtilizationByDispatchCode(x.PlantDispatchCode))))) * 60;
                this.CydHrActual = (this.Productivities.Sum(x => x.Quantity) / (this.Productivities.Sum(x => (x.Ticketing + x.LoadTemper + x.ToJob + x.Wait + x.Unload + x.Wash + x.FromJob) / Convert.ToDouble(plantList.Where(p => p.DispatchId == x.PlantDispatchCode).Select(p => p.Utilization).FirstOrDefault().GetValueOrDefault(1))))) * 60;
                this.RoundTripActual = this.OutGateActual + this.ToJobActual + this.WaitActual + this.UnloadActual + this.WashActual + this.FromJobActual;
                this.AvgLoadActual = this.Productivities.Sum(x => x.Quantity) / this.Productivities.Count();
            }
            if (this.Budgets != null && this.Budgets.Count > 0)
            {
                this.OutGateBudget = this.Budgets.Average(x => x.Ticketing.GetValueOrDefault() + x.Tempering.GetValueOrDefault() + x.Loading.GetValueOrDefault());
                this.ToJobBudget = this.Budgets.Average(x => x.ToJob.GetValueOrDefault());
                this.WaitBudget = this.Budgets.Average(x => x.Wait.GetValueOrDefault());
                this.UnloadBudget = this.Budgets.Average(x => x.Unload.GetValueOrDefault());
                this.WashBudget = this.Budgets.Average(x => x.Wash.GetValueOrDefault());
                this.FromJobBudget = this.Budgets.Average(x => x.FromJob.GetValueOrDefault());
                this.CydHrBudget = this.Budgets.Average(x => x.CydHr.GetValueOrDefault());
                this.RoundTripBudget = this.OutGateBudget + this.ToJobBudget + this.WaitBudget + this.UnloadBudget + this.WashBudget + this.FromJobBudget;
                this.AvgLoadBudget = this.Budgets.Average(x => x.AvgLoad.GetValueOrDefault());
            }
        }

        public string ChartData
        {
            get
            {
                JArray array = new JArray();
                JArray objLabels = new JArray();
                JArray objValues = new JArray();
                List<int[]> ranges = GetMinuteRanges();
                if (ranges != null)
                {
                    foreach (int[] range in ranges)
                    {
                        objLabels.Add(range[0] + "-" + range[1]);
                        objValues.Add(GetNumLoadsInRange(range[0], range[1]));
                    }
                }
                array.Add(objLabels);
                array.Add(objValues);
                return array.ToString();
            }
        }
        private int GetNumLoadsInRange(int min, int max)
        {
            if (this.Productivities.Count > 0)
            {
                return this.Productivities.Where(x => x.Wait + x.Unload + x.Wash >= min).Where(x => x.Wait + x.Unload + x.Wash < max).Count();
            }
            return 0;
        }
        private List<int[]> GetMinuteRanges()
        {
            if (this.Budgets != null && this.Budgets.Count > 0)
            {
                int cycleTarget = (int)Math.Floor(this.Budgets.Average(x => x.Wait.GetValueOrDefault()) +
                                        this.Budgets.Average(x => x.Unload.GetValueOrDefault()) +
                                        this.Budgets.Average(x => x.Wash.GetValueOrDefault()));

                List<int[]> ranges = new List<int[]>();
                for (int i = -33; i <= 27; i += 6)
                {
                    int minRange = (int)cycleTarget + i;
                    int maxRange = (int)cycleTarget + i + 6;

                    if (minRange >= 0)
                    {
                        ranges.Add(new int[] { minRange, maxRange });
                    }
                }
                return ranges;
            }
            else
            {
                return null;
            }
        }
    }
}
