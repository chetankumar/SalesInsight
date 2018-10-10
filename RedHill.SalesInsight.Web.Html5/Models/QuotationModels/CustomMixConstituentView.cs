using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class CustomMixConstituentView
    {
        public long Id { get; set; }
        public long QuotationMixId { get; set; }
        public long RawMaterialId { get; set; }
        public long AddonId { get; set; }
        public string Description { get; set; }
        public bool IsCementitious { get; set; }
        public long PlantId { get; set; }
        public double Quantity { get; set; }
        public long QuantityUomId { get; set; }
        public decimal Cost { get; set; }
        public long CostUomId { get; set; }
        public bool PerCementWeight { get; set; }
        public string RawMaterialType { get; set; }
        public CustomMixConstituentView()
        {
        }

        public CustomMixConstituentView(CustomMixConstituent entity)
        {
            this.Id = entity.Id;
            this.QuotationMixId = entity.QuotationMixId;
            this.RawMaterialId = entity.RawMaterialId.GetValueOrDefault();
            this.AddonId = entity.AddonId.GetValueOrDefault();
            this.Description = entity.Description;
            this.IsCementitious = entity.IsCementitious.GetValueOrDefault(false);
            this.Quantity = entity.Quantity;
            this.QuantityUomId = entity.QuantityUomId.GetValueOrDefault();

            this.Cost = entity.Cost.GetValueOrDefault();
            this.CostUomId = entity.CostUomId.GetValueOrDefault();

            this.PerCementWeight = entity.PerCementWeight.GetValueOrDefault();
        }

        public CustomMixConstituentView(long mixId)
        {
            this.QuotationMixId = mixId;
        }

        public CustomMixConstituent ToEntity()
        {
            CustomMixConstituent entity = new CustomMixConstituent();
            entity.Id = this.Id;
            entity.QuotationMixId = this.QuotationMixId;
            entity.Quantity = this.Quantity;
            if (this.AddonId > 0)
            {
                entity.AddonId = this.AddonId;
            }
            else if (this.RawMaterialId > 0)
            {
                entity.RawMaterialId = this.RawMaterialId;
                entity.QuantityUomId = this.QuantityUomId;
                entity.PerCementWeight = this.PerCementWeight;
            }
            else
            {
                entity.Description = this.Description;
                entity.IsCementitious = this.IsCementitious;
                entity.Quantity = this.Quantity;
                entity.QuantityUomId = this.QuantityUomId;
                entity.Cost = this.Cost;
                entity.CostUomId = this.CostUomId;
                entity.PerCementWeight = this.PerCementWeight;
            }

            return entity;
        }

        public SelectList ChooseQuantityUOM(long quantityUomId,long? rawMaterialId = null)
        {
            if (rawMaterialId != null)
            {
                RawMaterial r = SIDAL.FindRawMaterial(rawMaterialId.GetValueOrDefault());
                return new SelectList(SIDAL.GetUOMS().Where(x => x.Category == r.MeasurementType).OrderBy(x => x.Priority2).ThenBy(x => x.Category), "Id", "Name", quantityUomId);
            }
            else 
            {
                return new SelectList(SIDAL.GetUOMS().Where(x => x.Category == "Weight" || x.Category == "Volume").OrderBy(x => x.Priority2).ThenBy(x => x.Category), "Id", "Name", quantityUomId);
            }
        }

        public SelectList ChooseCostUOM(long costUomId)
        {
            return new SelectList(SIDAL.GetUOMS().Where(x => x.Category == "Weight" || x.Category == "Volume").OrderBy(x => x.Category).ThenBy(x => x.Priority), "Id", "Name", costUomId);
        }

        public SelectList ChooseRawMaterial(long rawMaterialId)
        {
            var rawMaterials = SIDAL.GetNonZeroRawMaterials(PlantId).Select(s => new
            {
                Name = s.RawMaterialType.Name + " - " + s.MaterialCode + " - " + s.Description,
                Id = s.Id
            });
            return new SelectList(rawMaterials, "Id", "Name", rawMaterialId);
        }

        public SelectList ChooseAddon(long addOnId)
        {
            QuotationMix m = SIDAL.FindQuotationMix(this.QuotationMixId);
            var list = SIDAL.GetActiveAddons("Mix", m.Quotation.PlantId.GetValueOrDefault(), m.Quotation.PricingMonth).Where(x => x.MixUom.Name != "N.A.").Select(x => new { Text = x.Code + " - " + x.Description + " (Per " + x.MixUom.Name + ")", Value = x.Id });
            return new SelectList(list, "Value", "Text", addOnId);
        }

    }
}