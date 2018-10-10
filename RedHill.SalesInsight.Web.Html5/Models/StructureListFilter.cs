using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class StructureListFilter
    {
        public string StructureType { get; set; }
        public string[] ParentIds { get; set; }
    }
}