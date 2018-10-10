using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL
{
    public partial class Addon
    {
        public string UomDispatchId { get; set; }

        public List<AddonPriceProjection> PriceProjections { get; set; }
    }
}
