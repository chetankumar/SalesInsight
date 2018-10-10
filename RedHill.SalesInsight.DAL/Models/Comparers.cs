using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedHill.SalesInsight.DAL.Models
{

    public class DriverDetailComparer : IEqualityComparer<DriverDetail>
    {
        public bool Equals(DriverDetail x, DriverDetail y)
        {
            return x.DriverNumber== y.DriverNumber &&
                    x.DriverName == y.DriverName;
        }

        public int GetHashCode(DriverDetail obj)
        {
            return obj.DriverNumber.GetHashCode() * 17 + obj.DriverName.GetHashCode();
        }
    }
}
