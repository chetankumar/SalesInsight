using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProductivityStat
    {
        public int DistrictId { get; set; }
        public string SegmentId { get; set; }
        private double TotalMins { get; set; }
        private decimal PlantUtilization { get; set; }
        private decimal PlantVariablePerMinute { get; set; }
        public decimal VariableCost { get; set; }

        private decimal PlantFixed { get; set; }
        private decimal DeliveryFixed { get; set; }
        private decimal SGA { get; set; }
        public double Quantity { get; set; }
        public decimal FixedCost { get; set; }

        private decimal BudgetRevenue { get; set; }
        private double BudgetVolume { get; set; }
        public decimal BudgetPrice { get; set; }
        private decimal BudgetMaterialCost { get; set; }
        public decimal BudgetMaterial { get; set; }
        public decimal BudgetSpread { get; set; }

        private double BudgetDeliveryVariable { get; set; }
        private double BudgetPlantVariable { get; set; }
        public double BudgetVariable { get; set; }

        private double BudgetDeliveryFixed { get; set; }
        private double BudgetPlantFixed { get; set; }
        private double BudgetSGA { get; set; }
        public double BudgetFixed { get; set; }

        public decimal BudgetProfit { get; set; }

        public ProductivityStat(CustomerProductivity prod,List<Plant> plantList = null)
        {
            this.TotalMins = prod.Ticketing + prod.LoadTemper + prod.ToJob + prod.Wait + prod.Unload + prod.Wash + prod.FromJob;
            this.Quantity = prod.Quantity;

            this.SegmentId = prod.SegmentId;
            Plant p = null;
            if (plantList != null)
            {
                p = plantList.Where(x => x.DispatchId == prod.PlantDispatchCode).FirstOrDefault();
            }
            else
            {
                p  = SIDAL.GetPlantByDispatchCode(prod.PlantDispatchCode);
            }
         
            if (p != null)
            {
                this.DistrictId = p.DistrictId;
                this.PlantUtilization = p.Utilization.GetValueOrDefault();
                this.PlantVariablePerMinute = p.VariableCostPerMin.GetValueOrDefault();
                this.VariableCost = Convert.ToDecimal(this.TotalMins) * this.PlantVariablePerMinute / this.PlantUtilization;

                this.PlantFixed = p.PlantFixedCost.GetValueOrDefault();
                this.DeliveryFixed = p.DeliveryFixedCost.GetValueOrDefault();
                this.SGA = p.SGA.GetValueOrDefault();
                this.FixedCost = (PlantFixed + DeliveryFixed + SGA) * Convert.ToDecimal(this.Quantity);

                PlantBudget b = SIDAL.GetPlantBudgets(p.DispatchId, prod.ReportDate,p);
                if (b != null)
                {
                    this.BudgetVolume = b.Budget.GetValueOrDefault();
                    if (this.BudgetVolume > 0)
                    {

                        PlantFinancialBudget fb = SIDAL.GetPlantFinancialBudgets(p.DispatchId, prod.ReportDate,p);
                        if (fb != null)
                        {

                            this.BudgetRevenue = fb.Revenue.GetValueOrDefault();
                            this.BudgetMaterialCost = fb.MaterialCost.GetValueOrDefault();

                            this.BudgetPrice = this.BudgetRevenue / Convert.ToDecimal(this.BudgetVolume);
                            this.BudgetMaterial = this.BudgetMaterialCost / Convert.ToDecimal(this.BudgetVolume);
                            this.BudgetSpread = this.BudgetPrice - this.BudgetMaterial;

                            this.BudgetDeliveryVariable = fb.DeliveryVariable.GetValueOrDefault();
                            this.BudgetPlantVariable = fb.PlantVariable.GetValueOrDefault();
                            this.BudgetVariable = (this.BudgetDeliveryVariable + this.BudgetPlantVariable) / this.BudgetVolume;

                            this.BudgetDeliveryFixed = fb.DeliveryFixed.GetValueOrDefault();
                            this.BudgetPlantFixed = fb.PlantFixed.GetValueOrDefault();
                            this.BudgetSGA = fb.SGA.GetValueOrDefault();
                            this.BudgetFixed = (this.BudgetDeliveryFixed + this.BudgetPlantFixed + this.BudgetSGA) / this.BudgetVolume;
                        }
                    }
                }
                this.BudgetProfit = this.BudgetSpread - Convert.ToDecimal(this.BudgetVariable) - Convert.ToDecimal(this.BudgetFixed);
            }
        }
    }
}
