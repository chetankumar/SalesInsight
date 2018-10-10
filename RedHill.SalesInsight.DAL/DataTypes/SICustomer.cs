namespace RedHill.SalesInsight.DAL.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Linq;
    using System.Data.SqlClient;

    public class SICustomer
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SICustomer()
        {
            this.customer = null;
            this.contacts = new List<SICustomerContact>(0);
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region public Customer Customer

        public Customer Customer
        {
            get
            {
                return this.customer;
            }
            set
            {
                this.customer = value;
            }
        }

        #endregion public Customer Customer

        #region public List<SICustomerContact> Contacts

        public List<SICustomerContact> Contacts
        {
            get
            {
                return this.contacts;
            }
            set
            {
                this.contacts = value;
            }
        }

        #endregion public List<SICustomerContact> Contacts


        #region public List<District> Districts

        public List<District> Districts
        {
            get
            {
                return this.district;
            }
            set
            {
                this.district = value;
            }
        }

        #endregion public List<SICustomerContact> Contacts

        //---------------------------------
        // Methods
        //---------------------------------

        #region Get

        internal static SICustomer Get(int customerId)
        {
            // Validate the parameter(s)
            if (customerId <= 0)
            {
                return null;
            }

            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Get the result
                SICustomer customer =
                (
                    from c in context.Customers
                    where c.CustomerId == customerId
                    select new SICustomer
                    {
                        Customer = c
                    }
                ).SingleOrDefault();

                if (customer != null)
                {
                    customer.Contacts = SICustomerContact.Get(customerId, context);
                }

                // Return the result
                return customer;
            }
        }

        #endregion Get

        #region Save

        internal SISaveCustomerStatus Save()
        {
            // Validate the parameter(s) and class data
            if (this.customer == null)
            {
                throw new NullReferenceException("The value 'null' was found where an instance of the Customer class was required.");
            }

            if (this.customer.CustomerId <= 0)
            {
                return this.Insert();
            }
            else
            {
                return this.Update();
            }
        }

        #endregion Save

        #region Insert

        private SISaveCustomerStatus Insert()
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Create the status
                SISaveCustomerStatus status = new SISaveCustomerStatus();

                // Insert the customer
                this.customer.CustomerId = 0;
                context.Customers.InsertOnSubmit(this.customer);

                // Commit to get the customer id
                context.SubmitChanges();

                // Update the customer id
                status.CustomerId = this.customer.CustomerId;

                // Insert all customer child entities to the context
                if ((status.Result = this.InsertContacts(context.CustomerContacts)) == SaveCustomerResult.Success)
                {
                    // Commit all customer child entities to the context
                    context.SubmitChanges();
                }

                return status;
            }
        }

        #endregion Insert

        #region InsertContacts

        private SaveCustomerResult InsertContacts(Table<CustomerContact> contextTable)
        {
            if (this.contacts != null)
            {
                // Update all contacts with retrieved customer id & insert to the context
                foreach (SICustomerContact contact in this.contacts)
                {
                    CustomerContact entity = contact.CustomerContact;

                    // Ensure contact to insert is unique
                    if (!SICustomerContact.IsUnique(this.contacts, contextTable, entity.CustomerId, entity.Name, entity.Id))
                    {
                        return SaveCustomerResult.DuplicateCustomerContactName;
                    }

                    entity.Id = 0;
                    entity.CustomerId = this.customer.CustomerId;
                    contextTable.InsertOnSubmit(entity);
                }
            }

            return SaveCustomerResult.Success;
        }

        #endregion InsertContacts

        #region Update

        private SISaveCustomerStatus Update()
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Create the status
                SISaveCustomerStatus status = new SISaveCustomerStatus();

                // Get customer from context
                SICustomer customerFromContext = SICustomer.Get(this.customer.CustomerId);

                // Update the customer
                context.Customers.Attach(this.customer);
                context.Refresh(RefreshMode.KeepCurrentValues, this.customer);

                // Update all customer child entities to the context
                if ((status.Result = this.UpdateContacts(context, customerFromContext.Contacts)) == SaveCustomerResult.Success)
                {
                    // Commit all customer child entities to the context
                    context.SubmitChanges();

                    // Get the id
                    status.CustomerId = customerFromContext.Customer.CustomerId;
                }

                return status;
            }
        }

        #endregion Update

        #region UpdateContacts

        private SaveCustomerResult UpdateContacts(SalesInsightDataContext context, List<SICustomerContact> contactsFromContext)
        {
            // Set new/old lists to empty list if null to ensure proper inserts/updates/deletes below
            if (this.contacts == null)
            {
                this.contacts = new List<SICustomerContact>(0);
            }
            if (contactsFromContext == null)
            {
                contactsFromContext = new List<SICustomerContact>(0);
            }

            // Delete all contacts in the context but not the parameter entity
            foreach (SICustomerContact contactFromContext in contactsFromContext)
            {
                if (!this.contacts.Exists(cc => cc.CustomerContact.Id == contactFromContext.CustomerContact.Id))
                {
                    context.CustomerContacts.DeleteOnSubmit(contactFromContext.CustomerContact);
                }
            }

            // Update or insert all contacts in the parameter entity
            foreach (SICustomerContact contact in this.contacts)
            {
                CustomerContact entity = contact.CustomerContact;

                // Ensure contact to update or insert is unique
                if (!SICustomerContact.IsUnique(this.contacts, context.CustomerContacts, this.customer.CustomerId, entity.Name, entity.Id))
                {
                    return SaveCustomerResult.DuplicateCustomerContactName;
                }

                if (contactsFromContext.Exists(cc => cc.CustomerContact.Id == entity.Id))
                {
                    // Update the contact
                    context.CustomerContacts.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                else
                {
                    // Insert the contact
                    entity.Id = 0;
                    entity.CustomerId = this.customer.CustomerId;
                    context.CustomerContacts.InsertOnSubmit(entity);
                }
            }

            return SaveCustomerResult.Success;
        }

        #endregion UpdateContacts

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        protected Customer customer;
        protected List<SICustomerContact> contacts;
        protected List<District> district;
        #endregion Fields
    }
}
