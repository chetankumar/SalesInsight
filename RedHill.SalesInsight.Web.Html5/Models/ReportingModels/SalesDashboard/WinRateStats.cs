using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class WinRateMonthlyStat
    {
        public DateTime MonthStart {get;set;}
        public double? Target {get;set;}
        public int SoldVolume {get;set;}
        public int LostVolume {get;set;}
        public int TotalVolume
        {
            get
            {
                return SoldVolume + LostVolume;
            }
        }
        public double WinRate 
        {
            get
            {
                return SoldVolume * 100.00f / (SoldVolume + LostVolume);
            }
        }
    }

    public class WinRateDistrictSegmentStat
    {
        public WinRateDistrictSegmentStat(int districtId, int marketSegmentId, int soldVolume, int lostVolume, DateTime date)
        {
            this.DistrictId = districtId;
            this.MarketSegmentId = marketSegmentId;
            this.MonthStart = date;
            this.SoldVolume = soldVolume;
            this.LostVolume = lostVolume;
            try
            {
                this.Target = SIDAL.FindDistrictMarketSegment(this.MarketSegmentId, this.DistrictId).WinRate.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                this.Target = null;
            }
        }
        public int DistrictId { get; set; }
        public int MarketSegmentId { get; set; }
        public DateTime MonthStart { get; set; }
        public double? Target { get; set; }
        public int SoldVolume { get; set; }
        public int LostVolume { get; set; }
        public int TotalVolume
        {
            get
            {
                return SoldVolume + LostVolume;
            }
        }
    }
}
