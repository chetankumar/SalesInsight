using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class MarketSegmentView
    {
        public SIUser User { get; set; }

        [Display(Name="Market Segment")]
        public int MarketSegmentId { get; set; }
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string Name { get; set; }

        [Display(Name = "Dispatch System Segment Code")]
        public string DispatchId { get; set; }

        public decimal? Spread { get; set; }
        public decimal? ContMarg { get; set; }
        public decimal? Profit { get; set; }

        [Display(Name="CYD/Hr")]
        public decimal? CYDsHr { get; set; }
        public bool Active { get; set; }

        public List<MarketSegmentDistrictProjection> DistrictProjections { get; set; }

        public MarketSegmentView()
        {
            this.Name = "Default Name";
        }

        public MarketSegmentView(MarketSegment segment,SIUser user = null)
        {
            this.User = user;
            this.MarketSegmentId = segment.MarketSegmentId;
            this.Name = segment.Name;
            this.DispatchId = segment.DispatchId;
            this.Spread = segment.Spread;
            this.ContMarg = segment.ContMarg;
            this.Profit = segment.Profit;
            this.CYDsHr = segment.CYDsHr;
            this.CompanyId = segment.CompanyId;
            this.Active = segment.Active.GetValueOrDefault(false);
            BindValues();
        }

        public void BindValues()
        {
            this.CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
            if (this.User != null)
            {
                DistrictProjections = new List<MarketSegmentDistrictProjection>();
                foreach (District d in SIDAL.GetDistricts(User.UserId).ToList())
                {
                    MarketSegmentDistrictProjection projection = new MarketSegmentDistrictProjection(MarketSegmentId,d);
                    DistrictProjections.Add(projection);
                }
            }
        }

    }
}
