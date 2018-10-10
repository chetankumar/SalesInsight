using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.ReportModels
{
    public class SortItem
    {
        public string SortBy { get; set; }
        public int Order { get; set; }
        public string  IsDescending { get; set; }
    }
}
