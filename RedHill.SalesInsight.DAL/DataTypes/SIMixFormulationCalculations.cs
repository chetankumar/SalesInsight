using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIMixFormulationCalculations
    {
        public MixFormulation MixFormulation { get; set; }
        public List<SIQuotationMixIngredient> MixIngredients { get; set; }

        public SIMixFormulationCalculations(MixFormulation mf)
        {
            this.MixFormulation = mf;
            MixIngredients = mf.StandardMixConstituents.Select(x => new SIQuotationMixIngredient(x)).ToList();
            SIQuotationMixIngredient.UpdateFinalWeights(MixIngredients);
        }

        public double TotalAshPercentage
        {
            get
            {
                // Sum of all material which is ash.
                var totalAshWeight = MixIngredients.Where(x => x.IsAsh).Sum(x => x.FinalBaseQuantity);
                // Sum of all material which is sack.
                var totalSackWeight = MixIngredients.Where(x => x.IsSack).Sum(x => x.FinalBaseQuantity);

                if (totalSackWeight > 0)
                {
                    return Math.Round(totalAshWeight / totalSackWeight, 2) * 100;
                }
                return 0;
            }
        }

        public double TotalFineAggPercentage
        {
            get
            {
                // Sum of total sand material weight.
                var totalSandWeight = MixIngredients.Where(x => x.IsSand).Sum(x => x.FinalBaseQuantity);
                // Sum of total rock material weight
                var totalRockWeight = MixIngredients.Where(x => x.IsRock).Sum(x => x.FinalBaseQuantity);


                if (totalRockWeight + totalSandWeight > 0)
                {
                    return Math.Round(totalSandWeight / (totalSandWeight + totalRockWeight), 2) * 100;
                }
                return 0;
            }
        }

        public double TotalSacks
        {
            get
            {
                var totalSackWeight = 2.20462 * MixIngredients.Where(x => x.IsSack).Sum(x => x.FinalBaseQuantity);
                return Math.Round((totalSackWeight / 94),1);
            }
        }
    }
}
