using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI.Payload
{
    public class DrillInResponsePayload
    {
        public string DimensionRefId { get; set; }
        public string DimensionName { get; set; }
        public string DisplayFormat { get; set; } 
        public Dictionary<string, dynamic> Values { get; set; }
    }
}