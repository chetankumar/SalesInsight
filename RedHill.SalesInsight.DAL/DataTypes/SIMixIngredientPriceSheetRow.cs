using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL
{
    public class SIMixIngredientPriceSheetRow
    {
       
        public SIMixIngredientPriceSheetRow(StandardMixConstituent ingredient)
        {
            this.RawMaterial = SIDAL.FindRawMaterial(ingredient.RawMaterialId);
            this.RawMaterialId = ingredient.RawMaterialId;
            this.Quantity = ingredient.Quantity;
            this.QuantityUOM = SIDAL.FindUOM(ingredient.UomId);
            this.PerCementWeight = ingredient.PerCementWeight.GetValueOrDefault();
            this.IsCementitious = this.RawMaterial.RawMaterialType.IsCementitious.GetValueOrDefault();
            this.IncludeIn5sk = this.RawMaterial.RawMaterialType.IncludeInFSK.GetValueOrDefault();
            this.IncludeInSackCalculation = this.RawMaterial.RawMaterialType.IncludeInSackCalculation.GetValueOrDefault();
            this.FskMarkup = this.RawMaterial.FSKMarkup.GetValueOrDefault();
            this.FskCode = this.RawMaterial.FSKCode;
        }

        public long RawMaterialId { get; set; }
        public RawMaterial RawMaterial { get; set; }

        public double Quantity { get; set; }
        public Uom QuantityUOM { get; set; }

        public bool PerCementWeight { get; set; }
        public bool IsCementitious { get; set; }
        public bool IncludeIn5sk { get; set; }
        public bool IncludeInSackCalculation { get; set; }

        public decimal Cost { get; set; }
        public Uom CostUom { get; set; }

        public double FskMarkup { get; set; }
        public string FskCode { get; set; }

        public double TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }

        public double WeightInPounds 
        {
            get
            {
                return this.Quantity * this.QuantityUOM.BaseConversion * 2.20462;
            }
        }

        public void CalculateTotalCost(double totalCementitiousWeight)
        {
            var row = this;
            this.TotalCost = Convert.ToDecimal(this.QuantityUOM.BaseConversion * row.Quantity / row.CostUom.BaseConversion * row.FskMarkup/100.00) * row.Cost;
            if (row.PerCementWeight)
                row.TotalCost = row.TotalCost * Convert.ToDecimal((totalCementitiousWeight / 100));
        }

        public double TotalQuantityInMix(double totalCementitiousWeight)
        {
            var row = this;
            if (row.PerCementWeight)
                return Quantity * (totalCementitiousWeight / 100);
            else
                return Quantity;
        }

        public decimal CostInQuantityUom
        {
            get
            {
                return this.Cost *  Convert.ToDecimal(this.QuantityUOM.BaseConversion / this.CostUom.BaseConversion ); 
            }
        }
    }
}
