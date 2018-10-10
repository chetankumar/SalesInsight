using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Data
{
    public class SyncResponse
    {
        public SyncStatus SyncStatus { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
