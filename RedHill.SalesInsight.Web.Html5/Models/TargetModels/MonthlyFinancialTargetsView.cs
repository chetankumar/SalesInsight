using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models.TargetModels
{
    public class MonthlyFinancialTargetsView
    {
        public string[] MetricList = new string[] { "Revenue", "Material Cost", "Delivery Variable", "Plant Variable", "Delivery Fixed", "Plant Fixed", "SG&A" };

        public Guid UserId;
        public string SelectedMetric { get; set; }
        public int SelectedPlant { get; set; }
        public string PlantName { get; set; }
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

        public SelectList AllPlants
        {
            get
            {
                var query = SIDAL.GetPlants(UserId);
                if (this.SelectedPlant == 0)
                    this.SelectedPlant = query.First().PlantId;
                return new SelectList(query, "PlantId", "Name", SelectedPlant);
            }
        }

        #endregion

        public MonthlyFinancialTargetsView()
        {
            
        }

        public MonthlyFinancialTargetsView(Guid guid)
        {
            this.UserId = guid;
            this.Year = DateTime.Now.Year;
            this.SelectedMetric = MetricList.First();
        }

        private MonthlyFinancialTargetModel _data { get; set; }

        public MonthlyFinancialTargetModel Targets
        {
            get
            {
                if (_data == null)
                {
                    _data = new MonthlyFinancialTargetModel(SelectedPlant, Year);
                }
                return _data;
            }
        }

    }
}
