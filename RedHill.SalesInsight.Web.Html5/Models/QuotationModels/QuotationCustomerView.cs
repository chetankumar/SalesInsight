using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.Web.Html5.Models;
using RedHill.SalesInsight.DAL;
using System.Web.Security;
using RedHill.SalesInsight.DAL.DataTypes;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuotationCustomerView
    {
        public Guid UserId { get; set; }
        public long QuotationId { get; set; }
        public QuotationProfile Profile { get; set; }

        [Required(ErrorMessage = "The Customer field is required.")]
        public int CustomerId { get; set; }
        public int CustomerContactId { get; set; }
        public CustomerView CustomerInfo { get; set; }
        public CustomerView NewCustomerInfo { get; set; }
        public CustomerContactView CustomerContactInfo { get; set; }
        public CustomerContactView NewCustomerContactInfo { get; set; }

        [Display(Name = "Tax Code")]
        public long TaxCodeId { get; set; }
        public string TaxExcemptReason { get; set; }

        public QuotationCustomerView()
        {
            this.CustomerContactInfo = new CustomerContactView();
            this.CustomerInfo = new CustomerView();
            this.NewCustomerContactInfo = new CustomerContactView();
            this.NewCustomerInfo = new CustomerView();
            this.Profile = new QuotationProfile();
        }

        public QuotationCustomerView(long id) : this(SIDAL.FindQuotation(id))
        {

        }

        public QuotationCustomerView(Quotation quotation)
        {
            //this.QuotationId = id;
            //Quotation quotation = SIDAL.FindQuotation(QuotationId);
            this.CustomerId = quotation.CustomerId.GetValueOrDefault();
            if (this.CustomerId > 0)
                this.CustomerInfo = new CustomerView(quotation.Customer);
            else
                this.CustomerInfo = new CustomerView();
            
            this.CustomerContactId = quotation.CustomerContactId.GetValueOrDefault();
            if (this.CustomerContactId > 0)
                this.CustomerContactInfo = new CustomerContactView(quotation.CustomerContact);
            else
                this.CustomerContactInfo = new CustomerContactView();

            this.TaxCodeId = quotation.TaxCodeId.GetValueOrDefault();
            this.TaxExcemptReason = quotation.TaxExemptReason;
            this.Profile = new QuotationProfile();

        }

        public void Load(Quotation quotation = null)
        {
            if (QuotationId > 0)
            {
                if (quotation == null)
                {
                    quotation = SIDAL.FindQuotation(QuotationId);
                }
                Profile = new QuotationProfile();
                Profile.CurrentUserId = this.UserId;
                Profile.Load(quotation);
            }
            else
            {
                Profile = new QuotationProfile();
            }
        }

        //public SelectList ChooseCustomers
        //{
        //    get
        //    {
        //        return new SelectList(SIDAL.GetCustomers(UserId), "CustomerId", "Name", CustomerId);
        //    }
        //}

        //public SelectList ChooseTaxCodes
        //{
        //    get
        //    {
        //        var selectOptions = SIDAL.GetTaxCodes().Select(x => new { Text = x.Code + " - " + x.Description, Value = x.Id });
        //        return new SelectList(selectOptions, "Value", "Text", TaxCodeId);
        //    }
        //}

        public SelectList ChooseContacts
        {
            get
            {
                if (CustomerId > 0)
                    return new SelectList(SIDAL.GetCustomerContacts(CustomerId, null, 0, 1000), "Id", "Name", CustomerId);
                else
                    return null;
            }
        }
    }
}