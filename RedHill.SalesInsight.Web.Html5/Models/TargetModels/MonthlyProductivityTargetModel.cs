using MVC_FIO.Utils;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models.TargetModels
{
    public class MonthlyProductivityTargetModel
    {
        public string       PlantName { get; set; }
        public int          PlantId    { get; set; }
        public List<string>  MetricValues { get; set; }

        public MonthlyProductivityTargetModel()
        {

        }

        public MonthlyProductivityTargetModel(Plant p, int year, string metric)
        {
            this.PlantId = p.PlantId;
            this.PlantName = p.Name;

            DateTime[] yearRange = DateUtils.GetFirstLastDateOfYear(year);

            List<PlantBudget> budgets = SIDAL.GetPlantBudgets(p.PlantId, yearRange.First(), yearRange.Last());

            this.MetricValues = new List<string>();
            DateTime tempDate = yearRange.First();
            while (tempDate <= yearRange.Last())
            {
                MetricValues.Add(GetMetric(tempDate,budgets,metric));
                tempDate = tempDate.AddMonths(1);
            }
            
        }

        private string GetMetric(DateTime tempDate, List<PlantBudget> budgets, string metric)
        {
            if (metric == "Ticketing") {return  budgets.Where(x=>x.BudgetDate == tempDate).Select(x=>x.Ticketing).FirstOrDefault().GetValueOrDefault().ToString("N1");}
            if (metric == "Loading") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Loading).FirstOrDefault().GetValueOrDefault().ToString("N1"); }
            if (metric == "Tempering") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Tempering).FirstOrDefault().GetValueOrDefault().ToString("N1"); }
            if (metric == "ToJob") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.ToJob).FirstOrDefault().GetValueOrDefault().ToString("N1"); }
            if (metric == "Wait") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Wait).FirstOrDefault().GetValueOrDefault().ToString("N1"); }
            if (metric == "Unload") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Unload).FirstOrDefault().GetValueOrDefault().ToString("N1"); }
            if (metric == "Wash") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Wash).FirstOrDefault().GetValueOrDefault().ToString("N1"); }
            if (metric == "FromJob") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.FromJob).FirstOrDefault().GetValueOrDefault().ToString("N1"); }
            if (metric == "CYDHr") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.CydHr).FirstOrDefault().GetValueOrDefault().ToString("N2"); }
            if (metric == "AvgLoad") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.AvgLoad).FirstOrDefault().GetValueOrDefault().ToString("N2"); }
            if (metric == "Volume") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Budget).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "Trucks") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Trucks).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "StartUp") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.StartUp).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "Shutdown") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Shutdown).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "InYard") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.InYard).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "FirstLoadOnTimePercent") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.FirstLoadOnTimePercent).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "TrucksPercentOperable") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.TrucksPercentOperable).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "Accidents") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.Accidents).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "PlantInterruptions") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.PlantInterruptions).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "TrucksDown") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.TrucksDown).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            if (metric == "BatchTolerance") { return budgets.Where(x => x.BudgetDate == tempDate).Select(x => x.BatchTolerance).FirstOrDefault().GetValueOrDefault().ToString("N0"); }
            return "0.0";
        }
    }
}
