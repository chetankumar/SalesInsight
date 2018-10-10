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
    public class CustomerContactView
    {
        private CustomerContact customerContact;

        [HiddenInput]
        public int Id { get; set; }

        [HiddenInput]
        public int CustomerId { get; set; }

        [HiddenInput]
        public int ProjectId { get; set; }

        [HiddenInput]
        public int QuoteId { get; set; }

        public string Title { get; set; }
        [Required]
        public string Name { get; set; }

        public string Fax { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsQuoteDefault { get; set; }
        public bool IsActive { get; set; }

        public CustomerContactView()
        {
        }

        public CustomerContactView(SICustomerContact contact)
        {
            this.Title = contact.CustomerContact.Title;
            this.Name = contact.CustomerContact.Name;
            this.Fax = contact.CustomerContact.Fax;
            this.Email = contact.CustomerContact.Email;
            this.Phone = contact.CustomerContact.Phone;
            this.CustomerId = contact.CustomerContact.CustomerId;
            this.Id = contact.CustomerContact.Id;
            this.IsQuoteDefault = contact.CustomerContact.IsQuoteDefault.GetValueOrDefault(false);
            this.IsActive = contact.CustomerContact.IsActive.GetValueOrDefault(false);
        }

        public CustomerContactView(CustomerContact contact)
        {
            this.Title = contact.Title;
            this.Name = contact.Name;
            this.Fax = contact.Fax;
            this.Email = contact.Email;
            this.Phone = contact.Phone;
            this.CustomerId = contact.CustomerId;
            this.Id = contact.Id;
            this.IsQuoteDefault = contact.IsQuoteDefault.GetValueOrDefault(false);
            this.IsActive = contact.IsActive.GetValueOrDefault(false);
        }
    }
}
