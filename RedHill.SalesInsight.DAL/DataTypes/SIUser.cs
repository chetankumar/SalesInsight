using System;
using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIUser
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public Guid UserId

        public Guid UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }

        #endregion

        #region public string Name

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        #endregion

        #region public string Username

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        #endregion

        #region public string Password

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

        #endregion

        #region public string Role

        public string Role
        {
            get
            {
                return role;
            }
            set
            {
                role = value;
            }
        }

        #endregion

        #region public string Email

        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }

        #endregion

        #region public Company Company

        public Company Company
        {
            get
            {
                return company;
            }
            set
            {
                company = value;
            }
        }

        #endregion

        #region public List<District> Districts

        public List<District> Districts
        {
            get
            {
                return districts;
            }
            set
            {
                districts = value;
            }
        }

        #endregion

        #region public List<Plant> Plants

        public List<Plant> Plants
        {
            get
            {
                return plants;
            }
            set
            {
                plants = value;
            }
        }

        #endregion

        #region public bool Active

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected Guid userId

        protected Guid userId = Guid.Empty;

        #endregion

        #region protected string username

        protected string username = null;

        #endregion

        #region protected string name

        protected string name = null;

        #endregion

        #region protected string password

        protected string password = null;

        #endregion

        #region protected string role

        protected string role = null;

        #endregion

        #region protected string email

        protected string email = null;

        #endregion

        #region protected Company company

        protected Company company = new Company();

        #endregion

        #region protected List<District> districts

        protected List<District> districts = new List<District>();

        #endregion

        #region protected List<Plant> plants

        protected List<Plant> plants = new List<Plant>();

        #endregion

        #region protected bool active

        protected bool active = true;

        #endregion
    }
}
