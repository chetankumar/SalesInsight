using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class StatusView
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "The Project Status field is required")]
        public int ProjectStatusId { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Status Type")]
        public short StatusType { get; set; }

        [Display(Name = "Include On Forecast Page")]
        public bool IncludeOnForecastPage { get; set; }

        [Display(Name = "Treat As Inactive for Pipeline Page")]
        public bool TreatAsInactiveForPipelinePage { get; set; }

        public string StatusTypeName { get; set; }

        public string DispatchId { get; set; }

        public List<SelectListItem> StatusTypes { get; set; }

        public StatusView()
        {
            this.Name = "Default Name";
        }

        public StatusView(ProjectStatus status)
        {
            this.ProjectStatusId = status.ProjectStatusId;
            this.Name = status.Name;
            this.CompanyId = status.CompanyId;
            this.StatusType = status.StatusType;
            this.StatusTypeName = SIStatusType.StatusTypes.Where(st => st.Id == status.StatusType).First().Name;
            this.Active = status.Active;
            this.DispatchId = status.DispatchId;
            this.IncludeOnForecastPage = status.IncludeOnForecastPage.GetValueOrDefault();
            this.TreatAsInactiveForPipelinePage = status.TreatAsInactiveForPipelinePage.GetValueOrDefault();

            BindValues();
        }

        public void BindValues()
        {
            this.CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
            StatusTypes = new List<SelectListItem>();

            foreach (SIStatusType type in SIStatusType.StatusTypes)
            {
                SelectListItem item = new SelectListItem();
                item.Text = type.Name;
                item.Value = type.Id.ToString();
                item.Selected = StatusType == type.Id;
                StatusTypes.Add(item);
            }
        }

    }
}
