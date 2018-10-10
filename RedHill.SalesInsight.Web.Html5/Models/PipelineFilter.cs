using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Web.Html5.Helpers;
using RedHill.SalesInsight.Web.Silverlight.Code.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class PipelineFilter
    {
        public string[] SortColumns { get; set; }
        public string SearchTerm { get; set; }
        public int CurrentStart { get; set; }
        public int[] Districts { get; set; }
        public int[] Statuses { get; set; }
        public int[] Plants { get; set; }
        public int[] Staffs { get; set; }
        public string ProjectionMonth { get; set; }
        public bool ShowInactives { get; set; }
        public int RowsPerPage { get; set; }
        public string DoPrint { get; set; }
        public int? DeletePipelineId { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<SelectListItem> PlantList { get; set; }
        public List<SelectListItem> StaffList { get; set; }

        public List<SelectListItem> ProductTypes { get; set; }

        public ProductType ProductTypeId { get; set; }

        [Display(Name = "Product Type Value")]
        public int ProductTypeValue
        {
            get
            {
                return (int)this.ProductTypeId;
            }
        }

        [Display(Name = "Product Type Id")]
        public ProductType? SelectProductTypeId { get; set; }

        public string PrintURL { get; set; }
        public string ParentPage { get; set; }


        public void Reset()
        {
            Districts = new int[] { };
            Statuses = new int[] { };
            Plants = new int[] { };
            Staffs = new int[] { };
        }

        public PipelineFilter()
        {
            SortColumns = new string[] { };
            Districts = new int[] { };
            Statuses = new int[] { };
            Plants = new int[] { };
            Staffs = new int[] { };
            RowsPerPage = 10;
            CurrentStart = 0;
            SearchTerm = "";
        }

        public PipelineFilter(Guid userId)
        {
            SortColumns = new string[] { };
            Districts = new int[] { };
            Statuses = new int[] { };
            Plants = new int[] { };
            Staffs = new int[] { };
            CurrentStart = 0;
            RowsPerPage = 10;
            SearchTerm = "";

            FillSelectItems(userId);
        }

        public void FillSelectItems(Guid userId)
        {

            SIUser user = SIDAL.GetUser(userId.ToString());
            int CompanyId = user.Company.CompanyId;

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

            StatusList = new List<SelectListItem>();
            foreach (ProjectStatus s in SIDAL.GetStatuses(CompanyId, null, 0, 1000, false))
            {
                SelectListItem item = new SelectListItem();
                item.Text = s.Name;
                item.Value = s.ProjectStatusId.ToString();
                item.Selected = Statuses.Contains(s.ProjectStatusId);
                StatusList.Add(item);
            }

            ProductTypes = new List<SelectListItem>();
            ProductTypes.Add(new SelectListItem { Value = ProductType.Concrete.ToString(), Text = ProductType.Concrete.ToString() });

            if (ConfigurationHelper.AggregateEnabled)
            {
                ProductTypes.Add(new SelectListItem { Value = ProductType.Aggregate.ToString(), Text = ProductType.Aggregate.ToString() });
            }

            if (ConfigurationHelper.BlockEnabled)
            {
                ProductTypes.Add(new SelectListItem { Value = ProductType.Block.ToString(), Text = ProductType.Block.ToString() });
            }

            string urlParams = SISalesInsightReports.GetPrintReportParams(userId, SortColumns, SearchTerm, Districts, Statuses, Plants, Staffs);
            string path = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("ReportServer");
            PrintURL = path + ParentPage + ".aspx?" + urlParams;
        }

        public DateTime? ProjectionDateTime
        {
            get
            {
                if (ProjectionMonth != null && !ProjectionMonth.Trim().Equals(""))
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