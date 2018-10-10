using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class MixConstituentView
    {
        public long Id { get; set; }
        public long RawMaterialId { get; set; }
        public long UomId { get; set; }
        public string UomName { get; set; }
        public double Quantity { get; set; }
        public bool PerCementWeight { get; set; }
        public long PlantId { get; set; }

        public MixConstituentView() { }

        public MixConstituentView(StandardMixConstituent entity)
        {
            this.Id = entity.Id;
            this.RawMaterialId = entity.RawMaterialId;
            this.UomId = entity.UomId;
            this.Quantity = entity.Quantity;
            this.PerCementWeight = entity.PerCementWeight.GetValueOrDefault(false);
        }

        public StandardMixConstituent ToEntity()
        {
            StandardMixConstituent entity = new StandardMixConstituent();
            entity.Id = this.Id;
            entity.RawMaterialId = this.RawMaterialId;
            entity.UomId = this.UomId;
            entity.Quantity = this.Quantity;
            entity.PerCementWeight = this.PerCementWeight;
            return entity;
        }

        public SelectList ChooseUOM
        {
            get
            {
                return new SelectList(SIDAL.GetUOMS().OrderBy(x=>x.Priority2), "Id", "Name", this.UomId);
            }
        }

        public SelectList ChooseRawMaterial
        {
            get
            {
                var rawMaterials = SIDAL.GetNonZeroRawMaterials(PlantId).Select(s=> new{
                    Name = s.RawMaterialType.Name + " - " + s.MaterialCode + " - " + s.Description,
                    Id = s.Id
                });
                return new SelectList(rawMaterials, "Id", "Name", this.UomId);
            }
        }

    }
}
