using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5
{
    public class IndicatorModel
    {
        public double Actual { get; set; }
        public double Target { get; set; }
        public double Ok { get; set; }
        public double Caution { get; set; }
        public bool LessIsBetter { get; set; }

        public IndicatorModel(double actual, double target, double ok, double caution, bool lessIsBetter)
        {
            this.Actual = Convert.ToDouble(actual);
            this.Target = Convert.ToDouble(target);
            this.Ok = ok;
            this.Caution = caution;
            this.LessIsBetter = lessIsBetter;
        }

        public IndicatorModel(decimal actual, double target, double ok, double caution, bool lessIsBetter)
        {
            this.Actual = Convert.ToDouble(actual);
            this.Target = Convert.ToDouble(target);
            this.Ok = ok;
            this.Caution = caution;
            this.LessIsBetter = lessIsBetter;
        }

        public IndicatorModel(double actual, decimal target, double ok, double caution, bool lessIsBetter)
        {
            this.Actual = Convert.ToDouble(actual);
            this.Target = Convert.ToDouble(target);
            this.Ok = ok;
            this.Caution = caution;
            this.LessIsBetter = lessIsBetter;
        }

        public IndicatorModel(decimal actual, decimal target, double ok, double caution, bool lessIsBetter)
        {
            this.Actual = Convert.ToDouble(actual);
            this.Target = Convert.ToDouble(target);
            this.Ok = ok;
            this.Caution = caution;
            this.LessIsBetter = lessIsBetter;
        }
    }
}
