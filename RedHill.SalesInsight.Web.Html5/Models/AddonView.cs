using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class AddonView
    {
        public long Id { get; set; }

        [Required]
        public String Code { get; set; }
        public String Description { get; set; }
        public String AddonType { get; set; }
        public long? QuoteUOM { get; set; }
        public long? MixUOM { get; set; }
        public bool Active { get; set; }
        public String DispatchId { get; set; }
        public AddonView()
        {
            Active = true;
        }

        public AddonView(Addon model)
        {
            this.Id = model.Id;
            this.Code = model.Code;
            this.Description = model.Description;
            this.AddonType = model.AddonType;
            this.QuoteUOM = model.QuoteUomId;
            this.MixUOM = model.MixCostUomId;
            this.Active = model.Active.GetValueOrDefault();
            this.DispatchId = model.DispatchId;
        }

        public Addon ToEntity()
        {
            Addon entity = new Addon();
            entity.Id = this.Id;
            entity.Code = this.Code;
            entity.Description = this.Description;
            entity.AddonType = this.AddonType;
            entity.MixCostUomId = this.MixUOM;
            entity.QuoteUomId = this.QuoteUOM;
            entity.Active = this.Active;
            entity.DispatchId = this.DispatchId;
            return entity;
        }


        #region HTML helpers
        public List<SelectListItem> ChooseAddonType
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();

                SelectListItem item = new SelectListItem();
                item.Text = "Product";
                item.Value = "Product";
                item.Selected = ("Product" == AddonType);
                items.Add(item);

                item = new SelectListItem();
                item.Text = "Fee";
                item.Value = "Fees";
                item.Selected = ("Fees" == AddonType);
                items.Add(item);

                item = new SelectListItem();
                item.Text = "Service Charge";
                item.Value = "Service Charge";
                item.Selected = ("Service Charge" == AddonType);
                items.Add(item);

                return items;
            }
        }
        public SelectList ChooseQuoteUOM
        {
            get
            {
                return new SelectList(SIDAL.GetUOMS(QuoteUOM), "Id", "Name", QuoteUOM);
            }
        }
        public SelectList ChooseMixUOM
        {
            get
            {
                return new SelectList(SIDAL.GetUOMS(MixUOM), "Id", "Name", MixUOM);
            }
        }
        #endregion
    }
}