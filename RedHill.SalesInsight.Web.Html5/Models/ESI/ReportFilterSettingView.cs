using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class ReportFilterSettingView
    {
        //public long ReportFilterId { get; set; }
        //public long ReportId { get; set; }
        //public string EntityType { get; set; }
        //public long EntityRefId { get; set; }
        //public DateTime CreatedAt { get; set; }
        public List<long> Regions { get; set; }
        public List<long> Districts { get; set; }
        public List<long> Plants { get; set; }
        public List<long> MarketSegments { get; set; }
        public List<long> Customers { get; set; }
        public List<long> SalesStaffs { get; set; }
        public List<SelectListItem> RegionList { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public List<SelectListItem> PlantList { get; set; }
        public List<SelectListItem> MarketSegmentList { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        public List<SelectListItem> SalesStaffList { get; set; }
        public ReportFilterSettingView()
        {
        }

        public ReportFilterSettingView(ReportFilterSettingView reportFilters, Guid userId)
        {
            Regions = new List<long>();
            Districts = new List<long>();
            Plants = new List<long>();
            MarketSegments = new List<long>();
            Customers = new List<long>();
            SalesStaffs = new List<long>();
            if (reportFilters != null)
            {
                if (reportFilters.Regions != null)
                {
                    this.Regions = reportFilters.Regions;
                }
                if (reportFilters.Districts != null)
                {
                    this.Districts = reportFilters.Districts;
                }
                if (reportFilters.Plants != null)
                {
                    this.Plants = reportFilters.Plants;
                }
                if (reportFilters.MarketSegments != null)
                {
                    this.MarketSegments = reportFilters.MarketSegments;
                }
                if (reportFilters.Customers != null)
                {
                    this.Customers = reportFilters.Customers;
                }
                if (reportFilters.SalesStaffs != null)
                {
                    this.SalesStaffs = reportFilters.SalesStaffs;
                }
            }
            FillSelectItems(userId);
        }

        public ReportFilterSettingView(Guid userId, long reportId)
        {
            Regions = new List<long>();
            Districts = new List<long>();
            Plants = new List<long>();
            MarketSegments = new List<long>();
            Customers = new List<long>();
            SalesStaffs = new List<long>();


            var reportFilterList = SIDAL.GetReportFilterSetting(reportId);

            if (reportFilterList != null)
            {
                foreach (var item in reportFilterList)
                {

                    if (item.EntityType == "Region")
                    {
                        Regions.Add(item.EntityRefId);
                    }
                    else if (item.EntityType == "District")
                    {
                        Districts.Add(Convert.ToInt32(item.EntityRefId));
                    }
                    else if (item.EntityType == "Plant")
                    {
                        Plants.Add(item.EntityRefId);
                    }
                    else if (item.EntityType == "Market")
                    {
                        MarketSegments.Add(item.EntityRefId);
                    }
                    else if (item.EntityType == "Customer")
                    {
                        Customers.Add(item.EntityRefId);
                    }
                    else if (item.EntityType == "SalesStaff")
                    {
                        SalesStaffs.Add(item.EntityRefId);
                    }

                }
            }

            FillSelectItems(userId);
        }

        public void FillSelectItems(Guid userId)
        {
            SIUser user = SIDAL.GetUser(userId.ToString());
            int CompanyId = user.Company.CompanyId;

            DistrictList = new List<SelectListItem>();
            List<int> districtIds = new List<int>();

            IEnumerable<District> districtDetailedList = SIDAL.GetDistricts(userId).OrderBy(x => x.Name);

            RegionList = new List<SelectListItem>();
            foreach (Region region in SIDAL.GetRegionByDistrict(districtDetailedList))
            {
                SelectListItem item = new SelectListItem();
                item.Text = region.Name;
                item.Value = region.RegionId.ToString();
                item.Selected = Regions.Contains(region.RegionId);
                this.RegionList.Add(item);
            }
            List<int> selectedDistrict = new List<int>();
            if (Regions.Count > 0)
            {
                foreach (District district in districtDetailedList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = district.Name;
                    item.Value = district.DistrictId.ToString();
                    if (Districts.Contains(district.DistrictId))
                    {
                        item.Selected = true;
                        selectedDistrict.Add(Convert.ToInt32(district.DistrictId));
                    }
                    this.DistrictList.Add(item);
                    districtIds.Add(Convert.ToInt32(district.DistrictId));
                }

                if (selectedDistrict.Count > 0)
                {

                    PlantList = new List<SelectListItem>();
                    foreach (Plant s in SIDAL.GetPlantsForDistricts(selectedDistrict.ToArray()))
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = s.Name;
                        item.Value = s.PlantId.ToString();
                        item.Selected = Plants.Contains(s.PlantId);
                        PlantList.Add(item);
                    }

                    MarketSegmentList = new List<SelectListItem>();

                    foreach (MarketSegment marketSegment in SIDAL.GetMarketSegmentsForDistricts(selectedDistrict.ToArray()).ToList().OrderBy(x => x.Name))
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = marketSegment.Name;
                        item.Value = marketSegment.MarketSegmentId.ToString();
                        item.Selected = MarketSegments.Contains(marketSegment.MarketSegmentId);
                        MarketSegmentList.Add(item);
                    }

                    CustomerList = new List<SelectListItem>();
                    foreach (Customer cust in SIDAL.GetCustomersForDistricts(selectedDistrict.Select(i => i.ToString()).ToArray()))
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = cust.Name;
                        item.Value = cust.CustomerId.ToString();
                        item.Selected = Customers.Contains(cust.CustomerId);
                        CustomerList.Add(item);
                    }

                    SalesStaffList = new List<SelectListItem>();
                    foreach (SalesStaff s in SIDAL.GetSalesStaffForDistricts(selectedDistrict.Select(i => i.ToString()).ToArray()))
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = s.Name;
                        item.Value = s.SalesStaffId.ToString();
                        item.Selected = SalesStaffs.Contains(s.SalesStaffId);
                        SalesStaffList.Add(item);
                    }
                }
            }
        }
    }
}