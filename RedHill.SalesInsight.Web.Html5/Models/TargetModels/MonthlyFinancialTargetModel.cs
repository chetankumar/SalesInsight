using MVC_FIO.Utils;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models.TargetModels
{
    public class MonthlyFinancialTargetModel
    {
        public string   MetricName { get; set; }
        public List<PlantFinancialBudget>   MetricValues { get; set; }

        public MonthlyFinancialTargetModel()
        {

        }

        public MonthlyFinancialTargetModel(int plantId, int year)
        {
            DateTime[] yearRange = DateUtils.GetFirstLastDateOfYear(year);
            MetricValues = SIDAL.GetPlantFinancialBudgets(plantId, yearRange.First(), yearRange.Last());
        }

        public String GetValues(string metric, int month, int year)
        {
            PlantFinancialBudget budget = MetricValues.Where(x => x.Month == month).Where(x => x.Year == year).FirstOrDefault();
            if (budget == null)
            {
                budget = new PlantFinancialBudget();
            }
            if (metric == "Revenue") { return budget.Revenue.GetValueOrDefault().ToString("N2"); }
            if (metric == "Material Cost") { return budget.MaterialCost.GetValueOrDefault().ToString("N2"); }
            if (metric == "Delivery Variable") { return budget.DeliveryVariable.GetValueOrDefault().ToString("N2"); }
            if (metric == "Plant Variable") { return budget.PlantVariable.GetValueOrDefault().ToString("N2"); }
            if (metric == "Delivery Fixed") { return budget.DeliveryFixed.GetValueOrDefault().ToString("N2"); }
            if (metric == "Plant Fixed") { return budget.PlantFixed.GetValueOrDefault().ToString("N2"); }
            if (metric == "SG&A") { return budget.SGA.GetValueOrDefault().ToString("N2"); }
            double profit = (
                            Convert.ToDouble(budget.Revenue.GetValueOrDefault()) - 
                            Convert.ToDouble(budget.MaterialCost.GetValueOrDefault()) - 
                            budget.DeliveryVariable.GetValueOrDefault() - 
                            budget.PlantVariable.GetValueOrDefault() - 
                            budget.PlantFixed.GetValueOrDefault() -
                            budget.DeliveryFixed.GetValueOrDefault() - 
                            budget.SGA.GetValueOrDefault()
                        );
            if (metric == "Profit") { return  profit.ToString("N2"); }
            return "0.00";
        }
    }
}
