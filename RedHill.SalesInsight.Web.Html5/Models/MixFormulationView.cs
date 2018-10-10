using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class MixFormulationView
    {
        public long StandardMixId { get; set; }
        public string MixName { get; set; }
        public string MixNumber { get; set; }
        public string FormulationName { get; set; }
        public string FormulationNumber { get; set; }
        public string PlantName { get; set; }
        public int PlantId { get; set; }
        public long MixConstituentId { get; set; }
        public MixConstituentView SelectedConstituent { get; set; }
        public MixFormulation Formulation { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public MixFormulationView()
        {

        }

        public MixFormulationView(long standardMixId, int plantId)
        {
            this.StandardMixId = standardMixId;
            this.PlantId = plantId; 
        }

        public void Load()
        {
            MixFormulation f = SIDAL.FindFormulation(PlantId, StandardMixId);
            if (f==null)
            {
                Formulation = new MixFormulation();
                Formulation.PlantId = PlantId;
                Formulation.StandardMixId = StandardMixId;
            }else
                Formulation = f;

            this.MixName = SIDAL.FindStandardMix(StandardMixId).Number;
            this.PlantName = SIDAL.GetPlant(PlantId).Name;
            this.UpdatedBy = Formulation.UpdatedBy;
            this.UpdatedOn = Formulation.UpdatedOn;

            if (MixConstituentId == 0)
            {
                this.SelectedConstituent = new MixConstituentView();
            }
            else
            {
                this.SelectedConstituent = new MixConstituentView(SIDAL.FindMixConstituent(MixConstituentId));
            }

            this.SelectedConstituent.PlantId = Formulation.PlantId;

            if (f != null)
            {
                // We need 2 separate lists for standard mix meta data, and other for pricing calculations.
                MixConstituents = SIDAL.GetMixConstituents(f.Id);
                MixConstituentPriceSheet = SIDAL.GetQuoteMixIngredientsWithCosts(f.StandardMixId, f.PlantId, DateTime.Today);
                SIQuotationMixIngredient.UpdateIngredientsFinalCostAndWeight(MixConstituentPriceSheet);
            }
        }

        public List<StandardMixConstituent> MixConstituents{get;set;}
        public List<SIQuotationMixIngredient> MixConstituentPriceSheet { get; set; }


        public decimal CurrentMixCost
        {
            get
            {
                return SIDAL.CalculateCurrentMixCost(StandardMixId,PlantId);
            }
        }

        public decimal CalculateTotalIngredientCost(long mixId)
        {
            if (MixConstituentPriceSheet != null)
            {
                return MixConstituentPriceSheet.Where(x => x.Id == mixId).Select(x=>x.FinalBaseCost).FirstOrDefault();
            }
            return 0;
        }
    }
}