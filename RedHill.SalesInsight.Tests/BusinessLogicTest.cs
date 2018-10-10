using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedHill.SalesInsight.DAL;
using Utils;
using RedHill.SalesInsight.AUJSIntegration.Model;
using RedHill.SalesInsight.AUJSIntegration.Data;
using RedHill.SalesInsight.Web.Html5.Models.QuotationModels;
using RedHill.SalesInsight.Web.Html5.Helpers;

namespace RedHill.SalesInsight.Tests
{
    [TestClass]
    public class BusinessLogicTest
    {
        //[TestMethod]
        //public void TestSearchMixFormulations()
        //{
        //    //var results = SIDAL.SearchMixFormulations(145, null, null, null, null, null, null, null, null, null, null, null, null);
        //    var results = SIDAL.SearchMixFormulations(145,DateUtils.GetFirstOf(DateTime.Today),
        //        new int[] { 3000, 3500, 4000 },
        //        new string[] { "1", "4" },
        //        new string[] { "0-3", "3-5" },
        //        new double[] { 0.00 },
        //        new double[] { 0.00 },
        //        new double[] { 6.35 },
        //        new string[] { "0.51", "0.44", "0.5" },
        //        new string[] { "Commercial" },
        //        new string[] { "15%", "0%", "25%", "20%" },
        //        new string[] { "7.4", "6.35", "6.2", "6.3" },
        //        new long[] { 75 },
        //        new long[] { 120 });

        //    var count = results.Count();
        //    Console.WriteLine(count);

        //}

        //[TestMethod]
        //public void TestRefreshAllMixFormulationCalculations()
        //{
        //    SIDAL.RefreshAllMixFormulationCalculations();
        //}

        //[TestMethod]
        //public void TestRefreshAllMixFormulationCostProjections()
        //{
        //    SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(0));
        //}

        //[TestMethod]
        //public void TestRefreshMixFormulationCostProjections()
        //{
        //    SIDAL.RefreshMixFormulationCosts(1907, DateTime.Today);
        //}

        //[TestMethod]
        //public void TestRawMatProjectionUpdateMixFormulationProjection()
        //{
        //    SIDAL.UpdateMixFormulationCostForRawMaterialCostProjection(12153);
        //}

        //[TestMethod]
        //public void TestUpdateMixFormulation()
        //{
        //    SIDAL.UpdateMixFormulationCostProjections(3918);
        //}

        [TestMethod]
        public void TestQuotePayload()
        {
            var quote = SIDAL.FindQuotationWithAllRefs(7);

            PushQuoteModel pushQuoteModel = new PushQuoteModel(2, 7);

            var payload = pushQuoteModel.GenerateQuotePayload(quote);

            string payloadObj = payload.ToJson();

        }
    }
}
