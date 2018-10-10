using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationAggregateModel
    {
        public long Id { get; set; }
        public long QuotationId { get; set; }
        public int Position { get; set; }
        public long AggregateProductId { get; set; }
        public string QuotedDescription { get; set; }
        public double Volume { get; set; }
        public decimal Price { get; set; }
        public double AvgLoad { get; set; }
        public decimal AddonCost { get; set; }
        public decimal MixCost { get; set; }
        public int Unload { get; set; }
        public decimal Spread { get; set; }
        public decimal Contribution { get; set; }
        public decimal Profit { get; set; }
        public string PrivateNotes { get; set; }
        public string PublicNotes { get; set; }
        public double CydHour { get; set; }
        public decimal Freight { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalRevenue { get; set; }
        public string Code { get; set; }

        public QuotationAggregateModel()
        {

        }

        public QuotationAggregateModel(QuotationAggregate quotationAggregate)
        {
            this.Id = quotationAggregate.Id;
            this.QuotationId = quotationAggregate.QuotationId.GetValueOrDefault();
            this.Position = quotationAggregate.Position.GetValueOrDefault();
            this.AggregateProductId = quotationAggregate.AggregateProductId.GetValueOrDefault();
            this.QuotedDescription = quotationAggregate.QuotedDescription;
            this.Volume =quotationAggregate.Volume.GetValueOrDefault();
            this.Price = quotationAggregate.Price.GetValueOrDefault();
            this.AvgLoad = quotationAggregate.AvgLoad.GetValueOrDefault();
            this.AddonCost = quotationAggregate.AddonCost.GetValueOrDefault();
            this.MixCost = quotationAggregate.MixCost.GetValueOrDefault();
            this.Unload = quotationAggregate.Unload.GetValueOrDefault();
            this.Spread = quotationAggregate.Spread.GetValueOrDefault();
            this.Contribution = quotationAggregate.Contribution.GetValueOrDefault();
            this.Profit = quotationAggregate.Profit.GetValueOrDefault();
            this.PrivateNotes = quotationAggregate.PrivateNotes;
            this.PublicNotes = quotationAggregate.PublicNotes;
            this.CydHour = quotationAggregate.CydHour.GetValueOrDefault();
            this.Freight = quotationAggregate.Freight.GetValueOrDefault();
            this.TotalPrice = quotationAggregate.TotalPrice.GetValueOrDefault();
            this.TotalRevenue = quotationAggregate.TotalRevenue.GetValueOrDefault();
            this.Code = SIDAL.FindAggregateProduct(this.AggregateProductId).Code;
        }


    }
}