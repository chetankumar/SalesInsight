using Redhill.SalesInsight.ESI.Enumerations;
using Redhill.SalesInsight.ESI.MetricHelpers;
using Redhill.SalesInsight.ESI.Mongo.QueryBuilders;
using Redhill.SalesInsight.ESI.Mongo.QueryBuilders.MetricHelpers;
using Redhill.SalesInsight.ESI.ReportModels;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Mongo.Models;
using RedHill.SalesInsight.ESI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Redhill.SalesInsight.ESI
{
    public class EsiReportManager
    {
        public List<MetricListRequest> GetData(List<MetricListRequest> requestList)
        {
            foreach (var request in requestList)
            {
                ProcessRequest(request);
            }
            return requestList;
        }

        public List<MetricRequest> GetData(List<MetricRequest> requestList)
        {
            foreach (var request in requestList)
            {
                ProcessRequest(request);
            }
            return requestList;
        }

        public List<TicketStats> GetTicketData(MetricRequest request, out long count)
        {
            List<MongoFilter> filters = ParseRawMongoFilters(request);

            MongoQuery query = new MongoQuery() { CollectionName = ESIDataManager.TicketStatsTable, Filters = filters };

            ESIDataManager manager = new ESIDataManager();
            count = 0;
            return manager.GetRawData(query, out count);
        }

        private void ProcessRequest(MetricListRequest request)
        {
            List<IMetricHelper> helpers = InstantiateHelpers(request.MetricDefinitions);
            List<MongoFilter> filters = ParseMongoFilters(request);

            List<MongoGroupQuery> queries = ParseMetricGroupRequest(helpers, filters, request);
            ESIDataManager manager = new ESIDataManager();
            foreach (var query in queries)
                manager.ProcessMetricGroupRequest(query);
            var allResults = new List<Aggregation>();
            foreach (var query in queries)
            {
                allResults.AddRange(query.AggregationList);
            }
            request.BucketValues = new List<MetricGroupResults>();
            foreach (var helper in helpers)
            {
                MetricGroupResults bucket = new MetricGroupResults();
                bucket.MetricDefinitionId = helper.HelperFor().Id;
                bucket.MetricGroupBuckets = helper.ProcessGroupValues(allResults);
                request.BucketValues.Add(bucket);
            }
        }

        private void ProcessRequest(MetricRequest request)
        {
            List<IMetricHelper> helpers = InstantiateHelpers(request.MetricDefinitions);
            List<MongoFilter> filters = ParseMongoFilters(request);
            // For each source, we will need a different Mongo Query
            List<MongoQuery> queries = ParseMetricRequest(helpers, filters);

            ESIDataManager manager = new ESIDataManager();
            foreach (var query in queries)
                manager.ProcessMetricRequest(query);

            var allResults = new List<Aggregation>();
            foreach (var query in queries)
            {
                allResults.AddRange(query.AggregationList);
            }
            request.Values = new List<MetricWiseBucket>();
            foreach (var helper in helpers)
            {
                MetricWiseBucket bucket = new MetricWiseBucket();
                bucket.MetricDefinitionId = helper.HelperFor().Id;
                bucket.Value = helper.ProcessValue(allResults);
                request.Values.Add(bucket);
            }
        }

        private List<IMetricHelper> InstantiateHelpers(List<MetricDefinition> metricDefinitions)
        {
            List<IMetricHelper> metricHelpers = new List<IMetricHelper>();
            foreach (var definition in metricDefinitions)
            {
                metricHelpers.Add(MetricHelperFactory.GetHelperFor(definition));
            }
            return metricHelpers;
        }

        private List<MongoQuery> ParseMetricRequest(List<IMetricHelper> metricHelpers, List<MongoFilter> filters)
        {
            List<MongoQuery> queries = new List<MongoQuery>();
            List<Aggregation> aggs = new List<Aggregation>();
            foreach (var helper in metricHelpers)
            {
                aggs.AddRange(helper.GetAggregations());
            }
            aggs = aggs.Distinct(new AggregationComparer()).ToList();

            if (aggs != null && aggs.Count > 0)
            {
                var ticketAggregations = aggs.GroupBy(x => x.DataSource);
                if (ticketAggregations.Any())
                {
                    foreach (var group in ticketAggregations)
                    {
                        queries.Add(new MongoQuery() { AggregationList = group.ToList(), CollectionName = group.Key, Filters = filters });
                    }
                }
            }
            return queries;
        }
        private List<MongoGroupQuery> ParseMetricGroupRequest(List<IMetricHelper> helpers, List<MongoFilter> filters, MetricListRequest request)
        {
            List<MongoGroupQuery> queries = new List<MongoGroupQuery>();
            List<Aggregation> aggs = new List<Aggregation>();
            foreach (var helper in helpers)
            {
                aggs.AddRange(helper.GetAggregations());
            }
            aggs = aggs.Distinct(new AggregationComparer()).ToList();

            if (aggs != null && aggs.Count > 0)
            {
                var aggregations = aggs.GroupBy(x => x.DataSource);
                if (aggregations.Any())
                {
                    foreach (var group in aggregations)
                    {
                        queries.Add(new MongoGroupQuery() { AggregationList = group.ToList(), CollectionName = group.Key, Filters = filters, IsDateGroup = request.IsDateGroup, ColumnProperty = request.ColumnProperty, Granularity = request.Granularity });
                    }
                }
            }
            return queries;
        }

        private List<MongoFilter> ParseMongoFilters(MetricRequest request)
        {
            List<MongoFilter> filters = new List<MongoFilter>();

            if (request.PlantIds != null && request.PlantIds.Any())
            {
                filters.Add(new MongoFilter() { PropertyName = "PlantId", ComparisionType = ComparisionType.In, Value = request.PlantIds });
            }
            if (request.DistrictIds != null && request.DistrictIds.Any())
            {
                filters.Add(new MongoFilter() { PropertyName = "DistrictId", ComparisionType = ComparisionType.In, Value = request.DistrictIds });
            }
            if (request.RegionIds != null && request.RegionIds.Any())
            {
                filters.Add(new MongoFilter() { PropertyName = "RegionId", ComparisionType = ComparisionType.In, Value = request.RegionIds });
            }
            if (request.CustomerIds != null && request.CustomerIds.Any())
            {
                filters.Add(new MongoFilter() { PropertyName = "CustomerId", ComparisionType = ComparisionType.In, Value = request.CustomerIds });
            }
            if (request.MarketSegmentIds != null && request.MarketSegmentIds.Any())
            {
                filters.Add(new MongoFilter() { PropertyName = "MarketSegmentId", ComparisionType = ComparisionType.In, Value = request.MarketSegmentIds });
            }
            if (request.SalesStaffIds != null && request.SalesStaffIds.Any())
            {
                filters.Add(new MongoFilter() { PropertyName = "SalesStaffId", ComparisionType = ComparisionType.In, Value = request.SalesStaffIds });
            }

            filters.Add(new MongoFilter() { PropertyName = "Date", ComparisionType = ComparisionType.Range, Value = request.StartDate, Value2 = request.EndDate, SkipRecords = request.Skip, ItemsPerPage = request.Limit ,order = request.order , search= request.search,SortType=request.SortType});

            return filters;
        }
        private List<MongoFilter> ParseRawMongoFilters(MetricRequest request)
        {
            List<MongoFilter> filters = ParseMongoFilters(request);

            if (request.DriverIds != null && request.DriverIds.Any())
            {
                if (filters == null)
                    filters = new List<MongoFilter>();
                filters.Add(new MongoFilter() { PropertyName = "DriverId", ComparisionType = ComparisionType.In, Value = request.DriverIds });
            }

            return filters;
        }
    }
}
