using System;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProjectProjection
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public DateTime ProjectionDate

        public DateTime ProjectionDate
        {
            get
            {
                return projectionDate;
            }
            set
            {
                projectionDate = value;
            }
        }

        #endregion

        #region public ProjectProjection ProjectProjection

        public ProjectProjection ProjectProjection
        {
            get
            {
                return projectProjection;
            }
            set
            {
                projectProjection = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected DateTime projectionDate

        protected DateTime projectionDate = DateTime.MinValue;

        #endregion

        #region protected ProjectProjection projectProjection

        protected ProjectProjection projectProjection = null;

        #endregion
    }
}
