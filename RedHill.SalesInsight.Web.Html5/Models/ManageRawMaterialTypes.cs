using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ManageRawMaterialTypes
    {
        public ManageRawMaterialTypes(long? RawMaterialId)
        {
            if (RawMaterialId != null && RawMaterialId.Value > 0)
            {
                this.SelectedRawMaterialType = new RawMaterialTypeView(SIDAL.FindRawMaterialType(RawMaterialId.Value));
                ShowModal = true;
            }
            else
            {
                this.SelectedRawMaterialType = new RawMaterialTypeView();
                this.SelectedRawMaterialType.Active = true;
            }
            LoadValues();
        }

        public ManageRawMaterialTypes()
        {
            this.SelectedRawMaterialType = new RawMaterialTypeView();
            this.SelectedRawMaterialType.Active = true;
        }

        public bool ShowInactives { get; set; }
        public List<RawMaterialType> RawMaterialTypes { get; set; }
        public RawMaterialTypeView SelectedRawMaterialType { get; set; }

        public bool ShowModal { get; set; }

        internal void LoadValues()
        {
            this.RawMaterialTypes = SIDAL.GetRawMaterialTypes(ShowInactives);
        }

    }
}