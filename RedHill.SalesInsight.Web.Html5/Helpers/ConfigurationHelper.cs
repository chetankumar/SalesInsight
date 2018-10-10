using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RedHill.SalesInsight.Web.Html5.Helpers
{
    public static class ConfigurationHelper
    {
        private static Company _company = null;
        private static List<SupportCategory> _userSupportCategories = null;
        private static string _userEmail = null;
        private static bool _notificationsEnabled = true;

        public static Company Company
        {
            get
            {
                var session = System.Web.HttpContext.Current.Session;
                //if (_company == null)
                //{
                if (session["CompanyId"] != null)
                {
                    _company = SIDAL.GetCompany(Convert.ToInt32(session["CompanyId"].ToString()));
                }
                else
                {
                    _company = SIDAL.GetCompany();
                }
                //}
                return _company;
            }
        }

        public static List<SupportCategory> UserSupportCategories
        {
            get
            {
                //if (_userSupportCategories == null || _userSupportCategories.Count == 0)
                {
                    _userSupportCategories = SIDAL.GetSupportCategories();
                }
                return _userSupportCategories;
            }
        }

        public static string UserEmail
        {
            get
            {
                var context = HttpContext.Current;
                if (context.User.Identity.IsAuthenticated)
                {
                    var user = SIDAL.GetUser(Membership.GetUser(context.User.Identity.Name).ProviderUserKey.ToString());
                    if (user != null)
                        _userEmail = user.Email;
                }
                else
                {
                    _userEmail = null;
                }

                return _userEmail;
            }
        }

        public static bool ChatEnabled
        {
            get
            {
                var chatEnabled = ConfigurationManager.AppSettings["ChatEnabled"];
                bool enabled = false;
                if (!string.IsNullOrWhiteSpace(chatEnabled))
                    Boolean.TryParse(chatEnabled, out enabled);
                return enabled;
            }
        }

        public static bool NotificationsEnabled
        {
            get
            {
                var context = HttpContext.Current;
                if (context.User.Identity.IsAuthenticated)
                {
                    _notificationsEnabled = SIDAL.ChatNotificationsEnabled(Membership.GetUser(context.User.Identity.Name).ProviderUserKey.ToString());
                }
                return _notificationsEnabled;
            }
        }

        public static bool BlockEnabled
        {
            get { return Company != null ? Company.EnableBlock.GetValueOrDefault() : false; }
        }

        public static bool AggregateEnabled
        {
            get { return Company != null ? Company.EnableAggregate.GetValueOrDefault() : false; }
        }

        public static bool APIEnabled
        {
            get
            {
                return SIDAL.IsApiEnabled();
            }
        }

        public static List<SelectListItem> ProductTypes
        {
            get
            {
                var productTypes = new List<SelectListItem>();
                productTypes.Add(new SelectListItem { Value = ProductType.Concrete.ToString(), Text = ProductType.Concrete.ToString() });

                if (ConfigurationHelper.AggregateEnabled)
                {
                    productTypes.Add(new SelectListItem { Value = ProductType.Aggregate.ToString(), Text = ProductType.Aggregate.ToString() });
                }

                if (ConfigurationHelper.BlockEnabled)
                {
                    productTypes.Add(new SelectListItem { Value = ProductType.Block.ToString(), Text = ProductType.Block.ToString() });
                }
                return productTypes;
            }
        }
    }
}