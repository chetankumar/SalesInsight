namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SISaveCustomerStatus
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public SaveCustomerResult Result

        public SaveCustomerResult Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        #endregion

        #region public int? CustomerId

        public int? CustomerId
        {
            get
            {
                return customerId;
            }
            set
            {
                customerId = value;
            }
        }

        #endregion
        
        //---------------------------------
        // Fields
        //---------------------------------

        #region protected SaveCustomerResult result

        protected SaveCustomerResult result = SaveCustomerResult.Undefined;

        #endregion

        #region protected int? customerId

        protected int? customerId = null;

        #endregion
    }
}
