using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationAggregateAddonsView
    {
        public long QuotationId { get; set; }
        public long[] SelectedAddons { get; set; }
        public List<QuotationAggregateAddonModel> QuoteAggAddonModel { get; set; }
        public List<AddonView> AllAggregateAddonsList { get; set; }

        public QuotationAggregateAddonsView()
        {

        }

        public QuotationAggregateAddonsView(long id)
        {
            this.QuotationId = id;
        }

        public void Load()
        {
            List<QuotationAggregateAddon> quoteAggAddon = SIDAL.GetQuotationAggregateAddon(this.QuotationId);
            if (quoteAggAddon != null)
            {
                QuoteAggAddonModel = new List<QuotationAggregateAddonModel>();
                foreach (var item in quoteAggAddon)
                {
                    QuoteAggAddonModel.Add(new QuotationAggregateAddonModel(item));
                }
            }

            var districtAddonDefaultList = SIDAL.GetAllDistrictAggregateAddons(this.QuotationId);
            this.AllAggregateAddonsList = new List<AddonView>();
            if (districtAddonDefaultList != null)
            {
                List<Addon> addonList = new List<Addon>();
                List<Addon> addOns = SIDAL.GetAddons(false, null, "Quote");
                foreach (var item in districtAddonDefaultList)
                {
                    Addon addon = addOns.FirstOrDefault(x => x.Id == item.AddonId); //SIDAL.FindAddon(item.AddonId.GetValueOrDefault());
                    this.AllAggregateAddonsList.Add(new AddonView(addon));
                }
            }
        }

    }
}