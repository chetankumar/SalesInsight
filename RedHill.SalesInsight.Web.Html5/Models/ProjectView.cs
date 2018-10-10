using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectView
    {
        public SIUser User { get; set; }

        #region Project Basic Fields
        public int ProjectId { get; set; }

        [Display(Name = "Plant")]
        public int PlantId { get; set; }

        [Display(Name = "Market Segment")]
        public int MarketSegmentId { get; set; }

        [Display(Name = "Sales Staff")]
        public int SalesStaffId { get; set; }
        public int ContractorId { get; set; }
        public int CustomerId { get; set; }
        public long? BackupPlantId { get; set; }

        [Display(Name = "Project Status")]
        public int ProjectStatusId { get; set; }
        public int? CompetitorId { get; set; }
        public Guid UserId { get; set; }
        public bool DisablePriceSpreadProfit { get; set; }

        [Display(Name = "Project Name")]
        [Required]
        public string ProjectName { get; set; }

        [Display(Name = "Customer Ref Name")]
        public string CustomerRefName { get; set; }

        [Display(Name = "Project Ref Id")]
        public string ProjectRefId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Address")]
        [CustomRequiredValidator]
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "ZipCode")]
        public string Zipcode { get; set; }

        public bool DistrictQcRequirement { get; set; }
        public bool IsAwardedQuoteExist { get; set; }
        public List<ProjectCompetitorView> Competitors { get; set; }
        public List<ProjectBidderView> Bidders { get; set; }
        public List<QuotationProfile> Quotations { get; set; }
        public List<ProjectPlantView> ProjectPlants { get; set; }
        public List<ProjectNoteView> ProjectNotes { get; set; }
        public int? TotalQuotes { get; set; }
        [Display(Name = "To Job")]
        public int? ToJobMinutes { get; set; }

        [Display(Name = "Wash")]
        public int? WashMinutes { get; set; }

        [Display(Name = "Return")]
        public int? ReturnMinutes { get; set; }

        [Display(Name = "Unload")]
        public int? Unload { get; set; }

        [Display(Name = "Wait on Job")]
        public int? WaitOnJob { get; set; }

        [Display(Name = "Average Load Size")]
        public double? AverageLoadSize { get; set; }

        [Display(Name = "Average Load Size")]
        public double? DistanceToJob { get; set; }

        [Display(Name = "Delivery Instructions")]
        public string DeliveryInstructions { get; set; }

        [Display(Name = "Bid Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BidDate { get; set; }

        [Display(Name = "Valuation")]
        public int? Valuation { get; set; }

        [Display(Name = "Volume")]
        public int? Volume { get; set; }

        [Display(Name = "Mix")]
        public string Mix { get; set; }

        [Display(Name = "Price")]
        public decimal? Price { get; set; }

        [Display(Name = "Spread")]
        public decimal? Spread { get; set; }

        [Display(Name = "Profit")]
        public decimal? Profit { get; set; }

        [Display(Name = "Reason Lost")]
        public int? ReasonLossId { get; set; }
        public decimal? PriceLost { get; set; }
        public int? CompetitorLost { get; set; }
        public string LostNotes { get; set; }

        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        //public string DistrictName { get; set; }

        #endregion

        public int NewQuoteCustomer { get; set; }
        public long CopyFromQuoteId { get; set; }
        public int NewQuoteProject { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? WonLostDate { get; set; }

        public bool ExcludeFromReports { get; set; }

        public int ProjectCompetitorId { get; set; }
        public ProjectCompetitorView SelectedCompetitor { get; set; }
        public ProjectBidderView SelectedBidder { get; set; }
        public ProjectNoteView SelectedNote { get; set; }
        public ProjectPlantView SelectedPlant { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        public List<ProjectStatus> StatusList { get; set; }
        public List<Plant> PlantList { get; set; }
        public List<MarketSegment> MarketList { get; set; }
        public List<Customer> CustomerList { get; set; }
        public List<Project> ProjectList { get; set; }
        public List<Contractor> ContractorList { get; set; }
        public List<QC_Requirement> QCRequirementList { get; set; }
        public List<long> ProjectQCRequirementList { get; set; }
        public List<Plant> BackupPlantList { get; set; }
        public List<Competitor> CompetitorList { get; set; }
        public List<ReasonsForLoss> ReasonsForLossList { get; set; }
        public List<SalesStaff> SalesStaffs { get; set; }
        public long? AwardedQuotationId { get; set; }
        public List<long> AwardedQuotes { get; set; }
        // References
        public List<int> LostBidsStatusIds { get; set; }
        public List<int> WonStatusIds { get; set; }

        public bool CompanyEnableAggregate { get; set; }
        public bool CompanyEnableBlock { get; set; }

        public bool LocationSet
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Latitude) || !string.IsNullOrWhiteSpace(this.Longitude);
            }
        }

        public List<SelectListItem> QuotationList { get; set; }
        public List<SelectListItem> QCRequirement { get; set; }
        public List<SelectListItem> BackupPlant { get; set; }

        public ProjectView()
        {
            SelectedNote = new ProjectNoteView();
            SelectedPlant = new ProjectPlantView();
            SelectedCompetitor = new ProjectCompetitorView();
            SelectedBidder = new ProjectBidderView();
        }

        public ProjectView(SIViewProjectDetails projectDetails)
        {
            this.ProjectId = projectDetails.Project.ProjectId;
            //this.DistrictName = SIDAL.GetProjectDistrictName(this.ProjectId);
            this.NewQuoteProject = this.ProjectId;

            this.ProjectName = projectDetails.Project.Name;
            this.CustomerRefName = projectDetails.Project.CustomerRefName;
            this.ProjectRefId = projectDetails.Project.ProjectRefId;
            this.BackupPlantId = projectDetails.Project.BackupPlantId;
            if (projectDetails.Project.ProjectStatus != null)
            {
                this.ProjectStatusId = projectDetails.Project.ProjectStatus.ProjectStatusId;
            }
            if (projectDetails.Project.MarketSegment != null)
                this.MarketSegmentId = projectDetails.Project.MarketSegment.MarketSegmentId;

            if (projectDetails.Project.Contractor != null)
                this.ContractorId = projectDetails.Project.Contractor.ContractorId;
            this.CompetitorId = projectDetails.Project.WinningCompetitorId.GetValueOrDefault();

            if (projectDetails.ProjectSalesStaffDetails != null && projectDetails.ProjectSalesStaffDetails.Count > 0)
                this.SalesStaffId = projectDetails.ProjectSalesStaffDetails.First().SalesStaff.SalesStaffId;

            if (projectDetails.Project.Plant != null)
                this.PlantId = projectDetails.Project.Plant.PlantId;

            if (projectDetails.Project.Customer != null)
                this.CustomerId = projectDetails.Project.Customer.CustomerId;

            this.StartDate = projectDetails.Project.StartDate;
            this.City = projectDetails.Project.City;
            this.Address = projectDetails.Project.Address;
            this.Latitude = projectDetails.Project.Latitude;
            this.Longitude = projectDetails.Project.Longitude;
            this.State = projectDetails.Project.State;
            this.Zipcode = projectDetails.Project.ZipCode;
            this.ExcludeFromReports = projectDetails.Project.ExcludeFromReports;
            this.ToJobMinutes = projectDetails.Project.ToJobMinutes;
            this.WashMinutes = projectDetails.Project.WashMinutes;
            this.ReturnMinutes = projectDetails.Project.ReturnMinutes;
            this.Unload = projectDetails.Project.Unload;
            this.WaitOnJob = projectDetails.Project.WaitOnJob;
            this.DeliveryInstructions = projectDetails.Project.DeliveryInstructions;
            this.DistanceToJob = projectDetails.Project.DistanceToJob;
            this.BidDate = projectDetails.Project.BidDate;
            this.Valuation = projectDetails.Project.Valuation;
            this.Volume = projectDetails.Project.Volume;
            this.Mix = projectDetails.Project.Mix;
            this.Price = projectDetails.Project.Price;
            this.Spread = projectDetails.Project.Spread;
            this.Profit = projectDetails.Project.Profit;
            this.UDF1 = projectDetails.Project.UDF1;
            this.UDF2 = projectDetails.Project.UDF2;
            this.WonLostDate = projectDetails.Project.WonLostDate;
            if (projectDetails.Project.ReasonForLoss != null)
                this.ReasonLossId = projectDetails.Project.ReasonForLoss.ReasonLostId;
            this.PriceLost = projectDetails.Project.PriceLost;
            this.LostNotes = projectDetails.Project.LossNotes;
            this.CompetitorLost = projectDetails.Project.WinningCompetitorId.GetValueOrDefault(Int32.MinValue);
            this.Active = projectDetails.Project.Active;
            this.DistrictQcRequirement = projectDetails.Project.DistrictQcRequirement;
            this.IsAwardedQuoteExist = SIDAL.CheckAwardedQuoteExist(this.ProjectId);
            SelectedNote = new ProjectNoteView();
            SelectedPlant = new ProjectPlantView();
            SelectedCompetitor = new ProjectCompetitorView();
            SelectedBidder = new ProjectBidderView();
                    }

        public void LoadTables()
        {

            LostBidsStatusIds = SIDAL.GetStatusesByType(SIStatusType.LostBid).Select(p => p.ProjectStatusId).ToList();
            WonStatusIds = SIDAL.GetStatusesByType(SIStatusType.Sold).Select(p => p.ProjectStatusId).ToList();

            if (this.ProjectPlants != null)
            {
                foreach (var projPlant in this.ProjectPlants)
                {
                    projPlant.Plant = SIDAL.GetPlant(projPlant.PlantId);
                    //this.ProjectPlants.Add(new ProjectPlantView(pp));
                }
            }

            this.QCRequirement = new List<SelectListItem>();
            foreach (QC_Requirement qcReq in QCRequirementList)
            {
                this.QCRequirement.Add(new SelectListItem { Value = qcReq.Id.ToString(), Text = qcReq.Name, Selected = ProjectQCRequirementList.Contains(qcReq.Id) });
            }

            if (ProjectId <= 0)
                return;

            SIViewProjectDetails projectDetails = SIDAL.GetProjectDetails(UserId, ProjectId);
            this.AwardedQuotes = new List<long>();
            this.Quotations = new List<QuotationProfile>();
            this.QuotationList = new List<SelectListItem>();
            QuotationProfile qProfile = null;
            foreach (Quotation q in SIDAL.GetQuotationsForProject(this.ProjectId))
            {
                qProfile = new QuotationProfile(q);
                this.Quotations.Add(qProfile);

                this.QuotationList.Add(new SelectListItem { Value = q.Id.ToString(), Text = qProfile.Description, Selected = q.Awarded.GetValueOrDefault() });
                if (q.Awarded.GetValueOrDefault())
                {
                    //TODO:Multiple Awarded Quotes - Project view
                    this.AwardedQuotationId = q.Id;
                    this.AwardedQuotes.Add(q.Id);
                }
            }

 

            this.DisablePriceSpreadProfit = this.Quotations.Where(x => x.Active == true).Count() > 0;
            this.TotalQuotes = this.Quotations.Count();
            if (projectDetails.ProjectCompetitorDetails != null)
            {
                this.Competitors = new List<ProjectCompetitorView>();
                foreach (SIViewProjectCompetitorDetails competitor in projectDetails.ProjectCompetitorDetails)
                {
                    this.Competitors.Add(new ProjectCompetitorView(competitor.ProjectCompetitor));
                }
            }


            if (projectDetails.ProjectBidderDetails != null)
            {
                this.Bidders = new List<ProjectBidderView>();
                foreach (SIViewProjectBidderDetails bidder in projectDetails.ProjectBidderDetails)
                {
                    this.Bidders.Add(new ProjectBidderView(bidder.ProjectBidder));
                }
            }

            if (this.ProjectPlants == null)
            {
                if (projectDetails.ProjectPlantDetails != null)
                {
                    this.ProjectPlants = new List<ProjectPlantView>();
                    foreach (SIViewProjectPlants pp in projectDetails.ProjectPlantDetails)
                    {
                        this.ProjectPlants.Add(new ProjectPlantView(pp));
                    }
                }
            }
            else
            {
                foreach (var projPlant in this.ProjectPlants)
                {
                    projPlant.Plant = SIDAL.GetPlant(projPlant.PlantId);
                    //this.ProjectPlants.Add(new ProjectPlantView(pp));
                }
            }

            if (projectDetails.ProjectNoteDetails != null)
            {
                this.ProjectNotes = new List<ProjectNoteView>();
                foreach (SIViewProjectNoteDetails note in projectDetails.ProjectNoteDetails)
                {
                    this.ProjectNotes.Add(new ProjectNoteView(note.ProjectNote, note.Username));
                }
            }
        }

        public ProjectView(Project projectDetails)
        {
            this.ProjectId = projectDetails.ProjectId;
            this.PlantId = projectDetails.ConcretePlantId.GetValueOrDefault();
            this.ProjectName = projectDetails.Name;
            this.CustomerRefName = projectDetails.CustomerRefName;
            this.ProjectRefId = projectDetails.ProjectRefId;
            this.Active = projectDetails.Active.GetValueOrDefault();
            this.ProjectStatusId = projectDetails.ProjectStatusId.GetValueOrDefault(0);
            if (projectDetails.ProjectSalesStaffs.Count > 0)
                this.SalesStaffId = projectDetails.ProjectSalesStaffs.First().SalesStaffId;
            this.ExcludeFromReports = projectDetails.ExcludeFromReports.GetValueOrDefault();

            this.StartDate = projectDetails.StartDate;
            this.BidDate = projectDetails.BidDate;
            this.WonLostDate = projectDetails.WonLostDate;

            this.City = projectDetails.City;
            this.Address = projectDetails.Address;
            this.Longitude = projectDetails.Longitude;
            this.Latitude = projectDetails.Latitude;
            this.State = projectDetails.State;
            this.Zipcode = projectDetails.ZipCode;

            this.DeliveryInstructions = projectDetails.DeliveryInstructions;
            this.MarketSegmentId = projectDetails.MarketSegmentId.GetValueOrDefault();

            Plant plant = SIDAL.GetPlant(projectDetails.ConcretePlantId);
            District district = SIDAL.GetDistrict(plant.DistrictId);
            //if (district != null)
            //{
            //    this.DistrictName = district.Name;
            //}
            if (projectDetails.ToJobMinutes != null)
                this.ToJobMinutes = projectDetails.ToJobMinutes;
            else
                this.ToJobMinutes = Convert.ToInt32(district.ToJob.GetValueOrDefault(0));

            if (projectDetails.WaitOnJob != null)
                this.WaitOnJob = projectDetails.ToJobMinutes;
            else
                this.WaitOnJob = Convert.ToInt32(plant.WaitMinutes.GetValueOrDefault(0));

            if (projectDetails.WashMinutes != null)
                this.WashMinutes = projectDetails.ToJobMinutes;
            else
                this.WashMinutes = Convert.ToInt32(district.Wash.GetValueOrDefault(0));

            if (projectDetails.ReturnMinutes != null)
                this.ReturnMinutes = projectDetails.ReturnMinutes;
            else
                this.ReturnMinutes = Convert.ToInt32(district.Return.GetValueOrDefault(0));

            this.ToJobMinutes = projectDetails.ToJobMinutes;
            this.WashMinutes = projectDetails.WashMinutes;
            this.ReturnMinutes = projectDetails.ReturnMinutes;
            this.Unload = projectDetails.Unload;
            this.WaitOnJob = projectDetails.WaitOnJob;
            this.AverageLoadSize = projectDetails.AverageLoadSize;
            this.DistanceToJob = projectDetails.DistanceToJob.GetValueOrDefault();

            this.Valuation = projectDetails.Valuation;
            this.Volume = projectDetails.Volume;
            this.Mix = projectDetails.Mix;
            this.Price = projectDetails.Price;
            this.Spread = projectDetails.Spread;
            this.Profit = projectDetails.Profit;
            this.TotalQuotes = 2;
            this.UDF1 = projectDetails.UDF1;
            this.UDF2 = projectDetails.UDF2;
        }

        public void LoadSelects(Guid userId)
        {
            this.UserId = userId;
            List<District> Districts = SIDAL.GetDistrictFilters(this.UserId);
            this.PlantList = SIDAL.GetPlantsForDistricts(Districts.Select(x => x.DistrictId).ToArray()).OrderBy(x => x.Name).ToList();
            this.MarketList = SIDAL.GetMarketSegmentsForDistricts(Districts.Select(x => x.DistrictId).ToArray()).OrderBy(x => x.Name).ToList();
            this.StatusList = SIDAL.GetStatuses(0, null, 0, 10000, false).OrderBy(x => x.Name).ToList();
            this.ContractorList = SIDAL.GetContractors(0, null, 0, 10000, false).OrderBy(x => x.Name).ToList();
            this.QCRequirementList = SIDAL.GetQCRequirementList();
            this.ProjectQCRequirementList = SIDAL.GetProjectQCRequirementList(this.ProjectId);
            this.BackupPlantList = PlantList;
            this.SalesStaffs = SIDAL.GetSalesStaffs(UserId).OrderBy(x => x.Name).ToList();
            //this.CustomerList = SIDAL.GetCustomers(UserId).OrderBy(x => x.Name).ToList();
            //this.ProjectList = SIDAL.GetProjects(UserId).OrderBy(x => x.Name).ToList();
            this.CompetitorList = SIDAL.GetCompetitors(UserId).OrderBy(x => x.Name).ToList();
            this.ReasonsForLossList = SIDAL.GetReasonsForLoss(0, null, 0, 1000, false).OrderBy(x => x.Reason).ToList();
            this.CompanyEnableAggregate = this.Company.EnableAggregate.GetValueOrDefault();
            this.CompanyEnableBlock = this.Company.EnableBlock.GetValueOrDefault();
        }

        public string UDF1Label
        {
            get
            {
                return SIDAL.GetGlobalSettings().JI1 ?? "UDF 1";
            }
        }

        public string UDF2Label
        {
            get
            {
                return SIDAL.GetGlobalSettings().JI2 ?? "UDF 2";
            }
        }

        public Company Company
        {
            get
            {
                return SIDAL.GetUser(UserId.ToString()).Company;
            }
        }

        public string PlantName
        {
            get
            {
                if (PlantId > 0)
                {
                    return SIDAL.GetPlant(PlantId).Name;
                }
                else
                {
                    return "";
                }
            }
        }

        public int ProjectStatusType
        {
            get
            {
                if (this.ProjectStatusId > 0)
                {
                    return SIDAL.GetProjectStatus(this.ProjectStatusId).StatusType;
                }
                else
                {
                    return -1;
                }
            }
        }

        public string AddressLine
        {
            get
            {
                return Address + "," + City + "," + State + "," + Zipcode;
            }
        }
    }
}