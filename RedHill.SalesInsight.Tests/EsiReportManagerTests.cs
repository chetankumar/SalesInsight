using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Redhill.SalesInsight.ESI.ReportModels;
using RedHill.SalesInsight.DAL;
using Redhill.SalesInsight.ESI;
using System.Linq;
using Redhill.SalesInsight.ESI.Mongo.QueryBuilders;
using RedHill.SalesInsight.ESI;
using System.Diagnostics;

namespace RedHill.SalesInsight.Tests
{
    [TestClass]
    public class EsiReportManagerTests
    {
        [TestMethod]
        public void GetDataTest()
        {
            List<MetricRequest> requests = new List<MetricRequest>();

            requests.Add(new MetricRequest
            {
                PlantIds = new List<long> { 141, 144 },
                CustomerIds = new List<long> { 6284 },
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                MetricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "Volume", "Startup", "Shutdown" })
            });

            requests.Add(new MetricRequest
            {
                PlantIds = new List<long> { 141, 144 },
                CustomerIds = new List<long> { 6284 },
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 05, 31),
                MetricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "Volume", "Startup", "Shutdown" })
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result[0].Values.Count);
        }
        [TestMethod]
        public void FormulaMetricRequest()
        {
            List<MetricRequest> requests = new List<MetricRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "VolumeVariance" });
            requests.Add(new MetricRequest
            {
                CustomerIds = new List<long> { 6284 },
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                MetricDefinitions = metricDefinitions
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            Assert.IsNotNull(result);
            Assert.IsTrue(result[0].Values.Any(x => x.MetricDefinitionId == metricDefinitions.FirstOrDefault().Id));
        }
        [TestMethod]
        public void BudgetStartupVolumeWeightTest()
        {
            List<MetricRequest> requests = new List<MetricRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "BudgetStartup" });
            requests.Add(new MetricRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                MetricDefinitions = metricDefinitions
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            Assert.IsNotNull(result);
            Assert.IsTrue(result[0].Values.Any(x => x.MetricDefinitionId == metricDefinitions.FirstOrDefault().Id));
        }
        [TestMethod]
        public void ProcessMetricRequestList()
        {
            MongoGroupQuery query = new MongoGroupQuery();
            query.CollectionName = "TicketStats";
            query.AggregationList = new List<Aggregation>();
            query.AggregationList.Add(new Aggregation(SIDAL.GetMetricDefinitionByName("Volume")));
            query.ColumnProperty = "Date";
            query.IsDateGroup = true;
            query.Granularity = Redhill.SalesInsight.ESI.Enumerations.RepeatPeriod.MONTHLY;
            ESIDataManager manager = new ESIDataManager();
            manager.ProcessMetricGroupRequest(query);

            Assert.IsNotNull(query);

        }
        [TestMethod]
        public void CYDHrGroupTest()
        {
            List<MetricListRequest> requests = new List<MetricListRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "CYDHr", "BudgetStartup" });
            requests.Add(new MetricListRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                MetricDefinitions = metricDefinitions,
                ColumnProperty = "Date",
                IsDateGroup = true,
                Granularity = Redhill.SalesInsight.ESI.Enumerations.RepeatPeriod.MONTHLY                
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            Assert.IsNotNull(result);

        }
        [TestMethod]
        public void CYDHrPlantGroupTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<MetricListRequest> requests = new List<MetricListRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] {"Volume","Startup","Shutdown", "CYDHr", "BudgetStartup" });
            requests.Add(new MetricListRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                MetricDefinitions = metricDefinitions,
                ColumnProperty = "PlantName",
                IsDateGroup = false
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            watch.Stop();

            Assert.IsNotNull(result);

        }
        [TestMethod]
        public void GoalAnalysisTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();


            List<MetricRequest> requests = new List<MetricRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "Volume", "BudgetVolume", "VolumeVariance", "CYDHr", "BudgetCYDHr","CYDHrVariance" });


            requests.Add(new MetricRequest
            {
                StartDate = new DateTime(2016, 1, 1),// StartDate and End Date should be same for Day
                EndDate = new DateTime(2016, 12, 31),//  StartDate and End Date should be same for Day
                MetricDefinitions = metricDefinitions,
                ClientRefCategory = "Day"
            });

            requests.Add(new MetricRequest
            {
                StartDate = new DateTime(2016, 1, 1),// Change Start Date According to MTD
                EndDate = new DateTime(2016, 12, 31),// Change End Date According to MTD
                MetricDefinitions = metricDefinitions,
                ClientRefCategory = "MTD"
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            watch.Stop();

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void BenchMarkTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();


            List<MetricRequest> requests = new List<MetricRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "Volume", "BudgetVolume", "VolumeVariance", "CYDHr", "BudgetCYDHr" });

            requests.Add(new MetricRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                DistrictIds = new List<long> { 26 },
                MetricDefinitions = metricDefinitions,
                ClientRefCategory = "Bullhead City District"
            });
            requests.Add(new MetricRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                PlantIds = new List<long> { 166 },
                MetricDefinitions = metricDefinitions,
                ClientRefCategory = "Blue Diamond Plant"
            });

            requests.Add(new MetricRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                PlantIds = new List<long> { 165 },
                MetricDefinitions = metricDefinitions,
                ClientRefCategory = "Anthem Plant"
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            watch.Stop();

            Assert.IsNotNull(result);

        }
        [TestMethod]
        public void TrendAnalysisTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<MetricListRequest> requests = new List<MetricListRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "Volume" });
            requests.Add(new MetricListRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 1, 31),
                MetricDefinitions = metricDefinitions,
                ColumnProperty = "Date",
                IsDateGroup = true,
                Granularity = Redhill.SalesInsight.ESI.Enumerations.RepeatPeriod.DAILY
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            watch.Stop();

            Assert.IsNotNull(result);

        }
        [TestMethod]
        public void DrillInTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<MetricListRequest> requests = new List<MetricListRequest>();
            var metricDefinitions = SIDAL.GetMetricDefinitions(new string[] { "Volume", "Startup", "Shutdown", "CYDHr", "BudgetStartup" });
            requests.Add(new MetricListRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                MetricDefinitions = metricDefinitions,
                ColumnProperty = "DriverNumber",
                IsDateGroup = false
            });

            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetData(requests);

            watch.Stop();

            Assert.IsNotNull(result);

        }
        [TestMethod]
        public void GetTicketDataTest()
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();
            MetricRequest request = new MetricListRequest
            {
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 12, 31),
                SalesStaffIds= new List<long> { 72},
                PlantIds = new List<long> { 140, 141 },
                DriverIds = new List<long> { 1001 }
            };
            long count = 0;
            EsiReportManager manager = new EsiReportManager();
            var result = manager.GetTicketData(request,out count);

            watch.Stop();

            Assert.IsNotNull(result);

        }
    }
}
