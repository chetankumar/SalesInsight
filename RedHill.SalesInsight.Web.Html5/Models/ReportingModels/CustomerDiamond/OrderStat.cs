using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class OrderStat
    {
        public string orderId { get; set; }
        public double FirstVolume { get; set; }
        public double LastVolume { get; set; }
        public double LostVolume { get; set; }
        public double GainedVolume { get; set; }
        public int    NumGainChanges { get; set; }
        public int    NumLossChanges { get; set; }
    }
}
