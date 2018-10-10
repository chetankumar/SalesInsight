using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedHill.SalesInsight.AUJSIntegration;
using RedHill.SalesInsight.AUJSIntegration.Consumer;
using RedHill.SalesInsight.AUJSIntegration.Data;
using RedHill.SalesInsight.AUJSIntegration.Helpers;
using RedHill.SalesInsight.AUJSIntegration.Model;
using RedHill.SalesInsight.DAL;
using System;

namespace RedHill.SalesInsight.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        [TestCategory("Login")]
        public void InvalidClientIdLoginTest()
        {
            Exception ex = null;
            try
            {
                APIConsumer consumer = new APIConsumer(AppSettings.APIBaseURL, "", "");
            }
            catch (Exception e)
            {
                ex = e;
            }
            Assert.IsTrue(ex is ArgumentException);
            Assert.IsTrue("Invalid ClientId provided".Equals(ex.Message));
        }

        [TestMethod]
        [TestCategory("Login")]
        public void InvalidClientKeyLoginTest()
        {
            Exception ex = null;
            try
            {
                APIConsumer consumer = new APIConsumer(AppSettings.APIBaseURL, AppSettings.ClientId, "");
            }
            catch (Exception e)
            {
                ex = e;
            }
            Assert.IsTrue(ex is ArgumentException);
            Assert.IsTrue("Invalid ClientKey provided".Equals(ex.Message));
        }

        [TestMethod]
        [TestCategory("Utils")]
        public void PayloadWrapperToJsonMethod()
        {
            PayloadWrapper pWrapper = new PayloadWrapper();

            pWrapper.Payload.Add("ClientId", "Test_Client");

            Assert.IsTrue(pWrapper.Payload.Count == 1);
        }

        [TestMethod]
        [TestCategory("Utils")]
        public void ParseCustomerListFromJson()
        {
            string json = "{ \"Payload\": {"
                            + "    \"TimeStamp\": 635859944000000000,"
                            + "    \"Customers\": ["
                            + "      {"
                            + "        \"Id\": 45,"
                            + "        \"Name\": \"Jonel Eng Systems\","
                            + "        \"Number\": \"1006\","
                            + "        \"Address1\": \"500 East Walnut Ave.\","
                            + "        \"Address2\": \"\","
                            + "        \"City\": \"Fullerton\","
                            + "        \"State\": \"Ca\","
                            + "        \"Zip\": \"92832\""
                            + "      },"
                            + "      {"
                            + "        \"Id\": 51,"
                            + "        \"Name\": \"Acme Ready Mix`s\","
                            + "        \"Number\": \"1001\","
                            + "        \"Address1\": \"665 Kalamath Ways\","
                            + "        \"Address2\": \"Suite 123\","
                            + "        \"City\": \"Fullerton\","
                            + "        \"State\": \"CA\","
                            + "        \"Zip\": \"92832\""
                            + "      }"
                            + "    ]"
                            + "  }"
                            + "}";

            var customers = ResponseParser.GetCustomerList(json);

            Assert.IsNotNull(customers);
            Assert.IsTrue(customers.Count == 2);
        }
    }
}
