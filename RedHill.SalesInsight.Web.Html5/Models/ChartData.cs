using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ChartData
    {
        public static string GetMarketShareSummaryJson(List<SIProjectSuccessMarketShareSummary> summaries)
        {
            JArray array = new JArray();
            JArray obj = new JArray();
            obj.Add("Company");
            obj.Add("Volume");
            array.Add(obj);
            for (int i = 0; i < summaries.Count(); i++)
            {
                SIProjectSuccessMarketShareSummary summary = summaries[i];
                obj = new JArray();
                obj.Add(summary.Name);
                obj.Add(summary.Volume);
                array.Add(obj);
            }
            return array.ToString();
        }

        public static string GetForecastVersesPlanData(List<SIForecastVersusPlan> forecastPlanData)
        {
            JArray array = new JArray();
            JArray obj = new JArray();
            obj.Add("Month");
            obj.Add("Projection");
            obj.Add("Budget");
            obj.Add("Actual");
            array.Add(obj);
            List<String> monthNames = forecastPlanData.Select(m => m.MonthName).Distinct().ToList();
            int i = 0;
            foreach (String month in monthNames)
            {
                obj = new JArray();
                obj.Add(month);
                if (i == 0)
                {
                    obj.Add(0);
                }
                else
                {
                    obj.Add(forecastPlanData.Where(m => m.MonthName == month).Sum(m => m.ForecastQuantity));
                }
                obj.Add(forecastPlanData.Where(m => m.MonthName == month).Sum(m => m.TargetQuantity));
                if (i == 0)
                {
                    obj.Add(forecastPlanData.Where(m => m.MonthName == month).Sum(m => m.ForecastQuantity));
                }
                else
                {
                    obj.Add(0);
                }
                array.Add(obj);
                i++;

                //obj = new JArray();
                //obj.Add(month);
                //obj.Add(forecastPlanData.Where(m => m.MonthName == month).Sum(m => m.ForecastQuantity));
                //obj.Add(forecastPlanData.Where(m => m.MonthName == month).Sum(m => m.TargetQuantity));
                //array.Add(obj);
            }
            return array.ToString();
        }

        internal static string GetCurrentSegmentsData(List<SISegmentationAnalysis> currentSegments)
        {
            JArray array = new JArray();
            JArray obj = new JArray();
            obj.Add("Market Segment");
            obj.Add("Forecast");
            array.Add(obj);
            foreach(SISegmentationAnalysis segment in currentSegments)
            {
                obj = new JArray();
                obj.Add(segment.MarketSegmentName);
                obj.Add(segment.ForecastQuantity);
                array.Add(obj);
            }
            return array.ToString();
        }
    }
}