using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class MixLevelAddonView
    {
        public long Id { get; set; }
        public long QuotationMixId { get; set; }
        public long AddonId { get; set; }
        public double Quantity { get; set; }
        public decimal Cost { get; set; }

        public MixLevelAddonView()
        {
        }

        public MixLevelAddonView(MixLevelAddon entity)
        {
            this.Id = entity.Id;
            this.Quantity = entity.Quantity.GetValueOrDefault();
            this.QuotationMixId = entity.QuotationMixId;
            this.AddonId = entity.AddonId;
            this.Cost = entity.Cost.GetValueOrDefault();
        }

        public MixLevelAddon ToEntity()
        {
            MixLevelAddon entity = new MixLevelAddon();
            entity.Id = this.Id;
            entity.QuotationMixId = this.QuotationMixId;
            entity.AddonId = this.AddonId;
            entity.Quantity = this.Quantity;
            entity.Cost = this.Cost;
            return entity;
        }

        public SelectList ChooseAddon
        {
            get
            {

                //var list = SIDAL.GetActiveAddons('Mix',)
                var list = SIDAL.GetAddons(false, null, "Mix");
                var list2 = list.Where(x => x.MixUom.Name != "N.A.").Select(x => new { Text = x.Code + " - " + x.Description + " (Per " + x.MixUom.Name + ")", Value = x.Id }).ToList();
                return new SelectList(list2, "Value", "Text", AddonId);
            }
        }

        public SelectList ChooseActiveAddon(long quotationMixId)
        {
                var m = SIDAL.FindQuotationMix(quotationMixId);
                var addonList = SIDAL.GetActiveAddons("Mix", m.Quotation.PlantId.GetValueOrDefault(), m.Quotation.PricingMonth).Where(x => x.MixUom.Name != "N.A.").Select(x => new { Text = x.Code + " - " + x.Description + " (Per " + x.MixUom.Name + ")", Value = x.Id });
                return new SelectList(addonList, "Value", "Text", AddonId);
        }
    }
}