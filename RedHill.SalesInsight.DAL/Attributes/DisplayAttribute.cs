using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedHill.SalesInsight.DAL.Attributes
{
    public class DisplayAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
