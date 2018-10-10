using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;


namespace RedHill.SalesInsight.Web.Html5.Models.QuotationModels
{
    public class QuotationAddonModel
    {
        public long Id { get; set; }
        public long QuotationId { get; set; }
        public long AddonId { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string Per { get; set; }
        public string QuoteUomName { get; set; }
        public bool? IsIncludeTable { get; set; }
        public decimal? Sort { get; set; }

        public QuotationAddonModel()
        { }
        public QuotationAddonModel(QuotationAddon quoteAddon, List<Addon> allAddons = null)
        {
            this.Id = quoteAddon.Id;
            this.AddonId = quoteAddon.AddonId;
            this.Description = quoteAddon.Description;
            this.QuotationId = quoteAddon.QuotationId;
            this.Price = quoteAddon.Price;
            if (allAddons == null)
            {
                allAddons = SIDAL.GetAddons(false, null, "Quote");
            }
            this.QuoteUomName = allAddons.Where(x => x.Id == quoteAddon.AddonId).Select(x => x.QuoteUom.Name).FirstOrDefault();

            this.Per = allAddons.Where(x => x.Id == this.AddonId).Select(x => x.AddonType).FirstOrDefault();
            this.IsIncludeTable = quoteAddon.IsIncludeTable;
            this.Sort = quoteAddon.Sort == null? SIDAL.GetAddonSortOrder(quoteAddon.AddonId) : quoteAddon.Sort;
        }
    }
}