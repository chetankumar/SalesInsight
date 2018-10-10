using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIQuotationMixIngredient
    {

        public long Id {get;set;}
        public long RawMaterialId { get; set; }
        public int AddonId { get; set; }
        public bool Cementitious { get; set; }
        public double Quantity { get; set; }
        public Uom QuantityUOM { get; set; }
        public decimal Cost { get; set; }
        public Uom CostUOM { get; set; }
        public bool PerCementWeight {get;set;}

        public bool IsAsh  { get; set; }
        public bool IsSand { get; set; }
        public bool IsRock { get; set; }
        public bool IsSack { get; set; }

        public double FinalBaseQuantity { get; set; }
        public decimal FinalBaseCost { get; set; }

        public const double CementWeightStandard = 45.359237;

        public SIQuotationMixIngredient(StandardMixConstituent mc)
        {
            this.Id = mc.Id;
            this.RawMaterialId = mc.RawMaterialId;
            this.Cementitious = mc.RawMaterial.RawMaterialType.IsCementitious.GetValueOrDefault(false);
            this.Quantity = mc.Quantity;
            this.QuantityUOM = mc.Uom;
            this.PerCementWeight = mc.PerCementWeight.GetValueOrDefault(false);

            this.IsAsh = mc.RawMaterial.RawMaterialType.IncludeInAshCalculation.GetValueOrDefault();
            this.IsSand = mc.RawMaterial.RawMaterialType.IncludeInSandCalculation.GetValueOrDefault();
            this.IsRock = mc.RawMaterial.RawMaterialType.IncludeInRockCalculation.GetValueOrDefault();
            this.IsSack = mc.RawMaterial.RawMaterialType.IncludeInSackCalculation.GetValueOrDefault();
        }

        public SIQuotationMixIngredient(CustomMixConstituent constituent)
        {
            this.Id = constituent.Id;
            this.PerCementWeight = constituent.PerCementWeight.GetValueOrDefault(false);
            this.Cementitious = constituent.IsCementitious.GetValueOrDefault(false);
            this.Quantity = constituent.Quantity;
            this.QuantityUOM = constituent.QuantityUom;
            this.Cost = constituent.Cost.GetValueOrDefault(0);
            this.CostUOM = constituent.CostUom;
        }
        internal static decimal CalculateStandardMixTotal(List<SIQuotationMixIngredient> quoteMixIngredients)
        {
            UpdateIngredientsFinalCostAndWeight(quoteMixIngredients);
            return quoteMixIngredients.Sum(x => x.FinalBaseCost);
        }

        public static void UpdateIngredientsFinalCostAndWeight(List<SIQuotationMixIngredient> quoteMixIngredients)
        {
            UpdateFinalWeights(quoteMixIngredients);
            foreach (SIQuotationMixIngredient ingredient in quoteMixIngredients)
            {
                if (ingredient.CostUOM != null)
                {
                    ingredient.FinalBaseCost = (ingredient.Cost / Convert.ToDecimal(ingredient.CostUOM.BaseConversion)) * Convert.ToDecimal(ingredient.FinalBaseQuantity);
                }
            }
        }

        public static void UpdateFinalWeights(List<SIQuotationMixIngredient> quoteMixIngredients)
        {
            foreach (SIQuotationMixIngredient ingredient in quoteMixIngredients.Where(x => x.PerCementWeight == false))
            {
                ingredient.FinalBaseQuantity = ingredient.Quantity * ingredient.QuantityUOM.BaseConversion;
            }
            double cementitiousBaseQuantity = quoteMixIngredients.Where(x => x.Cementitious == true).Sum(x => x.FinalBaseQuantity);
            foreach (SIQuotationMixIngredient ingredient in quoteMixIngredients.Where(x => x.PerCementWeight == true))
            {
                ingredient.FinalBaseQuantity = (ingredient.Quantity * ingredient.QuantityUOM.BaseConversion) * (cementitiousBaseQuantity / SIQuotationMixIngredient.CementWeightStandard);
                
            }
        }
    }
}
