using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedHill.SalesInsight.DAL.Constants;
using System;
using Utils;

namespace RedHill.SalesInsight.Tests
{
    [TestClass]
    public class DateUtilsTest
    {
        [TestMethod]
        [TestCategory("DateUtils")]
        public void ValidGetFirstOfMethod()
        {
            DateTime janFifteenthDate = new DateTime(2016, 01, 15); //Unit Parameter: 15/01/2016
            DateTime output = DateUtils.GetFirstOf(janFifteenthDate); //Unit Output: 01/01/2016

            DateTime expected = janFifteenthDate.AddDays(-14); //Expected: 01/01/2016

            Assert.IsFalse(DateTime.MinValue.Equals(output));
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        [TestCategory("DateUtils")]
        public void ValidateGetStartAndEndDateForPeriodTypeMethod()
        {
            //Day
            DateTime day = new DateTime(2016, 4, 15);

            DateTime[] output = DateUtils.GetStartAndEndDateForPeriodType(ESIPeriodType.Day.ToString(), day);

            Assert.AreEqual(output[0], output[1], "Day validation");

            //WTD
            output = DateUtils.GetStartAndEndDateForPeriodType(ESIPeriodType.WTD.ToString(), day);

            Assert.AreEqual(output[0], new DateTime(2016, 04, 11), "WTD Start validation");
            Assert.AreEqual(output[1], day, "WTD End validation");

            //MTD
            output = DateUtils.GetStartAndEndDateForPeriodType(ESIPeriodType.MTD.ToString(), day);

            Assert.AreEqual(output[0], new DateTime(2016, 04, 01), "MTD Start validation");
            Assert.AreEqual(output[1], day, "MTD End validation");

            //QTD
            output = DateUtils.GetStartAndEndDateForPeriodType(ESIPeriodType.QTD.ToString(), day);

            Assert.AreEqual(output[0], new DateTime(2016, 04, 01), "QTD Start validation");
            Assert.AreEqual(output[1], day, "QTD End validation");

            //YTD
            output = DateUtils.GetStartAndEndDateForPeriodType(ESIPeriodType.YTD.ToString(), day);

            Assert.AreEqual(output[0], new DateTime(2016, 01, 01), "YTD Start validation");
            Assert.AreEqual(output[1], day, "YTD End validation");

            //PY-YTD
            output = DateUtils.GetStartAndEndDateForPeriodType(ESIPeriodType.PYYTD.ToString(), day);

            Assert.AreEqual(output[0], new DateTime(2015, 01, 01), "PY-YTD Start validation");
            Assert.AreEqual(output[1], day.AddYears(-1), "PY-YTD End validation");
        }
    }
}
