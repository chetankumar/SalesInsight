using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Models.ESI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class DistrictView
    {
        public int DistrictId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool QCRequirement { get; set; }
        public bool LettingDate { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string MapCenterLat { get; set; }
        public string MapCenterLong { get; set; }
        public int MapScaleRadius10 { get; set; }
        public int MapScaleRadius100 { get; set; }
        public int Zoom { get; set; }
        public double Load { get; set; }
        public double Wash { get; set; }
        public double Unload { get; set; }
        public double ToJob { get; set; }
        public double Return { get; set; }
        public int AcceptanceExpiration { get; set; }
        public int QuoteExpiration { get; set; }
        public string QuotationApprovalRequestText { get; set; }
        public string ProjectEntryFormNotification { get; set; }
        public string EmailQuoteToCustomer { get; set; }
        public string Disclaimers { get; set; }
        public string Disclosures { get; set; }
        public string TermsAndConditions { get; set; }
        public string Acceptance { get; set; }
        public string DispatchAddress { get; set; }
        public string DispatchCityStateZip { get; set; }
        public string DispatchPhone { get; set; }
        public string DispatchFax { get; set; }
        public bool PriceInclude { get; set; }
        public int PriceSequence { get; set; }
        public bool QuantityInclude { get; set; }
        public int QuantitySequence { get; set; }
        public bool MixIdInclude { get; set; }
        public int MixIdSequence { get; set; }
        public bool DescriptionInclude { get; set; }
        public int DescriptionSequence { get; set; }
        public bool PsiInclude { get; set; }
        public int PsiSequence { get; set; }
        public bool PublicCommentsInclude { get; set; }
        public int PublicCommentsSequence { get; set; }
        public bool SlumpInclude { get; set; }
        public int SlumpSequence { get; set; }
        public bool AirInclude { get; set; }
        public int AirSequence { get; set; }
        public bool AshInclude { get; set; }
        public int AshSequence { get; set; }
        public bool FineAggInclude { get; set; }
        public int FineAggSequence { get; set; }
        public bool SacksInclude { get; set; }
        public int SacksSequence { get; set; }
        public bool MD1Include { get; set; }
        public int MD1Sequence { get; set; }
        public bool MD2Include { get; set; }
        public int MD2Sequence { get; set; }
        public bool MD3Include { get; set; }
        public int MD3Sequence { get; set; }
        public bool MD4Include { get; set; }
        public int MD4Sequence { get; set; }
        public string FileKey { get; set; }
        public string FileName { get; set; }
        public bool IncludeByDefault { get; set; }
        public bool CustomerNumberOnPDF { get; set; }

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
        [Display(Name = "Region")]
        public int RegionId { get; set; }

        public SelectList AvailableRegions { get; set; }
        public double? AdjustMixPrice { get; set; }
        public WorkDayCalendar Calendar { get; set; }
        public string PricingAvailabilityDisclaimer { get; set; }
        public string ExpirationDateLabel { get; set; }

        public DistrictView(DAL.District r)
        {
            this.DistrictId = r.DistrictId;
            this.Name = r.Name;
            this.CompanyId = r.CompanyId;
            this.RegionId = r.RegionId;
            this.Active = r.Active.GetValueOrDefault(false);
            this.QCRequirement = r.QCRequirement.GetValueOrDefault(false);
            this.LettingDate = r.LettingDate.GetValueOrDefault(false);

            this.MapCenterLat = r.MapCenterLat;
            this.MapCenterLong = r.MapCenterLong;
            this.Zoom = r.MapZoomLevel.GetValueOrDefault(4);
            this.MapScaleRadius10 = r.MapScaleRadius10.GetValueOrDefault();
            this.MapScaleRadius100 = r.MapScaleRadius100.GetValueOrDefault();

            this.Load = r.Load.GetValueOrDefault();
            this.ToJob = r.ToJob.GetValueOrDefault();
            this.Unload = r.Unload.GetValueOrDefault();
            this.Wash = r.Wash.GetValueOrDefault();
            this.Return = r.Return.GetValueOrDefault();

            this.QuotationApprovalRequestText = r.QuoteApprovalRequestText;
            this.ProjectEntryFormNotification = r.ProjectEntryFormNotification;
            this.EmailQuoteToCustomer = r.EmailQuoteToCustomer;

            this.QuoteExpiration = r.QuoteExpiration.GetValueOrDefault();
            this.AcceptanceExpiration = r.AcceptanceExpiration.GetValueOrDefault();

            this.Disclaimers = r.Disclaimers;
            this.Disclosures = r.Disclosures;
            this.TermsAndConditions = r.TermsAndConditions;
            this.Acceptance = r.Acceptance;
            this.DispatchAddress = r.DispatchAddress;
            this.DispatchCityStateZip = r.DispatchCityStateZip;
            this.DispatchPhone = r.DispatchPhone;
            this.DispatchFax = r.DispatchFax;

            this.PriceInclude = r.PriceInclude.GetValueOrDefault();
            this.PriceSequence = r.PriceSequence.GetValueOrDefault();
            this.QuantityInclude = r.QuantityInclude.GetValueOrDefault();
            this.QuantitySequence = r.QuantitySequence.GetValueOrDefault();
            this.MixIdInclude = r.MixIdInclude.GetValueOrDefault();
            this.MixIdSequence = r.MixIdSequence.GetValueOrDefault();
            this.DescriptionInclude = r.DescriptionInclude.GetValueOrDefault();
            this.DescriptionSequence = r.DescriptionSequence.GetValueOrDefault();
            this.PsiInclude = r.PsiInclude.GetValueOrDefault();
            this.PsiSequence = r.PsiSequence.GetValueOrDefault();
            this.PublicCommentsInclude = r.PublicCommentsInclude.GetValueOrDefault();
            this.PublicCommentsSequence = r.PublicCommentsSequence.GetValueOrDefault();
            this.SlumpInclude = r.SlumpInclude.GetValueOrDefault();
            this.SlumpSequence = r.SlumpSequence.GetValueOrDefault();
            this.AirInclude = r.AirInclude.GetValueOrDefault();
            this.AirSequence = r.AirSequence.GetValueOrDefault();
            //------------
            this.AshInclude = r.AshInclude.GetValueOrDefault();
            this.AshSequence = r.AshSequence.GetValueOrDefault();
            this.FineAggInclude = r.FineAggInclude.GetValueOrDefault();
            this.FineAggSequence = r.FineAggSequence.GetValueOrDefault();
            this.SacksInclude = r.SacksInclude.GetValueOrDefault();
            this.SacksSequence = r.SacksSequence.GetValueOrDefault();
            this.MD1Include = r.MD1Include.GetValueOrDefault();
            this.MD1Sequence = r.MD1Sequence.GetValueOrDefault();
            this.MD2Include = r.MD2Include.GetValueOrDefault();
            this.MD2Sequence = r.MD2Sequence.GetValueOrDefault();
            this.MD3Include = r.MD3Include.GetValueOrDefault();
            this.MD3Sequence = r.MD3Sequence.GetValueOrDefault();
            this.MD4Include = r.MD4Include.GetValueOrDefault();
            this.MD4Sequence = r.MD4Sequence.GetValueOrDefault();
            this.FileName = r.FileName;
            this.FileKey = r.FileKey;
            this.IncludeByDefault = r.IncludeByDefault.GetValueOrDefault(false);
            this.AdjustMixPrice = r.AdjustMixPrice.GetValueOrDefault();
            this.CustomerNumberOnPDF = r.CustomerNumberOnPDF.GetValueOrDefault(false);
            this.ExpirationDateLabel = string.IsNullOrEmpty(r.ExpirationDateLabel)? "All quoted prices expire on": r.ExpirationDateLabel;
            this.PricingAvailabilityDisclaimer = string.IsNullOrEmpty(r.PricingAvailabilityDisclaimer) ? "Price quoted are applicable pending availability of materials and are subject to change prior to commitment." : r.PricingAvailabilityDisclaimer;
            BindValues(r.DistrictId);
        }

        public DistrictView()
        {
            this.ExpirationDateLabel = "All quoted prices expire on";
            this.PricingAvailabilityDisclaimer = "Price quoted are applicable pending availability of materials and are subject to change prior to commitment.";

        }

        public District ToEntity()
        {
            District entity = new District();
            entity.DistrictId = this.DistrictId;
            entity.Name = this.Name;
            entity.CompanyId = this.CompanyId;
            entity.RegionId = this.RegionId;
            entity.Active = this.Active;
            entity.QCRequirement = this.QCRequirement;
            entity.LettingDate = this.LettingDate;
            entity.MapCenterLat = this.MapCenterLat;
            entity.MapCenterLong = this.MapCenterLong;
            entity.MapZoomLevel = this.Zoom;
            entity.MapScaleRadius10 = this.MapScaleRadius10;
            entity.MapScaleRadius100 = this.MapScaleRadius100;

            entity.Load = this.Load;
            entity.ToJob = this.ToJob;
            entity.Unload = this.Unload;
            entity.Wash = this.Wash;
            entity.Return = this.Return;

            entity.QuoteExpiration = this.QuoteExpiration;
            entity.AcceptanceExpiration = this.AcceptanceExpiration;

            entity.QuoteApprovalRequestText = this.QuotationApprovalRequestText;
            entity.ProjectEntryFormNotification = this.ProjectEntryFormNotification;
            entity.EmailQuoteToCustomer = this.EmailQuoteToCustomer;
            entity.Disclaimers = this.Disclaimers;
            entity.Disclosures = this.Disclosures;
            entity.TermsAndConditions = this.TermsAndConditions;
            entity.Acceptance = this.Acceptance;
            entity.DispatchAddress = this.DispatchAddress;
            entity.DispatchCityStateZip = this.DispatchCityStateZip;
            entity.DispatchPhone = this.DispatchPhone;
            entity.DispatchFax = this.DispatchFax;

            entity.PriceInclude = this.PriceInclude;
            entity.PriceSequence = this.PriceSequence;
            entity.QuantityInclude = this.QuantityInclude;
            entity.QuantitySequence = this.QuantitySequence;
            entity.MixIdInclude = this.MixIdInclude;
            entity.MixIdSequence = this.MixIdSequence;
            entity.DescriptionInclude = this.DescriptionInclude;
            entity.DescriptionSequence = this.DescriptionSequence;
            entity.PsiInclude = this.PsiInclude;
            entity.PsiSequence = this.PsiSequence;
            entity.PublicCommentsInclude = this.PublicCommentsInclude;
            entity.PublicCommentsSequence = this.PublicCommentsSequence;
            entity.SlumpInclude = this.SlumpInclude;
            entity.SlumpSequence = this.SlumpSequence;
            entity.AirInclude = this.AirInclude;
            entity.AirSequence = this.AirSequence;

            entity.AshInclude = this.AshInclude;
            entity.AshSequence = this.AshSequence;
            entity.FineAggInclude = this.FineAggInclude;
            entity.FineAggSequence = this.FineAggSequence;
            entity.SacksInclude = this.SacksInclude;
            entity.SacksSequence = this.SacksSequence;
            entity.MD1Include = this.MD1Include;
            entity.MD1Sequence = this.MD1Sequence;
            entity.MD2Include = this.MD2Include;
            entity.MD2Sequence = this.MD2Sequence;
            entity.MD3Include = this.MD3Include;
            entity.MD3Sequence = this.MD3Sequence;
            entity.MD4Include = this.MD4Include;
            entity.MD4Sequence = this.MD4Sequence;
            entity.FileName = this.FileName;
            entity.FileKey = this.FileKey;
            entity.IncludeByDefault = this.IncludeByDefault;
            entity.AdjustMixPrice = this.AdjustMixPrice;
            entity.CustomerNumberOnPDF = this.CustomerNumberOnPDF;
            entity.ExpirationDateLabel = string.IsNullOrEmpty(this.ExpirationDateLabel) ? "All quoted prices expire on" : this.ExpirationDateLabel;
            entity.PricingAvailabilityDisclaimer = string.IsNullOrEmpty(this.PricingAvailabilityDisclaimer) ? "Price quoted are applicable pending availability of materials and are subject to change prior to commitment." : this.PricingAvailabilityDisclaimer;

            return entity;
        }



        public void BindValues(int? districtId = null)
        {
            this.Calendar = new WorkDayCalendar(districtId.GetValueOrDefault());
            this.CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
            AvailableRegions = new SelectList(SIDAL.GetRegions(CompanyId, null, 0, 1000, false), "RegionId", "Name", RegionId);
            Calendar.Distribution = SIDAL.FindOrCreateWeeklyDistribution(districtId.GetValueOrDefault());
        }
    }
}
