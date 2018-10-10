using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL
{
    public class SIMixIngredientPriceSheet
    {

        public List<SIMixIngredientPriceSheetRow> PriceRows { get; set; }

        public SIMixIngredientPriceSheet(long standardMixId, int plantId, DateTime? pricingMonth = null)
        {
            this.PriceRows = SIDAL.GetMixPricingSheet(standardMixId, plantId, pricingMonth);
        }

    }
}
