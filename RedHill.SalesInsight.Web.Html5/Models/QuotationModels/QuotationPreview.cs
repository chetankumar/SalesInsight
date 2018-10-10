using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using RedHill.SalesInsight.Web.Html5.Models.QuotationModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Data;
using static iTextSharp.text.Font;
using System.Text.RegularExpressions;
using iTextSharp.tool.xml;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationPreview
    {
        
        public long Id { get; set; }
        public long QuoteFormSettingId { get; set; }
        public QuotationProfile Profile { get; set; }
        public string SelectedAction { get; set; }

        public string DispatchPhone { get; set; }
        public string DispatchFax { get; set; }
        public string DispatchAddress { get; set; }
        public string DispatchCityState { get; set; }
        public string SupplementFileName { get; set; }
        public string SupplementFilePath { get; set; }
        public bool IncludeQuoteSupplement { get; set; }
        public string Customer { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public string Project { get; set; }
        public string Address { get; set; }
        public string CityStateZip { get; set; }
        public string QuoteId { get; set; }
        public string TotalYards { get; set; }

        public string DisclaimerText { get; set; }

        public List<QuotationMix> MixDescriptions { get; set; }
        public List<String[]> TableData { get; set; }
        public String[] Headers { get; set; }
        public float[] Widths { get; set; }
        public List<int> IncludedColumns { get; set; }

        public string PublicComments { get; set; }
        public string PrivateComments { get; set; }

        public List<Addon> QuoteAddons { get; set; }

        public List<QuotationAddonModel> QuoteAddonModel { get; set; }

        public decimal PriceIncrease1 { get; set; }
        public decimal PriceIncrease2 { get; set; }
        public decimal PriceIncrease3 { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PriceIncreaseDate1 { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PriceIncreaseDate2 { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PriceIncreaseDate3 { get; set; }

        public string Disclosure { get; set; }
        public string TermsAndConditions { get; set; }
        public string AcceptanceText { get; set; }

        public bool CustomerPurchaseConcrete { get; set; }
        public bool CustomerPurchaseAggregate { get; set; }
        public bool CustomerPurchaseBlock { get; set; }
        public double AggregateQty { get; set; }
        public double BlockQty { get; set; }
        //public bool QuotationConcreteEnabled { get; set; }
        //public bool QuotationAggregateEnabled { get; set; }
        //public bool QuotationBlockEnabled { get; set; }

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
        public bool ShowCustomerNumberOnPDF { get; set; }
        public List<AddonsView> AllAddonlist { get; set; }
        public string PricingAvailabilityDisclaimer { get; set; }
        public string ExpirationDateLabel { get; set; }

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

        public DateTime? QuoteDate { get; set; }
        public DateTime? AcceptanceDate { get; set; }
        public DateTime? QuoteExpiry { get; set; }
        public DateTime? LettingDate { get; set; }

        public string SalesStaff { get; set; }
        public string UserPhone { get; set; }
        public string UserFax { get; set; }
        public string UserEmail { get; set; }
        public DateTime? ProjectFormNotificationDate { get; set; }
        public DateTime? PDFGeneratedDate { get; set; }

        public List<HttpPostedFileBase> Files { get; set; }

        public List<QuotationAggregateModel> AggregateProducts { get; set; }
        public List<QuotationAggregateAddonModel> AggregateAddons { get; set; }
        public List<QuotationBlockModel> BlockProducts { get; set; }
        public List<QuotationBlockAddonModel> BlockAddons { get; set; }

        public decimal AddonPrice(long addonId)
        {
            return SIDAL.FindCurrentAddonQuoteCost(addonId, SIDAL.GetPlant(SIDAL.FindQuotation(Id).PlantId).DistrictId);
        }

        public QuotationPreview()
        {
            Files = new List<HttpPostedFileBase>();
        }

        public QuotationPreview(long id)
        {
            this.Id = id;
        }

        public void Load(bool fromDistrict = true)
        {
            Quotation q = SIDAL.FindQuotation(this.Id);
            Profile = new QuotationProfile(q);

            //this.QuotationAggregateEnabled = Profile.ConcreteEnabled;
            //this.QuotationBlockEnabled = Profile.ConcreteEnabled;
            //this.QuotationConcreteEnabled = Profile.ConcreteEnabled;

            this.QuoteId = (q.Id).ToString();

            this.PublicComments = q.PublicNotes;
            this.PrivateComments = q.PrivateNotes;
            this.QuoteDate = q.QuoteDate;
            this.AcceptanceDate = q.AcceptanceExpirationDate;
            this.QuoteExpiry = q.QuoteExpirationDate;
            if (q.IncludeAsLettingDate.GetValueOrDefault(false))
            {
                this.LettingDate = q.BiddingDate;
            }
            this.ProjectFormNotificationDate = q.ProjectFormNotificationDate;
            this.PDFGeneratedDate = q.PDFGeneratedDate;

            this.PriceIncrease1 = q.PriceIncreaseAmount1.GetValueOrDefault(0);
            this.PriceIncrease2 = q.PriceIncreaseAmount2.GetValueOrDefault(0);
            this.PriceIncrease3 = q.PriceIncreaseAmount3.GetValueOrDefault(0);

            this.PriceIncreaseDate1 = q.PriceIncrease1;
            this.PriceIncreaseDate2 = q.PriceIncrease2;
            this.PriceIncreaseDate3 = q.PriceIncrease3;

            if (q.CustomerId != null)
            {
                SICustomer c = SIDAL.GetCustomer(q.CustomerId.Value);
                this.CustomerPurchaseAggregate = c.Customer.PurchaseAggregate.GetValueOrDefault();
                this.CustomerPurchaseConcrete = c.Customer.PurchaseConcrete.GetValueOrDefault();
                this.CustomerPurchaseBlock = c.Customer.PurchaseBlock.GetValueOrDefault();

                this.Customer = c.Customer.Name;
                CustomerContact cc = SIDAL.GetQuotationContact(q.Id);
                if (cc != null)
                {
                    this.Contact = cc.Name;
                    this.Phone = cc.Phone;
                    this.Fax = cc.Fax;
                    this.Email = cc.Email;
                }
            }

            this.TotalYards = q.TotalVolume.GetValueOrDefault(0).ToString("N0");

            if (q.ProjectId != null)
            {
                Project p = SIDAL.GetProject(q.ProjectId.Value);
                this.Project = p.Name;
                this.Address = p.Address;
                this.CityStateZip = p.City + ", " + p.State + ", " + p.ZipCode;

                if (q.SalesStaffId > 0)
                {
                    SISalesStaff ss = SIDAL.GetSalesStaff(q.SalesStaffId.Value);
                    this.SalesStaff = ss.SalesStaff.Name;
                    this.UserEmail = ss.SalesStaff.Email;
                    this.UserFax = ss.SalesStaff.Fax;
                    this.UserPhone = ss.SalesStaff.Phone;
                }
            }

            if (q.PlantId != null)
            {
                Plant p = SIDAL.GetPlant(q.PlantId.Value);
                District d = SIDAL.GetDistrict(p.DistrictId);

                if (q.CustomerNumberOnPDF== null)
                {
                    this.ShowCustomerNumberOnPDF = d.CustomerNumberOnPDF.GetValueOrDefault();
                }
                else
                {
                    this.ShowCustomerNumberOnPDF = q.CustomerNumberOnPDF.GetValueOrDefault(false);
                }

                this.ExpirationDateLabel = string.IsNullOrEmpty(d.ExpirationDateLabel) ? "All quoted prices expire on" : d.ExpirationDateLabel;
                this.PricingAvailabilityDisclaimer = string.IsNullOrEmpty(d.PricingAvailabilityDisclaimer) ? "Price quoted are applicable pending availability of materials and are subject to change prior to commitment." : d.PricingAvailabilityDisclaimer;
                this.AcceptanceText = d.Acceptance;
                this.DisclaimerText = q.Disclaimers;
                this.Disclosure = q.Disclosures;
                this.TermsAndConditions = q.TermsAndConditions;

                if (string.IsNullOrEmpty(q.Disclaimers))
                    this.DisclaimerText = d.Disclaimers;

                if (string.IsNullOrEmpty(q.Disclosures))
                    this.Disclosure = d.Disclosures;

                if (string.IsNullOrEmpty(q.TermsAndConditions))
                    this.TermsAndConditions = d.TermsAndConditions;

                this.DispatchAddress = d.DispatchAddress;
                this.DispatchCityState = d.DispatchCityStateZip;
                this.DispatchPhone = d.DispatchPhone;

                if (!string.IsNullOrEmpty(p.Phone))
                    this.DispatchPhone = p.Phone;

                if (!string.IsNullOrEmpty(p.Address) && !string.IsNullOrEmpty(p.CityStateZip))
                {
                    this.DispatchAddress = p.Address;
                    this.DispatchCityState = p.CityStateZip;
                }

                this.DispatchFax = d.DispatchFax;
                this.SupplementFileName = d.FileName;
                this.SupplementFilePath = d.FileKey;
                this.IncludeQuoteSupplement = d.IncludeByDefault.GetValueOrDefault(false);
                if (fromDistrict)
                {
                    QuotationFormSetting qfs = SIDAL.GetQuotationFormSetting(Convert.ToInt64(this.QuoteId));

                    if (qfs != null)
                    {
                        this.QuoteFormSettingId = qfs.Id;
                        this.PriceInclude = qfs.PriceInclude.GetValueOrDefault();
                        this.PriceSequence = qfs.PriceSequence.GetValueOrDefault();
                        this.QuantityInclude = qfs.QuantityInclude.GetValueOrDefault();
                        this.QuantitySequence = qfs.QuantitySequence.GetValueOrDefault();
                        this.MixIdInclude = qfs.MixIdInclude.GetValueOrDefault();
                        this.MixIdSequence = qfs.MixIdSequence.GetValueOrDefault();
                        this.DescriptionInclude = qfs.DescriptionInclude.GetValueOrDefault();
                        this.DescriptionSequence = qfs.DescriptionSequence.GetValueOrDefault();
                        this.PsiInclude = qfs.PsiInclude.GetValueOrDefault();
                        this.PsiSequence = qfs.PsiSequence.GetValueOrDefault();
                        this.PublicCommentsInclude = qfs.PublicCommentsInclude.GetValueOrDefault();
                        this.PublicCommentsSequence = qfs.PublicCommentsSequence.GetValueOrDefault();
                        this.SlumpInclude = qfs.SlumpInclude.GetValueOrDefault();
                        this.SlumpSequence = qfs.SlumpSequence.GetValueOrDefault();
                        this.AirInclude = qfs.AirInclude.GetValueOrDefault();
                        this.AirSequence = qfs.AirSequence.GetValueOrDefault();

                        this.AshInclude = qfs.AshInclude.GetValueOrDefault();
                        this.AshSequence = qfs.AshSequence.GetValueOrDefault();
                        this.FineAggInclude = qfs.FineAggInclude.GetValueOrDefault();
                        this.FineAggSequence = qfs.FineAggSequence.GetValueOrDefault();
                        this.SacksInclude = qfs.SacksInclude.GetValueOrDefault();
                        this.SacksSequence = qfs.SacksSequence.GetValueOrDefault();
                        this.MD1Include = qfs.MD1Include.GetValueOrDefault();
                        this.MD1Sequence = qfs.MD1Sequence.GetValueOrDefault();
                        this.MD2Include = qfs.MD2Include.GetValueOrDefault();
                        this.MD2Sequence = qfs.MD2Sequence.GetValueOrDefault();
                        this.MD3Include = qfs.MD3Include.GetValueOrDefault();
                        this.MD3Sequence = qfs.MD3Sequence.GetValueOrDefault();
                        this.MD4Include = qfs.MD4Include.GetValueOrDefault();
                        this.MD4Sequence = qfs.MD4Sequence.GetValueOrDefault();
                    }
                    else
                    {
                        this.PriceInclude = d.PriceInclude.GetValueOrDefault();
                        this.PriceSequence = d.PriceSequence.GetValueOrDefault();
                        this.QuantityInclude = d.QuantityInclude.GetValueOrDefault();
                        this.QuantitySequence = d.QuantitySequence.GetValueOrDefault();
                        this.MixIdInclude = d.MixIdInclude.GetValueOrDefault();
                        this.MixIdSequence = d.MixIdSequence.GetValueOrDefault();
                        this.DescriptionInclude = d.DescriptionInclude.GetValueOrDefault();
                        this.DescriptionSequence = d.DescriptionSequence.GetValueOrDefault();
                        this.PsiInclude = d.PsiInclude.GetValueOrDefault();
                        this.PsiSequence = d.PsiSequence.GetValueOrDefault();
                        this.PublicCommentsInclude = d.PublicCommentsInclude.GetValueOrDefault();
                        this.PublicCommentsSequence = d.PublicCommentsSequence.GetValueOrDefault();
                        this.SlumpInclude = d.SlumpInclude.GetValueOrDefault();
                        this.SlumpSequence = d.SlumpSequence.GetValueOrDefault();
                        this.AirInclude = d.AirInclude.GetValueOrDefault();
                        this.AirSequence = d.AirSequence.GetValueOrDefault();

                        this.AshInclude = d.AshInclude.GetValueOrDefault();
                        this.AshSequence = d.AshSequence.GetValueOrDefault();
                        this.FineAggInclude = d.FineAggInclude.GetValueOrDefault();
                        this.FineAggSequence = d.FineAggSequence.GetValueOrDefault();
                        this.SacksInclude = d.SacksInclude.GetValueOrDefault();
                        this.SacksSequence = d.SacksSequence.GetValueOrDefault();
                        this.MD1Include = d.MD1Include.GetValueOrDefault();
                        this.MD1Sequence = d.MD1Sequence.GetValueOrDefault();
                        this.MD2Include = d.MD2Include.GetValueOrDefault();
                        this.MD2Sequence = d.MD2Sequence.GetValueOrDefault();
                        this.MD3Include = d.MD3Include.GetValueOrDefault();
                        this.MD3Sequence = d.MD3Sequence.GetValueOrDefault();
                        this.MD4Include = d.MD4Include.GetValueOrDefault();
                        this.MD4Sequence = d.MD4Sequence.GetValueOrDefault();
                    }
                }
            }

            TableData = new List<string[]>();
            if (this.Profile.ConcreteEnabled)
            {
                Headers = new String[15];
                Widths = new float[15];
                float smallWidth = 8;
                float largeWidth = 20;
                IncludedColumns = new List<int>();
                if (PriceInclude)
                {
                    Headers[PriceSequence] = "Price";
                    IncludedColumns.Add(PriceSequence);
                    Widths[PriceSequence] = smallWidth;
                }
                if (QuantityInclude)
                {
                    Headers[QuantitySequence] = "Qty";
                    IncludedColumns.Add(QuantitySequence);
                    Widths[QuantitySequence] = smallWidth;
                }
                if (MixIdInclude)
                {
                    Headers[MixIdSequence] = "Mix Id";
                    IncludedColumns.Add(MixIdSequence);
                    Widths[MixIdSequence] = smallWidth;
                }
                if (DescriptionInclude)
                {
                    Headers[DescriptionSequence] = "Description";
                    IncludedColumns.Add(DescriptionSequence);
                    Widths[DescriptionSequence] = largeWidth;
                }
                if (PsiInclude)
                {
                    Headers[PsiSequence] = "PSI";
                    IncludedColumns.Add(PsiSequence);
                    Widths[PsiSequence] = smallWidth;
                }
                if (AirInclude)
                {
                    Headers[AirSequence] = "Air";
                    IncludedColumns.Add(AirSequence);
                    Widths[AirSequence] = smallWidth;
                }
                if (AshInclude)
                {
                    Headers[AshSequence] = "Ash %";
                    IncludedColumns.Add(AshSequence);
                    Widths[AshSequence] = smallWidth;
                }
                if (FineAggInclude)
                {
                    Headers[FineAggSequence] = "Fine Agg %";
                    IncludedColumns.Add(FineAggSequence);
                    Widths[FineAggSequence] = smallWidth;
                }
                if (SacksInclude)
                {
                    Headers[SacksSequence] = "Sacks";
                    IncludedColumns.Add(SacksSequence);
                    Widths[SacksSequence] = smallWidth;
                }
                if (MD1Include)
                {
                    Headers[MD1Sequence] = this.MD1String;
                    IncludedColumns.Add(MD1Sequence);
                    Widths[MD1Sequence] = smallWidth;
                }
                if (MD2Include)
                {
                    Headers[MD2Sequence] = this.MD2String;
                    IncludedColumns.Add(MD2Sequence);
                    Widths[MD2Sequence] = smallWidth;
                }
                if (MD3Include)
                {
                    Headers[MD3Sequence] = this.MD3String;
                    IncludedColumns.Add(MD3Sequence);
                    Widths[MD3Sequence] = smallWidth;
                }
                if (MD4Include)
                {
                    Headers[MD4Sequence] = this.MD4String;
                    IncludedColumns.Add(MD4Sequence);
                    Widths[MD4Sequence] = smallWidth;
                }
                if (SlumpInclude)
                {
                    Headers[SlumpSequence] = "Slump";
                    IncludedColumns.Add(SlumpSequence);
                    Widths[SlumpSequence] = smallWidth;
                }
                if (PublicCommentsInclude)
                {
                    Headers[PublicCommentsSequence] = "Comments";
                    IncludedColumns.Add(PublicCommentsSequence);
                    Widths[PublicCommentsSequence] = largeWidth;
                }

                this.MixDescriptions = SIDAL.GetQuotationMixes(q.Id);

                foreach (QuotationMix entity in MixDescriptions.OrderBy(x => x.Position))
                {
                    QuotationMixPreviewTable data = new QuotationMixPreviewTable(entity, q.PlantId);
                    String[] row = new String[15];
                    if (PriceInclude)
                    {
                        row[PriceSequence] = "$" + data.Price.ToString("N2");
                    }
                    if (QuantityInclude)
                    {
                        row[QuantitySequence] = data.Quantity.ToString();
                    }
                    if (MixIdInclude)
                    {
                        row[MixIdSequence] = data.MixId;
                    }
                    if (DescriptionInclude)
                    {
                        row[DescriptionSequence] = data.MixDescription;
                    }
                    if (PsiInclude)
                    {
                        row[PsiSequence] = data.PSI;
                    }
                    if (AirInclude)
                    {
                        row[AirSequence] = data.Air;
                    }
                    if (AshInclude)
                    {
                        row[AshSequence] = data.Ash;
                    }
                    if (FineAggInclude)
                    {
                        row[FineAggSequence] = data.FineAgg;
                    }
                    if (SacksInclude)
                    {
                        row[SacksSequence] = data.Sacks;
                    }
                    if (MD1Include)
                    {
                        row[MD1Sequence] = data.MD1;
                    }
                    if (MD2Include)
                    {
                        row[MD2Sequence] = data.MD2;
                    }
                    if (MD3Include)
                    {
                        row[MD3Sequence] = data.MD3;
                    }
                    if (MD4Include)
                    {
                        row[MD4Sequence] = data.MD4;
                    }
                    if (SlumpInclude)
                    {
                        row[SlumpSequence] = data.Slump;
                    }
                    if (PublicCommentsInclude)
                    {
                        row[PublicCommentsSequence] = data.Comments;
                    }
                    this.TableData.Add(row);
                }
            }

            this.MixDescriptions = SIDAL.GetQuotationMixes(q.Id);

            this.QuoteAddons = SIDAL.GetAddonsForQuote(q.Id);
            List<QuotationAddon> qaon = SIDAL.GetQuoatationAddonQuotationIdWise(q.Id, false);
            QuoteAddonModel = new List<QuotationAddonModel>();

            foreach (var quoteAddon in qaon)
            {
                this.QuoteAddonModel.Add(new QuotationAddonModel(quoteAddon));
            }

            if (this.QuoteAddonModel != null)
            {
                foreach (var qAddon in this.QuoteAddonModel.Where(x => x.IsIncludeTable == true))
                {
                    //Adding additional rows into the mixes table in quote priview and PDF
                    String[] row = new String[15];
                    if (PriceInclude)
                    {
                        row[PriceSequence] = "$" + qAddon.Price.GetValueOrDefault().ToString("N2");
                    }
                    if (QuantityInclude)
                    {
                        row[QuantitySequence] = "";
                    }
                    if (MixIdInclude)
                    {
                        row[MixIdSequence] = "";
                    }
                    if (DescriptionInclude)
                    {
                        row[DescriptionSequence] = qAddon.Description + " (Per " + qAddon.QuoteUomName + ")";
                    }
                    if (PsiInclude)
                    {
                        row[PsiSequence] = "";
                    }
                    if (AirInclude)
                    {
                        row[AirSequence] = "";
                    }
                    if (AshInclude)
                    {
                        row[AshSequence] = "";
                    }
                    if (FineAggInclude)
                    {
                        row[FineAggSequence] = "";
                    }
                    if (SacksInclude)
                    {
                        row[SacksSequence] = "";
                    }
                    if (MD1Include)
                    {
                        row[MD1Sequence] = "";
                    }
                    if (MD2Include)
                    {
                        row[MD2Sequence] = "";
                    }
                    if (MD3Include)
                    {
                        row[MD3Sequence] = "";
                    }
                    if (MD4Include)
                    {
                        row[MD4Sequence] = "";
                    }
                    if (SlumpInclude)
                    {
                        row[SlumpSequence] = "";
                    }
                    if (PublicCommentsInclude)
                    {
                        row[PublicCommentsSequence] = "";
                    }
                    this.TableData.Add(row);
                }
            }
            #region Agg and Block Products
            var company = SIDAL.GetCompany();

            if (this.Profile.AggregateEnabled)
            {
                this.AggregateProducts = new List<QuotationAggregateModel>();
                foreach (var aggProduct in SIDAL.GetQuotationAggregateProducts(q.Id))
                {
                    if (this.CustomerPurchaseAggregate && company.EnableAggregate.GetValueOrDefault())
                    {
                        AggregateQty = AggregateQty + aggProduct.Volume.GetValueOrDefault();
                    }
                    this.AggregateProducts.Add(new QuotationAggregateModel(aggProduct));
                }
                
                this.AggregateAddons = new List<QuotationAggregateAddonModel>();
                var aggAddonList = SIDAL.GetQuotationAggregateAddon(q.Id, false);
                foreach (var item in aggAddonList)
                {
                    if (this.CustomerPurchaseAggregate && company.EnableAggregate.GetValueOrDefault())
                    {                     
                        this.AggregateAddons.Add(new QuotationAggregateAddonModel(item));
                    }
                }
            }

            if (this.Profile.BlockEnabled)
            {
                this.BlockProducts = new List<QuotationBlockModel>();
                foreach (var blockProduct in SIDAL.GetQuotationBlockProducts(q.Id))
                {
                    if (this.CustomerPurchaseBlock && company.EnableBlock.GetValueOrDefault())
                    {
                        BlockQty = BlockQty + blockProduct.Volume.GetValueOrDefault();
                    }
                    this.BlockProducts.Add(new QuotationBlockModel(blockProduct));
                }

                this.BlockAddons = new List<QuotationBlockAddonModel>();
                foreach (var item in SIDAL.GetQuotationBlockAddon(q.Id,false))
                {
                    if (this.CustomerPurchaseBlock && company.EnableBlock.GetValueOrDefault())
                    {
                        this.BlockAddons.Add(new QuotationBlockAddonModel(item));
                    }
                }
            }
            List<string> totalQty = new List<string>();

            AllAddonlist = this.QuoteAddonModel.Select(x => new AddonsView
            {
                Description = x.Description,
                Price = x.Price,
                Per = x.Per,
                QuoteUomName = x.QuoteUomName,
                IsIncludeTable = x.IsIncludeTable,
                Sort = x.Sort
            }).ToList();

            if (this.AggregateAddons != null)
            {
                AllAddonlist = AllAddonlist.Union(this.AggregateAddons.Select(x => new AddonsView
                {
                    Description = x.Description,
                    Price = x.Price,
                    Per = x.Per,
                    QuoteUomName = x.QuoteUomName,
                    IsIncludeTable = x.IsIncludeTable,
                    Sort = x.Sort
                })).ToList();
            }

            if (this.BlockAddons != null)
            {
                AllAddonlist = AllAddonlist.Union(this.BlockAddons.Select(x => new AddonsView
                {
                    Description = x.Description,
                    Price = x.Price,
                    Per = x.Per,
                    QuoteUomName = x.QuoteUomName,
                    IsIncludeTable = x.IsIncludeTable,
                    Sort = x.Sort
                })).ToList();
            }

            AllAddonlist = AllAddonlist.OrderBy(x => x.Sort).ThenBy(x => x.Description).ToList();
 

            if (this.Profile.ConcreteEnabled && this.CustomerPurchaseConcrete)
            {
                totalQty.Add(this.TotalYards.ToString() + " " + company.DeliveryQtyUomSingular);
            }
            if (this.BlockQty != 0)
            {
                totalQty.Add(this.BlockQty.ToString("N0") + " Units");
            }
            if (this.AggregateQty != 0)
            {
                totalQty.Add(this.AggregateQty.ToString("N0") + " Tons");
            }
            this.TotalYards = string.Join(" , ", new List<string>(totalQty).ToArray());
            #endregion
        }

        public PdfPTable GetPdfTableFromDataTable(DataTable data, Font font)
        {
            PdfPTable table = new PdfPTable(data.Columns.Count)
            {
                WidthPercentage = 100
            };

            float[] widths = new float[] { 60f, 15f, 20f, 20f, 20f, 20f, 50f };

            table.SetWidths(widths);

            var baseFont = Font.FontFamily.HELVETICA;

            var cellFont = new Font(baseFont, 8, Font.NORMAL);

            Phrase phrase = null;
            PdfPCell cell = null;
            //Generate headers
            foreach (var col in data.Columns)
            {
                phrase = new Phrase((col as DataColumn).Caption, new Font(baseFont, 8, Font.BOLD));

                cell = new PdfPCell(phrase);
                table.AddCell(cell);
            }

            foreach (DataRow r in data.Rows)
            {
                if (data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        table.AddCell(new Phrase(r[i].ToString(), cellFont));
                    }
                }
            }

            return table;
        }

        public void GenerateQuotePDF(string path, string logoPath)
        {
            iTextSharp.text.Font.FontFamily baseFont = Font.FontFamily.HELVETICA;
            Document doc = new Document(new Rectangle(PageSize.A4.Width, PageSize.A4.Height));
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.SetMargins(30f, 30f, 10f, 10f);

            doc.Open();

            #region Header

            PdfPTable headerTable = new PdfPTable(3);
            headerTable.SetWidths(new float[] { 20, 50, 30 });
            headerTable.WidthPercentage = 100;

            //Logo
            PdfPCell logoCell = CreateWhiteCell();
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(logoPath);
            image.ScaleAbsolute(159f, 159f);
            logoCell.AddElement(image);
            logoCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            headerTable.AddCell(logoCell);

            //Heading here..
            PdfPCell headingCell = CreateWhiteCell();
            Paragraph headingContent = new Paragraph("PROJECT QUOTATION", new Font(baseFont, 16F));
            headingCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            headingContent.Alignment = Element.ALIGN_RIGHT;
            headingCell.AddElement(headingContent);
            headerTable.AddCell(headingCell);

            //Header Address Content..
            PdfPCell topAddressCell = CreateWhiteCell();

            string[] addresses = new string[] { "Customer Service: " + this.DispatchPhone, "Fax: " + this.DispatchFax, this.DispatchAddress, this.DispatchCityState };
            var addressList = addresses.ToList();
            if (this.DispatchFax == null || this.DispatchFax == "")
            {
                var fax = addressList[1];
                addressList.Remove(fax);
            }
            foreach (string address in addressList)
            {
                Paragraph headingContent1 = new Paragraph(address, new Font(baseFont, 8F));
                headingContent1.SetLeading(1.5f, 1.5f);
                headingContent1.Alignment = Element.ALIGN_RIGHT;
                topAddressCell.AddElement(headingContent1);
            }
            headerTable.AddCell(topAddressCell);

            doc.Add(headerTable);
            #endregion

            #region Customer Information
            PdfPTable bodyTable = new PdfPTable(4);
            bodyTable.WidthPercentage = 100;
            bodyTable.SetWidths(new float[] { 20, 30, 20, 30 });
            //(Model.PlantDispatchId != null && Model.PlantDispatchId.Trim().Length > 0)

            if (this.ShowCustomerNumberOnPDF)
            {
                this.Customer = this.Profile.CustomerName + " - (" + this.Profile.CustomerNumber + ")";
            }
            AddLabelTextRow(baseFont, "Customer", this.Customer, bodyTable, true);
            AddLabelTextRow(baseFont, "Project", this.Project, bodyTable, true);
            AddLabelTextRow(baseFont, "Contact", this.Contact, bodyTable);
            AddLabelTextRow(baseFont, "Address", this.Address, bodyTable);
            AddLabelTextRow(baseFont, "Phone", this.Phone, bodyTable);
            AddLabelTextRow(baseFont, "City,State,Zip", this.CityStateZip, bodyTable);
            AddLabelTextRow(baseFont, "Fax", this.Fax, bodyTable);
            AddLabelTextRow(baseFont, "Quote ID", this.QuoteId + ((this.Profile.PlantDispatchId != null && this.Profile.PlantDispatchId.Trim().Length > 0) ? "     (" + this.Profile.PlantDispatchId + ")" : ""), bodyTable);
            AddLabelTextRow(baseFont, "Email", this.Email, bodyTable);
            AddLabelTextRow(baseFont, "Total Qty", this.TotalYards + " (Estimated)", bodyTable);

            PdfPTable containerTable = new PdfPTable(1);
            containerTable.WidthPercentage = 100;
            containerTable.SpacingBefore = 5;

            PdfPCell containerCell = new PdfPCell();
            containerCell.AddElement(bodyTable);
            containerCell.BorderWidth = 0.5f;
            containerCell.BorderWidthBottom = 0.9f;
            containerCell.BorderColorBottom = BaseColor.BLACK;
            containerTable.AddCell(containerCell);
            doc.Add(containerTable);
            #endregion

            #region Disclaimers
            PdfPTable paraTable = CreateParagraphTable(baseFont, this.DisclaimerText);
            doc.Add(paraTable);
            #endregion

            #region Mix Table

            if (this.Profile.ConcreteEnabled)
            {
                PdfPTable paraTable9 = CreateParagraphTable(baseFont, "Concrete Products", 12, "light", false, true);
                paraTable9.SpacingBefore = 10;
                doc.Add(paraTable9);
                PdfPTable AltColorStyleTable = new PdfPTable(this.IncludedColumns.Count());
                AltColorStyleTable.SpacingBefore = 5;
                AltColorStyleTable.WidthPercentage = 100;

                float[] finalWidths = new float[this.IncludedColumns.Count];
                int y = 0;
                for (int i = 0; i < 15; i++)
                {
                    if (this.IncludedColumns.Contains(i))
                    {
                        finalWidths[y] = Widths[i];
                        y++;
                    }
                }
                AltColorStyleTable.SetWidths(finalWidths);
                var headerColumnCount = 0;
                for (int i = 0; i < 15; i++)
                {
                    if (this.IncludedColumns.Contains(i))
                    {

                        PdfPCell cell1 = new PdfPCell();
                        //cell1.UseVariableBorders = true;
                        //cell1.BackgroundColor = BaseColor.WHITE;
                        //cell1.BorderColor = BaseColor.BLACK;
                        cell1.BorderColorLeft = BaseColor.WHITE;
                        cell1.BorderWidthBottom = 0.5f;
                        if (headerColumnCount == 0)
                        {
                            cell1.BorderColorLeft = BaseColor.BLACK;
                            cell1.BorderColorRight = BaseColor.BLACK;
                        }
                        if (headerColumnCount == 1)
                        {
                            cell1.BorderWidthLeft = 0.5f;
                        }
                        Paragraph para1 = new Paragraph(this.Headers[i], new Font(baseFont, 8, Font.BOLD));
                        para1.Alignment = 0;
                        cell1.AddElement(para1);
                        AltColorStyleTable.AddCell(cell1);
                        headerColumnCount = headerColumnCount + 1;
                    }
                }

                for (int i = 0; i < this.TableData.Count; i++)
                {
                    string[] data = this.TableData[i];
                    var bodyColumnCount = 0;
                    for (int j = 0; j < 15; j++)
                    {
                        if (this.IncludedColumns.Contains(j))
                        {
                            string dataValue = data[j];
                            PdfPCell cell1 = new PdfPCell();
                            cell1.UseVariableBorders = true;
                            cell1.BorderColor = BaseColor.BLACK;
                            cell1.BorderColorLeft = BaseColor.WHITE;
                            cell1.BorderColorTop = BaseColor.WHITE;
                            cell1.BorderWidth = 0.5f;
                            if (bodyColumnCount == 0)
                            {
                                cell1.BorderColorLeft = BaseColor.BLACK;
                            }
                            if (bodyColumnCount == (this.TableData.Count - 1))
                            {
                                cell1.BorderWidthBottom = 0.8f;
                                cell1.BorderColorBottom = BaseColor.BLACK;
                            }
                            Paragraph para1 = new Paragraph(dataValue, new Font(baseFont, 8));
                            para1.Alignment = 0;
                            cell1.AddElement(para1);
                            AltColorStyleTable.AddCell(cell1);
                            bodyColumnCount = bodyColumnCount + 1;
                        }
                    }
                }

                doc.Add(AltColorStyleTable);
            }
            #endregion

            #region Aggregate Products

            if (this.Profile.AggregateEnabled)
            {
                PdfPTable paraTable10 = CreateParagraphTable(baseFont, "Aggregate Products", 12, "light", false, true);
                paraTable10.SpacingBefore = 0;
                doc.Add(paraTable10);
                DataTable aggDataTable = new DataTable();

                DataRow row = null;
                //Create Headers
                row = aggDataTable.NewRow();
                aggDataTable.Columns.Add(new DataColumn { ColumnName = "Description", Caption = "Description" });
                aggDataTable.Columns.Add(new DataColumn { ColumnName = "Quantity", Caption = "Qty" });
                aggDataTable.Columns.Add(new DataColumn { ColumnName = "Product", Caption = "Product" });
                aggDataTable.Columns.Add(new DataColumn { ColumnName = "Freight", Caption = "Freight" });
                aggDataTable.Columns.Add(new DataColumn { ColumnName = "UnitPrice", Caption = "Unit Price" });
                aggDataTable.Columns.Add(new DataColumn { ColumnName = "Total", Caption = "Total" });
                aggDataTable.Columns.Add(new DataColumn { ColumnName = "Comments", Caption = "Comments" });

                foreach (var aggProd in this.AggregateProducts)
                {
                    row = aggDataTable.NewRow();
                    row["Description"] = aggProd.QuotedDescription;
                    row["Quantity"] = aggProd.Volume;
                    row["Product"] = "$" + aggProd.Price.ToString("N2");
                    row["Freight"] = "$" + aggProd.Freight.ToString("N2");
                    row["UnitPrice"] = "$" + aggProd.TotalPrice.ToString("N2");
                    row["Total"] = "$" + aggProd.TotalRevenue.ToString("N2");
                    row["Comments"] = aggProd.PublicNotes;

                    aggDataTable.Rows.Add(row);
                }

                foreach (var aggAddon in AggregateAddons.Where(x=>x.IsIncludeTable.GetValueOrDefault(false)))
                {
                    row = aggDataTable.NewRow();
                    row["Description"] = aggAddon.Description;
                    row["Quantity"] = "";
                    row["Product"] = "";
                    row["Freight"] = "";
                    row["UnitPrice"] = aggAddon.Price;
                    row["Total"] = "";
                    row["Comments"] = "";

                    aggDataTable.Rows.Add(row);
                }
                var aggPdfTable = GetPdfTableFromDataTable(aggDataTable, new Font(baseFont, 8));
                aggPdfTable.SpacingBefore = 5;
                aggPdfTable.SpacingAfter = 1;
                doc.Add(aggPdfTable);
            }

            #endregion

            #region Block Products

            if (this.Profile.BlockEnabled)
            {
                PdfPTable paraTable11 = CreateParagraphTable(baseFont, "Block Products", 12, "light", false, true);
                paraTable11.SpacingBefore = 0;
                doc.Add(paraTable11);
                DataTable blockDataTable = new DataTable();

                DataRow row = null;

                //Create Headers 
                row = blockDataTable.NewRow();
                blockDataTable.Columns.Add(new DataColumn { ColumnName = "Description", Caption = "Description" });
                blockDataTable.Columns.Add(new DataColumn { ColumnName = "Quantity", Caption = "Qty" });
                blockDataTable.Columns.Add(new DataColumn { ColumnName = "Product", Caption = "Product" });
                blockDataTable.Columns.Add(new DataColumn { ColumnName = "Freight", Caption = "Freight" });
                blockDataTable.Columns.Add(new DataColumn { ColumnName = "UnitPrice", Caption = "Unit Price" });
                blockDataTable.Columns.Add(new DataColumn { ColumnName = "Total", Caption = "Total" });
                blockDataTable.Columns.Add(new DataColumn { ColumnName = "Comments", Caption = "Comments" });

                foreach (var blockProd in this.BlockProducts)
                {
                    row = blockDataTable.NewRow();
                    row["Description"] = blockProd.QuotedDescription;
                    row["Quantity"] = blockProd.Volume;
                    row["Product"] = "$" + blockProd.Price.ToString("N2");
                    row["Freight"] = "$" + blockProd.Freight.ToString("N2");
                    row["UnitPrice"] = "$" + blockProd.TotalPrice.ToString("N2");
                    row["Total"] = "$" + blockProd.TotalRevenue.ToString("N2");
                    row["Comments"] = blockProd.PublicNotes;

                    blockDataTable.Rows.Add(row);
                }

                foreach (var blockAddon in BlockAddons.Where(x=>x.IsIncludeTable.GetValueOrDefault(false)))
                {
                    row = blockDataTable.NewRow();
                    row["Description"] = blockAddon.Description;
                    row["Quantity"] = "";
                    row["Product"] = "";
                    row["Freight"] = "";
                    row["UnitPrice"] = blockAddon.Price;
                    row["Total"] = "";
                    row["Comments"] = "";

                    blockDataTable.Rows.Add(row);
                }
                var blockPdfTable = GetPdfTableFromDataTable(blockDataTable, new Font(baseFont, 8));
                blockPdfTable.SpacingBefore =5;
                blockPdfTable.SpacingAfter = 1;
                doc.Add(blockPdfTable);
            }

            #endregion

            #region Public Comments
            string str_paraContent2 = this.PublicComments;
            PdfPTable paraTable2 = CreateParagraphTable(baseFont, str_paraContent2, 8, "light", false, true);
            doc.Add(paraTable2);

            #endregion

            #region Add-on Products

            PdfPTable headingText2 = CreateParagraphTable(baseFont, "Add-On Products", 12, "light", false, true);
            headingText2.SpacingBefore = 0;
            doc.Add(headingText2);

            PdfPTable productsTable = new PdfPTable(2);
            productsTable.SetWidths(new float[] { 50, 50 });
            productsTable.WidthPercentage = 100;
            int ii = 0;
            foreach (var addon in this.AllAddonlist.Where(x => x.Per == "Product" && x.IsIncludeTable.GetValueOrDefault() == false).OrderBy(x => x.Sort == null).ThenBy(x => x.Sort).ThenBy(x => x.Description))
            {
                //var addonModel = this.QuoteAddonModel.Where(x => x.AddonId == addon.Id && x.IsIncludeTable.GetValueOrDefault() == false).FirstOrDefault();
                if (addon != null)
                {
                    ii = ii + 1;
                    AddLabelTextRowSingle(baseFont, addon.Description, "$" + addon.Price.GetValueOrDefault().ToString("N2") + "/" + addon.QuoteUomName, productsTable);
                    //AddLabelTextRowSingle(baseFont, addonModel.Description + " (Per" + addonModel.Per + ")", "$" + addonModel.Price + "/" + addonModel.QuoteUomName, productsTable);
                }
            }
            //foreach (Addon addon in this.QuoteAddons.Where(x => x.AddonType == "Product").OrderBy(x => x.Sort == null).ThenBy(x => x.Sort))
            //{
            //    var addonModel = this.QuoteAddonModel.Where(x => x.AddonId == addon.Id && x.IsIncludeTable.GetValueOrDefault() == false).FirstOrDefault();
            //    if (addonModel != null)
            //    {
            //        ii = ii + 1;
            //        AddLabelTextRowSingle(baseFont, addonModel.Description, "$" + addonModel.Price.GetValueOrDefault().ToString("N2") + "/" + addonModel.QuoteUomName, productsTable);
            //        //AddLabelTextRowSingle(baseFont, addonModel.Description + " (Per" + addonModel.Per + ")", "$" + addonModel.Price + "/" + addonModel.QuoteUomName, productsTable);
            //    }
            //}
            //if (this.AggregateAddons != null)
            //{
            //    foreach (var addon in this.AggregateAddons.Where(x => x.Per == "Product" && x.IsIncludeTable.GetValueOrDefault() == false).OrderBy(x => x.Sort == null).ThenBy(x => x.Sort))
            //    {
            //        ii = ii + 1;
            //        AddLabelTextRowSingle(baseFont, addon.Description, "$" + addon.Price.GetValueOrDefault().ToString("N2") + "/" + addon.QuoteUomName, productsTable);
            //    }
            //}

            //if (this.BlockAddons != null)
            //{
            //    foreach (var addon in this.BlockAddons.Where(x => x.Per == "Product" && x.IsIncludeTable.GetValueOrDefault() == false).OrderBy(x => x.Sort == null).ThenBy(x => x.Sort))
            //    {
            //        ii = ii + 1;
            //        AddLabelTextRowSingle(baseFont, addon.Description, "$" + addon.Price.GetValueOrDefault().ToString("N2") + "/" + addon.QuoteUomName, productsTable);
            //    }
            //}
        

            for (int xx = 0; xx < (2 * Math.Ceiling(ii / 2.0)) - ii; xx++)
            {
                AddLabelTextRowSingle(baseFont, "", "", productsTable);
            }
            PdfPTable containerTable3 = AddToContainerTable(productsTable);
            doc.Add(containerTable3);

            #endregion

            #region Price Increase

            PdfPTable headingText1 = CreateParagraphTable(baseFont, "Price Increase and Quote Expiration", 12, "light", false, true);
            headingText1.SpacingBefore = 0;
            headingText1.SpacingAfter = 10;
            doc.Add(headingText1);

            PdfPTable priceIncreaseTable = new PdfPTable(2);
            priceIncreaseTable.WidthPercentage = 100;
            priceIncreaseTable.SpacingBefore = 0;
            PdfPCell textCell = new PdfPCell();
            textCell.BorderWidth = 0.0005f;
            var content = this.PricingAvailabilityDisclaimer;
            var hasTags = false;
            if (content != null)
            {
                Regex tagRegex = new Regex(@"<.*?>");
                hasTags = tagRegex.IsMatch(content);
                //hasTags = textContent.StartsWith("<p>");
            }

            if (hasTags)
            {
                foreach (var element in XMLWorkerHelper.ParseToElementList(content, "p { font-family: 'Lato', sans-serif;font-size: 11px;}"))
                {
                    textCell.AddElement(element);
                }
            }
            else
            {
                textCell.AddElement(new Paragraph(this.PricingAvailabilityDisclaimer, new Font(baseFont, 8)));
            }

            priceIncreaseTable.AddCell(textCell);

            PdfPCell dataCell = new PdfPCell();
            dataCell.HorizontalAlignment = 2;
            dataCell.BorderWidth = 0.0005f;
            if (PriceIncreaseDate1 != null)
                dataCell.AddElement(CreateRightAlignedPara(baseFont, "Price Increase #1 : Add $" + PriceIncrease1.ToString("N2") + "/CYD" + " on " + (PriceIncreaseDate1 == null ? "" : PriceIncreaseDate1.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture))));
            if (PriceIncreaseDate2 != null)
                dataCell.AddElement(CreateRightAlignedPara(baseFont, "Price Increase #2 : Add $" + PriceIncrease2.ToString("N2") + "/CYD" + " on " + (PriceIncreaseDate2 == null ? "" : PriceIncreaseDate2.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture))));
            if (PriceIncreaseDate3 != null)
                dataCell.AddElement(CreateRightAlignedPara(baseFont, "Price Increase #3 : Add $" + PriceIncrease3.ToString("N2") + "/CYD" + " on " + (PriceIncreaseDate3 == null ? "" : PriceIncreaseDate3.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture))));
            if (LettingDate != null)
            {
                dataCell.AddElement(CreateRightAlignedPara(baseFont, "Project Letting Date " + (LettingDate != null ? LettingDate.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture) : ""), true));
            }

            var expContent = "<p><label>"+this.ExpirationDateLabel + "&nbsp;<p>" + (QuoteExpiry != null ? QuoteExpiry.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture)+ "</p>" : "")+ "</label></p>";
            var expHasTags = false;
            if (expContent != null)
            {
                Regex tagRegex = new Regex(@"<.*?>");
                expHasTags = tagRegex.IsMatch(expContent);
                //hasTags = textContent.StartsWith("<p>");
            }

            if (expHasTags)
            {
                foreach (var element in XMLWorkerHelper.ParseToElementList(expContent, "p { font-family: 'Lato', sans-serif;font-size: 11px;text-align: right;font-weight: bold;}"))
                {
                    dataCell.AddElement(element);
                }
            }
            else
            {
                dataCell.AddElement(CreateRightAlignedPara(baseFont, this.ExpirationDateLabel + " " + (QuoteExpiry != null ? QuoteExpiry.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture) : ""), true));
            }
            priceIncreaseTable.AddCell(dataCell);

            doc.Add(priceIncreaseTable);

            #endregion

            #region Charges and Fees

            PdfPTable paraTable3 = CreateParagraphTable(baseFont, "Charges and Fees", 12, "light", false, true);
            paraTable3.SpacingBefore = 0;
            doc.Add(paraTable3);

            PdfPTable chargesAndFeesTable = new PdfPTable(2);
            chargesAndFeesTable.SetWidths(new float[] { 50, 50 });
            chargesAndFeesTable.WidthPercentage = 100;
            ii = 0;
            foreach (var addon in AllAddonlist.Where(x => x.Per != "Product" && x.IsIncludeTable.GetValueOrDefault() == false).OrderBy(x => x.Sort == null).ThenBy(x => x.Sort))
            {
                ii = ii + 1;
                AddLabelTextRowSingle(baseFont, addon.Description, "$" + addon.Price + "/" + addon.QuoteUomName, chargesAndFeesTable);
            }
            //if (this.AggregateAddons != null)
            //{
            //    foreach (var addon in AggregateAddons.Where(x => x.Per != "Product" && x.IsIncludeTable.GetValueOrDefault() == false).OrderBy(x => x.Sort == null).ThenBy(x => x.Sort))
            //    {
            //        ii = ii + 1;
            //        AddLabelTextRowSingle(baseFont, addon.Description, "$" + addon.Price + "/" + addon.QuoteUomName, chargesAndFeesTable);
            //    }
            //}
            //if (this.BlockAddons != null)
            //{
            //    foreach (var addon in BlockAddons.Where(x => x.Per != "Product" && x.IsIncludeTable.GetValueOrDefault() == false).OrderBy(x => x.Sort == null).ThenBy(x => x.Sort))
            //    {
            //        ii = ii + 1;
            //        AddLabelTextRowSingle(baseFont, addon.Description, "$" + addon.Price + "/" + addon.QuoteUomName, chargesAndFeesTable);
            //    }
            //}
            for (int xx = 0; xx < (2 * Math.Ceiling(ii / 2.0)) - ii; xx++)
            {
                AddLabelTextRowSingle(baseFont, "", "", chargesAndFeesTable);
            }

            PdfPTable containerTable2 = AddToContainerTable(chargesAndFeesTable);
            doc.Add(containerTable2);

            #endregion

            #region Disclosures
            string str_paraContent3 = this.Disclosure;
            PdfPTable paraTable31 = CreateParagraphTable(baseFont, str_paraContent3);
            doc.Add(paraTable31);
            #endregion

            LineSeparator ls = new LineSeparator();
            doc.Add(new Chunk(ls));

            #region Terms and Conditions
            string str_paraContent4 = this.TermsAndConditions;
            PdfPTable paraTable4 = CreateParagraphTable(baseFont, str_paraContent4);
            doc.Add(paraTable4);
            #endregion

            //doc.Add(new Chunk(ls));

            #region Acceptance Text
            string str_paraContent5 = this.AcceptanceText;
            PdfPTable paraTable5 = CreateParagraphTable(baseFont, str_paraContent5, 8, "light", false);
            doc.Add(paraTable5);
            #endregion

            doc.Add(new Chunk(ls));

            #region Footer Signature

            PdfPTable footerTable = new PdfPTable(2);
            footerTable.WidthPercentage = 100;
            footerTable.SpacingBefore = 15;

            PdfPCell head1 = new PdfPCell();
            Paragraph p21 = new Paragraph("Prepared and Submitted By:", new Font(baseFont, 10));
            p21.Alignment = 1;
            head1.AddElement(p21);
            head1.HorizontalAlignment = 2;
            PdfPCell head2 = new PdfPCell();
            Paragraph p22 = new Paragraph("Acceptance (void if not accepted before " + this.AcceptanceDate.GetValueOrDefault(DateTime.Today).ToString("M/d/yyyy", CultureInfo.InvariantCulture) + ")", new Font(baseFont, 10));
            p22.Alignment = 1;
            head2.AddElement(p22);
            footerTable.AddCell(head1);
            footerTable.AddCell(head2);

            PdfPCell infoCell = new PdfPCell();
            PdfPTable infoTable = new PdfPTable(2);
            infoTable.WidthPercentage = 100;
            infoTable.SetWidths(new float[] { 30, 60 });

            AddLabelTextRow(baseFont, "Name", this.SalesStaff, infoTable);
            AddLabelTextRow(baseFont, "Date", (this.QuoteDate == null ? "" : this.QuoteDate.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture)), infoTable);
            AddLabelTextRow(baseFont, "Phone", this.UserPhone, infoTable);
            AddLabelTextRow(baseFont, "Fax", this.UserFax, infoTable);
            AddLabelTextRow(baseFont, "Email", this.UserEmail, infoTable);
            infoCell.AddElement(infoTable);
            footerTable.AddCell(infoCell);

            PdfPCell signatureCell = new PdfPCell();
            signatureCell.HorizontalAlignment = 2;

            PdfPTable borderedFooterTable = new PdfPTable(4);
            borderedFooterTable.WidthPercentage = 100;
            borderedFooterTable.SetWidths(new float[] { 25, 40, 15, 20 });

            PdfPCell sigCell = CreateUnderLinedCell("Signature", baseFont, false);
            PdfPCell dateCell = CreateUnderLinedCell("Date", baseFont, false);
            PdfPCell printNameCell = CreateUnderLinedCell("Print Name", baseFont, false);
            PdfPCell titleCell = CreateUnderLinedCell("Title", baseFont, false);
            PdfPCell companyCell = CreateUnderLinedCell("Company", baseFont, false);
            PdfPCell blankCell = CreateUnderLinedCell("", baseFont);

            borderedFooterTable.AddCell(sigCell);
            borderedFooterTable.AddCell(blankCell);
            borderedFooterTable.AddCell(dateCell);
            borderedFooterTable.AddCell(blankCell);

            borderedFooterTable.AddCell(printNameCell);
            borderedFooterTable.AddCell(blankCell);
            borderedFooterTable.AddCell(blankCell);
            borderedFooterTable.AddCell(blankCell);

            borderedFooterTable.AddCell(titleCell);
            borderedFooterTable.AddCell(blankCell);
            borderedFooterTable.AddCell(blankCell);
            borderedFooterTable.AddCell(blankCell);

            borderedFooterTable.AddCell(companyCell);
            borderedFooterTable.AddCell(blankCell);
            borderedFooterTable.AddCell(blankCell);
            borderedFooterTable.AddCell(blankCell);

            signatureCell.AddElement(borderedFooterTable);
            footerTable.AddCell(signatureCell);

            doc.Add(footerTable);

            #endregion

            doc.Close();
        }

        private PdfPCell GenerateRowCell(PdfPCell paragraph)
        {
            PdfPCell cellObj = new PdfPCell(paragraph);
            cellObj.BorderColorBottom = BaseColor.BLACK;
            cellObj.BorderColorLeft = BaseColor.BLACK;
            cellObj.BorderColorRight = BaseColor.BLACK;
            cellObj.BorderColorTop = BaseColor.BLACK;
            return cellObj;
        }

        protected PdfPCell OwnerParagraphCell(bool white = true, string text = "", iTextSharp.text.Font.FontFamily fontFamily = Font.FontFamily.HELVETICA, int size = 11, int fontStyle = Font.NORMAL, int alignment = Element.ALIGN_CENTER, bool bottomBorder = false, bool borderLeftBlack = false, bool borderRightBlack = false, bool borderTopBlack = false, bool paddingBottom = false, int colSpan = 0, float paddingLeft = 0, float paddingRight = 0, float marginRight = 0)
        {
            PdfPCell cell = new PdfPCell();
            if (white)
                cell = CreateWhiteCell();

            cell.HorizontalAlignment = alignment;
            Paragraph p = new Paragraph(text, new Font(fontFamily, size, fontStyle));
            p.Alignment = alignment;
            cell.AddElement(p);
            cell.BorderColor = BaseColor.WHITE;
            if (bottomBorder)
            {
                cell.BorderWidthBottom = 0;
                cell.BorderColorBottom = BaseColor.WHITE;
            }
            if (borderLeftBlack)
            {
                cell.BorderWidthLeft = 0;
                cell.BorderColorLeft = BaseColor.WHITE;
            }
            if (borderRightBlack)
            {
                cell.BorderWidthRight = 0;
                cell.BorderColorRight = BaseColor.WHITE;
            }
            if (borderTopBlack)
            {
                cell.BorderWidthTop = 0;
                cell.BorderColorTop = BaseColor.WHITE;
            }
            if (paddingBottom)
                cell.PaddingBottom = 10f;


            if (colSpan > 0)
                cell.Colspan = colSpan;

            if (paddingLeft > 0)
                cell.PaddingLeft = paddingLeft;

            if (paddingRight > 0)
                cell.PaddingRight = paddingRight;

            if (marginRight > 0)
                cell.Right = marginRight;

            return cell;
        }

        public void GenerateProjectEntryPDF(string path, string logoPath)
        {
            iTextSharp.text.Font.FontFamily baseFont = Font.FontFamily.HELVETICA;
            Document doc = new Document(new Rectangle(PageSize.A4.Width, PageSize.A4.Height));
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();

            #region Header

            PdfPTable headerTable = new PdfPTable(1);
            headerTable.WidthPercentage = 100;

            //Logo
            PdfPCell logoCell = CreateWhiteCell();
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(logoPath);
            image.ScalePercent(50);
            image.Alignment = Element.ALIGN_MIDDLE;
            logoCell.AddElement(image);
            logoCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            headerTable.AddCell(logoCell);

            //Heading here..
            PdfPCell headingCell = CreateWhiteCell();
            Paragraph headingContent = new Paragraph("Project Entry Form", new Font(baseFont, 20F));
            headingCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            headingContent.Alignment = Element.ALIGN_CENTER;
            headingCell.AddElement(headingContent);
            headerTable.AddCell(headingCell);

            //Header Address Content..
            PdfPCell topAddressCell = CreateWhiteCell();
            Paragraph headingContent2 = new Paragraph((this.PDFGeneratedDate == null ? "" : "Created On : " + this.PDFGeneratedDate.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture)), new Font(baseFont, 10F));
            headingCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            headingContent2.Alignment = Element.ALIGN_CENTER;
            topAddressCell.AddElement(headingContent2);
            headerTable.AddCell(topAddressCell);

            doc.Add(headerTable);
            #endregion

            #region Customer Information

            PdfPTable customerSectionHeading = CreateParagraphTable(baseFont, "Customer Information", 14);
            customerSectionHeading.SpacingAfter = 5;
            customerSectionHeading.SpacingBefore = 5;
            doc.Add(customerSectionHeading);

            PdfPTable bodyTable = new PdfPTable(2);
            bodyTable.WidthPercentage = 100;
            bodyTable.SetWidths(new float[] { 30, 70 });

            AddLabelTextRow(baseFont, "Customer Number", this.Profile.CustomerNumber, bodyTable);
            AddLabelTextRow(baseFont, "Customer Namer", this.Profile.CustomerName, bodyTable);
            AddLabelTextRow(baseFont, "Market Segment", this.Profile.MarketSegment, bodyTable);
            AddLabelTextRow(baseFont, "Tax Code - Description", this.Profile.TaxCode, bodyTable);
            AddLabelTextRow(baseFont, "Tax Exempt Reason", this.Profile.TaxExceptReason, bodyTable);

            PdfPTable containerTable = new PdfPTable(1);
            containerTable.WidthPercentage = 100;
            PdfPCell containerCell = new PdfPCell();
            containerCell.AddElement(bodyTable);
            containerTable.AddCell(containerCell);
            doc.Add(containerTable);

            #endregion

            #region Project Information

            PdfPTable projectSectionHeading = CreateParagraphTable(baseFont, "Project Information", 14);
            projectSectionHeading.SpacingBefore = 5;
            doc.Add(projectSectionHeading);

            PdfPTable projectInfoTable = new PdfPTable(2);
            projectInfoTable.WidthPercentage = 100;
            projectInfoTable.SetWidths(new float[] { 30, 70 });

            AddLabelTextRow(baseFont, "Customer Job Reference", this.Profile.CustomerJobRef, projectInfoTable);
            AddLabelTextRow(baseFont, "Project Name", this.Profile.ProjectName, projectInfoTable);
            AddLabelTextRow(baseFont, "Address", this.Address, projectInfoTable);
            AddLabelTextRow(baseFont, "City, State, Zip", this.CityStateZip, projectInfoTable);
            AddLabelTextRow(baseFont, "Project Bid Date", this.Profile.BidDate, projectInfoTable);
            AddLabelTextRow(baseFont, "Project Start Date", this.Profile.StartDate, projectInfoTable);
            AddLabelTextRow(baseFont, "To Job (minutes)", this.Profile.ProjectToJob, projectInfoTable);
            AddLabelTextRow(baseFont, "Distance To Job (miles)", this.Profile.DistanceToJob, projectInfoTable);
            AddLabelTextRow(baseFont, "Delivery Instructions", this.Profile.DeliveryInstructions, projectInfoTable);

            PdfPTable containerTable2 = AddToContainerTable(projectInfoTable);
            doc.Add(containerTable2);
            #endregion

            #region Pricing Information

            PdfPTable pricingSectionHeading = CreateParagraphTable(baseFont, "Pricing Information", 14);
            pricingSectionHeading.SpacingBefore = 5;
            doc.Add(pricingSectionHeading);

            PdfPTable princingInfoTable = new PdfPTable(2);
            princingInfoTable.WidthPercentage = 100;
            princingInfoTable.SetWidths(new float[] { 30, 70 });

            AddLabelTextRow(baseFont, "Quote Date", (this.QuoteDate != null ? this.QuoteDate.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture) : ""), princingInfoTable);
            AddLabelTextRow(baseFont, "Quote Expiration", (this.QuoteExpiry != null ? this.QuoteExpiry.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture) : ""), princingInfoTable);
            AddLabelTextRow(baseFont, "Sales Staff", this.SalesStaff, princingInfoTable);
            AddLabelTextRow(baseFont, "Pricing Plant", this.Profile.PlantName, princingInfoTable);
            AddLabelTextRow(baseFont, "Pricing Month", (this.Profile.PricingMonth != null ? this.Profile.PricingMonth.Value.ToString("M/d/yyyy") : ""), princingInfoTable);
            AddLabelTextRow(baseFont, "Price Increase #1", "$" + PriceIncrease1.ToString("N2") + "/CYD (effective " + (PriceIncreaseDate1 == null ? "" : PriceIncreaseDate1.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture)) + ")", princingInfoTable);
            AddLabelTextRow(baseFont, "Price Increase #2", "$" + PriceIncrease2.ToString("N2") + "/CYD (effective " + (PriceIncreaseDate2 == null ? "" : PriceIncreaseDate2.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture)) + ")", princingInfoTable);
            AddLabelTextRow(baseFont, "Price Increase #3", "$" + PriceIncrease3.ToString("N2") + "/CYD (effective " + (PriceIncreaseDate3 == null ? "" : PriceIncreaseDate3.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture)) + ")", princingInfoTable);

            PdfPTable containerTable3 = AddToContainerTable(princingInfoTable);
            doc.Add(containerTable3);
            #endregion

            #region Mix Table

            PdfPTable mixSectionHeading = CreateParagraphTable(baseFont, "Approved Mixes - Concrete", 12);
            mixSectionHeading.SpacingBefore = 5;
            doc.Add(mixSectionHeading);

            PdfPTable mixTable = new PdfPTable(4);
            mixTable.WidthPercentage = 100;
            mixTable.SetWidths(new float[] { 40, 20, 20,20 });

            string[] mixTableHeaders = new string[] { "Quoted Description", "Mix ID", "Price","Public Comments" };
            foreach (string header in mixTableHeaders)
            {
                PdfPCell cell1 = new PdfPCell();
                cell1.UseVariableBorders = true;
                cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell1.BorderColor = BaseColor.LIGHT_GRAY;
                Paragraph para1 = new Paragraph(header, new Font(baseFont, 8));
                para1.Alignment = 0;
                cell1.AddElement(para1);
                mixTable.AddCell(cell1);
            }

            int i = 0;
            foreach (QuotationMix mix in this.MixDescriptions)
            {
                QuotationMixPreviewTable model = new QuotationMixPreviewTable(mix);
                string[] data = new string[4];
                data[0] = model.MixDescription;
                data[1] = model.MixId;
                data[2] = "$" + model.Price.ToString("N2");
                data[3] = model.Comments;

                foreach (string dataValue in data)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.UseVariableBorders = true;
                    cell1.BorderColor = BaseColor.WHITE;
                    cell1.BackgroundColor = (i % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                    cell1.BorderColorBottom = (i % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                    cell1.BorderColorTop = (i % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                    Paragraph para1 = new Paragraph(dataValue, new Font(baseFont, 8));
                    para1.Alignment = 0;
                    cell1.AddElement(para1);
                    mixTable.AddCell(cell1);
                }
                i++;
            }

            PdfPTable mixTableContainer = AddToContainerTable(mixTable);
            doc.Add(mixTableContainer);
            #endregion

            #region Addon Table

            PdfPTable addonSectionHeading = CreateParagraphTable(baseFont, "Quoted Fees, Changes, and Add-On Products - Concrete", 12);
            addonSectionHeading.SpacingBefore = 5;
            doc.Add(addonSectionHeading);

            PdfPTable addonTable = new PdfPTable(2);
            addonTable.SetWidths(new float[] { 60, 40 });
            addonTable.WidthPercentage = 100;

            string[] addonTableHeaders = new string[] { "Description", "Price" };
            foreach (string header in addonTableHeaders)
            {
                PdfPCell cell1 = new PdfPCell();
                cell1.UseVariableBorders = true;
                cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell1.BorderColor = BaseColor.LIGHT_GRAY;
                Paragraph para1 = new Paragraph(header, new Font(baseFont, 8));
                para1.Alignment = 0;
                cell1.AddElement(para1);
                addonTable.AddCell(cell1);
            }

            i = 0;
            foreach (var a in this.QuoteAddonModel)
            {
                string[] data = new string[2];
                data[0] = a.Description;
                data[1] = "$" + a.Price + " Per " + a.QuoteUomName;
                foreach (string dataValue in data)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.UseVariableBorders = true;
                    cell1.BorderColor = BaseColor.WHITE;
                    cell1.BackgroundColor = (i % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                    cell1.BorderColorBottom = (i % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                    cell1.BorderColorTop = (i % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                    Paragraph para1 = new Paragraph(dataValue, new Font(baseFont, 8));
                    para1.Alignment = 0;
                    cell1.AddElement(para1);
                    addonTable.AddCell(cell1);
                }
                i++;
            }

            PdfPTable addonTableContainer = AddToContainerTable(addonTable);
            doc.Add(addonTableContainer);
            #endregion

            #region Aggregate product Table
            if (this.AggregateProducts != null && this.AggregateProducts.Count != 0)
            {
                PdfPTable aggProductSectionHeading = CreateParagraphTable(baseFont, "Approved Mixes - Aggregate", 12);
                aggProductSectionHeading.SpacingBefore = 5;
                doc.Add(aggProductSectionHeading);

                PdfPTable aggProductTable = new PdfPTable(4);
                aggProductTable.WidthPercentage = 100;
                aggProductTable.SetWidths(new float[] { 40, 20, 20 ,20 });

                string[] aggProductTableHeaders = new string[] { "Quoted Description", "Code","Price", "Public Comments" };
                foreach (string header in aggProductTableHeaders)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.UseVariableBorders = true;
                    cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell1.BorderColor = BaseColor.LIGHT_GRAY;
                    Paragraph para1 = new Paragraph(header, new Font(baseFont, 8));
                    para1.Alignment = 0;
                    cell1.AddElement(para1);
                    aggProductTable.AddCell(cell1);
                }

                int j = 0;
                foreach (var mix in this.AggregateProducts)
                {
                    string[] data = new string[4];
                    data[0] = mix.QuotedDescription;
                    data[1] = mix.Code;
                    data[2] = "$" + mix.Price.ToString("N2");
                    data[3] = mix.PublicNotes;

                    foreach (string dataValue in data)
                    {
                        PdfPCell cell1 = new PdfPCell();
                        cell1.UseVariableBorders = true;
                        cell1.BorderColor = BaseColor.WHITE;
                        cell1.BackgroundColor = (j % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorBottom = (j % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorTop = (j % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        Paragraph para1 = new Paragraph(dataValue, new Font(baseFont, 8));
                        para1.Alignment = 0;
                        cell1.AddElement(para1);
                        aggProductTable.AddCell(cell1);
                    }
                    j++;
                }

                PdfPTable aggProductTableContainer = AddToContainerTable(aggProductTable);
                doc.Add(aggProductTableContainer);
            }
            #endregion

            #region Aggregate Addon Table
            if (this.AggregateProducts != null && this.AggregateAddons.Count != 0)
            {
                PdfPTable aggregateAddonSectionHeading = CreateParagraphTable(baseFont, "Quoted Fees, Changes, and Add-On Products - Aggregate", 12);
                aggregateAddonSectionHeading.SpacingBefore = 5;
                doc.Add(aggregateAddonSectionHeading);

                PdfPTable aggregateAddonTable = new PdfPTable(2);
                aggregateAddonTable.SetWidths(new float[] { 60, 40 });
                aggregateAddonTable.WidthPercentage = 100;

                string[] aggregateAddonTableHeaders = new string[] { "Description", "Price" };
                foreach (string header in aggregateAddonTableHeaders)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.UseVariableBorders = true;
                    cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell1.BorderColor = BaseColor.LIGHT_GRAY;
                    Paragraph para1 = new Paragraph(header, new Font(baseFont, 8));
                    para1.Alignment = 0;
                    cell1.AddElement(para1);
                    aggregateAddonTable.AddCell(cell1);
                }

                int l = 0;
                foreach (var a in this.AggregateAddons)
                {
                    string[] data = new string[2];
                    data[0] = a.Description;
                    data[1] = "$" + a.Price + " Per " + a.QuoteUomName;
                    foreach (string dataValue in data)
                    {
                        PdfPCell cell1 = new PdfPCell();
                        cell1.UseVariableBorders = true;
                        cell1.BorderColor = BaseColor.WHITE;
                        cell1.BackgroundColor = (l % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorBottom = (l % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorTop = (l % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        Paragraph para1 = new Paragraph(dataValue, new Font(baseFont, 8));
                        para1.Alignment = 0;
                        cell1.AddElement(para1);
                        aggregateAddonTable.AddCell(cell1);
                    }
                    l++;
                }
                PdfPTable aggregateAddonTableContainer = AddToContainerTable(aggregateAddonTable);
                doc.Add(aggregateAddonTableContainer);
            }
            #endregion

            #region Block product Table
            if (this.BlockProducts != null && this.BlockProducts.Count != 0)
            {
                PdfPTable blockProductSectionHeading = CreateParagraphTable(baseFont, "Approved Mixes - Block", 12);
                blockProductSectionHeading.SpacingBefore = 5;
                doc.Add(blockProductSectionHeading);

                PdfPTable blockProductTable = new PdfPTable(4);
                blockProductTable.WidthPercentage = 100;
                blockProductTable.SetWidths(new float[] { 40, 20, 20 ,20});

                string[] blockProductTableHeaders = new string[] { "Quoted Description", "Code","Price", "Public Comments" };
                foreach (string header in blockProductTableHeaders)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.UseVariableBorders = true;
                    cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell1.BorderColor = BaseColor.LIGHT_GRAY;
                    Paragraph para1 = new Paragraph(header, new Font(baseFont, 8));
                    para1.Alignment = 0;
                    cell1.AddElement(para1);
                    blockProductTable.AddCell(cell1);
                }

                int k = 0;
                foreach (var mix in this.BlockProducts)
                {
                    string[] data = new string[4];
                    data[0] = mix.QuotedDescription;
                    data[1] = mix.Code;
                    data[2] = "$" + mix.Price.ToString("N2");
                    data[3] = mix.PublicNotes;

                    foreach (string dataValue in data)
                    {
                        PdfPCell cell1 = new PdfPCell();
                        cell1.UseVariableBorders = true;
                        cell1.BorderColor = BaseColor.WHITE;
                        cell1.BackgroundColor = (k % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorBottom = (k % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorTop = (k % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        Paragraph para1 = new Paragraph(dataValue, new Font(baseFont, 8));
                        para1.Alignment = 0;
                        cell1.AddElement(para1);
                        blockProductTable.AddCell(cell1);
                    }
                    k++;
                }

                PdfPTable blockProductTableContainer = AddToContainerTable(blockProductTable);
                doc.Add(blockProductTableContainer);
            }
            #endregion

            #region Block Addon Table
            if (this.BlockProducts != null && this.BlockAddons.Count != 0)
            {
                PdfPTable blockAddonSectionHeading = CreateParagraphTable(baseFont, "Quoted Fees, Changes, and Add-On Products - Block", 12);
                blockAddonSectionHeading.SpacingBefore = 5;
                doc.Add(blockAddonSectionHeading);

                PdfPTable blockAddonTable = new PdfPTable(2);
                blockAddonTable.SetWidths(new float[] { 60, 40 });
                blockAddonTable.WidthPercentage = 100;

                string[] blockAddonTableHeaders = new string[] { "Description", "Price" };
                foreach (string header in blockAddonTableHeaders)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.UseVariableBorders = true;
                    cell1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell1.BorderColor = BaseColor.LIGHT_GRAY;
                    Paragraph para1 = new Paragraph(header, new Font(baseFont, 8));
                    para1.Alignment = 0;
                    cell1.AddElement(para1);
                    blockAddonTable.AddCell(cell1);
                }

                int m = 0;
                foreach (var a in this.BlockAddons)
                {
                    string[] data = new string[2];
                    data[0] = a.Description;
                    data[1] = "$" + a.Price + " Per " + a.QuoteUomName;
                    foreach (string dataValue in data)
                    {
                        PdfPCell cell1 = new PdfPCell();
                        cell1.UseVariableBorders = true;
                        cell1.BorderColor = BaseColor.WHITE;
                        cell1.BackgroundColor = (m % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorBottom = (m % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        cell1.BorderColorTop = (m % 2 == 0 ? BaseColor.WHITE : new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ede9da").ToArgb()));
                        Paragraph para1 = new Paragraph(dataValue, new Font(baseFont, 8));
                        para1.Alignment = 0;
                        cell1.AddElement(para1);
                        blockAddonTable.AddCell(cell1);
                    }
                    m++;
                }

                PdfPTable blockAddonTableContainer = AddToContainerTable(blockAddonTable);
                doc.Add(blockAddonTableContainer);
            }
            #endregion

            #region Quotation Notes

            PdfPTable notesSectionHeading = CreateParagraphTable(baseFont, "Quotation Notes", 14);
            notesSectionHeading.SpacingBefore = 5;
            doc.Add(notesSectionHeading);

            PdfPTable notesInfoTable = new PdfPTable(2);
            notesInfoTable.WidthPercentage = 100;
            notesInfoTable.SetWidths(new float[] { 30, 70 });

            AddLabelTextRow(baseFont, "Public Note", this.PublicComments, notesInfoTable);
            AddLabelTextRow(baseFont, "Private Note", this.PrivateComments, notesInfoTable);

            PdfPTable containerTable4 = AddToContainerTable(notesInfoTable);
            doc.Add(containerTable4);
            #endregion
            doc.Close();
        }

        private PdfPTable AddToContainerTable(PdfPTable chargesAndFeesTable)
        {
            PdfPTable containerTable = new PdfPTable(1);
            containerTable.WidthPercentage = 100;
            PdfPCell containerCell = new PdfPCell();
            containerCell.BorderWidth = 0.5f;
            containerCell.BorderWidthBottom = 0.9f;
            containerCell.BorderColorBottom = BaseColor.BLACK;
            containerCell.AddElement(chargesAndFeesTable);
            containerTable.AddCell(containerCell);
            containerTable.SpacingBefore = 5;
            return containerTable;
        }

        private PdfPCell CreateWhiteCell()
        {
            PdfPCell whiteCell = new PdfPCell();
            whiteCell.BorderColor = BaseColor.WHITE;
            return whiteCell;
        }

        private PdfPCell CreateUnderLinedCell(string text, Font.FontFamily baseFont, bool createUnderline = true)
        {
            PdfPCell cell = new PdfPCell();
            cell.UseVariableBorders = true;
            if (createUnderline)
                cell.BorderColorBottom = BaseColor.LIGHT_GRAY;
            else
                cell.BorderColorBottom = BaseColor.WHITE;
            cell.BorderColorLeft = BaseColor.WHITE;
            cell.BorderColorTop = BaseColor.WHITE;
            cell.BorderColorRight = BaseColor.WHITE;
            Paragraph p = new Paragraph(text, new Font(baseFont, 8));
            p.Alignment = 2;
            cell.AddElement(p);
            return cell;
        }

        private PdfPTable CreateParagraphTable(Font.FontFamily baseFont, string textContent, int fontSize = 8, string theme = "light", bool addNewLine = false, bool bold = false)
        {
            PdfPTable paraTable = new PdfPTable(1);
            paraTable.SpacingBefore = 0.2f;
            paraTable.SpacingAfter = 0f;
            paraTable.WidthPercentage = 100;
            paraTable.DefaultCell.PaddingBottom = 0f;
            PdfPCell paraCol1 = new PdfPCell();
            paraCol1.UseVariableBorders = true;
            paraCol1.BorderColorRight = BaseColor.WHITE;
            paraCol1.BorderColorBottom = BaseColor.WHITE;
            paraCol1.BorderColorLeft = BaseColor.WHITE;
            paraCol1.BorderColorTop = BaseColor.WHITE;
            Paragraph paraContent = new Paragraph(textContent, new Font(baseFont, fontSize, (bold ? Font.BOLD : Font.NORMAL)));
            paraContent.Alignment = 0;
            bool hasTags = false;
            if (textContent != null)
            {
                Regex tagRegex = new Regex(@"<.*?>");
                hasTags = tagRegex.IsMatch(textContent);
                //hasTags = textContent.StartsWith("<p>");
            }

            if (hasTags)
            {
                foreach (var element in XMLWorkerHelper.ParseToElementList(textContent, "p { font-family: 'Lato', sans-serif;font-size: 11px;}"))
                {
                    paraCol1.AddElement(element);
                }
            }
            else
            {
                paraCol1.AddElement(paraContent);
            }

            if (theme == "dark")
            {
                paraCol1.BackgroundColor = BaseColor.LIGHT_GRAY;
            }
            if (addNewLine)
            {
                paraCol1.AddElement(Chunk.NEWLINE);
            }
            paraTable.AddCell(paraCol1);
            return paraTable;
        }

        private Paragraph CreateRightAlignedPara(Font.FontFamily baseFont, string text, bool bold = false)
        {
            Paragraph p = new Paragraph(text, new Font(baseFont, 8, (bold ? Font.BOLD : Font.NORMAL)));
            p.Alignment = 2;
            return p;
        }

        private Paragraph CreateLeftAlignedPara(Font.FontFamily baseFont, string text)
        {
            Paragraph p = new Paragraph(text, new Font(baseFont, 8));
            p.Alignment = 0;
            return p;
        }

        private void AddLabelTextRowSingle(iTextSharp.text.Font.FontFamily baseFont, string label1, string value1, PdfPTable bodyTable, bool capsAndBold = false)
        {
            PdfPCell Col1_cell = new PdfPCell();
            Col1_cell.UseVariableBorders = true;
            Col1_cell.BorderColorTop = BaseColor.WHITE;
            Col1_cell.BorderColorLeft = BaseColor.WHITE;
            Col1_cell.BorderColorRight = BaseColor.WHITE;
            Col1_cell.BorderColorBottom = BaseColor.WHITE;
            Col1_cell.Padding = 0;
            Paragraph para1;
            if (label1 != "")
                label1 = label1 + " : " + value1;

            para1 = new Paragraph(label1, new Font(baseFont, 8));
            para1.Alignment = 0;
            Col1_cell.AddElement(para1);

            bodyTable.AddCell(Col1_cell);
        }

        private void AddLabelTextRow(iTextSharp.text.Font.FontFamily baseFont, string label1, string value1, PdfPTable bodyTable, bool capsAndBold = false)
        {
            PdfPCell Col1_cell = new PdfPCell();
            Col1_cell.UseVariableBorders = true;
            Col1_cell.BorderColorTop = BaseColor.WHITE;
            Col1_cell.BorderColorLeft = BaseColor.WHITE;
            Col1_cell.BorderColorRight = BaseColor.WHITE;
            Col1_cell.BorderColorBottom = BaseColor.WHITE;
            Col1_cell.Padding = 0;
            Paragraph para1;
            if (capsAndBold)
            {
                para1 = new Paragraph(label1.ToUpper() + (label1 != "" ? ": " : ""), new Font(baseFont, 8, Font.BOLD));
            }
            else
            {
                para1 = new Paragraph(label1 + (label1 != "" ? ": " : ""), new Font(baseFont, 8));
            }
            para1.Alignment = 0;
            Col1_cell.AddElement(para1);

            PdfPCell Col2_cell = new PdfPCell();
            Col2_cell.UseVariableBorders = true;
            Col2_cell.BorderColorTop = BaseColor.WHITE;
            Col2_cell.BorderColorLeft = BaseColor.WHITE;
            Col2_cell.BorderColorRight = BaseColor.WHITE;
            Col2_cell.BorderColorBottom = BaseColor.WHITE;
            Col2_cell.Padding = 0;
            Col2_cell.PaddingLeft = 5f;
            Paragraph para2 = new Paragraph(value1, new Font(baseFont, 8));
            para2.Alignment = 0;
            Col2_cell.AddElement(para2);

            bodyTable.AddCell(Col1_cell);
            bodyTable.AddCell(Col2_cell);
        }
    }
}