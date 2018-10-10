using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class DSOStat
    {
        public string  CustomerNumber { get; set; }
        public decimal DSO { get; set; }
        public decimal SixMonRevenue { get; set; }

        public DSOStat(string customerNumber, decimal dso, decimal balance,DateTime asOfDate)
        {
            this.CustomerNumber = customerNumber;

            DateTime endDate = asOfDate;            
            DateTime salesStartDate = endDate.AddMonths(-6);
            
            var profitablities = SIDAL.GetCustomerProfitabilityReport(new string[] { customerNumber }, salesStartDate, endDate);
            if (profitablities != null && profitablities.Count > 0)
            {
                this.SixMonRevenue = profitablities.Sum(x => x.Revenue);
            }
            if (dso < 999 && dso > 0){
                this.DSO = dso;
            }else{
                if (profitablities != null && profitablities.Count > 0)
                {
                    DateTime firstDate = profitablities.Min(x => x.ReportDate);
                    DateTime lastDate = profitablities.Max(x => x.ReportDate);
                    TimeSpan span = endDate - salesStartDate;
                    double totalDays = span.TotalDays;
                    if (this.SixMonRevenue > 0)
                        this.DSO = balance / (this.SixMonRevenue / Convert.ToDecimal(totalDays));
                    else
                        this.DSO = 0;
                }
                else
                {
                    this.DSO = 0;
                }
            }
            
        }
    }
}
