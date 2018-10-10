using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ManageRawMaterialCostProjections
    {
        public SIUser User { get; set; }
        public List<RawMaterialPlantProjectionView> Projections { get; set; }
        public long RawMaterialId { get; set; }
        public RawMaterial Material { get; set; }

        public long     ExpandUOMId { get; set; }

        public decimal  SavePrice { get; set; }
        public int      SelectedPlantId { get; set; }
        public long     SaveUOMId { get; set; }
        public int      FutureMonth { get; set; }

        public String   MonthYear { get; set; }

        public string ScrollTo { get; set; }

        public List<RawMaterialPlantProjectionView> Originals { get; set; }


        public ManageRawMaterialCostProjections()
        {
        }

        public ManageRawMaterialCostProjections(long id)
        {
            this.RawMaterialId = id;
        }

        public void LoadValues()
        {
            if (MonthYear == null)
            {
                this.ProjectionDate = DateTime.Today;
            }
            Material = SIDAL.FindRawMaterial(RawMaterialId);
            if (ExpandUOMId == 0){
                ExpandUOMId = SIDAL.GetUOMSByType(Material.MeasurementType).OrderBy(x=>x.Priority).Select(x=>x.Id).FirstOrDefault();
            }
            //var actualProjections = SIDAL.GetRawMaterialCostProjections(this.RawMaterialId);
            this.Projections = new List<RawMaterialPlantProjectionView>();
            foreach (Plant plant in SIDAL.GetPlants(User.UserId))
            {
                RawMaterialPlantProjectionView view = new RawMaterialPlantProjectionView(RawMaterialId, plant, ProjectionDate.Value, ExpandUOMId);
                this.Projections.Add(view);
            }
        }

        public SelectList ChooseExpandUOM
        {
            get
            {
                return new SelectList(SIDAL.GetUOMSByType(Material.MeasurementType), "Id", "Name", ExpandUOMId);
            }
        }

        public void UpdatePrice()
        {
            if (SelectedPlantId > 0 )
            {
                SIDAL.UpdateRawMaterialCostProjection(RawMaterialId, SelectedPlantId, ProjectionDate.Value.AddMonths(FutureMonth), SavePrice, SaveUOMId);
                ScrollTo = "Plant_Row_" + SelectedPlantId;
                SelectedPlantId = 0;
                SaveUOMId = 0;
                SavePrice = 0;
                FutureMonth = 0;
            }
        }

        public DateTime? ProjectionDate
        {
            get
            {
                if (MonthYear != null && !MonthYear.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact("01 " + MonthYear, "dd MMM, yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return DateTime.Now;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    MonthYear = value.Value.ToString("MMM, yyyy");
                }
            }
        }
    }
}