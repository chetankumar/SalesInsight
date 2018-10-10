using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class PlantView
    {
        public SIUser User { get; set; }
        [Display(Name = "Plant")]
        public int PlantId { get; set; }
        [Required]
        public string Name { get; set; }

        [Display(Name = "Dispatch System Plant Code")]
        public string DispatchId { get; set; }

        [Display(Name = "Product Type")]
        public ProductType ProductTypeId { get; set; }

        public string DistrictName { get; set; }
        public int? Trucks { get; set; }
        public int? Ticket { get; set; }
        public int? Load { get; set; }
        public int? Temper { get; set; }
        public int? Wait { get; set; }
        public decimal? Utilization { get; set; }
        [Display(Name = "Default Sack Pricing City")]
        public long? FSKPriceId { get; set; }

        [Display(Name = "Variable Cost")]
        public decimal? VariableCost { get; set; }
        [Display(Name = "Plant Cost")]
        public decimal? PlantCost { get; set; }
        [Display(Name = "Delivery Cost")]
        public decimal? DeliveryCost { get; set; }
        [Display(Name = "Utilization Percentage")]
        public decimal? UtilizationPercentage
        {
            get
            {
                if (Utilization == null)
                    return null;
                else
                {
                    return Utilization * 100;
                }
            }
            set
            {
                if (value == null)
                    Utilization = null;
                else
                {
                    Utilization = value / 100;
                }
            }
        }

        [Display(Name = "SG&A")]
        public decimal? SGA { get; set; }
        public bool Active { get; set; }

        [Display(Name = "District")]
        public int DistrictId { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public SelectList AvailableDistricts { get; set; }
        public SelectList Available5skPrices { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string DistrictLatitude { get; set; }
        public string DistrictLongitude { get; set; }

        [Display(Name = "Post Load")]
        public int? PostLoad { get; set; }

        [Display(Name = "To Job")]
        public int? ToJob { get; set; }

        [Display(Name = "Unload")]
        public int? Unload { get; set; }

        [Display(Name = "To Plant")]
        public int? ToPlant { get; set; }

        [Display(Name = "Avg Load Size")]
        public int? AvgLoadSize { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City State Zip")]
        public string CityStateZip { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        public PlantView(DAL.Plant r)
        {
            this.PlantId = r.PlantId;
            try
            {
                this.DistrictName = r.District.Name;
                this.DistrictLatitude = r.District.MapCenterLat;
                this.DistrictLongitude = r.District.MapCenterLong;
            }
            catch (Exception) {/* District not loaded. */ }
            this.Name = r.Name;
            this.DispatchId = r.DispatchId;
            this.Trucks = r.Trucks;
            this.Ticket = r.TicketMinutes;
            this.Load = r.LoadMinutes;
            this.Temper = r.TemperMinutes;
            this.PostLoad = r.PostLoadMinutes;
            this.ToJob = r.ToJobMinutes;
            this.Unload = r.UnloadMinutes;
            this.ToPlant = r.ToPlantMinutes;
            this.AvgLoadSize = r.AvgLoadSize;
            this.Wait = r.WaitMinutes;
            this.Utilization = r.Utilization;
            this.VariableCost = r.VariableCostPerMin;
            this.PlantCost = r.PlantFixedCost;
            this.DeliveryCost = r.DeliveryFixedCost;
            this.SGA = r.SGA;
            this.Active = r.Active.GetValueOrDefault(false);
            this.CompanyId = r.CompanyId;
            this.DistrictId = r.DistrictId;
            this.FSKPriceId = r.FSKId;
            this.Latitude = r.Latitude;
            this.Longitude = r.Longitude;
            this.Address = r.Address;
            this.CityStateZip = r.CityStateZip;
            this.Phone = r.Phone;
            this.ProductTypeId = (ProductType)r.ProductTypeId;
        }

        public PlantView()
        {
            this.Name = "Default Name";
        }

        public void BindValues()
        {
            this.CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
            if (User == null)
            {
                this.AvailableDistricts = new SelectList(SIDAL.GetDistricts(this.CompanyId, null, 0, 1000, false), "DistrictId", "Name", DistrictId);
            }
            else
            {
                this.AvailableDistricts = new SelectList(SIDAL.GetDistricts(this.User.UserId), "DistrictId", "Name", DistrictId);
            }
            this.Available5skPrices = new SelectList(SIDAL.GetFSKPrices().Select(x => new { Name = x.FSKCode + " - " + x.City, Value = x.Id }).OrderBy(x => x.Name), "Value", "Name", FSKPriceId);
        }
    }
}
