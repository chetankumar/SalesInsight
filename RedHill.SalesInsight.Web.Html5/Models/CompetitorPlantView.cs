using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class CompetitorPlantView
    {
        public long Id { get; set; }
        public int CompetitorId { get; set; }
        public int DistrictId { get; set; }

        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }

        public string DistrictLatitude { get; set; }
        public string DistrictLongitude { get; set; }

        public List<CompetitorPlant> Plants
        {
            get
            {
                return SIDAL.GetCompetitorPlants(CompetitorId);
            }
        }

        public CompetitorPlantView()
        {
            Active = true;
        }

        public CompetitorPlantView(CompetitorPlant plant)
        {
            this.Id = plant.Id;
            this.CompetitorId = plant.CompetitorId;
            this.DistrictId = plant.DistrictId;
            this.Name = plant.Name;
            this.Latitude = plant.Latitude;
            this.Longitude = plant.Longitude;
            this.Active = plant.Active.GetValueOrDefault();
            this.DistrictLatitude = plant.District.MapCenterLat;
            this.DistrictLongitude = plant.District.MapCenterLong;
        }

        public SelectList AvailableDistricts
        {
            get
            {
                var options = SIDAL.GetCompetitorDistricts(this.CompetitorId).Select(x=> new {Name=x.Name,Value=x.DistrictId}).ToList();
                return new SelectList(options,"Value","Name",this.DistrictId);
            }
        }

        public string CompetitorName
        {
            get
            {
                return SIDAL.GetCompetitor(this.CompetitorId).Name;
            }
        }

        public CompetitorPlant ToEntity()
        {
            CompetitorPlant entity = new CompetitorPlant();
            entity.Id = this.Id;
            entity.CompetitorId = this.CompetitorId;
            entity.DistrictId = this.DistrictId;
            entity.Name = this.Name;
            entity.Latitude = this.Latitude;
            entity.Longitude = this.Longitude;
            entity.Active = this.Active;
            return entity;
        }
    }
}
