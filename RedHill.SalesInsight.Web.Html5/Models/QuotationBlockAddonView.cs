using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationBlockAddonView
    {

        public long QuotationId { get; set; }
        public long[] SelectedAddons { get; set; }
        public List<QuotationBlockAddonModel> QuoteBlockAddonModel { get; set; }
        public List<AddonView> AllBlockAddonsList { get; set; }

        public QuotationBlockAddonView()
        {

        }

        public QuotationBlockAddonView(long id)
        {
            this.QuotationId = id;
        }

        public void Load()
        {
            List<QuotationBlockAddon> quoteBlockAddon = SIDAL.GetQuotationBlockAddon(this.QuotationId);
            if (quoteBlockAddon != null)
            {
                QuoteBlockAddonModel = new List<QuotationBlockAddonModel>();
                foreach (var item in quoteBlockAddon)
                {
                    QuoteBlockAddonModel.Add(new QuotationBlockAddonModel(item));
                }
            }

            var districtAddonDefaultList = SIDAL.GetAllDistrictBlockAddons(this.QuotationId);
            this.AllBlockAddonsList = new List<AddonView>();
            if (districtAddonDefaultList != null)
            {
                List<Addon> addonList = new List<Addon>();
                List<Addon> addOns = SIDAL.GetAddons(false, null, "Quote");
                foreach (var item in districtAddonDefaultList)
                {
                    Addon addon = addOns.FirstOrDefault(x => x.Id == item.AddonId); //SIDAL.FindAddon(item.AddonId.GetValueOrDefault());
                    this.AllBlockAddonsList.Add(new AddonView(addon));
                }
            }
        }
    }
}