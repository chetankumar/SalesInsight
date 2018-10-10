using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models.QuotationModels
{
    public class QuotationReportItem
    {
        public long QuoteId { get; set; }
        public string Status { get; set; }
        public bool Awarded { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? AcceptanceExpirationDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }
        public string MarketSegment { get; set; }
        public string PlantName { get; set; }
        public string SalesStaffName { get; set; }
        public double TotalVolume { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgSellingPrice { get; set; }
        public decimal Spread { get; set; }
        public decimal Profit { get; set; }
        public string BackupPlant { get; set; }
        public int ProjectId { get; set; }

        public long QuoteRefNumber
        {
            get
            {
                return QuoteId;
            }
        }
        public QuotationReportItem()
        {

        }

        public QuotationReportItem(Quotation quote)
        {
            this.ProjectId = quote.ProjectId.GetValueOrDefault();
            var backupPlantId = SIDAL.GetProject(this.ProjectId).BackupPlantId;
            this.QuoteId = quote.Id;
            this.Status = quote.Status;
            this.Awarded = quote.Awarded.GetValueOrDefault();
            this.ProjectName = quote.Project.Name;
            if (quote.Customer != null)
            {
                this.CustomerName = quote.Customer.Name;
            }
            this.QuoteDate = quote.QuoteDate;
            this.AcceptanceExpirationDate = quote.AcceptanceExpirationDate;
            this.QuoteExpirationDate = quote.QuoteExpirationDate;
            this.MarketSegment = SIDAL.GetMarketSegment(quote.Project.MarketSegmentId).Name;
            this.PlantName = quote.Plant.Name;
            this.SalesStaffName = quote.SalesStaff.Name;
            this.TotalVolume = quote.TotalVolume.GetValueOrDefault();
            this.TotalRevenue = quote.TotalRevenue.GetValueOrDefault();
            this.AvgSellingPrice = quote.AvgSellingPrice.GetValueOrDefault();
            this.Spread = quote.Spread.GetValueOrDefault();
            this.Profit = quote.Profit.GetValueOrDefault();
            this.BackupPlant = backupPlantId.GetValueOrDefault() != 0 ? SIDAL.GetPlant(Convert.ToInt32(backupPlantId.GetValueOrDefault())).Name : "";


        }
    }
}