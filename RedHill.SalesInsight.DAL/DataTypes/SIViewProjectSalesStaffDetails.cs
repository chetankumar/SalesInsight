namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIViewProjectSalesStaffDetails
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public ProjectSalesStaff ProjectSalesStaff

        public ProjectSalesStaff ProjectSalesStaff
        {
            get
            {
                return projectSalesStaff;
            }
            set
            {
                projectSalesStaff = value;
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

        #region protected ProjectSalesStaff projectSalesStaff

        protected ProjectSalesStaff projectSalesStaff = null;

        #endregion

        #region protected SalesStaff salesStaff

        protected SalesStaff salesStaff = null;

        #endregion
    }
}
