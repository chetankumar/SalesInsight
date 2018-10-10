using System;
using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIForecastProjects
    {
        //---------------------------------
        // Properties
        //---------------------------------

        public int RowCount { get; set; }

        #region public List<DateTime> ProjectionMonths

        public List<DateTime> ProjectionMonths
        {
            get
            {
                return projectionMonths;
            }
            set
            {
                projectionMonths = value;
            }
        }

        #endregion

        #region public List<SIForecastProject> Projects

        public List<SIForecastProject> Projects
        {
            get
            {
                return projects;
            }
            set
            {
                projects = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected List<DateTime> projectionMonths

        protected List<DateTime> projectionMonths = null;

        #endregion

        #region protected List<SIForecastProject> projects

        protected List<SIForecastProject> projects = null;

        #endregion
    }
}
