using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.Web.Html5.Models.QuotationModels;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationPageView
    {
        public List<QuotationProfile> Quotations { get; set; }
        public List<QuotationReportItem> QuoteReport { get; set; }

        public PipelineFilter Filter { get; set; }
        public int NumResults { get; set; }
        public int[] Districts { get; set; }
        public int[] Plants { get; set; }
        public int[] Staffs { get; set; }
        public Guid UserId { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }

        public QuotationPageView()
        {
            this.SortColumn = "Id";
            this.SortDirection = "desc";
        }

        public void LoadQuotation()
        {
            int count = 0;
            List<Quotation> quotations = SIDAL.GetQuotations(this.UserId, this.Districts, this.Plants, this.Staffs, Filter.ShowInactives,
                                                            Filter.SearchTerm, Filter.CurrentStart, Filter.RowsPerPage, this.SortColumn,
                                                            this.SortDirection, out count);
            this.NumResults = count;
            this.QuoteReport = new List<QuotationReportItem>();
            if (quotations != null)
            {
                foreach (Quotation q in quotations)
                {
                    this.QuoteReport.Add(new QuotationReportItem(q));
                }
            }
        }

        public void Load()
        {
            int count = 0;
            List<Quotation> quotations = SIDAL.GetQuotations(this.UserId,this.Districts,this.Plants,this.Staffs, Filter.ShowInactives,
                                                            Filter.SearchTerm, Filter.CurrentStart, Filter.RowsPerPage,this.SortColumn,
                                                            this.SortDirection, out count);
            this.NumResults = count;
            this.Quotations = new List<QuotationProfile>();
            if (quotations != null)
            {
                foreach (Quotation q in quotations)
                {
                    this.Quotations.Add(new QuotationProfile(q));
                }
            }
        }

        public SelectList ChooseProjects
        {
            get
            {
                return new SelectList(SIDAL.GetProjects(UserId).OrderBy(x => x.Name), "ProjectId", "Name", "");
            }
        }

        public SelectList ChooseCustomers
        {
            get
            {
                return new SelectList(SIDAL.GetCustomers(UserId), "CustomerId", "Name", "");
            }
        }
    }
}