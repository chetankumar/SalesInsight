using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class BlockProductView
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public long UomId { get; set; }
        public bool Active { get; set; }
        public string DispatchId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public BlockProductView()
        {

        }
        public BlockProductView(BlockProduct blockProduct)
        {
            this.Id = blockProduct.Id;
            this.Code = blockProduct.Code;
            this.Description = blockProduct.Description;
            this.UomId = blockProduct.UomId.GetValueOrDefault();
            this.Active = blockProduct.Active.GetValueOrDefault(false);
            this.DispatchId = blockProduct.DispatchId;
            this.CreatedAt = blockProduct.CreatedAt.GetValueOrDefault();

        }
    }
}