using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIStandardMixPlant
    {
        public StandardMix Mix { get; set; }
        public MixFormulation Formulation { get; set; }
        public MixFormulationCostProjection Projection { get; set; }

        public decimal Cost
        {
            get
            {
                if (Projection != null)
                {
                    return Projection.Cost;
                }
                return 0;
            }
        }
    }
}
