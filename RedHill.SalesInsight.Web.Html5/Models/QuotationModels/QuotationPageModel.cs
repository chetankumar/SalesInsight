using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Utils;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationPageModel
    {
        public long QuotationId { get; set; }
        [Required(ErrorMessage = "Pricing Plant is required")]
        public int? PlantId { get; set; }
        public int? AggregatePlantId { get; set; }
        public int? BlockPlantId { get; set; }
        public bool EnableAggregateProduct { get; set; }
        public bool EnableBlockProduct { get; set; }
        public Guid _UserId { get; set; }
        public string ActionClicked { get; set; }
        public Quotation Quote { get; set; }
        //public IEnumerable<Plant> plants { get;set; }
        public long? BackupPlantId { get; set; }

        public QuotationCustomerView CustomerView { get; set; }
        public QuotationAddonsView AddOnsView { get; set; }
        public QuotationDetailsView DetailsView { get; set; }
        public QuotationMixView MixView { get; set; }
        public QuotationAuditLogView AuditLogView { get; set; }
        public QuotationProjectView ProjectView { get; set; }
        public QuotationAggregateView QuotationAgg { get; set; }
        public QuotationAggregateAddonsView QuoteAggAddon { get; set; }
        public QuotationBlockView QuotationBlock { get; set; }
        public QuotationBlockAddonView QuoteBlockAddon { get; set; }
        public List<SelectListItem> QCRequirement { get; set; }
        public List<QC_Requirement> QCRequirementList { get; set; }
        public List<long> ProjectQCRequirementList { get; set; }
        public List<Plant> BackupPlantList { get; set; }
        public bool DistrictQcRequirement { get; set; }

        public QuotationPageModel()
        {
            this.CustomerView = new QuotationCustomerView();
            this.ProjectView = new QuotationProjectView();
            this.AddOnsView = new QuotationAddonsView();
            this.DetailsView = new QuotationDetailsView();
            this.AuditLogView = new QuotationAuditLogView();
            this.MixView = new QuotationMixView();
            this.PricingMonthActual = DateUtils.GetFirstOf(DateTime.Today);
            this.QuotationAgg = new QuotationAggregateView();
            this.QuotationBlock = new QuotationBlockView();
            this.QuoteAggAddon = new QuotationAggregateAddonsView();
            this.QuoteBlockAddon = new QuotationBlockAddonView();
            LoadList();
        }

        public QuotationPageModel(long quotationId)
        {
            this.QuotationId = quotationId;
            this.Quote = SIDAL.FindQuotation(this.QuotationId);
            this.CustomerView = new QuotationCustomerView(this.Quote);
            this.AddOnsView = new QuotationAddonsView(this.QuotationId);
            this.ProjectView = new QuotationProjectView(this.Quote);
            this.MixView = new QuotationMixView(this.QuotationId);
            this.DetailsView = new QuotationDetailsView(this.QuotationId);
            this.AuditLogView = new QuotationAuditLogView(this.QuotationId);
            this.QuotationAgg = new QuotationAggregateView(this.QuotationId);
            this.QuotationBlock = new QuotationBlockView(this.QuotationId);
            this.QuoteAggAddon = new QuotationAggregateAddonsView(this.QuotationId);
            this.QuoteBlockAddon = new QuotationBlockAddonView(this.QuotationId);
        }

        public void LoadList(int projectId = 0)
        {
            this.QCRequirementList = SIDAL.GetQCRequirementList();
            this.QCRequirement = new List<SelectListItem>();
            this.ProjectQCRequirementList = SIDAL.GetProjectQCRequirementList(this.Quote != null ? this.Quote.ProjectId.GetValueOrDefault() : projectId);
            List<District> Districts = SIDAL.GetDistrictFilters(this.UserId);
            var PlantList = SIDAL.GetPlantsForDistricts(Districts.Select(x => x.DistrictId).ToArray()).OrderBy(x => x.Name).ToList();
            this.BackupPlantList = PlantList;
            this.DistrictQcRequirement = SIDAL.GetProjectDistrictQcRequirement(this.Quote != null ? this.Quote.ProjectId.GetValueOrDefault() : projectId);

            foreach (QC_Requirement qcReq in QCRequirementList)
            {
                this.QCRequirement.Add(new SelectListItem { Value = qcReq.Id.ToString(), Text = qcReq.Name, Selected = ProjectQCRequirementList.Contains(qcReq.Id) });
            }
        }

        public IEnumerable<Plant> Plants
        {
            get
            {
                return SIDAL.GetPlants(UserId);
            }
        }

        public SelectList ChoosePlants
        {
            get
            {
                return new SelectList(this.Plants.Where(x => x.ProductTypeId == (int)ProductType.Concrete), "PlantId", "Name", this.PlantId);
            }
        }
        public SelectList ChooseAggregatePlants
        {
            get
            {
                return new SelectList(this.Plants.Where(x => x.ProductTypeId == (int)ProductType.Aggregate), "PlantId", "Name", this.PlantId);
            }
        }

        public SelectList ChooseBlockPlants
        {
            get
            {
                return new SelectList(this.Plants.Where(x => x.ProductTypeId == (int)ProductType.Block), "PlantId", "Name", this.PlantId);
            }
        }

        public SelectList Choose5skPricingPlant
        {
            get
            {
                try
                {
                    var default5skId = this.Plant.FSKId;
                    if (DetailsView.FskPriceId != null)
                        default5skId = DetailsView.FskPriceId.GetValueOrDefault();

                    return new SelectList(SIDAL.GetFSKPrices().Select(x => new { Name = x.FSKCode + " - " + x.City, Value = x.Id }).OrderBy(y => y.Name), "Value", "Name", default5skId);
                }
                catch
                {
                    return new SelectList(SIDAL.GetFSKPrices().Select(x => new { Name = x.FSKCode + " - " + x.City, Value = x.Id }).OrderBy(y => y.Name), "Value", "Name", "");
                }
            }
        }

        private List<StandardMix> _StandardMixes;
        public List<StandardMix> StandardMixes
        {
            get
            {
                if (_StandardMixes == null)
                {
                    if (PlantId.HasValue)
                    {
                        _StandardMixes = SIDAL.GetValidStandardMixes(PlantId.GetValueOrDefault(0), PricingMonthActual);
                    }
                    else
                    {
                        _StandardMixes = new List<StandardMix>();
                    }
                }
                return _StandardMixes;
            }
        }



        public void Load()
        {
            var customerPurchaseAggregate = true;
            var enableCompanyAggregate = true;
            var customerPurchaseBlock = true;
            var enableCompanyBlock = true;
            this.EnableBlockProduct = true;
            this.EnableAggregateProduct = true;
            if (this.QuotationId > 0)
            {
                if (this.CustomerView != null)
                {
                    this.CustomerView.QuotationId = this.QuotationId;
                    this.CustomerView.Load(this.Quote);
                    this.PlantId = this.CustomerView.Profile.PlantId;
                    this.AggregatePlantId = this.CustomerView.Profile.AggregatePlantId;
                    this.BlockPlantId = this.CustomerView.Profile.BlockPlantId;
                    this.BackupPlantId = this.CustomerView.Profile.BackupPlantId;
                    this.PricingMonthActual = this.CustomerView.Profile.PricingMonth.Value;
                    var siCustomer = SIDAL.GetCustomer(this.CustomerView.CustomerId);

                    if (siCustomer != null && siCustomer.Customer != null)
                    {
                        customerPurchaseAggregate = siCustomer.Customer.PurchaseAggregate.GetValueOrDefault();
                        customerPurchaseBlock = siCustomer.Customer.PurchaseBlock.GetValueOrDefault();
                    }
                }

                if (this.MixView != null)
                {
                    this.MixView.QuotationId = this.QuotationId;
                    this.MixView.Load();
                }

                if (this.ProjectView != null)
                {
                    this.ProjectView.QuotationId = this.QuotationId;
                    //this.ProjectView.Load();
                }

                if (this.AddOnsView != null)
                {
                    this.AddOnsView.QuotationId = this.QuotationId;
                    this.AddOnsView.UserId = this.UserId;
                    this.AddOnsView.Load(this.Quote);
                }

                if (this.QuotationAgg != null)
                {
                    this.QuotationAgg.QuotationId = this.QuotationId;
                    this.QuotationAgg.Load();
                }
                if (this.QuotationBlock != null)
                {
                    this.QuotationBlock.QuotationId = this.QuotationId;
                    this.QuotationBlock.Load();
                }
                if (this.QuoteAggAddon != null)
                {
                    this.QuoteAggAddon.QuotationId = this.QuotationId;
                    this.QuoteAggAddon.Load();
                }
                if (this.QuoteBlockAddon != null)
                {
                    this.QuoteBlockAddon.QuotationId = this.QuotationId;
                    this.QuoteBlockAddon.Load();
                }
                if (this.AuditLogView != null)
                {
                    this.AuditLogView.QuotationId = this.QuotationId;
                    //this.AuditLogView.Load();
                }

                if (this.DetailsView != null)
                {
                    this.DetailsView.QuotationId = this.QuotationId;
                    this.DetailsView.Load(this.Quote);
                }

                var company = SIDAL.GetCompany();
                if (company != null)
                {
                    enableCompanyAggregate = company.EnableAggregate.GetValueOrDefault(false);
                    enableCompanyBlock = company.EnableBlock.GetValueOrDefault(false);

                }

                if (!enableCompanyBlock || !customerPurchaseBlock)
                {
                    this.EnableBlockProduct = false;
                }
                if (!enableCompanyAggregate || !customerPurchaseAggregate)
                {
                    this.EnableAggregateProduct = false;
                }
            }
            LoadList();
        }

        private Plant _Plant;
        public Plant Plant
        {
            get
            {
                if (_Plant == null)
                {
                    if (PlantId != null)
                    {
                        _Plant = SIDAL.GetPlant(PlantId);
                        return _Plant;
                    }
                    else
                    {
                        return new Plant();
                    }
                }
                else
                {
                    return _Plant;
                }
            }
            set
            {
                _Plant = value;
            }
        }

        private Project _Project;
        public Project Project
        {
            get
            {
                if (_Project == null)
                {
                    if (this.ProjectView.ProjectId != 0)
                    {
                        _Project = SIDAL.GetProject(this.ProjectView.ProjectId);
                        return _Project;
                    }
                    else
                    {
                        return new Project();
                    }
                }
                else
                {
                    return _Project;
                }
            }
            set
            {
                _Project = value;
            }
        }

        public Guid UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                this._UserId = value;
                if (CustomerView != null)
                {
                    CustomerView.UserId = value;
                }
                if (ProjectView != null)
                {
                    ProjectView.UserId = value;
                }
            }
        }

        public Tuple<string, decimal> FindFskPriceContent(long mixId, long? fskPriceId)
        {
            if (fskPriceId != null && this.PlantId != null)
            {
                QuotationMix mix = SIDAL.FindQuotationMix(mixId);
                if (mix.StandardMixId != null)
                {
                    SIMixIngredientPriceSheet sheet = new SIMixIngredientPriceSheet(mix.StandardMixId.GetValueOrDefault(), this.PlantId.GetValueOrDefault());
                    FskCalculation calculation = new FskCalculation(sheet, fskPriceId.GetValueOrDefault(), mix.Quotation.FskBasePrice.GetValueOrDefault(0));
                    return new Tuple<string, decimal>(calculation.Content, calculation.FinalPrice);
                }
                else
                    return new Tuple<string, decimal>("", 0);
            }
            return new Tuple<string, decimal>("", 0);
        }

        public string PricingMonth
        {
            get
            {
                return PricingMonthActual.ToString("MMM, yyyy");
            }
            set
            {
                PricingMonthActual = DateUtils.GetDateFromMonthString(value, "01");
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

        public DateTime PricingMonthActual;
    }
}
