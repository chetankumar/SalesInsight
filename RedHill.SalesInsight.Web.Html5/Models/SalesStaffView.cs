using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class SalesStaffView
    {
        [HiddenInput]
        public int SalesStaffId { get; set; }

        [HiddenInput]
        public int CompanyId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public string CompanyName { get; set; }

        public string DispatchId { get; set; }

        [ArrayRequired(1,ErrorMessage="Please assign at least 1 district")]
        public int[] Districts { get; set; }

        public List<SelectListItem> AvailableDistricts { get; set; }


        public SalesStaffView()
        {
            Districts = new int[]{};
        }

        public SalesStaffView(SISalesStaff siStaff)
        {
            SalesStaff staff = siStaff.SalesStaff;
            this.SalesStaffId = staff.SalesStaffId;
            this.Name = staff.Name;
            this.Phone = staff.Phone;
            this.Fax = staff.Fax;
            this.Email = staff.Email;
            this.CompanyId = staff.CompanyId.GetValueOrDefault();
            this.Active = staff.Active.GetValueOrDefault(false) ;
            this.DispatchId = staff.DispatchId;

            Districts = new int[siStaff.Districts.Count] ;
            int i = 0;
            foreach (District d in siStaff.Districts)
            {
                Districts[i] = d.DistrictId;
                i++;
            }
            BindValues();
        }

        public SalesStaffView(SalesStaff staff)
        {
            this.SalesStaffId = staff.SalesStaffId;
            this.Name = staff.Name;
            this.Phone = staff.Phone;
            this.Fax = staff.Phone;
            this.Email = staff.Email;
            this.CompanyId = staff.CompanyId.GetValueOrDefault();
            this.Active = staff.Active.GetValueOrDefault(false);
            this.DispatchId = staff.DispatchId;
            BindValues();
        }

        public void BindValues()
        {
            this.CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
            List<District> districts = SIDAL.GetDistricts(CompanyId, null, 0, 1000,false);
            AvailableDistricts = new List<SelectListItem>();
            foreach (District d in districts)
            {
                SelectListItem i = new SelectListItem();
                i.Text = d.Name;
                i.Value = d.DistrictId.ToString();
                if (Districts!=null)
                    i.Selected = Districts.Contains(d.DistrictId);
                AvailableDistricts.Add(i);
            }
        }
    }
}
