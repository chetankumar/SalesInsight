using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationMixPreviewTable
    {
        public decimal Price { get; set; }
        public double Quantity { get; set; }
        public string MixDescription { get; set; }
        public string MixId { get; set; }
        public string CustomMixId { get; set; }
        public string PSI { get; set; }
        public string Slump { get; set; }
        public string Air { get; set; }
        public string Ash { get; set; }
        public string FineAgg { get; set; }
        public string Sacks { get; set; }
        public string MD1 { get; set; }
        public string MD2 { get; set; }
        public string MD3 { get; set; }
        public string MD4 { get; set; }
        public string Comments { get; set; }

        public QuotationMixPreviewTable(QuotationMix mix, int? plantId = null)
        {
            this.Price = mix.Price.GetValueOrDefault(0);
            this.Quantity = mix.Volume.GetValueOrDefault(0);
            this.MixDescription = mix.QuotedDescription;
            this.CustomMixId = mix.CustomMixId;
            if (mix.StandardMixId != null)
            {
                StandardMix sm = SIDAL.FindStandardMix(mix.StandardMixId.Value);
                if (plantId != null)
                {
                    MixFormulation mf = SIDAL.FindFormulation(plantId.GetValueOrDefault(), mix.StandardMixId.Value);
                    if (mf != null)
                    {
                        this.Ash = mf.AshPercentage.GetValueOrDefault().ToString();
                        this.FineAgg = mf.AshPercentage.GetValueOrDefault().ToString();
                        this.Sacks = mf.Sacks.GetValueOrDefault().ToString();
                    }
                }
                this.MixId = sm.Number;
                this.PSI = sm.PSI.GetValueOrDefault(0).ToString();
                this.Slump = sm.Slump;
                this.Air = sm.Air;
                this.MD1 = sm.MD1;
                this.MD2 = sm.MD2;
                this.MD3 = sm.MD3;
                this.MD4 = sm.MD4;
            }
            else
            {
                this.MixId = mix.CustomMixId;
            }
            this.Comments = mix.PublicNotes;
        }
    }
}