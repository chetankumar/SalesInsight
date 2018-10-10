using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Helpers
{
    public class ResponseParser
    {
        public static Customer ParseCustomerFromJson(string json)
        {
            Customer customer = new Customer();



            return customer;
        }

        public static List<Customer> GetCustomerList(string json)
        {
            JObject obj = JObject.Parse(json);

            List<Customer> customers = new List<Customer>();

            Customer cust = null;
            foreach (var item in (JArray)obj["Payload"]["Customers"])
            {
                cust = new Customer();
                cust.Name = item["Name"].Value<string>();
                cust.CustomerNumber = item["Number"].Value<string>();

                customers.Add(cust);
            }

            return customers;
        }

        public static SalesStaff ParseSalesStaff(string json)
        {
            SalesStaff salesStaff = new SalesStaff();

            return salesStaff;
        }

        public static List<SalesStaff> GetSalesStaffList(string json)
        {
            List<SalesStaff> salesPeople = new List<SalesStaff>();

            return salesPeople;
        }
    }
}
