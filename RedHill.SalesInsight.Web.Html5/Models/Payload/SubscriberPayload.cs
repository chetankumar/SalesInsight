using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.Payload
{
    public class SubscriberPayload
    {
        public List<Guid> New { get; set; }
        public List<long> Remove { get; set; }
    }
}