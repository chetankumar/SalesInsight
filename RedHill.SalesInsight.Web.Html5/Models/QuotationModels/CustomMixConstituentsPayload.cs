using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.QuotationModels
{
    public class CustomMixConstituentsPayload
    {
        public long QuotationMixId { get; set; }
        public List<RawMaterialPayload> RawMaterials { get; set; }
        public List<AddOnPayload> AddOns { get; set; }
        public List<NonStandardConstituentPayload> NonStandardConstituents { get; set; }

        public void Save()
        {
            #region Save RawMaterials
            if (this.RawMaterials != null && this.RawMaterials.Count > 0)
            {
                QuotationMix qMix = SIDAL.FindQuotationMix(this.QuotationMixId);
                Quotation q = SIDAL.FindQuotation(qMix.QuotationId.GetValueOrDefault());
                Plant p = SIDAL.GetPlant(q.PlantId);
                foreach (var rawMaterial in this.RawMaterials)
                {
                    CustomMixConstituent cMixConst = new CustomMixConstituent();
                    cMixConst.Id = rawMaterial.Id;
                    cMixConst.RawMaterialId = rawMaterial.RawMaterialId;
                    cMixConst.QuantityUomId = rawMaterial.UomId;
                    cMixConst.Quantity = rawMaterial.Quantity;
                    cMixConst.PerCementWeight = rawMaterial.PerCementWeight;
                    cMixConst.QuotationMixId = this.QuotationMixId;

                    //CustomMixConstituent entity = model.RawMaterialConstituent.ToEntity();
                    RawMaterial material = SIDAL.FindRawMaterial(rawMaterial.RawMaterialId);
                    cMixConst.Description = material.MaterialCode + " - " + material.Description;
                    RawMaterialCostProjection projection = SIDAL.FindRawMaterialCost(rawMaterial.RawMaterialId, p.PlantId, q.PricingMonth);
                    if (projection != null && projection.Cost > 0)
                    {
                        cMixConst.Cost = projection.Cost;
                        cMixConst.CostUomId = projection.UomId;
                        cMixConst.IsCementitious = material.RawMaterialType.IsCementitious;
                        SIDAL.UpdateCustomMixConstituent(cMixConst);
                    }
                }
            }
            #endregion

            #region Save Add ons

            if (this.AddOns != null && this.AddOns.Count > 0)
            {
                QuotationMix qMix = SIDAL.FindQuotationMix(this.QuotationMixId);
                Quotation q = SIDAL.FindQuotation(qMix.QuotationId.GetValueOrDefault());
                Plant p = SIDAL.GetPlant(q.PlantId);
                District d = SIDAL.GetDistrict(SIDAL.GetPlant(q.PlantId).DistrictId);
                foreach (var addOn in this.AddOns)
                {
                    CustomMixConstituent custMixConst = new CustomMixConstituent();
                    Addon addon = SIDAL.FindAddon(addOn.AddOnId);
                    
                    custMixConst.Id = addon.Id;
                    custMixConst.QuotationMixId = this.QuotationMixId;
                    custMixConst.Description = addon.Code + " - " + addon.Description;
                    custMixConst.Cost = SIDAL.FindAddonMixCost(addOn.AddOnId, d.DistrictId, q.PricingMonth);
                    if (custMixConst.Cost > 0)
                    {
                        SIDAL.UpdateCustomMixConstituent(custMixConst);
                    }
                }
            }

            #endregion

            #region Save Non Standard Constituents

            if (this.NonStandardConstituents != null && this.NonStandardConstituents.Count > 0)
            {
                foreach (var nonStandardConst in this.NonStandardConstituents)
                {
                    CustomMixConstituent entity = new CustomMixConstituent();
                    entity.Id = nonStandardConst.Id;
                    entity.QuotationMixId = this.QuotationMixId;
                    entity.Description = nonStandardConst.Description;
                    entity.IsCementitious = nonStandardConst.IsCementitious;
                    entity.Quantity = nonStandardConst.Quantity;
                    entity.QuantityUomId = nonStandardConst.QuantityUomId;
                    entity.Cost = nonStandardConst.Cost;
                    entity.CostUomId = nonStandardConst.CostUomId;
                    entity.PerCementWeight = nonStandardConst.PerCementWeight;
                    if (entity.Cost > 0)
                    {
                        SIDAL.UpdateCustomMixConstituent(entity);
                    }
                }
            }

            #endregion
        }
    }

    public class RawMaterialPayload
    {
        public long Id { get; set; }
        public long RawMaterialId { get; set; }
        public long UomId { get; set; }
        public bool PerCementWeight { get; set; }
        public float Quantity { get; set; }
    }

    public class AddOnPayload
    {
        public long Id { get; set; }
        public long AddOnId { get; set; }
        public float Quantity { get; set; }
    }

    public class NonStandardConstituentPayload
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public bool IsCementitious { get; set; }
        public float Quantity { get; set; }
        public long QuantityUomId { get; set; }
        public bool PerCementWeight { get; set; }
        public decimal? Cost { get; set; }
        public long CostUomId { get; set; }
    }
}