using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class AddonProjectionView
    {
        public long AddOnId { get; set; }
        public int DistrictId { get; set; }
        public String DistrictName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsAggregateDefault { get; set; }
        public bool IsBlockDefault { get; set; }
        public decimal  Month1 { get; set; }
        public decimal  Month2 { get; set; }
        public decimal  Month3 { get; set; }
        public decimal  Month4 { get; set; }
        public decimal  Month5 { get; set; }
        public decimal  Month6 { get; set; }
        public List<Dictionary<string, bool>> DefaultList { get; set; }

        public bool Month1Actual { get; set; }
        public bool Month2Actual { get; set; }
        public bool Month3Actual { get; set; }
        public bool Month4Actual { get; set; }
        public bool Month5Actual { get; set; }
        public bool Month6Actual { get; set; }

        public AddonProjectionView(long addonId, District district, DateTime currentMonth,string priceMode, long TargetUomId)
        {
            this.AddOnId = addonId;
            this.DistrictId = district.DistrictId;
            this.DistrictName = district.Name;
            this.DefaultList = SIDAL.DefaultList(addonId, district.DistrictId);
            foreach (var item in this.DefaultList)
            {
                if (item.ContainsKey("IsDefault"))
                {
                    this.IsDefault = item["IsDefault"];
                }
                else if (item.ContainsKey("IsAggregateDefault"))
                {
                    this.IsAggregateDefault = item["IsAggregateDefault"];
                }
                else if (item.ContainsKey("IsBlockDefault"))
                {
                    this.IsBlockDefault = item["IsBlockDefault"];
                }
            }
            //this.IsDefault = SIDAL.IsDefault(addonId, district.DistrictId);
            var monthActual = false;
            this.Month1 = SIDAL.FindPrice(AddOnId, DistrictId, currentMonth, 0, priceMode,out monthActual, TargetUomId);
            this.Month1Actual = monthActual;
            this.Month2 = SIDAL.FindPrice(AddOnId, DistrictId, currentMonth, 1, priceMode,out monthActual, TargetUomId);
            this.Month2Actual = monthActual;
            this.Month3 = SIDAL.FindPrice(AddOnId, DistrictId, currentMonth, 2, priceMode,out monthActual, TargetUomId);
            this.Month3Actual = monthActual;
            this.Month4 = SIDAL.FindPrice(AddOnId, DistrictId, currentMonth, 3, priceMode,out monthActual, TargetUomId);
            this.Month4Actual = monthActual;
            this.Month5 = SIDAL.FindPrice(AddOnId, DistrictId, currentMonth, 4, priceMode,out monthActual, TargetUomId);
            this.Month5Actual = monthActual;
            this.Month6 = SIDAL.FindPrice(AddOnId, DistrictId, currentMonth, 5, priceMode,out monthActual, TargetUomId);
            this.Month6Actual = monthActual;
        }
    }
}
