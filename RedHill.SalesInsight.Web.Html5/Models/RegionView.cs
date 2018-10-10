using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class RegionView
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public RegionView()
        {

        }

        public RegionView(Region r)
        {
            this.RegionId = r.RegionId;
            this.Name = r.Name;
            this.Active = r.Active.GetValueOrDefault(false);
            this.CompanyId = r.CompanyId;
            BindValues();
        }

        public void BindValues()
        {
            this.CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
        }
    }
}
