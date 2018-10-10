using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models.TargetModels
{
    public class MonthlyProductivityTargetsView
    {
        // Metric DB key, Name pairs
        private string[][] metricList = new string[][] {
                new string[] {"Volume","Volume"},
                new string[] {"Trucks","Trucks"} ,
                new string[] {"Ticketing","Ticketing"},
                new string[] {"Loading","Loading"},
                new string[] {"Tempering","Tempering"},
                new string[] {"ToJob","To Job"},
                new string[] {"Wait","Wait"},
                new string[] {"Unload","Unload"},
                new string[] {"Wash","Wash"},
                new string[] {"FromJob","From Job"},
                new string[] {"CYDHr", ConfigurationHelper.Company.DeliveryQtyUomSingular + " / Hr"},
                new string[] {"AvgLoad","Avg Load"},
                new string[] { "StartUp", "StartUp"},
                new string[] { "Shutdown", "Shutdown"},
                new string[] { "InYard", "In Yard"},
                new string[] { "FirstLoadOnTimePercent", "First Load On Time Percent"},
                new string[] { "TrucksPercentOperable", "Trucks Percent Operable"},
                new string[] { "Accidents", "Accidents"},
                new string[] { "PlantInterruptions", "Plant Intrruptions"},
                new string[] { "TrucksDown", "Trucks Down"},
                new string[] { "BatchTolerance", "Batch Tolerance"} };


        public Guid UserId;
        public string SelectedMetric { get; set; }
        public int[] SelectedDistricts { get; set; }
        public int Year { get; set; }


        #region Dropdowns

        public List<SelectListItem> AllYears
        {
            get
            {
                List<SelectListItem> years = new List<SelectListItem>();
                int currentYear = DateTime.Now.Year;
                for (int i = -5; i <= 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = (currentYear + i) + "";
                    item.Text = (currentYear + i) + "";
                    item.Selected = currentYear + i == Year;
                    years.Add(item);
                }
                return years;
            }
        }

        public List<SelectListItem> AllMetrics
        {
            get
            {
                List<SelectListItem> metrics = new List<SelectListItem>();
                foreach (var text in metricList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = text[0];
                    item.Text = text[1];
                    item.Selected = text[0] == SelectedMetric;
                    metrics.Add(item);
                }
                return metrics;
            }
        }

        public SelectList AllDistricts
        {
            get
            {
                var query = SIDAL.GetDistricts(UserId);
                if (SelectedDistricts == null)
                    SelectedDistricts = query.Select(x => x.DistrictId).ToArray();
                return new SelectList(query, "DistrictId", "Name", SelectedDistricts);
            }
        }

        #endregion

        public MonthlyProductivityTargetsView()
        {

        }

        public MonthlyProductivityTargetsView(Guid guid)
        {
            this.UserId = guid;
            this.Year = DateTime.Now.Year;
            this.SelectedMetric = metricList.First().First();
        }

        public List<MonthlyProductivityTargetModel> PlantMetrics
        {
            get
            {
                List<MonthlyProductivityTargetModel> metrics = new List<MonthlyProductivityTargetModel>();
                foreach (var plant in SIDAL.GetPlantsForDistricts(SelectedDistricts))
                {
                    var metric = new MonthlyProductivityTargetModel(plant, Year, SelectedMetric);
                    metrics.Add(metric);
                }
                return metrics;
            }
        }


        public string Message
        {
            get
            {
                if (SelectedMetric == "Trucks")
                {
                    return "Truck productivity calculations use truck counts in Plant Setup when monthly truck value is zero.";
                }
                return null;
            }
        }
    }
}
