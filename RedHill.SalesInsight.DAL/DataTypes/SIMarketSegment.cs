using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
   public class SIMarketSegment
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

        #region public MarketSegment MarketSegment

        public MarketSegment MarketSegment
        {
            get
            {
                return marketSegment;
            }
            set
            {
                marketSegment = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected MarketSegment marketSegment

        protected MarketSegment marketSegment = null;

        #endregion

        #region protected List<District> districts

        protected List<District> districts = null;

        #endregion
    }
}
