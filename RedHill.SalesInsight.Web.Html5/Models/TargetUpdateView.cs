using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class TargetUpdateView
    {
        public int PlantId { get; set; }
        public int PlantBudgetId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BudgetDate { get; set; }

        public int SalesStaff { get; set; }
        public int MarketSegment { get; set; }

        public int PlantBudgetSalesStaff { get; set; }
        public int PlantBudgetMarketSegment { get; set; }

        public int Value { get; set; }

        public string OperationType { get; set; }


    }
}