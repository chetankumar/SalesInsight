using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL
{
    public partial class RawMaterial
    {
        public string UomDispatchId { get; set; }
        public long RawMaterialTypeDispatchId { get; set; }

        /// <summary>
        /// Custom list for keeping raw material cost projections
        /// </summary>
        public List<RawMaterialCostProjection> CostProjections { get; set; }
    }
}
