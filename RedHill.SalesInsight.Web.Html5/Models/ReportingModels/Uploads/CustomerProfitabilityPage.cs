using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class CustomerProfitabilityPage
    {
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        public int PageNum { get; set; }
        public int RowCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumRecords { get; set; }

        public List<CustomerProfitability> Rows { get; set; }

        public CustomerProfitabilityPage()
        {
            this.PageNum = 1;
            this.RowCount = 10;
            this.SortBy = "CustomerName";
            this.SortDirection = "asc";
            this.StartDate = DateTime.Today.AddMonths(-1);
            this.EndDate = DateTime.Today;
        }

        public void LoadRows()
        {
            int numRecords = 0;
            this.Rows = SIDAL.GetCustomerProfitabilityData(SortBy,SortDirection, PageNum, RowCount, StartDate, EndDate,out numRecords);
            this.NumRecords = numRecords;
        }
    }
}
