using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class RawMaterialPlantProjectionView
    {
        public long RawMaterialId { get; set; }
        public int PlantId { get; set; }
        public String PlantName { get; set; }
        public decimal Month1 { get; set; }
        public decimal Month2 { get; set; }
        public decimal Month3 { get; set; }
        public decimal Month4 { get; set; }
        public decimal Month5 { get; set; }
        public decimal Month6 { get; set; }

        public bool Month1Actual { get; set; }
        public bool Month2Actual { get; set; }
        public bool Month3Actual { get; set; }
        public bool Month4Actual { get; set; }
        public bool Month5Actual { get; set; }
        public bool Month6Actual { get; set; }

        public RawMaterialPlantProjectionView(long rawMaterialId, Plant plant, DateTime currentMonth, long TargetUomId)
        {
            this.RawMaterialId = rawMaterialId;
            this.PlantId = plant.PlantId;
            this.PlantName = plant.Name;
            var monthActual = false;
            this.Month1 = SIDAL.FindRawMaterialPrice(RawMaterialId, PlantId, currentMonth, 0, out monthActual, TargetUomId);
            this.Month1Actual = monthActual;
            this.Month2 = SIDAL.FindRawMaterialPrice(RawMaterialId, PlantId, currentMonth, 1, out monthActual, TargetUomId);
            this.Month2Actual = monthActual;
            this.Month3 = SIDAL.FindRawMaterialPrice(RawMaterialId, PlantId, currentMonth, 2, out monthActual, TargetUomId);
            this.Month3Actual = monthActual;
            this.Month4 = SIDAL.FindRawMaterialPrice(RawMaterialId, PlantId, currentMonth, 3, out monthActual, TargetUomId);
            this.Month4Actual = monthActual;
            this.Month5 = SIDAL.FindRawMaterialPrice(RawMaterialId, PlantId, currentMonth, 4, out monthActual, TargetUomId);
            this.Month5Actual = monthActual;
            this.Month6 = SIDAL.FindRawMaterialPrice(RawMaterialId, PlantId, currentMonth, 5, out monthActual, TargetUomId);
            this.Month6Actual = monthActual;
        }
    }
}