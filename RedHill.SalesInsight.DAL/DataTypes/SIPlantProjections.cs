namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIPlantProjections
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

        #region public string PlantName

        public string PlantName
        {
            get
            {
                return plantName;
            }
            set
            {
                plantName = value;
            }
        }

        #endregion

        #region public int ForecastQuantity

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

        public int ActualQuantity { get; set; }

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

        public string DisplayQuantity { get; set; }

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected string monthName

        protected string monthName = string.Empty;

        #endregion

        #region protected string plantName

        protected string plantName = string.Empty;

        #endregion

        #region protected int forecastQuantity

        protected int forecastQuantity = 0;

        #endregion

        #region protected int targetQuantity

        protected double targetQuantity = 0;

        #endregion
    }
}
