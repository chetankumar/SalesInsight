using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIPipelineResults
    {
        public List<SIPipelineProject> Pipelines { get; set; }
        public int RowCount { get; set; }
        public int CurrentPage { get; set; }
        public int NumRows { get; set; }
    }
}
