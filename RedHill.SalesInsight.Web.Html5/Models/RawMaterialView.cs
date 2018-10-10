using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class RawMaterialView
    {
        public long Id { get; set; }

        [Required]
        public String MaterialCode { get; set; }
        public String Description { get; set; }
        public double? FskMarkup { get; set; }
        public String FskCode { get; set; }
        [Required]
        public long RawMaterialTypeId { get; set; }

        public bool Active { get; set; }

        [Required]
        public String MeasurementType { get; set; }

        public string DispatchId { get; set; }

        public RawMaterialView()
        {
        }

        public RawMaterialView(RawMaterial entity)
        {
            this.Id = entity.Id;
            this.MaterialCode = entity.MaterialCode;
            this.Description = entity.Description;
            this.FskCode = entity.FSKCode;
            this.FskMarkup = entity.FSKMarkup;
            this.FskCode = entity.FSKCode;
            this.RawMaterialTypeId = entity.RawMaterialTypeId;
            this.Active = entity.Active;
            this.MeasurementType = entity.MeasurementType;
            this.DispatchId = entity.DispatchId;
        }

        public RawMaterial ToEntity()
        {
            RawMaterial entity = new RawMaterial();
            entity.Id = this.Id;
            entity.MaterialCode = this.MaterialCode;
            entity.Description = this.Description;
            entity.FSKCode = this.FskCode;
            entity.FSKMarkup = this.FskMarkup;
            entity.RawMaterialTypeId = this.RawMaterialTypeId;
            entity.Active = this.Active;
            entity.MeasurementType = this.MeasurementType;
            entity.DispatchId = this.DispatchId;
            return entity;
        }

        public RawMaterialType RawMaterialType
        {
            get
            {
                if (this.RawMaterialTypeId > 0)
                {
                    return SIDAL.FindRawMaterialType(this.RawMaterialTypeId);
                }
                else
                {
                    return null;
                }
            }
        }

        public SelectList ChooseMeasurementTypes
        {
            get
            {
                return new SelectList(new String[] {"Volume","Weight" }, this.MeasurementType);
            }
        }

        public SelectList ChooseRawMaterialTypes
        {
            get
            {
                return new SelectList(SIDAL.GetRawMaterialTypes().OrderBy(x=>x.Name), "Id", "Name", this.RawMaterialTypeId);
            }
        }
    }
}