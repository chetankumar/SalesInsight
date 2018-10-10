using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationDetailsView
    {
        public Guid UserId { get; set; }
        public long QuotationId { get; set; }
        public int PlantId { get; set; }

        public bool Active { get; set; }

        public string QuoteDateString { get; set; }
        public string AcceptanceExpirationString { get; set; }
        public string QuoteExpirationString { get; set; }
        public string PriceChangeDate1String { get; set; }
        public string PriceChangeDate2String { get; set; }
        public string PriceChangeDate3String { get; set; }

        public decimal PriceIncrease1 { get; set; }
        public decimal PriceIncrease2 { get; set; }
        public decimal PriceIncrease3 { get; set; }

        public string PrivateNotes { get; set; }
        public string PublicNotes { get; set; }
        public string Disclaimers { get; set; }
        public string Disclosures { get; set; }
        public string TermsAndConditions { get; set; }

        public long? FskPriceId { get; set; }
        public decimal FskBasePrice { get; set; }
        public bool ConcreteEnabled { get; set; }
        public bool AggregateEnabled { get; set; }
        public bool BlockEnabled { get; set; }
        public int AggregatePlantId { get; set; }
        public int BlockPlantId { get; set; }
        public double AdjustMixPrice { get; set; }
        public DateTime? BiddingDate { get; set; }
        public bool IncludeAsLettingDate { get; set; }
        public bool CustomerNumberOnPDF { get; set; }

        public QuotationDetailsView()
        {

        }

        public QuotationDetailsView(long id)
        {
            this.QuotationId = id;
        }

        public void Load(Quotation quotation = null)
        {
            if (quotation == null)
            {
                quotation = SIDAL.FindQuotation(QuotationId);
            }

            Active = quotation.Active.GetValueOrDefault(true);

            if (quotation.QuoteDate == null)
                QuoteDate = quotation.CreatedOn;
            else
                QuoteDate = quotation.QuoteDate;

            this.PlantId = quotation.PlantId.GetValueOrDefault();
            this.AggregatePlantId = quotation.AggregatePlantId.GetValueOrDefault();
            this.BlockPlantId = quotation.BlockPlantId.GetValueOrDefault();
            District d = null;
            if (PlantId > 0)
            {
                d = SIDAL.GetDistrict(SIDAL.GetPlant(PlantId).DistrictId);
                if (quotation.AcceptanceExpirationDate == null)
                {
                    AcceptanceExpirationDate = QuoteDate.Value.AddDays(d.AcceptanceExpiration.GetValueOrDefault(30));
                }
                else
                {
                    AcceptanceExpirationDate = quotation.AcceptanceExpirationDate;
                }
                if (quotation.QuoteExpirationDate == null)
                {
                    QuoteExpiration = QuoteDate.Value.AddDays(d.QuoteExpiration.GetValueOrDefault(60));
                }
                else
                {
                    QuoteExpiration = quotation.QuoteExpirationDate;
                }
            }

            PriceChangeDate1 = quotation.PriceIncrease1;
            PriceChangeDate2 = quotation.PriceIncrease2;
            PriceChangeDate3 = quotation.PriceIncrease3;

            PriceIncrease1 = quotation.PriceIncreaseAmount1.GetValueOrDefault(0);
            PriceIncrease2 = quotation.PriceIncreaseAmount2.GetValueOrDefault(0);
            PriceIncrease3 = quotation.PriceIncreaseAmount3.GetValueOrDefault(0);

            PrivateNotes = quotation.PrivateNotes;
            PublicNotes = quotation.PublicNotes;
            this.Disclaimers = quotation.Disclaimers;
            this.Disclosures = quotation.Disclosures;
            this.TermsAndConditions = quotation.TermsAndConditions;

            if (string.IsNullOrEmpty(this.Disclaimers))
                this.Disclaimers = d.Disclaimers;

            if (string.IsNullOrEmpty(this.Disclosures))
                this.Disclosures = d.Disclosures;

            if (string.IsNullOrEmpty(this.TermsAndConditions))
                this.TermsAndConditions = d.TermsAndConditions;
            this.AdjustMixPrice = quotation.AdjustMixPrice.GetValueOrDefault();
            if (this.AdjustMixPrice == 0 )
            {
                this.AdjustMixPrice = d.AdjustMixPrice.GetValueOrDefault();
            }
            this.IncludeAsLettingDate = quotation.IncludeAsLettingDate.GetValueOrDefault(false);
            this.CustomerNumberOnPDF = quotation.CustomerNumberOnPDF.GetValueOrDefault(false);

            if (quotation.CustomerNumberOnPDF == null)
            {
                this.CustomerNumberOnPDF = d.CustomerNumberOnPDF.GetValueOrDefault();
            }
            this.FskPriceId = quotation.FskPriceId;
            this.FskBasePrice = quotation.FskBasePrice.GetValueOrDefault(0);
            this.ConcreteEnabled = quotation.ConcreteEnabled.GetValueOrDefault(false);
            this.AggregateEnabled = quotation.AggregateEnabled.GetValueOrDefault(false);
            this.BlockEnabled = quotation.BlockEnabled.GetValueOrDefault(false);
            this.BiddingDate = quotation.BiddingDate;
        }

        public SelectList ChoosePlants
        {
            get
            {
                return new SelectList(SIDAL.GetPlants(UserId), "PlantId", "Name", PlantId);
            }
        }

        public DateTime? QuoteDate
        {
            get
            {
                if (QuoteDateString != null && !QuoteDateString.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact(QuoteDateString, "M/d/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    QuoteDateString = value.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTime? AcceptanceExpirationDate
        {
            get
            {
                if (AcceptanceExpirationString != null && !AcceptanceExpirationString.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact(AcceptanceExpirationString, "M/d/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    AcceptanceExpirationString = value.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            }
        }

  
        public DateTime? QuoteExpiration
        {
            get
            {
                if (QuoteExpirationString != null && !QuoteExpirationString.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact(QuoteExpirationString, "M/d/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    QuoteExpirationString = value.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTime? PriceChangeDate1
        {
            get
            {
                if (PriceChangeDate1String != null && !PriceChangeDate1String.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact(PriceChangeDate1String, "M/d/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    PriceChangeDate1String = value.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTime? PriceChangeDate2
        {
            get
            {
                if (PriceChangeDate2String != null && !PriceChangeDate2String.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact(PriceChangeDate2String, "M/d/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    PriceChangeDate2String = value.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTime? PriceChangeDate3
        {
            get
            {
                if (PriceChangeDate3String != null && !PriceChangeDate3String.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact(PriceChangeDate3String, "M/d/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    PriceChangeDate3String = value.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            }
        }

    }
}