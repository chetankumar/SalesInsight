using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedHill.SalesInsight.DAL.DataTypes;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL;
using System.ComponentModel.DataAnnotations;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectPlantView
    {
        public int? ProjectPlantId { get; set; }
        public string PlantName { get; set; }
        public int ProjectId { get; set; }
        
        [Display(Name="Plant")]
        public int PlantId { get; set; }

        [Display(Name = "Plant Volume")]
        public int Volume { get; set; }

        public SelectList AvailablePlants { get; set;}

        public Plant Plant { get; set; }

        public ProjectPlantView()
        {

        }

        public ProjectPlantView(SIViewProjectPlants plants)
        {
            this.PlantId = plants.PlantId;
            this.ProjectPlantId = plants.ProjectPlantId;
            this.ProjectId = plants.ProjectId;
            this.Volume = plants.Volume;
            this.PlantName = plants.Name;
            this.Plant = SIDAL.GetPlant(this.PlantId);
        }

        public ProjectPlantView(ProjectPlant pp)
        {
            this.PlantName = pp.Plant.Name;
            this.Volume = pp.Volume.GetValueOrDefault(0);
            this.PlantId = pp.PlantId;
            this.ProjectId = pp.ProjectId;
        }
    }
}
