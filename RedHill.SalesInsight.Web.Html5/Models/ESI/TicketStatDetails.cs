using RedHill.SalesInsight.DAL.Attributes;
using RedHill.SalesInsight.DAL.Utilities;
using System;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class TicketStatDetails
    {
        [Display(Name="City")]
        public string CustomerCity { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "District")]
        public string DistrictName { get; set; }

        [Display(Name = "Est. Clock Hours")]
        public double EstimatedClockHours { get; set; }

        [Display(Name = "Load Mins")]
        public double LoadMinutes { get; set; }

        [Display(Name = "Material Cost")]
        public double MaterialCost { get; set; }

        [Display(Name = "Region")]
        public string RegionName { get; set; }
        public double Revenue { get; set; }
        public double ShutdownMinutes { get; set; }
        public double StartupMinutes { get; set; }
        public double Temper { get; set; }
        public string TicketId { get; set; }
        public double TicketingMinutes { get; set; }
        public string TicketNumber { get; set; }
        public double TotalMinutes { get; set; }
        public string TruckNumber { get; set; }
        public double Volume { get; set; }
        public double WaitMinutes { get; set; }
        public double WashMinutes { get; set; }

    }
}