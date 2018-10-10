namespace RedHill.SalesInsight.DAL.DataTypes
{
   public class SIViewProjectBidderDetails
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public ProjectBidder ProjectBidder

        public ProjectBidder ProjectBidder
        {
            get
            {
                return projectBidder;
            }
            set
            {
                projectBidder = value;
            }
        }

        #endregion

        #region public Competitor Competitor

        public Customer Customer
        {
            get
            {
                return customer;
            }
            set
            {
                customer = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected ProjectBidder projectBidder

        protected ProjectBidder projectBidder = null;

        #endregion

        #region protected Customer customer

        protected Customer customer = null;

        #endregion
    }
}
