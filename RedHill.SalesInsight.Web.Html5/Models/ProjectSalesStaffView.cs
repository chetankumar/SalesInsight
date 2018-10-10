using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectSalesStaffView
    {
        
        public int ProjectId { get; set; }
        public int ProjectSalesStaffId { get; set; }

        [Display(Name="Sales Staff")]
        [Required]
        public int SalesStaffId { get; set; }
        public string Name { get; set; }

        public SelectList AvailableSalesStaff { get; set; }

        public ProjectSalesStaffView(DAL.DataTypes.SIViewProjectSalesStaffDetails staff)
        {
            if (staff != null)
            {
                this.ProjectId = staff.ProjectSalesStaff.ProjectId;
                this.ProjectSalesStaffId = staff.ProjectSalesStaff.ProjectSalesStaffId;
                this.SalesStaffId = staff.SalesStaff.SalesStaffId;
                this.Name = staff.SalesStaff.Name;
            }
        }

        public ProjectSalesStaffView()
        {
        }

        public ProjectSalesStaffView(DAL.ProjectSalesStaff staff)
        {
            this.ProjectId = staff.ProjectId;
            this.ProjectSalesStaffId = staff.ProjectSalesStaffId;
            this.SalesStaffId = staff.SalesStaffId;
        }

    }
}