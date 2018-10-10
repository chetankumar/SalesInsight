using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Model
{
    public class QuotePayload
    {
        public string CustomerId { get; set; }
        public string QuoteName { get; set; }
        public string QuoteDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CustomerPO { get; set; }
        public double SalesLimit { get; set; }
        public long PlantId { get; set; }
        public long SalesPersonId { get; set; }
        public long TaxScheduleId { get; set; }
        public bool IsNonTaxable { get; set; }

        public string ConcreteStartDate { get; set; }
        public string ConcreteEndDate { get; set; }
        public long ConcretePlantId { get; set; }
        public long ConcreteSalesPersonId { get; set; }

        public string AggregateStartDate { get; set; }
        public string AggregateEndDate { get; set; }
        public long AggregatePlantId { get; set; }
        public long AggregateSalesPersonId { get; set; }

        public string BlockStartDate { get; set; }
        public string BlockEndDate { get; set; }
        public long BlockPlantId { get; set; }
        public long BlockSalesPersonId { get; set; }

        public string Notes { get; set; }
        public string TicketNotes { get; set; }
        public List<Product> Products { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }

    public class Product
    {
        public string ProductId { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal FreightRate { get; set; }
        public long? UsageTypeId { get; set; }
        public int SystemTypeId { get; set; }
    }
}
