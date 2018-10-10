using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ReportFilter
    {
        public bool FilterByBidDate { get; set; }
        public DateTime? BidDateBegin { get; set; }
        public DateTime? BidDateEnd { get; set; }

        public bool FilterByStartDate { get; set; }
        public DateTime? StartDateBegin { get; set; }
        public DateTime? StartDateEnd { get; set; }

        public bool FilterByWLDate { get; set; }
        public DateTime? WLDateBegin { get; set; }
        public DateTime? WLDateEnd { get; set; }

        public int[] Regions { get; set; }
        public int[] Districts { get; set; }
        public int[] Plants { get; set; }
        public int[] Staffs { get; set; }

        public String Print { get; set; }

        public string ProjectionMonth { get; set; }
        public bool FilterBacklogLimit { get; set; }

        public ProductType? SelectProductTypeId { get; set; }

        public int ProductTypeValue
        {
            get
            {
                return (int)this.SelectProductTypeId.GetValueOrDefault();
            }
        }

        public List<SelectListItem> RegionList { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public List<SelectListItem> PlantList { get; set; }
        public List<SelectListItem> StaffList { get; set; }

        public ReportFilter()
        {
        }

        public ReportFilter(Guid userId)
        {
            //DateTime endDate = DateTime.Now;
            //DateTime beginDate = endDate.AddMonths(-7);

            //BidDateBegin = beginDate;
            //StartDateBegin = beginDate;
            //WLDateBegin = beginDate;

            //BidDateEnd = endDate;
            //StartDateEnd = endDate;
            //WLDateEnd = endDate;

            FilterByBidDate = true;
            FilterByStartDate = false;
            FilterByWLDate = false;
            FilterBacklogLimit = false;

            FillSelectItems(userId);
        }

        public void FillSelectItems(Guid userId)
        {

            SIUser user = SIDAL.GetUser(userId.ToString());
            int CompanyId = user.Company.CompanyId;

            if (Districts == null)
                Districts = new int[] { };

            if (Regions == null)
                Regions = new int[] { };

            if (Plants == null)
                Plants = new int[] { };

            if (Staffs == null)
                Staffs = new int[] { };

            DistrictList = new List<SelectListItem>();
            foreach (District district in SIDAL.GetDistricts(userId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = district.Name;
                item.Value = district.DistrictId.ToString();
                item.Selected = Districts.Contains(district.DistrictId);
                DistrictList.Add(item);
            }

            StaffList = new List<SelectListItem>();
            foreach (SISalesStaff s in SIDAL.GetSalesStaff(CompanyId, null, 0, 1000, false))
            {
                SelectListItem item = new SelectListItem();
                item.Text = s.SalesStaff.Name;
                item.Value = s.SalesStaff.SalesStaffId.ToString();
                item.Selected = Staffs.Contains(s.SalesStaff.SalesStaffId);
                StaffList.Add(item);
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

            RegionList = new List<SelectListItem>();
            foreach (Region s in SIDAL.GetRegions(CompanyId, null, 0, 1000, false))
            {
                SelectListItem item = new SelectListItem();
                item.Text = s.Name;
                item.Value = s.RegionId.ToString();
                item.Selected = Regions.Contains(s.RegionId);
                RegionList.Add(item);
            }
        }

        public DateTime? ProjectionDateTime
        {
            get
            {
                if (ProjectionMonth != null || ProjectionMonth.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact("04 " + ProjectionMonth, "dd MMM, yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return DateTime.Now;
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
                    ProjectionMonth = value.Value.ToString("MMM, yyyy");
            }
        }
    }
}