using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationAggregateView
    {
        public long QuotationId { get; set; }
        public long[] SelectedAddons { get; set; }
        public List<QuotationAggregateModel> QuoteAggModel { get; set; }
        public List<AggregateProductView> AggProductList { get; set; }

        public QuotationAggregateView()
        {

        }

        public QuotationAggregateView(long id)
        {
            this.QuotationId = id;
        }

        public void Load()
        {
            List<QuotationAggregate> QuoteAgg = SIDAL.GetQuotationAggregate(this.QuotationId);
            if (QuoteAgg != null)
            {
                QuoteAggModel = new List<QuotationAggregateModel>();
                foreach (var item in QuoteAgg)
                {
                    QuoteAggModel.Add(new QuotationAggregateModel(item));
                }
            }
            var AllAggregateProduct = SIDAL.GetAggregateProducts(true);
            AggProductList = new List<AggregateProductView>();
            foreach (var item in AllAggregateProduct)
            {
                AggProductList.Add(new AggregateProductView(item));
            }
        }

    }
}