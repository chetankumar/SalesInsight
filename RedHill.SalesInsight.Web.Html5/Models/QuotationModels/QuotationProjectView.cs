using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.Web.Html5.Models;
using RedHill.SalesInsight.DAL;
using System.Web.Security;
using RedHill.SalesInsight.DAL.DataTypes;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationProjectView
    {
        public Guid UserId {get;set;}
        public long QuotationId { get; set; }
        public int ProjectId { get; set; }
        public ProjectView ProjectView { get; set; }

        [Required(ErrorMessage = "The Sales Staff field is required.")]
        public int SalesStaffId { get; set; }

        public QuotationProjectView()
        {
            ProjectView = new ProjectView();
            ProjectView.Active = true;
        }

        public QuotationProjectView(long id):this(SIDAL.FindQuotation(id))
        {

        }

        public QuotationProjectView(Quotation quotation)
        {
            this.ProjectId = quotation.ProjectId.GetValueOrDefault();
            if (this.ProjectId > 0)
                this.ProjectView = new ProjectView(quotation.Project);
            else
            {
                this.ProjectView = new ProjectView();
                this.ProjectView.Active = true;
            }
            try
            {
                if (quotation.SalesStaffId.GetValueOrDefault() > 0)
                {
                    this.SalesStaffId = quotation.SalesStaffId.GetValueOrDefault();
                }
                else
                {
                    this.SalesStaffId = SIDAL.FindSalesStaffForProject(quotation.Id).SalesStaffId;
                }
            }
            catch (Exception) { }
        }

        public void Load()
        {
            Quotation quotation = SIDAL.FindQuotation(QuotationId);
        }


        public SelectList ChooseSalesStaff
        {
            get
            {
                return new SelectList(SIDAL.GetSalesStaffForDistricts(UserId), "SalesStaffId", "Name", this.SalesStaffId);
            }
        }

        public SelectList ChooseMarketSegment
        {
            get
            {
                int[] districts = SIDAL.GetDistricts(UserId).Select(x => x.DistrictId).ToArray();
                return new SelectList(SIDAL.GetMarketSegmentsForDistricts(districts), "MarketSegmentId", "Name", "");
            }
        }

        public SelectList ChooseProjectStatuses
        {
            get
            {
                return new SelectList(SIDAL.GetStatuses(0,null,0,1000,false), "ProjectStatusId", "Name", ProjectView.ProjectStatusId);
            }
        }

        
    }
}