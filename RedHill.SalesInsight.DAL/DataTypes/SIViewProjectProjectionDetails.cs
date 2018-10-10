using System;
using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIViewProjectProjectionDetails
    {
        //---------------------------------
        // Static
        //---------------------------------

        #region public static List<DateTime> GetMonthRange(DateTime startMonth, int monthCount)

        public static List<DateTime> GetMonthRange(DateTime startMonth, int monthCount)
        {
            // Create the list
            List<DateTime> monthRange = new List<DateTime>();

            // For each month
            for(int index = 0; index < monthCount; index++)
            {
                monthRange.Add(startMonth.Date.AddMonths(index));
            }

            // Return the month range
            return monthRange;
        }

        #endregion

        //---------------------------------
        // Properties
        //---------------------------------

        public int? PlantId { get; set; }

        #region public int? TotalProjected

        public int? TotalProjected
        {
            get
            {
                return totalProjected;
            }
            set
            {
                totalProjected = value;
            }
        }

        #endregion

        #region public int? TotalShipped

        public int? TotalShipped
        {
            get
            {
                return totalShipped;
            }
            set
            {
                totalShipped = value;
            }
        }

        #endregion

        #region public int? CurrentProjected

        public int? CurrentProjected
        {
            get
            {
                return currentProjected;
            }
            set
            {
                currentProjected = value;
            }
        }

        #endregion

        #region public SIProjectProjection ProjectionMonth

        public SIProjectProjection ProjectionMonth
        {
            get
            {
                return projectionMonth;
            }
            set
            {
                projectionMonth = value;
            }
        }

        #endregion

        #region public List<SIProjectProjection> ProjectProjections

        public List<SIProjectProjection> ProjectProjections
        {
            get
            {
                return projectProjections;
            }
            set
            {
                projectProjections = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected int? totalProjected

        protected int? totalProjected = int.MinValue;

        #endregion

        #region protected int? totalShipped

        protected int? totalShipped = int.MinValue;

        #endregion

        #region protected int? currentProjected

        protected int? currentProjected = int.MinValue;

        #endregion

        #region protected SIProjectProjection projectionMonth

        protected SIProjectProjection projectionMonth = null;

        #endregion

        #region protected List<SIProjectProjection> projectProjections

        protected List<SIProjectProjection> projectProjections = null;

        #endregion
    }
}
