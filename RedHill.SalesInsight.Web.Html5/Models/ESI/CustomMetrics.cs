using System;
using System.Collections.Generic;
using RedHill.SalesInsight.DAL;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL.Models;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class CustomMetricsModel 
    {

        #region Properties
        public DateTime DayDateTime { get; set; }
        public int PlantId { get; set; } 
        #endregion
        public DailyPlantSummary customMetrics { get; set; }
        public CustomMetricsModel()
        {
            DayDateTime = DateTime.Now;
            customMetrics = new DailyPlantSummary();
        }
         
    }
}