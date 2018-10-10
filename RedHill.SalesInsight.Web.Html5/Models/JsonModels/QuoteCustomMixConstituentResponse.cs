using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.JsonModels
{
    public class QuoteCustomMixConstituentResponse : JsonResponse
    {
        [JsonProperty("customMixConstId")]
        public long CustomMixConstId { get; set; }

        public double Quantity { get; set; }
        public long QuantityUomId { get; set; }
        public decimal Cost { get; set; }
        public long CostUomId { get; set; }
        public string CostUomName { get; set; }
        public string Description { get; set; }
    }
}