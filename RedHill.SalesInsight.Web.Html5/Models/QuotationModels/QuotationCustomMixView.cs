using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationCustomMixView
    {
        public long QuotationId { get; set; }
        public long QuotationMixId { get; set; }
        public int DistrictId { get; set; }
        public QuotationProfile Profile { get; set; }
        public string QuotationCustomMixId { get; set; }
        [Required(ErrorMessage="Please enter a description")]
        public string QuotedDescription { get; set; }

        [Required(ErrorMessage = "Volume is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please volume needs to be more than 0")]
        public double Volume { get; set; }
        public decimal Price { get; set; }
        public double AverageLoad { get; set; }
        public int Unload { get; set; }

        public decimal MixCost { get; set; }
        public decimal AddOnCost { get; set; }
        public decimal Spread { get; set; }
        public decimal Contribution { get; set; }
        public decimal Profit { get; set; }

        public int Position { get; set; }
        public string PrivateNotes { get; set; }
        public string PublicNotes { get; set; }

        public CustomMixConstituentView AddonConstituent { get; set; }
        public CustomMixConstituentView RawMaterialConstituent { get; set; }
        public CustomMixConstituentView NonStandardConstituent { get; set; }

        public List<CustomMixConstituent> CustomMixConstituents { get; set; }

        public QuotationCustomMixView()
        {

        }

        public QuotationCustomMixView(Quotation quotation)
        {
            this.QuotationId = quotation.Id;
            Plant configPlant = SIDAL.GetPlant(quotation.PlantId);
            District configDistrict = SIDAL.GetDistrict(configPlant.DistrictId);
            this.DistrictId = configDistrict.DistrictId ;
            this.Volume = configDistrict.Load.GetValueOrDefault();
            this.AverageLoad = configDistrict.Load.GetValueOrDefault();
            this.Unload = (int)configDistrict.Unload.GetValueOrDefault();
            DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(quotation.Project.MarketSegmentId.GetValueOrDefault(), configDistrict.DistrictId);
            if (dms != null)
            {
                this.Spread = dms.Spread.GetValueOrDefault();
                this.Price = this.Spread;
            }
            LoadProfile();
        }

        public QuotationCustomMixView(QuotationMix qm)
        {
            Quotation q = SIDAL.FindQuotation(qm.QuotationId.GetValueOrDefault());
            this.QuotationId = qm.QuotationId.GetValueOrDefault();
            this.QuotationMixId = qm.Id;
            this.QuotationCustomMixId = qm.CustomMixId;
            this.QuotedDescription = qm.QuotedDescription;
            this.Volume = qm.Volume.GetValueOrDefault();
            this.Price = qm.Price.GetValueOrDefault();
            this.AverageLoad = qm.AvgLoad.GetValueOrDefault();
            this.Unload = qm.Unload.GetValueOrDefault();
            this.MixCost = qm.MixCost.GetValueOrDefault();
            this.AddOnCost = qm.AddonCost.GetValueOrDefault();
            this.Spread = qm.Spread.GetValueOrDefault();
            this.Contribution = qm.Contribution.GetValueOrDefault();
            this.Profit = qm.Profit.GetValueOrDefault();
            this.PrivateNotes = qm.PrivateNotes;
            this.PublicNotes = qm.PublicNotes;
            
            LoadProfile();
        }

        public QuotationMix ToEntity()
        {
            QuotationMix mix = new QuotationMix();
            mix.Id = this.QuotationMixId;
            mix.QuotationId = this.QuotationId;
            mix.CustomMixId = this.QuotationCustomMixId;
            mix.QuotedDescription = this.QuotedDescription;
            mix.Volume = this.Volume;
            mix.Price = this.Price;
            mix.AvgLoad = this.AverageLoad;
            mix.Unload = this.Unload;
            mix.MixCost = this.MixCost;
            mix.Spread = this.Spread;
            mix.Contribution = this.Contribution;
            mix.Profit = this.Profit;
            mix.StandardMixId = null;

            mix.PrivateNotes = this.PrivateNotes;
            mix.PublicNotes = this.PublicNotes;

            return mix;
        }

        public void LoadProfile()
        {
            this.Profile = new QuotationProfile(QuotationId);
            this.Profile.Load();
            if (QuotationMixId > 0)
            {
                this.CustomMixConstituents = SIDAL.GetCustomMixConstituents(this.QuotationMixId);
            }
        }
    }
}