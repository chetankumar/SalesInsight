using Newtonsoft.Json.Linq;
using Redhill.SalesInsight.ESI;
using Redhill.SalesInsight.ESI.ReportModels;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Models.POCO;
using RedHill.SalesInsight.DAL.Mongo.Models;
using RedHill.SalesInsight.Web.Html5.Models.ESI.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class DrillInTicketStatsView : ReportSettingView
    {

        public TicketStatDetails TicketStats { get; set; }
        public List<TicketStatDetails> Response { get; set; }

        public string  DimensionName { get; set; }
        public string  MetricName { get; set; }

        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }

        public string search { get; set; }

        public List<SortItem> order { get; set; }

        public int PageNumber
        {
            get
            {
                return (length > 0 ? (start / length) : 0) + 1;
            }
        }

        public int PageSize
        {
            get
            {
                return length;
            }
        }

        public long recordsTotal { get; set; }
        public int recordsFiltered { get; set; }


        public void LoadData(long dimensionId, string metricName,DrillInTicketStatsView reportModel)
        {
            MetricRequest request;
            switch (metricName)
            {
                case "PlantName":
                    request = new MetricListRequest
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        PlantIds = new List<long> { dimensionId },
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                case "CustomerName":
                    request = new MetricListRequest
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        CustomerIds = new List<long> { dimensionId },
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                case "RegionName":
                    request = new MetricListRequest
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        RegionIds = new List<long> { dimensionId },
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                case "DistrictName":
                    request = new MetricListRequest
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        DistrictIds = new List<long> { dimensionId },
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                case "SalesStaffName":
                    request = new MetricListRequest
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        SalesStaffIds = new List<long> { dimensionId },
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                case "CustomerSegmentId":
                    request = new MetricListRequest
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        CustomerIds = new List<long> { dimensionId },
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                case "DriverNumber":
                    request = new MetricListRequest
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        DriverIds = new List<long> { dimensionId },
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                default:
                    request = new MetricListRequest()
                    {
                        StartDate = new DateTime(2016, 1, 1),
                        EndDate = new DateTime(2016, 12, 31),
                        Limit = reportModel.PageSize,
                        Skip = reportModel.PageSize * (reportModel.PageNumber - 1),
                        order = reportModel.order,
                        search = reportModel.search
                    };
                    break;
                    
            } 
            EsiReportManager manager = new EsiReportManager();
            long count = 0;
            var query = from s in manager.GetTicketData(request,out count)
                        select new TicketStatDetails
                        {
                           TicketId = s.TicketId,
                           TicketNumber = s.TicketNumber,
                           Date = s.Date,
                           StartupMinutes = s.StartupMinutes,
                           ShutdownMinutes = s.ShutdownMinutes,
                           TotalMinutes = s.TotalMinutes,
                           EstimatedClockHours = s.EstimatedClockHours,
                           TicketingMinutes = s.TicketingMinutes,
                           LoadMinutes = s.LoadMinutes,
                           Temper = s.Temper,
                           WaitMinutes = s.WaitMinutes,
                           WashMinutes = s.WashMinutes,
                           Volume = s.Volume,
                           DistrictName = s.DistrictName,
                           RegionName = s.RegionName,
                           TruckNumber = s.TruckNumber,
                           CustomerName = s.CustomerName,
                           CustomerCity = s.CustomerCity,
                           Revenue = s.Revenue,
                           MaterialCost = s.MaterialCost
                           
                        };
            this.Response = query.ToList();
            this.recordsFiltered = query.ToList().Count();
            this.recordsTotal = count;
          //  this.Response = manager.GetTicketData(request);
        }
    }
}