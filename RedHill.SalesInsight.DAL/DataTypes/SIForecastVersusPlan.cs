namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIForecastVersusPlan
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public string MonthName

        public string MonthName
        {
            get
            {
                return monthName;
            }
            set
            {
                monthName = value;
            }
        }

        #endregion

        #region public string DistrictName

        public string DistrictName
        {
            get
            {
                return districtName;
            }
            set
            {
                districtName = value;
            }
        }

        #endregion

        #region public  ForecastQuantity

        public int ForecastQuantity
        {
            get
            {
                return forecastQuantity;
            }
            set
            {
                forecastQuantity = value;
            }
        }

        #endregion

        #region public int TargetQuantity

        public double TargetQuantity
        {
            get
            {
                return targetQuantity;
            }
            set
            {
                targetQuantity = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected string monthName

        protected string monthName = string.Empty;

        #endregion

        #region protected string districtName

        protected string districtName = string.Empty;

        #endregion

        #region protected int forecastQuantity

        protected int forecastQuantity = 0;

        #endregion

        #region protected int targetQuantity

        protected double targetQuantity = 0;

        #endregion
    }
}