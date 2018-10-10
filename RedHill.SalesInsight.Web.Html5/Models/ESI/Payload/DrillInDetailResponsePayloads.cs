using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI.Payload
{
    public class DrillInDetailResponsePayloads
    {
        #region DrillIn Ticket Stats
        public string TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string MetricName { get; set; }
        public string MetricType { get; set; }
        public string DisplayFormat { get; set; }
        public Dictionary<string, dynamic> Values { get; set; }
        #endregion
    }
}