using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class DailyPlantSummaryModel
    {
        public DailyPlantSummary DailyPlantSummary { get; set; }

        public DailyPlantSummaryModel(DailyPlantSummary dailyPlantSummary)
        {
            this.DailyPlantSummary = dailyPlantSummary;
        }

        public string ToJson()
        {
            if (this.DailyPlantSummary != null)
            {
                return JsonConvert.SerializeObject(DailyPlantSummary,
                    Formatting.None,
                    new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            return "";
        }
    }
}