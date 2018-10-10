using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SISalesStaff
    {
        //---------------------------------
        // Properties
        //---------------------------------

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
        
        #region public SalesStaff SalesStaff

        public SalesStaff SalesStaff
        {
            get
            {
                return salesStaff;
            }
            set
            {
                salesStaff = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected SalesStaff salesStaff

        protected SalesStaff salesStaff = null;

        #endregion

        #region protected List<District> districts

        protected List<District> districts = null;

        #endregion
    }
}
