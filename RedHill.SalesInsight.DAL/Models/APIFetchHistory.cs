using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL
{
    public partial class APIFetchHistory
    {
        public static APIFetchHistory FindOrCreate(long? id, string entityType, DateTime? startDate = null, string status = null, DateTime? lastImportDate = null)
        {
            APIFetchHistory apiFetchHistory = SIDAL.FindOrCreateAPIFetchHistory(id, entityType, startDate, status, lastImportDate);
            return apiFetchHistory;
        }

        public void UpdateStatus(string status, DateTime? lastImportDate = null,string messsage = null, int? recordCount = null)
        {
            SIDAL.UpdateAPIFetchHistory(this.Id, status, lastImportDate,messsage,recordCount);
        }
    }
}
