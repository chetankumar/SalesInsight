using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Models.QuotationModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationAddonsView
    {
        public Guid UserId { get; set; }
        public long QuotationId { get; set; }        
        public int PlantId { get; set; }
        public long[] SelectedAddons { get; set; }
        public List<QuotationAddonModel> QuoteAddon { get; set; }
        public List<Addon> AllAddons { get; set; }
        public List<AddonView> AllAddonsList { get; set; }


        public QuotationAddonsView()
        {

        }

        public QuotationAddonsView(long id)
        {
            this.QuotationId = id;
        }

        public void Load(Quotation q = null)
        {
            if (q == null)
            {
                q = SIDAL.FindQuotation(this.QuotationId);
            }

            AllAddons = SIDAL.GetAddons(false, null, "Quote");
            List<QuotationAddon> quoteAddonList = SIDAL.GetQuotationAddOns(QuotationId);
            if (quoteAddonList != null)
            {
                this.QuoteAddon = new List<QuotationAddonModel>();
                foreach (QuotationAddon qon in quoteAddonList)
                {
                    this.QuoteAddon.Add(new QuotationAddonModel(qon, AllAddons));
                }
            }
            this.PlantId = q.PlantId.GetValueOrDefault();
            List<Addon> addons = quoteAddonList.Select(x => x.Addon).ToList(); //SIDAL.GetAddonsForQuote(QuotationId);
            this.SelectedAddons = addons.Select(x => x.Id).ToArray();

            var plant = SIDAL.GetPlant(this.PlantId);

            var addonQuoteCosts = SIDAL.GetCurrentAddonQuoteCosts(plant.DistrictId, q.PricingMonth);

            //AllAddons = AllAddons.Where(x => x.QuoteUom.Name != "N.A.").Where(x => FindCurrentCost(x.Id, q.PricingMonth) > 0).OrderBy(x => x.AddonType).ThenBy(x => x.Description).ToList();
            AllAddons = AllAddons.Where(x => x.QuoteUom.Name != "N.A.")
                .Where(x => addonQuoteCosts.FirstOrDefault(y => y.AddonId == x.Id)?.Price > 0)
                .OrderBy(x => x.AddonType)
                .ThenBy(x => x.Description)
                .ToList();

            if (AllAddons != null)
            {
                this.AllAddonsList = new List<AddonView>();
                foreach (Addon aon in AllAddons)
                {
                    this.AllAddonsList.Add(new AddonView(aon));
                }
            }
        }

        public decimal FindCurrentCost(long addonId, DateTime? pricingMonth)
        {
            if (this.PlantId > 0)
                return SIDAL.FindCurrentAddonQuoteCost(addonId, SIDAL.GetPlant(this.PlantId).DistrictId, pricingMonth);
            else
                return 0;
        }
    }
}