using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class RawMaterialTypeView
    {

        public long Id { get; set; }

        [Required]
        public String Name { get; set; }
        public bool IsCementitious { get; set; }
        public bool FSKDetail { get; set; }
        public bool Active { get; set; }
        public bool IncludeInSackCalculation { get; set; }
        public bool IncludeInAshCalculation { get; set; }
        public bool IncludeInSandCalculation { get; set; }
        public bool IncludeInRockCalculation { get; set; }

        public string DispatchId { get; set; }

        public RawMaterialTypeView()
        {
        }

        public RawMaterialTypeView(RawMaterialType model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.IsCementitious = model.IsCementitious.GetValueOrDefault();
            this.FSKDetail = model.IncludeInFSK.GetValueOrDefault();
            this.IncludeInSackCalculation = model.IncludeInSackCalculation.GetValueOrDefault();
            this.IncludeInAshCalculation = model.IncludeInAshCalculation.GetValueOrDefault();
            this.IncludeInSandCalculation = model.IncludeInSandCalculation.GetValueOrDefault();
            this.IncludeInRockCalculation = model.IncludeInRockCalculation.GetValueOrDefault();
            this.Active = model.Active.GetValueOrDefault();
            this.DispatchId = model.DispatchId;
        }

        public RawMaterialType ToEntity()
        {
            RawMaterialType entity = new RawMaterialType();
            entity.Active = this.Active;
            entity.Name = this.Name;
            entity.IsCementitious = this.IsCementitious;
            entity.IncludeInFSK = this.FSKDetail;
            entity.IncludeInSackCalculation = this.IncludeInSackCalculation;
            entity.IncludeInAshCalculation = this.IncludeInAshCalculation;
            entity.IncludeInSandCalculation = this.IncludeInSandCalculation;
            entity.IncludeInRockCalculation = this.IncludeInRockCalculation;
            entity.Id = this.Id;
            entity.DispatchId = this.DispatchId;
            return entity;
        }
    }
}