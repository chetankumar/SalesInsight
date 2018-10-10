using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redhill.SalesInsight.ESI;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Constants;
using RedHill.SalesInsight.DAL.Utilities;
using RedHill.SalesInsight.Web.Html5.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utils;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI.Payload
{
    public class WidgetResponsePayload
    {
        public DashboardFilterSettingView Filters { get; set; }

        public WidgetResponsePayload(long widgetId, DateTime? date)
        {
            this.WidgetId = widgetId;
            this.Date = date.GetValueOrDefault(DateTime.Today);
        }

        public DateTime Date { get; set; }

        public long WidgetId { get; set; }

        //public string GetResponse()
        //{
        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();
        //    Dictionary<string, List<long>> filters = new Dictionary<string, List<long>>();

        //    filters.Add("Plant", this.Filters.Plants);
        //    filters.Add("Customer", this.Filters.Customers);
        //    filters.Add("MarketSegment", this.Filters.MarketSegments);
        //    filters.Add("SalesStaff", this.Filters.SalesStaffs);
        //    filters.Add("Region", this.Filters.Regions);
        //    filters.Add("District", this.Filters.Districts);

        //    var widget = SIDAL.GetWidgetSettings(this.WidgetId);
        //    JsonResponse errorResponse = new JsonResponse();
        //    if (widget == null)
        //    {
        //        errorResponse.Message = "Failed to retrieve widget data";
        //        errorResponse.Success = false;
        //        return errorResponse.ToString();
        //    }

        //    DateTime[] startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, this.Date);

        //    var esiReportBroker = new EsiReportBroker();

        //    var metricDefinition = SIDAL.GetMetricDefinition(widget.PrimaryMetricDefinitionId.GetValueOrDefault());
        //    var comparisonMetricDef = SIDAL.GetMetricDefinition(widget.ComparisonMetricDefinitionId.GetValueOrDefault());

        //    //get Metric values
        //    var metricValue = esiReportBroker.GetMetricValue(metricDefinition, startEnd[0], startEnd[1], filters);

        //    metricValue = Double.IsNaN(metricValue) ? 0 : metricValue;
        //    //Generate response
        //    dynamic obj = new JObject(), data = new JObject(), primaryMetric = new JObject(), primaryValues = new JObject(), comparisonMetric = new JObject(), visualIndication = new JObject();

        //    primaryValues.Actual = metricValue;
        //    primaryValues.Formatted = Math.Round(metricValue, widget.DecimalPlaces.GetValueOrDefault(0)).ToString();

        //    primaryMetric.Id = metricDefinition.Id;
        //    primaryMetric.Name = metricDefinition.DisplayName;
        //    primaryMetric.Value = primaryValues;

        //    data.PrimaryMetric = primaryMetric;

        //    double comparisonMetricValue = 0;
        //    if (comparisonMetricDef != null)
        //    {
        //        startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.ComparisonMetricPeriod, this.Date);
        //        comparisonMetricValue = esiReportBroker.GetMetricValue(comparisonMetricDef, startEnd[0], startEnd[1], filters);

        //        comparisonMetricValue = Double.IsNaN(comparisonMetricValue) ? 0 : comparisonMetricValue;

        //        comparisonMetric.Id = comparisonMetricDef.Id;
        //        comparisonMetric.Name = comparisonMetricDef.DisplayName;
        //        comparisonMetric.Value = new JObject();

        //        comparisonMetric.Value.Actual = comparisonMetricValue;
        //        comparisonMetric.Value.Formatted = Math.Round(comparisonMetricValue, widget.DecimalPlaces.GetValueOrDefault(0)).ToString();

        //        data.ComparisonMetric = comparisonMetric;
        //    }

        //    data.Variance = new JObject();

        //    data.Variance.Value = new JObject();
        //    var variance = (metricValue - comparisonMetricValue);

        //    variance = Double.IsNaN(variance) ? 0 : variance;

        //    data.Variance.Value.Actual = variance;
        //    data.Variance.Value.Formatted = Math.Round(variance, widget.DecimalPlaces.GetValueOrDefault(2)).ToString();

        //    data.Variance.ActionIndicator = new JObject();

        //    var indication = "None";

        //    if (widget.ShowActionIcon.GetValueOrDefault())
        //    {
        //        var okLimit = Convert.ToDouble(widget.SuccessLimitPercent.GetValueOrDefault(0));
        //        var cautionLimit = Convert.ToDouble(widget.AlertLimitPercent.GetValueOrDefault(0));
        //        var okThreshold = (okLimit * (comparisonMetricValue / 100));
        //        var cautionThreshold = (cautionLimit * (comparisonMetricValue / 100));

        //        if (widget.MetricType == "higherbetter")
        //        {
        //            if (metricValue >= (comparisonMetricValue - okThreshold))
        //            {
        //                indication = "Ok";
        //            }
        //            else if (metricValue >= (comparisonMetricValue - cautionThreshold))
        //            {
        //                indication = "Alert";
        //            }
        //            else
        //            {
        //                indication = "Danger";
        //            }
        //        }
        //        else if (widget.MetricType == "lowerbetter")
        //        {
        //            if (metricValue <= (comparisonMetricValue + okThreshold))
        //            {
        //                indication = "Ok";
        //            }
        //            else if (metricValue <= (comparisonMetricValue + cautionThreshold))
        //            {
        //                indication = "Alert";
        //            }
        //            else
        //            {
        //                indication = "Danger";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        indication = "None";
        //    }

        //    data.Variance.ActionIndicator.Indication = indication;

        //    obj.WidgetId = this.WidgetId;
        //    obj.Data = data;

        //    visualIndication.Config = new JObject();
        //    visualIndication.Config.BarGraph = new JObject();
        //    visualIndication.Config.FrequencyDistribution = new JObject();
        //    visualIndication.Config.LineGraph = new JObject();
        //    visualIndication.Config.POPSummary = new JObject();
        //    visualIndication.Config.StaticMessage = new JObject();

        //    if (widget.HasBarGraph.GetValueOrDefault())
        //    {
        //        visualIndication.Type = "BarGraph";
        //        visualIndication.Config.BarGraph.Data = new JArray();

        //        startEnd[0] = this.Date.AddDays(-16);
        //        startEnd[1] = this.Date;

        //        esiReportBroker.GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters)
        //                       .ForEach(x => visualIndication.Config.BarGraph.Data.Add(x));
        //    }
        //    else if (widget.HasFrequencyDistribution.GetValueOrDefault())
        //    {
        //        visualIndication.Type = "FrequencyDistribution";

        //        visualIndication.Config.FrequencyDistribution.Data = new JArray();

        //        startEnd[0] = this.Date.AddDays(-60);
        //        startEnd[1] = this.Date;

        //        esiReportBroker.GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters)
        //                       .ForEach(x => visualIndication.Config.FrequencyDistribution.Data.Add(x));
        //    }
        //    else if (widget.HasLineGraph.GetValueOrDefault())
        //    {
        //        visualIndication.Type = "LineGraph";
        //        visualIndication.Config.LineGraph.Data = new JArray();

        //        startEnd[0] = this.Date.AddDays(-widget.LineGraphRangeInDays.GetValueOrDefault(60));
        //        startEnd[1] = this.Date;

        //        esiReportBroker.GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters)
        //                       .ForEach(x => visualIndication.Config.LineGraph.Data.Add(double.IsNaN(x) ? 0 : x));
        //    }
        //    else if (widget.HasPOPSummary.GetValueOrDefault())
        //    {
        //        visualIndication.Type = "POPSummary";
        //        visualIndication.Config.POPSummary.Data = new JObject();

        //        startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, this.Date);

        //        double[] values = esiReportBroker.GetPOPSummaryValues(metricDefinition, widget.PrimaryMetricPeriod, startEnd[0], startEnd[1], filters);

        //        if (values != null && values.Length > 0)
        //        {
        //            values[0] = Double.IsNaN(values[0]) ? 0 : values[0];

        //            visualIndication.Config.POPSummary.Data.Current = values[0];

        //            if (values.Length > 1)
        //            {
        //                values[1] = Double.IsNaN(values[1]) ? 0 : values[1];

        //                visualIndication.Config.POPSummary.Data.Previous = values[1];
        //            }
        //        }

        //        visualIndication.Config.POPSummary.Data.Variance = new JObject();

        //        var popVariance = values[0] - values[1];

        //        popVariance = Double.IsNaN(popVariance) ? 0 : popVariance;

        //        visualIndication.Config.POPSummary.Data.Variance.Actual = popVariance;
        //        visualIndication.Config.POPSummary.Data.Variance.Formatted = Math.Round(Math.Abs(popVariance), widget.DecimalPlaces.GetValueOrDefault(2)).ToString();

        //    }
        //    else if (widget.HasStaticMessage.GetValueOrDefault())
        //    {
        //        visualIndication.Type = "StaticMessage";
        //        visualIndication.Config.StaticMessage.Message = widget.StaticMessage;
        //    }
        //    obj.VisualIndication = visualIndication;


        //    obj.Stats = new JObject();
        //    watch.Stop();

        //    obj.Stats.Took = watch.ElapsedMilliseconds / 60;

        //    return obj.ToString();
        //}

        public string GetResponse()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Dictionary<string, List<long>> filters = new Dictionary<string, List<long>>();

            filters.Add("Plant", this.Filters.Plants);
            filters.Add("Customer", this.Filters.Customers);
            filters.Add("MarketSegment", this.Filters.MarketSegments);
            filters.Add("SalesStaff", this.Filters.SalesStaffs);
            filters.Add("Region", this.Filters.Regions);
            filters.Add("District", this.Filters.Districts);

            var widget = SIDAL.GetWidgetSettings(this.WidgetId);
            JsonResponse errorResponse = new JsonResponse();
            if (widget == null)
            {
                errorResponse.Message = "Failed to retrieve widget data";
                errorResponse.Success = false;
                return errorResponse.ToString();
            }

            DateTime[] startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, this.Date);

            var esiReportBroker = new EsiReportBroker();

            var metricDefinition = SIDAL.GetMetricDefinition(widget.PrimaryMetricDefinitionId.GetValueOrDefault());
            var comparisonMetricDef = SIDAL.GetMetricDefinition(widget.ComparisonMetricDefinitionId.GetValueOrDefault());

            //get Metric values
            var metricValue = esiReportBroker.GetMetricValue(metricDefinition, startEnd[0], startEnd[1], filters);

            metricValue = Double.IsNaN(metricValue) ? 0 : metricValue;
            //Generate response
            dynamic obj = new JObject(), data = new JObject(), primaryMetric = new JObject(), primaryValues = new JObject(), comparisonMetric = new JObject(), visualIndication = new JObject();


            primaryValues.Actual = metricValue;
            primaryValues.Formatted = string.Format("{0:n" + widget.DecimalPlaces.GetValueOrDefault(0) + "}", metricValue);//Math.Round(metricValue, widget.DecimalPlaces.GetValueOrDefault(0)).ToString();

            primaryMetric.Id = metricDefinition.Id;
            primaryMetric.Name = metricDefinition.DisplayName;
            primaryMetric.Value = primaryValues;

            data.PrimaryMetric = primaryMetric;

            double comparisonMetricValue = 0;
            if (comparisonMetricDef != null)
            {
                startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.ComparisonMetricPeriod, this.Date);
                comparisonMetricValue = esiReportBroker.GetMetricValue(comparisonMetricDef, startEnd[0], startEnd[1], filters);

                comparisonMetricValue = Double.IsNaN(comparisonMetricValue) ? 0 : comparisonMetricValue;

                comparisonMetric.Id = comparisonMetricDef.Id;
                comparisonMetric.Name = comparisonMetricDef.DisplayName;
                comparisonMetric.Value = new JObject();

                comparisonMetric.Value.Actual = comparisonMetricValue;
                comparisonMetric.Value.Formatted = Math.Round(comparisonMetricValue, widget.DecimalPlaces.GetValueOrDefault(0)).ToString();

                data.ComparisonMetric = comparisonMetric;
            }

            data.Variance = new JObject();

            data.Variance.Value = new JObject();
            var variance = (metricValue - comparisonMetricValue);

            variance = Double.IsNaN(variance) ? 0 : variance;

            data.Variance.Value.Actual = variance;
            data.Variance.Value.Formatted = Math.Round(variance, widget.DecimalPlaces.GetValueOrDefault(2)).ToString();

            data.Variance.ActionIndicator = new JObject();

            var indication = "None";

            if (widget.ShowActionIcon.GetValueOrDefault())
            {
                var okLimit = Convert.ToDouble(widget.SuccessLimitPercent.GetValueOrDefault(0));
                var cautionLimit = Convert.ToDouble(widget.AlertLimitPercent.GetValueOrDefault(0));
                var okThreshold = (okLimit * (comparisonMetricValue / 100));
                var cautionThreshold = (cautionLimit * (comparisonMetricValue / 100));

                if (widget.MetricType == "higherbetter")
                {
                    if (metricValue >= (comparisonMetricValue - okThreshold))
                    {
                        indication = "Ok";
                    }
                    else if (metricValue >= (comparisonMetricValue - cautionThreshold))
                    {
                        indication = "Alert";
                    }
                    else
                    {
                        indication = "Danger";
                    }
                }
                else if (widget.MetricType == "lowerbetter")
                {
                    if (metricValue <= (comparisonMetricValue + okThreshold))
                    {
                        indication = "Ok";
                    }
                    else if (metricValue <= (comparisonMetricValue + cautionThreshold))
                    {
                        indication = "Alert";
                    }
                    else
                    {
                        indication = "Danger";
                    }
                }
            }
            else
            {
                indication = "None";
            }

            data.Variance.ActionIndicator.Indication = indication;

            obj.WidgetId = this.WidgetId;
            obj.Data = data;

            visualIndication.Config = new JObject();
            visualIndication.Config.BarGraph = new JObject();
            visualIndication.Config.FrequencyDistribution = new JObject();
            visualIndication.Config.LineGraph = new JObject();
            visualIndication.Config.POPSummary = new JObject();
            visualIndication.Config.StaticMessage = new JObject();

            if (widget.HasBarGraph.GetValueOrDefault())
            {
                visualIndication.Type = "BarGraph";
                visualIndication.Config.BarGraph.Data = new JArray();

                // startEnd[0] = this.Date.AddDays(-16);
                // startEnd[1] = this.Date;
                startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, this.Date);

                esiReportBroker.GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters)
                               .ForEach(x => visualIndication.Config.BarGraph.Data.Add(x));
            }
            else if (widget.HasFrequencyDistribution.GetValueOrDefault())
            {
                visualIndication.Type = "FrequencyDistribution";

                visualIndication.Config.FrequencyDistribution.Data = new JArray();

                //startEnd[0] = this.Date.AddDays(-60);
                //startEnd[1] = this.Date;
                startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, this.Date);

                esiReportBroker.GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters)
                               .ForEach(x => visualIndication.Config.FrequencyDistribution.Data.Add(x));
            }
            else if (widget.HasLineGraph.GetValueOrDefault())
            {
                visualIndication.Type = "LineGraph";
                visualIndication.Config.LineGraph.Data = new JArray();

                //startEnd[0] = this.Date.AddDays(-widget.LineGraphRangeInDays.GetValueOrDefault(60));
                //startEnd[1] = this.Date;

                startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, this.Date);

                if ((startEnd[1] - startEnd[0]).Days < widget.LineGraphRangeInDays.GetValueOrDefault())
                {
                    startEnd[0] = startEnd[1].AddDays(-1 * (Math.Abs(widget.LineGraphRangeInDays.GetValueOrDefault())));
                }

                esiReportBroker.GetMetricValues(metricDefinition, startEnd[0], startEnd[1], filters)
                               .ForEach(x => visualIndication.Config.LineGraph.Data.Add(double.IsNaN(x) ? 0 : x));
            }
            else if (widget.HasPOPSummary.GetValueOrDefault())
            {
                visualIndication.Type = "POPSummary";
                visualIndication.Config.POPSummary.Data = new JObject();

                startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, this.Date);

                double[] values = esiReportBroker.GetPOPSummaryValues(metricDefinition, widget.PrimaryMetricPeriod, startEnd[0], startEnd[1], filters);

                if (values != null && values.Length > 0)
                {
                    values[0] = Double.IsNaN(values[0]) ? 0 : values[0];

                    visualIndication.Config.POPSummary.Data.Current = values[0];

                    if (values.Length > 1)
                    {
                        values[1] = Double.IsNaN(values[1]) ? 0 : values[1];

                        visualIndication.Config.POPSummary.Data.Previous = values[1];
                    }
                }

                visualIndication.Config.POPSummary.Data.Variance = new JObject();

                var popVariance = values[0] - values[1];

                popVariance = Double.IsNaN(popVariance) ? 0 : popVariance;

                visualIndication.Config.POPSummary.Data.Variance.Actual = popVariance;
                visualIndication.Config.POPSummary.Data.Variance.Formatted = string.Format("{0:n" + widget.DecimalPlaces.GetValueOrDefault(2) + "}", Math.Round(Math.Abs(popVariance), widget.DecimalPlaces.GetValueOrDefault(2)));// Math.Round(Math.Abs(popVariance), widget.DecimalPlaces.GetValueOrDefault(2)).ToString();
            }
            else if (widget.HasStaticMessage.GetValueOrDefault())
            {
                visualIndication.Type = "StaticMessage";
                visualIndication.Config.StaticMessage.Message = widget.StaticMessage;
            }
            obj.VisualIndication = visualIndication;


            obj.Stats = new JObject();
            watch.Stop();

            obj.Stats.Took = watch.ElapsedMilliseconds / 60;

            return obj.ToString();
        }
    }
}