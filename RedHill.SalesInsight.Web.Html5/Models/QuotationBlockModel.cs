using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationBlockModel
    {
        public long Id { get; set; }
        public long QuotationId { get; set; }
        public int Position { get; set; }
        public long BlockProductId { get; set; }
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

        public QuotationBlockModel()
        {

        }

        public QuotationBlockModel(QuotationBlock quotationBlock)
        {
            this.Id = quotationBlock.Id;
            this.QuotationId = quotationBlock.QuotationId.GetValueOrDefault();
            this.Position = quotationBlock.Position.GetValueOrDefault();
            this.BlockProductId = quotationBlock.BlockProductId.GetValueOrDefault();
            this.QuotedDescription = quotationBlock.QuotedDescription;
            this.Volume = quotationBlock.Volume.GetValueOrDefault();
            this.Price = quotationBlock.Price.GetValueOrDefault();
            this.AvgLoad = quotationBlock.AvgLoad.GetValueOrDefault();
            this.AddonCost = quotationBlock.AddonCost.GetValueOrDefault();
            this.MixCost = quotationBlock.MixCost.GetValueOrDefault();
            this.Unload = quotationBlock.Unload.GetValueOrDefault();
            this.Spread = quotationBlock.Spread.GetValueOrDefault();
            this.Contribution = quotationBlock.Contribution.GetValueOrDefault();
            this.Profit = quotationBlock.Profit.GetValueOrDefault();
            this.PrivateNotes = quotationBlock.PrivateNotes;
            this.PublicNotes = quotationBlock.PublicNotes;
            this.CydHour = quotationBlock.CydHour.GetValueOrDefault();
            this.Freight = quotationBlock.Freight.GetValueOrDefault();
            this.TotalPrice = quotationBlock.TotalPrice.GetValueOrDefault();
            this.TotalRevenue = quotationBlock.TotalRevenue.GetValueOrDefault();
            this.Code = SIDAL.FindBlockProduct(BlockProductId).Code;
        }
    }
}