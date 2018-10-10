using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models.QuotationModels
{
    public class QuotationFormSettingView
    {
        public long Id { get; set; }
        public long QuoteId { get; set; }
        public bool PriceInclude { get; set; }
        public int PriceSequence { get; set; }
        public bool QuantityInclude { get; set; }
        public int QuantitySequence { get; set; }
        public bool MixIdInclude { get; set; }
        public int MixIdSequence { get; set; }
        public bool DescriptionInclude { get; set; }
        public int DescriptionSequence { get; set; }
        public bool PsiInclude { get; set; }
        public int PsiSequence { get; set; }
        public bool PublicCommentsInclude { get; set; }
        public int PublicCommentsSequence { get; set; }     
        public bool SlumpInclude { get; set; }
        public int SlumpSequence { get; set; }
        public bool AirInclude { get; set; }
        public int AirSequence { get; set; }
        public bool AshInclude { get; set; }
        public int AshSequence { get; set; }
        public bool FineAggInclude { get; set; }
        public int FineAggSequence { get; set; }
        public bool SacksInclude { get; set; }
        public int SacksSequence { get; set; }
        public bool MD1Include { get; set; }
        public int MD1Sequence { get; set; }
        public bool MD2Include { get; set; }
        public int MD2Sequence { get; set; }
        public bool MD3Include { get; set; }
        public int MD3Sequence { get; set; }
        public bool MD4Include { get; set; }
        public int MD4Sequence { get; set; }


        public QuotationFormSettingView(DAL.QuotationFormSetting r)
        {
            this.Id = r.Id;
            this.QuoteId = r.QuoteId;
            this.PriceInclude = r.PriceInclude.GetValueOrDefault();
            this.PriceSequence = r.PriceSequence.GetValueOrDefault();
            this.QuantityInclude = r.QuantityInclude.GetValueOrDefault();
            this.QuantitySequence = r.QuantitySequence.GetValueOrDefault();
            this.MixIdInclude = r.MixIdInclude.GetValueOrDefault();
            this.MixIdSequence = r.MixIdSequence.GetValueOrDefault();
            this.DescriptionInclude = r.DescriptionInclude.GetValueOrDefault();
            this.DescriptionSequence = r.DescriptionSequence.GetValueOrDefault();
            this.PsiInclude = r.PsiInclude.GetValueOrDefault();
            this.PsiSequence = r.PsiSequence.GetValueOrDefault();
            this.PublicCommentsInclude = r.PublicCommentsInclude.GetValueOrDefault();
            this.PublicCommentsSequence = r.PublicCommentsSequence.GetValueOrDefault();
            this.SlumpInclude = r.SlumpInclude.GetValueOrDefault();
            this.SlumpSequence = r.SlumpSequence.GetValueOrDefault();
            this.AirInclude = r.AirInclude.GetValueOrDefault();
            this.AirSequence = r.AirSequence.GetValueOrDefault();
            this.AshInclude = r.AshInclude.GetValueOrDefault();
            this.AshSequence = r.AshSequence.GetValueOrDefault();
            this.FineAggInclude = r.FineAggInclude.GetValueOrDefault();
            this.FineAggSequence = r.FineAggSequence.GetValueOrDefault();
            this.SacksInclude = r.SacksInclude.GetValueOrDefault();
            this.SacksSequence = r.SacksSequence.GetValueOrDefault();
            this.MD1Include = r.MD1Include.GetValueOrDefault();
            this.MD1Sequence = r.MD1Sequence.GetValueOrDefault();
            this.MD2Include = r.MD2Include.GetValueOrDefault();
            this.MD2Sequence = r.MD2Sequence.GetValueOrDefault();
            this.MD3Include = r.MD3Include.GetValueOrDefault();
            this.MD3Sequence = r.MD3Sequence.GetValueOrDefault();
            this.MD4Include = r.MD4Include.GetValueOrDefault();
            this.MD4Sequence = r.MD4Sequence.GetValueOrDefault();
        }

        public QuotationFormSettingView()
        {

        }

        public QuotationFormSetting ToEntity()
        {
            QuotationFormSetting entity = new QuotationFormSetting();

            entity.Id = this.Id;
            entity.QuoteId = this.QuoteId;
            entity.PriceInclude = this.PriceInclude;
            entity.PriceSequence = this.PriceSequence;
            entity.QuantityInclude = this.QuantityInclude;
            entity.QuantitySequence = this.QuantitySequence;
            entity.MixIdInclude = this.MixIdInclude;
            entity.MixIdSequence = this.MixIdSequence;
            entity.DescriptionInclude = this.DescriptionInclude;
            entity.DescriptionSequence = this.DescriptionSequence;
            entity.PsiInclude = this.PsiInclude;
            entity.PsiSequence = this.PsiSequence;
            entity.PublicCommentsInclude = this.PublicCommentsInclude;
            entity.PublicCommentsSequence = this.PublicCommentsSequence;
            entity.SlumpInclude = this.SlumpInclude;
            entity.SlumpSequence = this.SlumpSequence;
            entity.AirInclude = this.AirInclude;
            entity.AirSequence = this.AirSequence;
            entity.AshInclude = this.AshInclude;
            entity.AshSequence = this.AshSequence;
            entity.FineAggInclude = this.FineAggInclude;
            entity.FineAggSequence = this.FineAggSequence;
            entity.SacksInclude = this.SacksInclude;
            entity.SacksSequence = this.SacksSequence;
            entity.MD1Include = this.MD1Include;
            entity.MD1Sequence = this.MD1Sequence;
            entity.MD2Include = this.MD2Include;
            entity.MD2Sequence = this.MD2Sequence;
            entity.MD3Include = this.MD3Include;
            entity.MD3Sequence = this.MD3Sequence;
            entity.MD4Include = this.MD4Include;
            entity.MD4Sequence = this.MD4Sequence;
            return entity;
        }

    }
}