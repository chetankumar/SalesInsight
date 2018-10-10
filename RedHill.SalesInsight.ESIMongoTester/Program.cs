using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedHill.SalesInsight.DAL;
using Redhill.SalesInsight.ESI;
using System.Diagnostics;
using Utils;
using Redhill.SalesInsight.ESI.ReportModels;

namespace RedHill.SalesInsight.ESIMongoTester
{
    class Program
    {
        public DateTime Date { get; set; }

        //{ 33, 10, 11, 34, 31, 25, 27, 29, 14, 21, 23, 19, 32, 26, 28, 30, 15, 22, 24, 20, 12, 13, 16, 17 };
        static List<long> AllWidgetIds = new List<long>() { 33, 10, 11, 34, 31, 25, 27, 29, 14, 21, 23, 19, 32, 26, 28, 30, 15, 22, 20, 12, 13, 16, 17 };

        static string[] ReportHeader = new string[] { "WidgetId", "WigetName", "Primary Metric", "comparison Martic", "Bar Graph", "Frequency Distribution", "Line Graph", "POP Summary" };

        static List<long> PlantId = new List<long>() { };
        static List<long> CustomerId = new List<long>() { };
        static List<long> MarketSegmentId = new List<long>() { 18 };
        static List<long> SalesStaffId = new List<long>() { };
        static List<long> RegionId = new List<long>() { 12 };
        static List<long> DistrictId = new List<long>() { };

        static int tableWidth = 150;

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static bool colorSwitch = true;

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }
            if (colorSwitch)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;
                colorSwitch = false;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.White;
                colorSwitch = true;
            }
            Console.WriteLine(row);
            Console.ResetColor();
        }

        static string AlignCentre(string text, int width)
        {
            text = text == null ? "" : text;
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        static double GetMetricValue(MetricDefinition metric, DateTime startDate, DateTime endDate, Dictionary<string, List<long>> filters = null)
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

        static double[] GetPOPSummaryValues(MetricDefinition metric, string metricPeriod, DateTime startDate, DateTime endDate, Dictionary<string, List<long>> filters)
        {
            double[] values = new double[2];

            //Get the Current Value
            values[0] = GetMetricValue(metric, startDate, endDate, filters);

            DateTime[] startEnd = DateUtils.GetStartAndEndDateForPreviousPeriod(metricPeriod, startDate, endDate);

            values[1] = GetMetricValue(metric, startEnd[0], startEnd[1], filters);

            return values;
        }

        static List<double> GetMetricValues(MetricDefinition metric, DateTime startDate, DateTime endDate, Dictionary<string, List<long>> filters = null)
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

        static void PrintFilters(Dictionary<string, List<long>> filters, DateTime date)
        {
            foreach (var item in filters)
            {
                string filterNames;
                filterNames = item.Key + " : ";
                foreach (var itemIds in item.Value)
                {
                    filterNames += itemIds + " ";
                }
                Console.WriteLine(filterNames);
            }
            Console.WriteLine("Date : " + date);
        }

        static void ReportGenerator( List<long> inputWidgetId = null)
        {
            Stopwatch watch, watch1, watch2, watch3, watch4, watch5, watch6, watch7, watch8, watch9;

            Dictionary<string, List<long>> filters = new Dictionary<string, List<long>>();
            filters.Add("Plant", PlantId);
            filters.Add("Customer", CustomerId);
            filters.Add("MarketSegment", MarketSegmentId);
            filters.Add("SalesStaff", SalesStaffId);
            filters.Add("Region", RegionId);
            filters.Add("District", DistrictId);

            Program program = new Program();
            program.Date = Convert.ToDateTime("05/03/2016 12:00:00 AM");
            PrintFilters(filters, program.Date);

            PrintLine();
            PrintRow(ReportHeader);
            PrintLine();

            //StringBuilder timeElapsedOutput;
            string[] rowData;
            List<long> widgetIds = new List<long>();
            if (inputWidgetId != null)
            {
                widgetIds = inputWidgetId;
            }
            else
            {
                 widgetIds = AllWidgetIds;
            }
            foreach (var widgetId in widgetIds)
            {
                rowData = new string[8];
                //timeElapsedOutput = new StringBuilder();
                var widget = SIDAL.GetWidgetSettings(widgetId);
                //timeElapsedOutput.Append("[ " + widget.Title + "   " + widgetId + "]" + Environment.NewLine);
                rowData[0] = widgetId.ToString();
                rowData[1] = widget.Title;
                DateTime[] startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, program.Date);
                var metricDefinition = SIDAL.GetMetricDefinition(widget.PrimaryMetricDefinitionId.GetValueOrDefault());
                var comparisonMetricDef = SIDAL.GetMetricDefinition(widget.ComparisonMetricDefinitionId.GetValueOrDefault());

                watch = new Stopwatch();
                watch.Start();
                GetMetricValue(metricDefinition, startEnd[0], startEnd[1], filters);
                watch.Stop();
                //timeElapsedOutput.Append("Metric definition (GetMetricValue) : " + watch.ElapsedMilliseconds + " ms" + Environment.NewLine);
                rowData[2] = watch.ElapsedMilliseconds + " ms ("+widget.PrimaryMetricPeriod+")";
                if (comparisonMetricDef != null)
                {
                    startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.ComparisonMetricPeriod, program.Date);
                    watch1 = new Stopwatch();
                    watch1.Start();
                    GetMetricValue(comparisonMetricDef, startEnd[0], startEnd[1], filters);
                    watch1.Stop();
                    rowData[3] = watch1.ElapsedMilliseconds + " ms (" + widget.ComparisonMetricPeriod + ")";
                    //timeElapsedOutput.Append(string.Format("Comparison Metric Def (GetMetricValue) : {2}", widget.Title, widgetId, watch1.ElapsedMilliseconds + " ms" + Environment.NewLine));
                }

                if (widget.HasBarGraph.GetValueOrDefault())
                {
                    startEnd[0] = program.Date.AddDays(-16);
                    startEnd[1] = program.Date;

                    watch2 = new Stopwatch();
                    watch2.Start();
                    GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters);
                    watch2.Stop();
                    rowData[4] = watch2.ElapsedMilliseconds + " ms";
                    //timeElapsedOutput.Append(string.Format("Has Bar Graph (GetMetricValues) : {2}", widget.Title, widgetId, watch2.ElapsedMilliseconds + " ms" + Environment.NewLine));
                }
                else if (widget.HasFrequencyDistribution.GetValueOrDefault())
                {
                    startEnd[0] = program.Date.AddDays(-60);
                    startEnd[1] = program.Date;

                    watch3 = new Stopwatch();
                    watch3.Start();
                    GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters);
                    watch3.Stop();
                    rowData[5] = watch3.ElapsedMilliseconds + " ms";
                    //timeElapsedOutput.Append(string.Format("Has Frequency Distribution (GetMetricValues) : {2}", widget.Title, widgetId, watch3.ElapsedMilliseconds + " ms" + Environment.NewLine));
                }
                else if (widget.HasLineGraph.GetValueOrDefault())
                {
                    startEnd[0] = program.Date.AddDays(-widget.LineGraphRangeInDays.GetValueOrDefault(60));
                    startEnd[1] = program.Date;

                    watch4 = new Stopwatch();
                    watch4.Start();
                    GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters);
                    watch4.Stop();
                    rowData[6] = watch4.ElapsedMilliseconds + " ms";
                    //timeElapsedOutput.Append(string.Format("Has Line Graph (GetMetricValues) : {2}", widget.Title, widgetId, watch4.ElapsedMilliseconds + " ms" + Environment.NewLine));
                }
                else if (widget.HasPOPSummary.GetValueOrDefault())
                {
                    startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, program.Date);

                    watch5 = new Stopwatch();
                    watch5.Start();
                    GetPOPSummaryValues(metricDefinition, widget.PrimaryMetricPeriod, startEnd[0], startEnd[1], filters);
                    watch5.Stop();
                    rowData[7] = watch5.ElapsedMilliseconds + " ms";
                    //timeElapsedOutput.Append(string.Format("Has POP Summary (GetPOPSummaryValues) : {2}", widget.Title, widgetId, watch5.ElapsedMilliseconds + " ms" + Environment.NewLine));
                }
                //Console.WriteLine(timeElapsedOutput);
                PrintRow(rowData);
                //Console.WriteLine("------------------------------------------------------------------");
            }
            PrintLine();

            Console.WriteLine("\n");
            Console.WriteLine("Generate report (y/WidgetId/n)");
            string isGenerate = Console.ReadLine();
            List<long> inputValue;
            if (isGenerate == "y")
            {
                ReportGenerator();
            }
            else if (isGenerate != "n")
            {
                 long widgetId = Convert.ToInt64(isGenerate);

                inputValue = new List<long>() { widgetId };
                ReportGenerator(inputValue);
            }
        }

        static void Main(string[] args)
        {
            ReportGenerator();
        }
    }
}
