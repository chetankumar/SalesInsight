using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class MarketSegmentDistrictProjection
    {
        public int MarketSegmentId { get; set; }
        public int DistrictId { get; set; }
        public String DistrictName { get; set; }
        public decimal Spread { get; set; }
        public decimal ContMarg { get; set; }
        public decimal Profit { get; set; }
        public double CydHr { get; set; }
        public double WinRate { get; set; }

        public MarketSegmentDistrictProjection(int marketSegmentId, District district)
        {
            this.MarketSegmentId = marketSegmentId;
            this.DistrictId = district.DistrictId;
            this.DistrictName = district.Name;
            DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(marketSegmentId, DistrictId);
            if (dms != null)
            {
                this.Spread = dms.Spread.GetValueOrDefault(0);
                this.ContMarg = dms.ContMarg.GetValueOrDefault(0);
                this.Profit = dms.Profit.GetValueOrDefault(0);
                this.CydHr = dms.CydHr.GetValueOrDefault(0);
                this.WinRate = dms.WinRate.GetValueOrDefault();
            }
        }
    }
}
