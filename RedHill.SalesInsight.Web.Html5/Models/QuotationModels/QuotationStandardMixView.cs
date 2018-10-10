using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utils;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationStandardMixView
    {
        public long QuotationId { get; set; }
        public long QuotationMixId { get; set; }
        public int PlantId { get; set; }
        public QuotationProfile Profile { get; set; }

        public long StandardMixId { get; set; }
        public string QuotedDescription { get; set; }

        [Required(ErrorMessage="Volume is required")]
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

        public int?    PSI { get; set; }
        public string Slump { get; set; }
        public string Air { get; set; }
        public string MD1 { get; set; }
        public string MD2 { get; set; }
        public string MD3 { get; set; }
        public string MD4 { get; set; }
        public long SelectedAddonId { get; set; }

        public DateTime? PricingMonth { get; set; }

        public List<MixLevelAddon> MixLevelAddons { get; set; }
        public MixLevelAddonView SelectedAddon { get; set; }
        public int MixLevelAddonId { get; set; }

        public QuotationStandardMixView()
        {
            this.PricingMonth = DateUtils.GetFirstOf(DateTime.Today);
        }

        public QuotationStandardMixView(int plantId, int marketSegmentId, long standardMixId)
        {
            Plant configPlant = SIDAL.GetPlant(plantId);
            District configDistrict = SIDAL.GetDistrict(configPlant.DistrictId);
            this.Volume = 100;
            this.AverageLoad = configDistrict.Load.GetValueOrDefault();
            this.Unload = (int)configDistrict.Unload.GetValueOrDefault();
            DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(marketSegmentId, configDistrict.DistrictId);
            if (dms != null)
                this.Spread = dms.Spread.GetValueOrDefault();

            this.PlantId = PlantId;
            this.StandardMixId = standardMixId;
            this.PricingMonth = DateUtils.GetFirstOf(DateTime.Today);
        }

        public QuotationStandardMixView(Quotation quotation)
        {
            this.QuotationId = quotation.Id;
            Plant configPlant = SIDAL.GetPlant(quotation.PlantId);
            District configDistrict = SIDAL.GetDistrict(configPlant.DistrictId);
            this.Volume = 100;
            this.AverageLoad = configDistrict.Load.GetValueOrDefault();
            this.Unload = (int)configDistrict.Unload.GetValueOrDefault();
            DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(quotation.Project.MarketSegmentId.GetValueOrDefault(), configDistrict.DistrictId);
            if (dms != null)
                this.Spread = dms.Spread.GetValueOrDefault();

            this.PlantId = quotation.PlantId.GetValueOrDefault();
            if (quotation.PricingMonth != null)
                this.PricingMonth = quotation.PricingMonth;
            else
                this.PricingMonth = DateUtils.GetFirstOf(DateTime.Today);
        }

        public QuotationStandardMixView(QuotationMix qm)
        {
            Quotation q = SIDAL.FindQuotation(qm.QuotationId.GetValueOrDefault());
            this.QuotationId = qm.QuotationId.GetValueOrDefault();
            this.QuotationMixId = qm.Id;
            this.StandardMixId = qm.StandardMixId.GetValueOrDefault();
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
            this.PlantId = q.PlantId.GetValueOrDefault();
            this.SelectedAddon = new MixLevelAddonView();

            if (q.PricingMonth != null)
                this.PricingMonth = q.PricingMonth;
            else
                this.PricingMonth = DateUtils.GetFirstOf(DateTime.Today);

            LoadProfile();
        }

        public QuotationMix ToEntity()
        {
            QuotationMix mix = new QuotationMix();
            mix.Id = this.QuotationMixId;
            mix.QuotationId = this.QuotationId;
            mix.StandardMixId = this.StandardMixId;
            mix.QuotedDescription = this.QuotedDescription;
            mix.Volume = this.AverageLoad;
            mix.Price = this.Price;
            mix.AvgLoad = this.AverageLoad;
            mix.Unload = this.Unload;
            mix.MixCost = this.MixCost;
            mix.Spread = this.Spread;
            mix.Contribution = this.Contribution;
            mix.Profit = this.Profit;

            mix.PrivateNotes = this.PrivateNotes;
            mix.PublicNotes = this.PublicNotes;

            return mix;
        }

        public void LoadProfile()
        {
            this.Profile = new QuotationProfile(QuotationId);
            this.Profile.Load();

            if (this.QuotationMixId > 0)
                this.MixLevelAddons = SIDAL.GetMixLevelAddons(this.QuotationMixId);
        }


        public SelectList ChoosePSI
        {
            get
            {
                return new SelectList(SIDAL.GetPSIValues(),PSI);
            }
        }

        public SelectList ChooseAir
        {
            get
            {
                return new SelectList(SIDAL.GetAirValues(), Air);
            }
        }

        public SelectList ChooseSlump
        {
            get
            {
                return new SelectList(SIDAL.GetSlumpValues(), Slump);
            }
        }

        public SelectList ChooseMD1
        {
            get
            {
                return new SelectList(SIDAL.GetMixCustom1Values(), MD1);
            }
        }

        public SelectList ChooseMD2
        {
            get
            {
                return new SelectList(SIDAL.GetMixCustom2Values(), MD2);
            }
        }

        public SelectList ChooseMD3
        {
            get
            {
                return new SelectList(SIDAL.GetMixCustom3Values(), MD3);
            }
        }

        public SelectList ChooseMD4
        {
            get
            {
                return new SelectList(SIDAL.GetMixCustom4Values(), MD4);
            }
        }
        

        public SelectList ChooseStandardMix
        {
            get
            {
                var list = SIDAL.SearchStandardMix(PlantId, PSI, Slump, Air, MD1, MD2, MD3, MD4).Select(s=> new {Text = s.Number+" - "+s.SalesDesc,Value=s.Id});
                return new SelectList(list, "Value", "Text", StandardMixId);
            }
        }

        public String MixNumber
        {
            get
            {
                return SIDAL.FindStandardMix(this.StandardMixId).Number;
            }
        }

        public GlobalSetting _Setting;
        public GlobalSetting Setting
        {
            get
            {
                if (_Setting == null)
                {
                    _Setting = SIDAL.GetGlobalSettings();
                }
                return _Setting;
            }
        }

        public string MD1String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD1;
            }
        }

        public string MD2String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD2;
            }
        }

        public string MD3String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD3;
            }
        }

        public string MD4String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD4;
            }
        }

    }
}