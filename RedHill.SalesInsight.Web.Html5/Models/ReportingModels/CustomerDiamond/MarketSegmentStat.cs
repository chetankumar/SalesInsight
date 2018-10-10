using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class MarketSegmentStat
    {
        public string MarketSegmentName { get; set; }
        public int MarketSegmentId { get; set; }
        public string MarketSegmentDispatchCode { get; set; }

        public decimal Spread { get; set; }
        public double Quantity { get; set; }
        public decimal Target { get; set; }

        public MarketSegmentStat(string segmentId, decimal spread, double quantity, List<DistrictVolumeStat> districtStats)
        {
            MarketSegment m = SIDAL.GetMarketSegmentByDispatch(segmentId);
            if (m != null)
            {
                this.MarketSegmentName = m.Name;
                this.MarketSegmentId = m.MarketSegmentId;
                this.Quantity = quantity;
                this.Spread = spread;
                List<DistrictMarketSegment> dmList = new List<DistrictMarketSegment>();

                foreach (var item in districtStats.Where(x=>x.SegmentId == segmentId).GroupBy(x=>x.DistrictId))
                {

                    dmList.Add(SIDAL.FindDistrictMarketSegment(m.MarketSegmentId, item.Key));
                }

                foreach (DistrictVolumeStat dvs in districtStats.Where(x => x.SegmentId == segmentId))
                {
                    DistrictMarketSegment districtTarget = dmList.Where(x=>x.DistrictId ==  dvs.DistrictId).FirstOrDefault();
                    if (districtTarget != null)
                    {
                        this.Target += districtTarget.Spread.GetValueOrDefault() * Convert.ToDecimal(dvs.DistrictVolume);
                    }
                }

                //decimal totalVolume = Convert.ToDecimal(districtStats.Sum(x => x.DistrictVolume));
                if (quantity > 0)
                    this.Target = this.Target / Convert.ToDecimal(quantity);
            }
            //public MarketSegmentStat(string segmentId, decimal spread, double quantity, List<DistrictVolumeStat> districtStats)
            //{
            //    MarketSegment m = SIDAL.GetMarketSegmentByDispatch(segmentId);
            //    if (m != null)
            //    {
            //        this.MarketSegmentName = m.Name;
            //        this.MarketSegmentId = m.MarketSegmentId;
            //        this.Quantity = quantity;
            //        this.Spread = spread;
            //        foreach (DistrictVolumeStat dvs in districtStats)
            //        {
            //            DistrictMarketSegment districtTarget = SIDAL.FindDistrictMarketSegment(m.MarketSegmentId, dvs.DistrictId);
            //            if (districtTarget != null)
            //            {
            //                this.Target += districtTarget.Spread.GetValueOrDefault() * Convert.ToDecimal(dvs.DistrictVolume);
            //            }
            //        }

            //        decimal totalVolume = Convert.ToDecimal(districtStats.Sum(x => x.DistrictVolume));
            //        if (totalVolume > 0)
            //            this.Target = this.Target / totalVolume;
            //    }

        }
    }
}
