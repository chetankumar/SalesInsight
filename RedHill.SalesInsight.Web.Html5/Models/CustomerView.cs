using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Web.Html5.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class CustomerView
    {
        public SIUser User { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CustomerId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CompanyId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Number { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "Address 3")]
        public string Address3 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public bool Active { get; set; }

        public int? ProjectId { get; set; }

        public int? QuotationId { get; set; }

        public string DispatchId { get; set; }

        public string Group { get; set; }
        public string GroupHandle { get; set; }

        public bool PurchaseConcrete { get; set; }
        public bool PurchaseAggregate { get; set; }
        public bool PurchaseBlock { get; set; }

        public bool APIActiveStatus { get; set; }

        [Display(Name = "Override AU Status")]
        public bool OverrideAUStatus { get; set; }

        public List<CustomerContactView> CustomerContacts { get; set; }

        [ArrayRequired(1, ErrorMessage = "Please select the districts")]
        public string[] Districts { get; set; }

        public IList<DistrictListView> SelectedDistricts { get; set; }

        public List<SelectListItem> AvailableDefaultQuoteProducts { get; set; }

        [Display(Name = "Purchase Products")]
        public string[] PostedProductIds { get; set; }

        public List<DistrictListView> AllDistricts
        {
            get;
            private set;
        }

        public CustomerView(SICustomer sicustomer, SIUser user)
        {
            this.User = user;
            Customer customer = sicustomer.Customer;
            this.CustomerId = customer.CustomerId;
            this.CompanyId = customer.CompanyId;
            this.Name = customer.Name;
            this.Number = customer.CustomerNumber;
            this.Address1 = customer.Address1;
            this.Address2 = customer.Address2;
            this.Address3 = customer.Address3;
            this.City = customer.City;
            this.State = customer.State;
            this.Zip = customer.Zip;
            this.Active = customer.Active.GetValueOrDefault(false);
            this.DispatchId = customer.DispatchId;
            this.PurchaseConcrete = customer.PurchaseConcrete.GetValueOrDefault();
            this.PurchaseAggregate = customer.PurchaseAggregate.GetValueOrDefault();
            this.PurchaseBlock = customer.PurchaseBlock.GetValueOrDefault();
            this.APIActiveStatus = customer.APIActiveStatus.GetValueOrDefault();
            this.OverrideAUStatus = customer.OverrideAUStatus.GetValueOrDefault();

            this.CustomerContacts = new List<CustomerContactView>();
            foreach (SICustomerContact contact in sicustomer.Contacts)
            {
                this.CustomerContacts.Add(new CustomerContactView(contact));
            }

            BindValues();
        }

        public CustomerView(Customer customer)
        {
            this.CustomerId = customer.CustomerId;
            this.CompanyId = customer.CompanyId;
            this.Name = customer.Name;
            this.Number = customer.CustomerNumber;
            this.Address1 = customer.Address1;
            this.Address2 = customer.Address2;
            this.Address3 = customer.Address3;
            this.City = customer.City;
            this.State = customer.State;
            this.Zip = customer.Zip;
            this.Active = customer.Active.GetValueOrDefault(false);
            this.DispatchId = customer.DispatchId;
            this.CustomerContacts = new List<CustomerContactView>();
            this.Group = "";
            this.GroupHandle = "";

            this.PurchaseConcrete = customer.PurchaseConcrete.GetValueOrDefault();
            this.PurchaseAggregate = customer.PurchaseAggregate.GetValueOrDefault();
            this.PurchaseBlock = customer.PurchaseBlock.GetValueOrDefault();
            this.APIActiveStatus = customer.APIActiveStatus.GetValueOrDefault();
            this.OverrideAUStatus = customer.OverrideAUStatus.GetValueOrDefault();
        }

        public CustomerView()
        {
            this.CustomerContacts = new List<CustomerContactView>();
            //this.Name = "Name";
            //this.Number = "Number";
        }

        public void BindValues()
        {
            AllDistricts = new List<DistrictListView>();
            foreach (District d in SIDAL.GetDistricts(User.UserId))
            {
                AllDistricts.Add(new DistrictListView(d, SIDAL.GetCustomerDistricts(this.CustomerId).Select(x => x.DistrictId).Contains(d.DistrictId)));
            }

            LoadDefaultProducts();
        }

        public void LoadDefaultProducts()
        {
            var availableProducts = new List<SelectListItem>();

            availableProducts.Add(new SelectListItem { Value = ((int)ProductType.Concrete).ToString(), Text = ProductType.Concrete.ToString(), Selected = this.PurchaseConcrete });

            if (ConfigurationHelper.AggregateEnabled)
            {
                availableProducts.Add(new SelectListItem { Value = ((int)ProductType.Aggregate).ToString(), Text = ProductType.Aggregate.ToString(), Selected = this.PurchaseAggregate });
            }

            if (ConfigurationHelper.BlockEnabled)
            {
                availableProducts.Add(new SelectListItem { Value = ((int)ProductType.Block).ToString(), Text = ProductType.Block.ToString(), Selected = this.PurchaseBlock });
            }

            this.AvailableDefaultQuoteProducts = availableProducts;
        }
    }
}
