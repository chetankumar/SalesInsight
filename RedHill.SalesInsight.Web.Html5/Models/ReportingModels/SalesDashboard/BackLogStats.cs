using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class BackLogPlantStat
    {
        public DateTime MonthDate { get; set; }
        public int PlantId { get; set; }
        public int TotalProjected { get; set; }

        public BackLogPlantStat(DateTime month, int plantId, int projection)
        {
            this.MonthDate = month;
            this.PlantId = plantId;
            this.TotalProjected = projection;
        }
    }

    public class BackLogStat
    {
        public DateTime MonthDate { get; set; }
        public int TotalProjected { get; set; }
        public double Budget { get; set; }

        public BackLogStat(DateTime month, int projection, double budget)
        {
            this.MonthDate = month;
            this.TotalProjected = projection;
            this.Budget = budget;
        }
    }
}
