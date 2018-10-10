using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class FskCalculation
    {

        public FskCalculation(SIMixIngredientPriceSheet sheet, long fskPriceId, decimal basePrice)
        {
            this.Ingredients = sheet.PriceRows;
            this.FskPrice = SIDAL.FindFSKPrice(fskPriceId);
            this.BasePrice = basePrice;
        }

        public int SackCount { get; set; }
        public decimal BasePrice { get; set; }
        public FSKPrice FskPrice { get; set; }

        private List<SIMixIngredientPriceSheetRow> _ingredients = null;

        public List<SIMixIngredientPriceSheetRow> Ingredients
        {
            get
            {
                return _ingredients != null ? _ingredients : new List<SIMixIngredientPriceSheetRow>();
            }
            set
            {
                _ingredients = value;
            }
        }

        public double TotalSackWeight
        {
            get
            {
                return Ingredients.Where(x => x.IncludeInSackCalculation).Sum(x => x.WeightInPounds);
            }
        }

        public double TotalCementitiousWeight
        {
            get
            {
                return Ingredients.Where(x => x.IsCementitious).Sum(x => x.WeightInPounds);
            }
        }

        public decimal TotalCementitiousCost
        {
            get
            {
                return BasePrice + SackAdjPrice;
            }
        }

        public double TotalSacks
        {
            get
            {
                return TotalSackWeight / 94.00;
            }
        }

        public double AddedSacks
        {
            get
            {
                if (TotalSacks > FskPrice.SackCount)
                {
                    return TotalSacks - FskPrice.SackCount;
                }
                else
                    return 0;
            }
        }

        public double RemovedSacks
        {
            get
            {
                if (TotalSacks < FskPrice.SackCount)
                {
                    return FskPrice.SackCount - TotalSacks;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal SackAdjPrice
        {
            get
            {
                return Convert.ToDecimal(AddedSacks) * FskPrice.AddPrice - Convert.ToDecimal(RemovedSacks) * FskPrice.DeductPrice;
            }
        }

        public double SackAdjustment
        {
            get
            {
                return AddedSacks - RemovedSacks;
            }
        }

        public List<NameValuePair> RawMaterialTypeVolume
        {
            get
            {
                return this.Ingredients.Where(x => x.IncludeInSackCalculation).GroupBy(x => x.RawMaterial.RawMaterialType.Name).Select(x => new NameValuePair
                {
                    Name = x.Key,
                    Value = x.Sum(y => y.WeightInPounds).ToString("N0")
                }).ToList();
            }
        }
        
        public decimal FinalPrice
        {
            get
            {
                return TotalCementitiousCost + TotalValueAddCost;
            }
        }

        public decimal TotalValueAddCost
        {
            get
            {
                decimal totalCost = 0;
                if (Ingredients != null)
                {
                    foreach (var row in Ingredients.Where(x => x.IsCementitious == false).Where(x => x.IncludeIn5sk == true))
                    {
                        row.CalculateTotalCost(TotalCementitiousWeight);
                        totalCost += row.TotalCost;
                    }
                }
                return totalCost;
            }
        }

        public string Content
        {
            get
            {
                var content = TotalSacks.ToString("N2") + "sk";
                foreach (var row in Ingredients.Where(x => x.IsCementitious == false).Where(x => x.IncludeIn5sk == true))
                {
                    content += ", " + row.FskCode;
                }
                return content;
            }
        }
    }
}
