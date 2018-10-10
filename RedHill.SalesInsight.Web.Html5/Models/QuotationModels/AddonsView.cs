using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.QuotationModels
{
    public class AddonsView
    {
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string Per { get; set; }
        public string QuoteUomName { get; set; }
        public bool? IsIncludeTable { get; set; }
        public decimal? Sort { get; set; }
    }
}