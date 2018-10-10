using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.Models.POCO
{
    public class SyncHistory
    {
        public string Entity { get; set; }
        public string LastImportDate { get; set; }
    }
}
