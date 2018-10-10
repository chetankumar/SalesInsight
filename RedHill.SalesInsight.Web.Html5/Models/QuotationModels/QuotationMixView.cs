using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationMixView
    {
        public Guid UserId {get;set;}
        public long QuotationId { get; set; }
        public QuotationProfile Profile{get;set;}
        public List<QuotationMix> QuotationMixes { get; set; }

        public QuotationMixView()
        {
           
        }

        public QuotationMixView(long id)
        {
            this.QuotationId = id;
        }

        public void Load()
        {
            QuotationMixes = SIDAL.GetQuotationMixes(QuotationId);
        }
    }
}