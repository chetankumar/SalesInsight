using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SICustomerContact
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SICustomerContact()
        {
            this.customerContact = null;
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region public CustomerContact CustomerContact

        public CustomerContact CustomerContact
        {
            get
            {
                return this.customerContact;
            }
            set
            {
                this.customerContact = value;
            }
        }

        #endregion public CustomerContact CustomerContact

        //---------------------------------
        // Methods
        //---------------------------------

        #region Get

        internal static List<SICustomerContact> Get(int customerId, SalesInsightDataContext context)
        {
            // Validate the parameter(s)
            if (customerId <= 0)
            {
                return new List<SICustomerContact>(0);
            }

            // Get the result
            var result =
            (
                from cc in context.CustomerContacts
                where cc.CustomerId == customerId
                select new SICustomerContact
                {
                    CustomerContact = cc
                }
            );

            return (result == null ? new List<SICustomerContact>(0) : result.ToList());
        }

        #endregion Get

        #region IsUnique

        internal static bool IsUnique(List<SICustomerContact> customerContacts, Table<CustomerContact> contextTable, int customerId, string name, int customerContactId)
        {
            // Verify that specified product/plant is the only one in the list
            if (customerContacts.Count(cc => cc.CustomerContact.CustomerId == customerId && cc.CustomerContact.Name == name) > 1)
            {
                return false;
            }

            // Count the matching product/plant records in the context not including the specified id
            int count =
            (
                from ct in contextTable
                where ct.CustomerId == customerId && ct.Name == name && ct.Id != customerContactId
                select ct
             ).Count();

            return (count <= 0);
        }

        #endregion IsUnique

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        protected CustomerContact customerContact;

        #endregion Fields
    }
}
