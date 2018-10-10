using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class CompanyView
    {
        public string SelectedTab { get; set; }
        public SIUser User { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Licence { get; set; }

        public bool ShowInactiveCustomers { get; set; }
        public bool ShowInactiveCompetitors { get; set; }
        public bool ShowInactiveSalesStaff { get; set; }
        public bool ShowInactiveContractors { get; set; }
        public bool ShowInactiveMarketSegments { get; set; }
        public bool ShowInactiveStatuses { get; set; }
        public bool ShowInactiveStructures { get; set; }
        
        public List<CustomerView> Customers { get; set; }
        public List<CompetitorView> Competitors { get; set; }
        public List<SalesStaffView> SalesStaff { get; set; }
        public List<ContractorView> Contractors { get; set; }
        public List<MarketSegmentView> MarketSegments { get; set; }
        public List<StatusView> Statuses { get; set; }
        public List<RegionView> Regions { get; set; }
        public List<DistrictView> Districts { get; set; }
        public List<PlantView> Plants { get; set; }
        public List<ReasonLossView> ReasonsForLoss { get; set; }

        public SelectList AvailableCompanies { get; set; }
        

        public CompanyView()
        {

        }

        public CompanyView(Company company,SIUser User,string selectedTab)
        {
            this.CompanyId = company.CompanyId;
            this.Name = company.Name;
            this.Licence = company.LicenseKey;
            if (selectedTab == null)
                SelectedTab = "customers";
            else
                SelectedTab = selectedTab;
            this.User = User;
            BindValues();
        }

        public string PageName
        {
            get
            {
                if (SelectedTab == "customers")
                    return "Customers";
                if (SelectedTab == "competitors")
                    return "Competitors";
                if (SelectedTab == "contractors")
                    return "GCs";
                if (SelectedTab == "salesstaff")
                    return "Sales Staff";
                if (SelectedTab == "marketsegments")
                    return "Market Segments";
                if (SelectedTab == "statuses")
                    return "Statuses & Reasons Lost";
                if (SelectedTab == "structure")
                    return "Structure";
                return "Setup";
            }
        }

        public void BindValues()
        {
            this.Customers = new List<CustomerView>();
            if (SelectedTab == "customers")
            {
                foreach (Customer c in SIDAL.GetCustomers(this.CompanyId, null, 0, 10000, ShowInactiveCustomers))
                {
                    this.Customers.Add(new CustomerView(c));
                }
            }

            this.Competitors = new List<CompetitorView>();
            if (SelectedTab == "competitors")
            {
                foreach (Competitor c in SIDAL.GetCompetitors(this.CompanyId, null, 0, 1000, ShowInactiveCompetitors))
                {
                    this.Competitors.Add(new CompetitorView(c,User));
                }
            }

            this.SalesStaff = new List<SalesStaffView>();
            if (SelectedTab == "salesstaff")
            {
                foreach (SISalesStaff s in SIDAL.GetSalesStaff(this.CompanyId, null, 0, 1000, ShowInactiveSalesStaff))
                {
                    this.SalesStaff.Add(new SalesStaffView(s));
                }
            }

            this.Contractors = new List<ContractorView>();
            if (SelectedTab == "contractors")
            {
                foreach (Contractor c in SIDAL.GetContractors(this.CompanyId, null, 0, 10000, ShowInactiveContractors))
                {
                    this.Contractors.Add(new ContractorView(c));
                }
            }

            this.MarketSegments = new List<MarketSegmentView>();
            if (SelectedTab == "marketsegments")
            {
                foreach (MarketSegment m in SIDAL.GetMarketSegments(this.CompanyId, null, 0, 1000, ShowInactiveMarketSegments))
                {
                    this.MarketSegments.Add(new MarketSegmentView(m));
                }
            }

            this.Statuses = new List<StatusView>();
            this.ReasonsForLoss = new List<ReasonLossView>();
            if (SelectedTab == "statuses")
            {
                
                foreach (ProjectStatus s in SIDAL.GetStatuses(this.CompanyId, null, 0, 1000, ShowInactiveStatuses))
                {
                    this.Statuses.Add(new StatusView(s));
                }
            
                foreach (ReasonsForLoss s in SIDAL.GetReasonsForLoss(this.CompanyId, null, 0, 1000,ShowInactiveStatuses))
                {
                    this.ReasonsForLoss.Add(new ReasonLossView(s));
                }
            }

            this.Regions = new List<RegionView>();
            this.Districts = new List<DistrictView>();
            this.Plants = new List<PlantView>();

            if (SelectedTab == "structure")
            {
                
                foreach (Region r in SIDAL.GetRegions(this.CompanyId, null, 0, 1000, ShowInactiveStructures))
                {
                    this.Regions.Add(new RegionView(r));
                }

                foreach (District r in SIDAL.GetDistricts(this.CompanyId, null, 0, 1000, ShowInactiveStructures))
                {
                    this.Districts.Add(new DistrictView(r));
                }

                foreach (Plant r in SIDAL.GetPlants(this.User.UserId, ShowInactiveStructures)) 
                {
                    this.Plants.Add(new PlantView(r));
                }
            }

            this.AvailableCompanies = new SelectList(SIDAL.GetCompanies(),"CompanyId","Name",CompanyId);
        }

    }
}