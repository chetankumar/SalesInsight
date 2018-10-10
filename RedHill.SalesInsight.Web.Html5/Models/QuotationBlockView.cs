using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationBlockView
    {

        public long QuotationId { get; set; }
        public long[] SelectedAddons { get; set; }
        public List<QuotationBlockModel> QuoteBlockModel { get; set; }
        public List<BlockProductView> BlockProductList { get; set; }

        public QuotationBlockView()
        {

        }

        public QuotationBlockView(long id)
        {
            this.QuotationId = id;
        }

        public void Load()
        {
            List<QuotationBlock> QuoteBlock = SIDAL.GetQuotationBlock(this.QuotationId);
            if (QuoteBlock != null)
            {
                QuoteBlockModel = new List<QuotationBlockModel>();
                foreach (var item in QuoteBlock)
                {
                    QuoteBlockModel.Add(new QuotationBlockModel(item));
                }
            }
            var AllBlockProduct = SIDAL.GetBlockProducts(true);
            BlockProductList = new List<BlockProductView>();
            foreach (var item in AllBlockProduct)
            {
                BlockProductList.Add(new BlockProductView(item));
            }
        }
    }
}