using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ContractorView
    {
        [HiddenInput]
        public int ContractorId { get; set; }

        [HiddenInput]
        public int CompanyId { get; set; }

        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }
        
        public string CompanyName { get; set; }

        [Display(Name="Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "Address 3")]
        public string Address3 { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public int? ProjectId { get; set; }


        public ContractorView()
        {
            this.Name = "";
        }

        public ContractorView(Contractor contractor)
        {
            this.ContractorId = contractor.ContractorId;
            this.CompanyId = contractor.CompanyId;
            this.Name = contractor.Name;
            this.Address1 = contractor.Address1;
            this.Address2 = contractor.Address2;
            this.Address3 = contractor.Address3;
            this.City = contractor.City;
            this.State = contractor.State;
            this.Zip = contractor.Zip;
            this.Phone = contractor.Phone;
            this.Fax = contractor.Fax;
            this.Email = contractor.Email;
            this.Active = contractor.Active.GetValueOrDefault(false);
            BindValues();
        }

        public void BindValues()
        {
            CompanyName = SIDAL.GetCompany(this.CompanyId).Name;
        }
    }
}
