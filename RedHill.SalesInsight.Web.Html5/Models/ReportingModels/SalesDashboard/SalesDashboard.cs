using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class SalesDashboard
    {
        public SalesDashboard(Guid guid)
        {
            this.UserId = guid;
        }
        public SalesDashboard() { }

        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate
        {
            get
            {
                return StartDate.AddMonths(1).AddDays(-1);
            }
        }
        public SelectList AllDistricts
        {
            get
            {
                return new SelectList(SIDAL.GetDistricts(UserId), "DistrictId", "Name", SelectedDistricts);
            }
        }
        public int[] SelectedDistricts { get; set; }
        public WinRateSection WinRate { get; set; }
        public BackLogSection BackLog { get; set; }

        public ProductivityReport ProductivityReport { get; set; }
        public ProfitabilityReport ProfitabilityReport { get; set; }
        public EaseOfBusinessReport EaseOfBusinessReport { get; set; }
        public AgingReport  AgingReport { get; set; }

        public void LoadReports()
        {
            var plantList = SIDAL.GetAllPlantWithBudget();
            this.ProductivityReport = new ProductivityReport(this, plantList);
            this.ProfitabilityReport = new ProfitabilityReport(this, plantList);
            this.EaseOfBusinessReport = new EaseOfBusinessReport(this);
            this.AgingReport = new AgingReport(this);

            this.WinRate = new WinRateSection(this);
            this.BackLog = new BackLogSection(this);
        }
        public string DiamondChartData
        {
            get
            {
                int y_max = 20;
                int y_min = 0;
                double[] dataPoints = new double[] { 0, 0, 0, 0 };
                if (ProfitabilityReport != null)
                {
                    int x_min = -10;
                    int x_max = 10;
                    double variance = Convert.ToDouble(ProfitabilityReport.VarianceProfit);
                    dataPoints[0] = GetNormalizedY(x_min, y_min, x_max, y_max, variance);
                }
                if (AgingReport != null)
                {
                    int x_min = -30;
                    int x_max = 30;
                    double variance = Convert.ToDouble(AgingReport.DSOVariance);
                    dataPoints[1] = GetNormalizedY(x_min, y_max, x_max, y_min, variance);
                }
                if (ProductivityReport != null)
                {
                    int x_min = -3;
                    int x_max = 3;
                    double variance = ProductivityReport.CydHrVariance;
                    dataPoints[2] = GetNormalizedY(x_min, y_min, x_max, y_max, variance);
                }
                if (this.EaseOfBusinessReport != null)
                {
                    int x_min = -5;
                    int x_max = 5;
                    double variance = Convert.ToDouble(EaseOfBusinessReport.CancellationVariance);
                    dataPoints[3] = GetNormalizedY(x_min, y_max, x_max, y_min, variance);
                }
                return "[" + dataPoints[0] + "," + dataPoints[1] + "," + dataPoints[2] + "," + dataPoints[3] + "]";
            }
        }
        private double GetNormalizedY(int x_0, int y_0, int x_1, int y_1, double x)
        {
            if (y_1 > y_0)
            {
                if (x > x_1)
                {
                    return y_1;
                }
                else if (x < x_0)
                {
                    return y_0;
                }
            }
            if (y_1 < y_0)
            {
                if (x < x_0)
                {
                    return y_0;
                }
                else if (x > x_1)
                {
                    return y_1;
                }
            }
            return ((y_1 - y_0) / (x_1 - x_0) * (x - x_0)) + y_0;
        }
    }
}
