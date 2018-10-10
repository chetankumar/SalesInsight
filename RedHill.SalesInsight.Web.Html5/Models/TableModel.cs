using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class TableModel
    {
        public List<String> Headers {get;set;}
        public List<List<String>> Cells {get;set;}

        public void AddHeader(string header)
        {
            if (Headers == null)
                Headers = new List<string>();
            Headers.Add(header);
        }

        public void AddRow(List<string> row)
        {
            if (Cells == null)
                Cells = new List<List<string>>();
            Cells.Add(row);
        }
    }
}