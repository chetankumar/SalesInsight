using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class TaxCodeView
    {
        public long Id { get; set; }
        [Required]
        public String Code { get; set; }
        [Required]
        public string Description { get; set; }

        public TaxCodeView()
        {
        }

        public TaxCodeView(TaxCode entity)
        {
            this.Id = entity.Id;
            this.Code = entity.Code;
            this.Description = entity.Description;
        }

        public TaxCode ToEntity()
        {
            TaxCode entity = new TaxCode();
            entity.Id = this.Id;
            entity.Code = this.Code;
            entity.Description = this.Description;
            return entity;
        }
    }
}
