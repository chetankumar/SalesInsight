using Foolproof;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.Web.Html5.Helpers;


namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class User
    {
        public string Guid { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter an email")]
        public string Email { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Username")]

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a username")]
        public string Username { get; set; }

        [Display(Name = "Change Password?")]
        public bool ChangePassword { get; set; }

        [Display(Name = "Password")]
        [RequiredIfTrue("ChangePassword", ErrorMessage = "You need to specify a password")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "The password is too short")]
        public string Password { get; set; }

        [Display(Name = "Password Confirmation")]
        [RequiredIfTrue("ChangePassword", ErrorMessage = "You need to specify a password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match"),]
        public string PasswordConfirmation { get; set; }

        [Display(Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a role")]
        public String Role { get; set; }

        [Display(Name = "Company")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a company")]
        public int Company { get; set; }

        [Display(Name = "Active")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select if the user is active")]
        public bool Active { get; set; }

        [ArrayRequired(1, ErrorMessage = "Please select the districts")]
        public string[] Districts { get; set; }

        public bool QuotationAccess { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double QuotationLimit { get; set; }

        public IList<DistrictListView> SelectedDistricts { get; set; }

        public IEnumerable<SelectListItem> AvailableDefaultQuoteProducts { get; set; }

        [Display(Name = "Default Quote Products")]
        public string[] PostedProductIds { get; set; }

        public SelectList AllRoles
        {
            get;
            private set;
        }

        public SelectList AllCompanies
        {
            get;
            private set;
        }

        public List<DistrictListView> AllDistricts
        {
            get;
            private set;
        }

        public void LoadDefaultProducts()
        {
            var selectedDefaultProducts = SIDAL.GetDefaultQuoteProducts(this.Guid).Select(x => x.ProductTypeId).ToList();

            var availableProducts = new List<SelectListItem>();

            availableProducts.Add(new SelectListItem { Value = ((int)ProductType.Concrete).ToString(), Text = ProductType.Concrete.ToString(), Selected = (selectedDefaultProducts.Count == 0 ? true : selectedDefaultProducts.Contains(((int)ProductType.Concrete))) });

            if (ConfigurationHelper.AggregateEnabled)
            {
                availableProducts.Add(new SelectListItem { Value = ((int)ProductType.Aggregate).ToString(), Text = ProductType.Aggregate.ToString(), Selected = selectedDefaultProducts.Contains(((int)ProductType.Aggregate)) });
            }

            if (ConfigurationHelper.BlockEnabled)
            {
                availableProducts.Add(new SelectListItem { Value = ((int)ProductType.Block).ToString(), Text = ProductType.Block.ToString(), Selected = selectedDefaultProducts.Contains(((int)ProductType.Block)) });
            }

            this.AvailableDefaultQuoteProducts = availableProducts;
        }

        public User()
        {
            AllRoles = new SelectList(SIDAL.GetRoles());
            AllCompanies = new SelectList(SIDAL.GetCompanies(), "CompanyId", "Name");
            AllDistricts = new List<DistrictListView>();
            foreach (District d in SIDAL.GetDistricts(Int32.Parse(AllCompanies.First().Value), null, 0, 1000, false))
            {
                AllDistricts.Add(new DistrictListView(d, false));
            }
            Active = true;

            LoadDefaultProducts();
        }

        public User(bool? isAdmin = false)
        {
            AllRoles = new SelectList(SIDAL.GetRoles());

            if (!isAdmin.GetValueOrDefault())
            {
                AllRoles = new SelectList(SIDAL.GetRoles().Where(x => !x.Contains("System Admin")));
            }
            AllCompanies = new SelectList(SIDAL.GetCompanies(), "CompanyId", "Name");
            AllDistricts = new List<DistrictListView>();
            foreach (District d in SIDAL.GetDistricts(Int32.Parse(AllCompanies.First().Value), null, 0, 1000, false))
            {
                AllDistricts.Add(new DistrictListView(d, false));
            }
            Active = true;

            LoadDefaultProducts();
        }

        public User(SIUser u, bool? isAdmin = false)
        {
            AllRoles = new SelectList(SIDAL.GetRoles());
            if (!isAdmin.GetValueOrDefault())
            {
                AllRoles = new SelectList(SIDAL.GetRoles().Where(x => !x.Contains("System Admin")));
            }
            AllCompanies = new SelectList(SIDAL.GetCompanies(), "CompanyId", "Name");
            AllDistricts = new List<DistrictListView>();

            this.Username = u.Username;
            this.Role = u.Role;
            this.Active = u.Active;
            this.Company = u.Company.CompanyId;
            this.Name = u.Name;
            this.Email = u.Email;
            this.Guid = u.UserId.ToString();

            QuotationRecipent QuotationSetting = SIDAL.FindQuotationRecipient(u.UserId);
            this.QuotationAccess = QuotationSetting.QuotationAccess.GetValueOrDefault(false);
            this.QuotationLimit = QuotationSetting.QuotationLimit.GetValueOrDefault(0);

            foreach (District d in SIDAL.GetDistricts(Int32.Parse(AllCompanies.First().Value), null, 0, 1000, false))
            {
                AllDistricts.Add(new DistrictListView(d, u.Districts.Select(d2 => d2.DistrictId).Contains(d.DistrictId)));
            }

            LoadDefaultProducts();
        }
    }
}
