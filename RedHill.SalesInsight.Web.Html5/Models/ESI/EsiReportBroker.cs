using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Redhill.SalesInsight.ESI;
using Redhill.SalesInsight.ESI.ReportModels;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Models.POCO;
using RedHill.SalesInsight.DAL.Constants;
using RedHill.SalesInsight.Web.Html5.Models.ESI.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utils;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class EsiReportBroker
    {
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public EsiReportBroker()
        {
        }

        public EsiReportBroker(DateTime startDate, DateTime? endDate, ReportFilterSettingView filters, ReportConfigSettingView configuration)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Filters = filters;
            this.Configuration = configuration;
            DrillInDetailReportConfigView = new ReportDimensionConfig();
        }

        public EsiReportBroker(DateTime startDate, DateTime? endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        public ReportFilterSettingView Filters { get; set; }

        public ReportConfigSettingView Configuration { get; set; }

        public ReportDimensionConfig DrillInDetailReportConfigView { get; set; }

        public DrillinReportConfig DrillInReportConfiguration { get; set; }

        public SpecialReportConfig SpecialReportConfig { get; set; }

        public List<SortItem> order { get; set; }

        public double GetMetricValue(MetricDefinition metric, DateTime startDate, DateTime endDate, Dictionary<string, List<long>> filters = null)
        {
            var requestObj = new MetricRequest();
            requestObj.StartDate = startDate;
            requestObj.EndDate = endDate;

            requestObj.MetricDefinitions = new List<MetricDefinition>() { metric };

            if (filters != null)
            {
                requestObj.PlantIds = filters["Plant"]?.ToList();
                requestObj.CustomerIds = filters["Customer"]?.ToList();
                requestObj.MarketSegmentIds = filters["MarketSegment"]?.ToList();
                requestObj.SalesStaffIds = filters["SalesStaff"]?.ToList();
                requestObj.RegionIds = filters["Region"]?.ToList();
                requestObj.DistrictIds = filters["District"]?.ToList();
            }

            var esiReportManager = new EsiReportManager();
            var requestList = new List<MetricRequest>();
            requestList.Add(requestObj);

            var response = esiReportManager.GetData(requestList);

            double value = 0;
            if (response != null)
            {
                foreach (var item in response)
                {
                    value += Convert.ToDouble(item.Values
                                 .Where(x => x.MetricDefinitionId == metric.Id)
                                 .Where(x => x.Value != null && x.Value != 0)
                                 .Sum(x => (double?)x.Value).GetValueOrDefault());
                }
            }
            return value;
        }

        public List<double> GetMetricValues(MetricDefinition metric, DateTime startDate, DateTime endDate, Dictionary<string, List<long>> filters = null)
        {
            var metricDefinitions = new List<MetricDefinition>() { metric };

            var esiReportManager = new EsiReportManager();
            var requestList = new List<MetricListRequest>();

            var metricRequestList = new MetricListRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                MetricDefinitions = metricDefinitions,
                ColumnProperty = "Date",
                IsDateGroup = true,
                Granularity = Redhill.SalesInsight.ESI.Enumerations.RepeatPeriod.DAILY
            };

            if (filters != null)
            {
                metricRequestList.PlantIds = filters["Plant"]?.ToList();
                metricRequestList.CustomerIds = filters["Customer"]?.ToList();
                metricRequestList.MarketSegmentIds = filters["MarketSegment"]?.ToList();
                metricRequestList.SalesStaffIds = filters["SalesStaff"]?.ToList();
                metricRequestList.RegionIds = filters["Region"]?.ToList();
                metricRequestList.DistrictIds = filters["District"]?.ToList();
            }

            requestList.Add(metricRequestList);

            var response = esiReportManager.GetData(requestList);

            List<double> values = new List<double>();
            if (response != null)
            {
                foreach (var item in response)
                {
                    foreach (var bV in item.BucketValues)
                    {
                        foreach (var mgB in bV.MetricGroupBuckets)
                        {
                            values.Add(Convert.ToDouble(mgB.Value ?? 0));
                        }
                    }
                }
            }
            return values;
        }

        public List<GoalAnalysisResponsePayload> GetGoalAnalysisResponse(long reportId,Guid userId)
        {
            if (this.Configuration == null)
            {
                return new List<GoalAnalysisResponsePayload>();
            }
            //Prepare request
            var reportManager = new EsiReportManager();

            var requestList = new List<MetricRequest>();

            List<MetricDefinition> metricDefinitions = new List<MetricDefinition>();
            foreach (var rr in Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId.HasValue))
            {
                metricDefinitions.Add(SIDAL.GetMetricDefinition(rr.MetricDefinitionId.Value));
            }

            MetricRequest requestObj = null;
            foreach (var cc in Configuration.ReportColumnConfigList)
            {
                requestObj = new MetricRequest();

                DateTime[] startEnd = DateUtils.GetStartAndEndDateForPeriodType(cc.ColumnName, this.StartDate);

                requestObj.ClientRefCategory = cc.ColumnName;
                requestObj.StartDate = startEnd[0];
                requestObj.EndDate = startEnd[1];

                requestObj.MetricDefinitions = metricDefinitions;

                //district filter for standard report
                var report = SIDAL.GetReportById(reportId);
                if (report.AccessType == "Standard" && report.UserId != userId)
                {
                    var districts = SIDAL.GetDistrictFilterSetting(userId);
                    if (districts != null && Filters.Districts.Count == 0)
                    {
                        foreach (var district in districts)
                        {
                            Filters.Regions.Add(district.RegionId);
                            Filters.Districts.Add(Convert.ToInt32(district.DistrictId));
                        }
                    }
                }

                //Fill Filters
                requestObj.PlantIds = Filters.Plants.ToList();
                requestObj.MarketSegmentIds = Filters.MarketSegments.ToList();
                requestObj.RegionIds = Filters.Regions.ToList();
                requestObj.SalesStaffIds = Filters.SalesStaffs.ToList();
                requestObj.DistrictIds = Filters.Districts.ToList();
                requestObj.CustomerIds = Filters.Customers.ToList();

                requestList.Add(requestObj);
            }

            //Get Response
            var esiResponse = reportManager.GetData(requestList);

            List<GoalAnalysisResponsePayload> responsePayload = new List<GoalAnalysisResponsePayload>();

            GoalAnalysisResponsePayload payloadObj = null;

            foreach (var mDef in metricDefinitions)
            {
                payloadObj = new GoalAnalysisResponsePayload();
                payloadObj.MetricName = mDef.DisplayName;
                payloadObj.DisplayFormat = mDef.DefaultDisplayFormat;
                payloadObj.HasHorizontalSeparator = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.IsHorizontalSeperator).FirstOrDefault().GetValueOrDefault();
                payloadObj.ShowActionIcons = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.ShowActionIcons).FirstOrDefault().GetValueOrDefault();
                payloadObj.OkLimit = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.OkLimit).FirstOrDefault().GetValueOrDefault();
                payloadObj.CautionLimit = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.CautionLimit).FirstOrDefault().GetValueOrDefault();
                payloadObj.WarningLimit = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.WarningLimit).FirstOrDefault().GetValueOrDefault();
                payloadObj.ComparisonMetricId = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.ComparisonMetricId).FirstOrDefault().GetValueOrDefault();
                payloadObj.Values = new Dictionary<string, dynamic>();

                double? comparisonMetric = 0;
                foreach (var item in esiResponse)
                {
                    payloadObj.Values.Add(item.ClientRefCategory, item.Values.FirstOrDefault(x => x.MetricDefinitionId == mDef.Id).Value);
                    if (item.Values.FirstOrDefault(x => x.MetricDefinitionId == payloadObj.ComparisonMetricId) != null)
                    {
                        comparisonMetric = (double?)item.Values.FirstOrDefault(x => x.MetricDefinitionId == payloadObj.ComparisonMetricId).Value;
                    }
                }

                payloadObj.ComparisonValue = comparisonMetric.GetValueOrDefault();
                responsePayload.Add(payloadObj);
            }

            return responsePayload;
        } 

        public double[] GetPOPSummaryValues(MetricDefinition metric, string metricPeriod, DateTime startDate, DateTime endDate, Dictionary<string, List<long>> filters)
        {
            double[] values = new double[2];

            //Get the Current Value
            values[0] = GetMetricValue(metric, startDate, endDate, filters);

            DateTime[] startEnd = DateUtils.GetStartAndEndDateForPreviousPeriod(metricPeriod, startDate, endDate);

            values[1] = GetMetricValue(metric, startEnd[0], startEnd[1], filters);

            return values;
        }

        public List<BenchmarkAnalysisResponsePayload> GetBenchmarkAnalysisReponse(long reportId,Guid userId)
        {
            if (this.Configuration == null)
            {
                return new List<BenchmarkAnalysisResponsePayload>();
            }
            //Prepare request
            var reportManager = new EsiReportManager();

            var requestList = new List<MetricRequest>();

            List<MetricDefinition> metricDefinitions = new List<MetricDefinition>();
            foreach (var rr in Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId.HasValue))
            {
                metricDefinitions.Add(SIDAL.GetMetricDefinition(rr.MetricDefinitionId.Value));
            }

            MetricRequest requestObj = null;
            foreach (var cc in Configuration.ReportColumnConfigList)
            {
                requestObj = new MetricRequest();

                requestObj.ClientRefCategory = cc.DisplayName;
                requestObj.ClientRefId = cc.ColumnName;
                requestObj.StartDate = this.StartDate;
                requestObj.EndDate = this.EndDate.GetValueOrDefault();

                requestObj.MetricDefinitions = metricDefinitions;

                //filter for standard report
                var report = SIDAL.GetReportById(reportId);
                if (report.AccessType == "Standard" && report.UserId != userId)
                {
                    var districts = SIDAL.GetDistrictFilterSetting(userId);
                    if (districts != null && Filters.Districts.Count==0)
                    {
                        foreach (var district in districts)
                        {
                           Filters.Regions.Add(district.RegionId);
                           Filters.Districts.Add(Convert.ToInt32(district.DistrictId));
                        }
                    }
                }

                //Fill Filters
                requestObj.PlantIds = Filters.Plants.ToList();
                requestObj.MarketSegmentIds = Filters.MarketSegments.ToList();
                requestObj.RegionIds = Filters.Regions.ToList();
                requestObj.SalesStaffIds = Filters.SalesStaffs.ToList();
                requestObj.DistrictIds = Filters.Districts.ToList();
                requestObj.CustomerIds = Filters.Customers.ToList();

                switch (cc.EntityName)
                {
                    case "Plant":
                        requestObj.PlantIds.Add(cc.EntityRefId.GetValueOrDefault());
                        break;
                    case "Region":
                        requestObj.RegionIds.Add(cc.EntityRefId.GetValueOrDefault());
                        break;
                    case "District":
                        requestObj.DistrictIds.Add(cc.EntityRefId.GetValueOrDefault());
                        break;
                    case "MarketSegment":
                        requestObj.MarketSegmentIds.Add(cc.EntityRefId.GetValueOrDefault());
                        break;
                    case "SalesStaff":
                        requestObj.SalesStaffIds.Add(cc.EntityRefId.GetValueOrDefault());
                        break;
                    case "Customer":
                        requestObj.CustomerIds.Add(cc.EntityRefId.GetValueOrDefault());
                        break;
                }

                requestList.Add(requestObj);
            }

            //Get Response
            var esiResponse = reportManager.GetData(requestList);

            List<BenchmarkAnalysisResponsePayload> responsePayload = new List<BenchmarkAnalysisResponsePayload>();

            BenchmarkAnalysisResponsePayload payloadObj = null;

            foreach (var mDef in metricDefinitions)
            {
                payloadObj = new BenchmarkAnalysisResponsePayload();
                payloadObj.MetricName = mDef.DisplayName ?? mDef.MetricName;
                payloadObj.DisplayFormat = mDef.DefaultDisplayFormat;
                payloadObj.HasHorizontalSeparator = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.IsHorizontalSeperator).FirstOrDefault().GetValueOrDefault();
                payloadObj.ShowActionIcons = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.ShowActionIcons).FirstOrDefault().GetValueOrDefault();
                payloadObj.OkLimit = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.OkLimit).FirstOrDefault().GetValueOrDefault();
                payloadObj.CautionLimit = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.CautionLimit).FirstOrDefault().GetValueOrDefault();
                payloadObj.WarningLimit = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.WarningLimit).FirstOrDefault().GetValueOrDefault();
                payloadObj.ComparisonMetricId = Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId == mDef.Id).Select(x => x.ComparisonMetricId).FirstOrDefault().GetValueOrDefault();
                payloadObj.Values = new Dictionary<string, dynamic>();

                double dimTotal = 0;
                double? comparisonMetric = 0;
                foreach (var item in esiResponse)
                {
                    double? value = (double?)item.Values.FirstOrDefault(x => x.MetricDefinitionId == mDef.Id).Value;
                    payloadObj.Values.Add(item.ClientRefCategory, value.GetValueOrDefault(0));
                    dimTotal += value.GetValueOrDefault(0);
                    if (item.Values.FirstOrDefault(x => x.MetricDefinitionId == payloadObj.ComparisonMetricId)!=null)
                    {
                        comparisonMetric = (double?)item.Values.FirstOrDefault(x => x.MetricDefinitionId == payloadObj.ComparisonMetricId).Value;
                    }
                }

                //Add Column 'Total'
                payloadObj.Values.Add("Total", dimTotal);
                payloadObj.ComparisonValue = comparisonMetric.GetValueOrDefault();
                responsePayload.Add(payloadObj);
            }

            return responsePayload;
        }

        public List<TrendAnalysisResponsePayload> GetTrendAnalysisResponse(TrendAnalysisReportConfig configuration,long reportId,Guid userId)
        {
            if (configuration == null)
            {
                return new List<TrendAnalysisResponsePayload>();
            }

            var response = new List<TrendAnalysisResponsePayload>();
            //Prepare request
            var reportManager = new EsiReportManager();

            var requestList = new List<MetricRequest>();

            List<MetricDefinition> metricDefinitions = new List<MetricDefinition>();

            var metric = SIDAL.GetMetricDefinition(configuration.MetricDefinitionId);
            var targetMetric = SIDAL.GetMetricDefinition(configuration.TargetMetricDefinitionId.GetValueOrDefault());

            metricDefinitions.Add(metric);
            if (targetMetric != null)
            {
                metricDefinitions.Add(targetMetric);
            }

            //district filter for standard report
            var report = SIDAL.GetReportById(reportId);
            if (report.AccessType == "Standard" && report.UserId != userId)
            {
                var districts = SIDAL.GetDistrictFilterSetting(userId);
                if (districts != null && Filters.Districts.Count == 0)
                {
                    foreach (var district in districts)
                    {
                        Filters.Regions.Add(district.RegionId);
                        Filters.Districts.Add(Convert.ToInt32(district.DistrictId));
                    }
                }
            }

            List<MetricListRequest> requests = new List<MetricListRequest>();
            requests.Add(new MetricListRequest
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate.GetValueOrDefault(this.StartDate),
                MetricDefinitions = metricDefinitions,
                ColumnProperty = "Date",
                IsDateGroup = true,
                Granularity = Redhill.SalesInsight.ESI.Enumerations.RepeatPeriod.DAILY,
                PlantIds = Filters.Plants.ToList(),
                RegionIds = Filters.Regions.ToList(),
                SalesStaffIds = Filters.SalesStaffs.ToList(),
                CustomerIds = Filters.Customers.ToList(),
                MarketSegmentIds = Filters.MarketSegments.ToList(),
                DistrictIds = Filters.Districts.ToList()
            });

            var result = reportManager.GetData(requests);

            if (result != null)
            {
                TrendAnalysisResponsePayload pObj = null;
                JArray arrItem = null;
                foreach (var r in result)
                {
                    foreach (var bV in r.BucketValues)
                    {
                        pObj = new TrendAnalysisResponsePayload();
                        pObj.MetricDefinitionId = bV.MetricDefinitionId;
                        pObj.MetricName = metricDefinitions.Where(x => x.Id == bV.MetricDefinitionId).Select(x => x.DisplayName).FirstOrDefault();

                        pObj.Values = new List<JArray>();

                        foreach (var item in bV.MetricGroupBuckets.OrderBy(x => x.GroupName))
                        {
                            try
                            {
                                arrItem = new JArray();
                                var date = ((DateTime?)item.GroupName).Value;

                                arrItem.Add(DateUtils.GetMillis(date));
                                arrItem.Add(Convert.ToDouble(item.Value ?? 0));

                                pObj.Values.Add(arrItem);
                            }
                            catch { }
                        }

                        response.Add(pObj);
                    }
                }
            }
            return response;
        }

        public List<DrillInResponsePayload> GetDrillInResponse(long reportId,Guid userId)
        {
            //Prepare request
            var reportManager = new EsiReportManager();

            var requestList = new List<MetricRequest>();

            List<MetricDefinition> metricDefinitions = new List<MetricDefinition>();

            var metricDefs = SIDAL.GetAllMetricDefinition(true);

            foreach (var cc in this.Configuration.ReportColumnConfigList)
            {
                metricDefinitions.Add(metricDefs.FirstOrDefault(x => x.Id == cc.MetricDefinitionId.GetValueOrDefault()));
            }

            //district filter for standard report
            var report = SIDAL.GetReportById(reportId);
            if (report.AccessType == "Standard" && report.UserId != userId)
            {
                var districts = SIDAL.GetDistrictFilterSetting(userId);
                if (districts != null && Filters.Districts.Count == 0)
                {
                    foreach (var district in districts)
                    {
                        Filters.Regions.Add(district.RegionId);
                        Filters.Districts.Add(Convert.ToInt32(district.DistrictId));
                    }
                }
            }

            List<MetricListRequest> requests = new List<MetricListRequest>();
            
            requests.Add(new MetricListRequest
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate.GetValueOrDefault(this.StartDate),
                MetricDefinitions = metricDefinitions,
                ColumnProperty = this.DrillInReportConfiguration.DimensionName,
                IsDateGroup = false,
                //Fill Filters
                PlantIds = Filters.Plants.ToList(),
                MarketSegmentIds = Filters.MarketSegments.ToList(),
                RegionIds = Filters.Regions.ToList(),
                SalesStaffIds = Filters.SalesStaffs.ToList(),
                DistrictIds = Filters.Districts.ToList(),
                CustomerIds = Filters.Customers.ToList(),
                order = this.order,
                Limit = this.SpecialReportConfig.SortCount,
                SortType = this.SpecialReportConfig.SortType
            });

            EsiReportManager manager = new EsiReportManager();
            
            var result = manager.GetData(requests);

            List<DrillInResponsePayload> responsePayload = new List<DrillInResponsePayload>();

            DrillInResponsePayload payloadObj = null;

            //List<ReportDimensionConfig> entityList = new List<ReportDimensionConfig>();

            //entityList = SIDAL.GetEntityList(this.DrillInReportConfiguration.DimensionName);

            foreach (var item in result)
            {
                foreach (var bV in item.BucketValues)
                {
                    foreach (var mgb in bV.MetricGroupBuckets.OrderBy(x => x.GroupName))
                    {
                        try
                        {
                            payloadObj = responsePayload.FirstOrDefault(x => x.DimensionName == mgb.GroupName);
                            if (payloadObj == null)
                            {
                                payloadObj = new DrillInResponsePayload();
                                payloadObj.DimensionName = mgb.GroupName.ToString(); //entityList.Where(x => x.Id.ToString().Equals(mgb.GroupName)).Select(x => x.Name).FirstOrDefault();
                                payloadObj.DimensionRefId = this.DrillInReportConfiguration.DimensionName;
                                payloadObj.DisplayFormat = item.MetricDefinitions.FirstOrDefault(x => x.Id == bV.MetricDefinitionId).DefaultDisplayFormat;
                                responsePayload.Add(payloadObj);
                                payloadObj.Values = new Dictionary<string, dynamic>();
                            }
                            payloadObj.Values.Add(item.MetricDefinitions.FirstOrDefault(x => x.Id == bV.MetricDefinitionId).DisplayName, mgb.Value);
                        }
                        catch (Exception e)
                        {
                        }
                    }

                }
            }

            return responsePayload;
        }

        public List<DrillInDetailResponsePayloads> GetDrillInDetailResponse()
        {
            //Prepare request
            var reportManager = new EsiReportManager();

            var requestList = new List<MetricRequest>();

            List<MetricDefinition> metricDefinitions = new List<MetricDefinition>();
            foreach (var rr in Configuration.ReportRowConfigList.Where(x => x.MetricDefinitionId.HasValue))
            {
                metricDefinitions.Add(SIDAL.GetMetricDefinition(rr.MetricDefinitionId.Value));
            }

            MetricRequest requestObj = null;
            foreach (var cc in Configuration.ReportColumnConfigList)
            {
                requestObj = new MetricRequest();

                DateTime[] startEnd = DateUtils.GetStartAndEndDateForPeriodType(cc.ColumnName, this.StartDate);

                requestObj.ClientRefCategory = cc.ColumnName;
                requestObj.StartDate = startEnd[0];
                requestObj.EndDate = startEnd[1];

                requestObj.MetricDefinitions = metricDefinitions;

                //Fill Filters
                requestObj.PlantIds = Filters.Plants.ToList();
                requestObj.MarketSegmentIds = Filters.MarketSegments.ToList();
                requestObj.RegionIds = Filters.Regions.ToList();
                requestObj.SalesStaffIds = Filters.SalesStaffs.ToList();
                requestObj.DistrictIds = Filters.Districts.ToList();
                requestObj.CustomerIds = Filters.Customers.ToList();

                requestList.Add(requestObj);
            }

            //Get Response
            var esiResponse = reportManager.GetData(requestList);
            List<DrillInDetailResponsePayloads> responsePayload = new List<DrillInDetailResponsePayloads>();

            DrillInDetailResponsePayloads payloadObj = null;

            foreach (var mDef in metricDefinitions)
            {
                payloadObj = new DrillInDetailResponsePayloads();
                payloadObj.MetricName = mDef.DisplayName;
                payloadObj.DisplayFormat = mDef.DefaultDisplayFormat; 

                payloadObj.Values = new Dictionary<string, dynamic>();

                foreach (var item in esiResponse)
                {
                    payloadObj.Values.Add(item.ClientRefCategory, item.Values.FirstOrDefault(x => x.MetricDefinitionId == mDef.Id).Value);
                }

                responsePayload.Add(payloadObj);
            }

            return responsePayload;
        }

        public class PseudoEntity
        {
            public int DimensionId { get; set; }
            public string DimensionName { get; set; }
        }
    }
}