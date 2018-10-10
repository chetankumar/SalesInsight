using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ManageFSKPrices
    {
        public ManageFSKPrices(long? RawMaterialId)
        {
            if (RawMaterialId != null && RawMaterialId.Value > 0)
            {
                this.SelectedFSKPrice = new FSKPriceView(SIDAL.FindFSKPrice(RawMaterialId.Value));
                ShowModal = true;
            }
            else
            {
                this.SelectedFSKPrice = new FSKPriceView();
                this.SelectedFSKPrice.Active = true;
            }
            LoadValues();
        }

        public ManageFSKPrices()
        {
            this.SelectedFSKPrice = new FSKPriceView();
            this.SelectedFSKPrice.Active = true;
        }

        public bool ShowInactives { get; set; }
        public List<FSKPrice> FSKPrices { get; set; }
        public FSKPriceView SelectedFSKPrice { get; set; }

        public bool ShowModal { get; set; }

        internal void LoadValues()
        {
            this.FSKPrices = SIDAL.GetFSKPrices(ShowInactives);
        }

    }
}