using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class StandardMixView
    {
        public SIUser   User { get; set; }
        public long     Id { get; set; }

        [Required]
        public string   Number { get; set; }
        
        [Required]
        public string   Description { get; set; }
        
        public string   SalesDescription { get; set; }
        public int      Psi { get; set; }
        public string   Slump { get; set; }
        public string   Air { get; set; }
        public string   MD1 { get; set; }
        public string   MD2 { get; set; }
        public string   MD3 { get; set; }
        public string   MD4 { get; set; }
        public bool     Active { get; set; }
        public string   UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public StandardMixView()
        {
            this.Active = true;
        }

        public StandardMixView(StandardMix entity)
        {
            this.Id = entity.Id;
            this.Number = entity.Number;
            this.Description = entity.Description;
            this.SalesDescription = entity.SalesDesc;
            this.Psi = entity.PSI.GetValueOrDefault();
            this.Slump = entity.Slump;
            this.Air = entity.Air;
            this.MD1 = entity.MD1;
            this.MD2 = entity.MD2;
            this.MD3 = entity.MD3;
            this.MD4 = entity.MD4;
            this.UpdatedBy = entity.UpdatedBy;
            this.UpdatedOn = entity.UpdatedOn;
            this.Active = entity.Active.GetValueOrDefault();
        }

        public StandardMix ToEntity()
        {
            StandardMix entity = new StandardMix();
            entity.Id = this.Id;
            entity.Number = this.Number;
            entity.Description = this.Description;
            entity.SalesDesc = this.SalesDescription;
            entity.PSI = this.Psi;
            entity.Slump = this.Slump;
            entity.Air = this.Air;
            entity.MD1 = this.MD1;
            entity.MD2 = this.MD2;
            entity.MD3 = this.MD3;
            entity.MD4 = this.MD4;
            entity.Active = this.Active;

            return entity;
        }

        public GlobalSetting Setting
        {
            get 
            {
                return SIDAL.GetGlobalSettings();
            }
        }

        public List<Plant> Plants
        {
            get
            {
                return SIDAL.GetPlants(User.UserId).ToList();
            }
        }

        // Empty list for the MVC Checklistfor control in the copy formulation to plant control.
        public List<Plant> SelectedPlants
        {
            get
            {
                return new List<Plant>();
            }
        }

        public MixFormulation FindFormulation(int plantId)
        {
            return SIDAL.FindFormulation(plantId, Id);
        }
    }
}