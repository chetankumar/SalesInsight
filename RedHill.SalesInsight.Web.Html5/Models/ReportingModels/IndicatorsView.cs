using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class IndicatorsView
    {
        public List<string> PerformanceMetrics = new List<string>() { "Ticketing", "Loading", "Tempering", "ToJob", "Wait", "Unload", "Wash", "FromJob", "CYDHr", "AvgLoad" };
        public List<string> FinancialMetrics = new List<string>() { "Price", "Material","Spread", "Delivery Variable", "Plant Variable", "Delivery Fixed", "Plant Fixed", "SG&A", "Profit" };

        public MetricIndicatorAllowance GetMetricIndicator(string metricName)
        {
            MetricIndicatorAllowance allowance = SIDAL.GetMetricIndicator(metricName);
            if (allowance == null)
            {
                allowance =  new MetricIndicatorAllowance();
                allowance.Metric = metricName;
            }
            return allowance;
        }

        public TargetIndicatorAllowance GetTargetIndicator(string metricName)
        {
            TargetIndicatorAllowance allowance = SIDAL.GetTargetIndicator(metricName);
            if (allowance == null)
            {
                allowance = new TargetIndicatorAllowance();
                allowance.Metric = metricName;
            }
            return allowance;
        }
    }
}
