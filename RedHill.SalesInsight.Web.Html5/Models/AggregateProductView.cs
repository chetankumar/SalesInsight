using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class AggregateProductView
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public long UomId { get; set; }
        public bool Active { get; set; }
        public string DispatchId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public AggregateProductView()
        {

        }
        public AggregateProductView(AggregateProduct aggProduct)
        {
            this.Id = aggProduct.Id;
            this.Code = aggProduct.Code;
            this.Description = aggProduct.Description;
            this.UomId = aggProduct.UomId.GetValueOrDefault();
            this.Active = aggProduct.Active.GetValueOrDefault(false);
            this.DispatchId = aggProduct.DispatchId;
            this.CreatedAt = aggProduct.CreatedAt.GetValueOrDefault();

        }
    }
}