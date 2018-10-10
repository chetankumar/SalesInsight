using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ManageRawMaterials
    {
        public String CompanyName { get; set; }
        public long SelectedRawMaterialId { get; set; }
        public RawMaterialView SelectedRawMat { get; set; }
        public List<RawMaterial> RawMaterials { get; set; }

        public bool ShowInactives { get; set; }
        public String SearchTerm { get; set; }

        public bool ShowModal { get; set; }

        public ManageRawMaterials()
        {
            
        }

        public ManageRawMaterials(long? id)
        {
            if (id.HasValue)
                this.SelectedRawMaterialId = id.Value;
            LoadValues();
        }

        public void LoadValues()
        {
            if (SelectedRawMaterialId > 0)
            {
                SelectedRawMat = new RawMaterialView(SIDAL.FindRawMaterial(SelectedRawMaterialId));
                ShowModal = true;
            }
            else
            {
                SelectedRawMat = new RawMaterialView();
                SelectedRawMat.Active = true;
            }
            RawMaterials = SIDAL.GetRawMaterials(ShowInactives,SearchTerm);
        }
    }
}