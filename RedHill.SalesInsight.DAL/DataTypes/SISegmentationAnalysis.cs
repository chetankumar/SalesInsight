namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SISegmentationAnalysis
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public string MonthName

        public string MonthName
        {
            get
            {
                return this.monthName;
            }
            set
            {
                this.monthName = value;
            }
        }

        #endregion

        #region public string MarketSegmentName

        public string MarketSegmentName
        {
            get
            {
                return this.marketSegmentName;
            }
            set
            {
                this.marketSegmentName = value;
            }
        }

        #endregion

        #region public int ForecastQuantity

        public int ForecastQuantity
        {
            get
            {
                return this.forecastQuantity;
            }
            set
            {
                this.forecastQuantity = value;
            }
        }

        #endregion

        #region public int TargetQuantity

        public int TargetQuantity
        {
            get
            {
                return this.targetQuantity;
            }
            set
            {
                this.targetQuantity = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected string monthName

        protected string monthName;

        #endregion

        #region protected string marketSegmentName

        protected string marketSegmentName;

        #endregion

        #region protected int forecastQuantity

        protected int forecastQuantity;

        #endregion

        #region protected int targetQuantity

        protected int targetQuantity;

        #endregion
    }
}
