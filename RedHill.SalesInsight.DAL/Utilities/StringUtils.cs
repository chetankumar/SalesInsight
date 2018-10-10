using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public class StringUtils
    {
        public static string Commatize(string[] values)
        {
            if (values != null && values.Count() > 0)
                return String.Join(",", values.ToArray());
            return null;
        }

        public static string Commatize(int[] values)
        {
            if (values != null && values.Count() > 0)
            {
                return String.Join(",", values.Select(x=>x.ToString()).ToArray());
            }
            return null;
        }

        internal static string Commatize(double[] values)
        {
            if (values != null && values.Count() > 0)
            {
                return String.Join(",", values.Select(x => x.ToString("N2")).ToArray());
            }
            return null;
        }

        internal static string Commatize(long[] values)
        {
            if (values != null && values.Count() > 0)
            {
                return String.Join(",", values.Select(x => x.ToString("N0")).ToArray());
            }
            return null;
        }
    }
}
