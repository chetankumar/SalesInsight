using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class FSKPriceView
    {

        public long Id { get; set; }
        [Required]
        public string FSKCode { get; set; }
        [Required]
        public double SackCount { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public decimal BasePrice { get; set; }
        [Required]
        public decimal AddPrice { get; set; }
        [Required]
        public decimal DeductPrice { get; set; }

        public bool Active { get; set; }

        
        public FSKPriceView()
        {
            this.Active = true;
        }

        public FSKPriceView(FSKPrice model)
        {
            this.Id = model.Id;
            this.FSKCode = model.FSKCode;
            this.City = model.City;
            this.SackCount = model.SackCount;
            this.BasePrice = model.BasePrice;
            this.AddPrice = model.AddPrice;
            this.DeductPrice = model.DeductPrice;
            this.Active = model.Active;
        }

        public FSKPrice ToEntity()
        {
            FSKPrice entity = new FSKPrice();
            entity.Id = this.Id;
            entity.DeductPrice = this.DeductPrice;
            entity.AddPrice = this.AddPrice;
            entity.BasePrice = this.BasePrice;
            entity.SackCount = this.SackCount;
            entity.City = this.City;
            entity.Active = this.Active;
            entity.FSKCode = this.FSKCode;
            return entity;
        }
    }
}