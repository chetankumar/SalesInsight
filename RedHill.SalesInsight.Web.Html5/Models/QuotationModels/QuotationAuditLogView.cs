using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationAuditLogView
    {
        public Guid UserId { get; set; }
        public long QuotationId { get; set; }
        public List<QuoteAuditLog> AuditLogs { get; set; }

        public QuotationAuditLogView()
        {
        }

        public QuotationAuditLogView(long id)
        {
            this.QuotationId = id;
        }

        public void Load()
        {
            if (this.QuotationId > 0)
            {
                this.AuditLogs = SIDAL.GetQuoteAuditLogs(this.QuotationId);
            }
        }
    }
}
