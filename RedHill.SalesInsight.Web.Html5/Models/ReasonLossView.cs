using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ReasonLossView
    {
        public int CompanyId { get; set; }
        public int ReasonLossId { get; set; }

        [Required]
        public string Reason { get; set; }

        public string CompanyName { get;set;}

        public bool Active { get; set; }

        public ReasonLossView()
        {
            Active = true;
        }

        public ReasonLossView(ReasonsForLoss lossReason)
        {
            this.CompanyId = lossReason.CompanyId;
            this.ReasonLossId = lossReason.ReasonLostId;
            this.Reason = lossReason.Reason;
            this.Active = lossReason.Active.GetValueOrDefault(false);
            BindValues();
        }

        public void BindValues()
        {
            this.CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
        }
    }
}