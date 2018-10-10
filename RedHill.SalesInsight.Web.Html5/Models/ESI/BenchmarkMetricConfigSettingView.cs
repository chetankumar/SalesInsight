﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class BenchmarkMetricConfigSettingView
    {
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
        public BenchmarkMetricConfigSettingView()
        {
        }
        public BenchmarkMetricConfigSettingView(Guid userId, long reportId)
        {
            Regions = new List<long>();
            Districts = new List<long>();
            Plants = new List<long>();
            MarketSegments = new List<long>();
            Customers = new List<long>();
            SalesStaffs = new List<long>();

            var reportFilterList = SIDAL.GetReportColumnConfig(reportId);
            if (reportFilterList != null)
            {
                foreach (var item in reportFilterList)
                {
                    if (item.EntityName == "Region")
                    {
                        Regions.Add(item.EntityRefId.GetValueOrDefault());
                    }
                    else if (item.EntityName == "District")
                    {
                        Districts.Add(Convert.ToInt32(item.EntityRefId));
                    }
                    else if (item.EntityName == "Plant")
                    {
                        Plants.Add(item.EntityRefId.GetValueOrDefault());
                    }
                    else if (item.EntityName == "MarketSegment")
                    {
                        MarketSegments.Add(item.EntityRefId.GetValueOrDefault());
                    }
                    else if (item.EntityName == "Customer")
                    {
                        Customers.Add(item.EntityRefId.GetValueOrDefault());
                    }
                    else if (item.EntityName == "SalesStaff")
                    {
                        SalesStaffs.Add(item.EntityRefId.GetValueOrDefault());
                    }
                }
            }
            FillSelectItems(userId);
        }

        public void FillSelectItems(Guid userId)
        {
            SIUser user = SIDAL.GetUser(userId.ToString());
            int CompanyId = user.Company.CompanyId;

            RegionList = new List<SelectListItem>();
            foreach (Region region in SIDAL.GetRegions(CompanyId, null, 0, 1000, false))
            {
                SelectListItem item = new SelectListItem();
                item.Text = region.Name;
                item.Value = region.RegionId.ToString();
                item.Selected = Regions.Contains(region.RegionId);
                this.RegionList.Add(item);
            }

            DistrictList = new List<SelectListItem>();
            List<int> districtIds = new List<int>();

            foreach (District district in SIDAL.GetDistricts(userId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = district.Name;
                item.Value = district.DistrictId.ToString();
                item.Selected = Districts.Contains(district.DistrictId);
                this.DistrictList.Add(item);
                districtIds.Add(Convert.ToInt32(district.DistrictId));
            }

            PlantList = new List<SelectListItem>();
            foreach (Plant s in SIDAL.GetPlants(CompanyId, null, 0, 1000, false))
            {
                SelectListItem item = new SelectListItem();
                item.Text = s.Name;
                item.Value = s.PlantId.ToString();
                item.Selected = Plants.Contains(s.PlantId);
                PlantList.Add(item);
            }

            MarketSegmentList = new List<SelectListItem>();

            foreach (MarketSegment marketSegment in SIDAL.GetMarketSegmentsForDistricts(districtIds.ToArray()).ToList())
            {
                SelectListItem item = new SelectListItem();
                item.Text = marketSegment.Name;
                item.Value = marketSegment.MarketSegmentId.ToString();
                item.Selected = MarketSegments.Contains(marketSegment.MarketSegmentId);
                MarketSegmentList.Add(item);
            }

            CustomerList = new List<SelectListItem>();
            foreach (Customer cust in SIDAL.GetCustomers(userId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = cust.Name;
                item.Value = cust.CustomerId.ToString();
                item.Selected = Customers.Contains(cust.CustomerId);
                CustomerList.Add(item);
            }

            SalesStaffList = new List<SelectListItem>();
            foreach (SISalesStaff s in SIDAL.GetSalesStaff(CompanyId, null, 0, 1000, false))
            {
                SelectListItem item = new SelectListItem();
                item.Text = s.SalesStaff.Name;
                item.Value = s.SalesStaff.SalesStaffId.ToString();
                item.Selected = SalesStaffs.Contains(s.SalesStaff.SalesStaffId);
                SalesStaffList.Add(item);
            }
        }

    }
}