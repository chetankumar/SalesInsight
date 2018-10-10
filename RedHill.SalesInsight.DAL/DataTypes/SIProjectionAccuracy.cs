namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProjectionAccuracy
    {
        //---------------------------------
        // Constructors
        //---------------------------------

        #region Constructors

        public SIProjectionAccuracy()
        {
            this.salesStaffName = string.Empty;
            this.forecastQuantity = 0;
            this.actualQuantity = 0;
        }

        public SIProjectionAccuracy(string salesStaffName) : base()
        {
            this.salesStaffName = salesStaffName;
        }

        public SIProjectionAccuracy(string salesStaffName, int actualQuantity, int forecastQuantity)
        {
            this.salesStaffName = salesStaffName;
            this.forecastQuantity = actualQuantity;
            this.actualQuantity = forecastQuantity;
        }

        #endregion

        //---------------------------------
        // Properties
        //---------------------------------

        #region public string SalesStaffName

        public string SalesStaffName
        {
            get
            {
                return this.salesStaffName;
            }
            set
            {
                this.salesStaffName = value;
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

        #region public int ActualQuantity

        public int ActualQuantity
        {
            get
            {
                return this.actualQuantity;
            }
            set
            {
                this.actualQuantity = value;
            }
        }

        #endregion

        public string PlantName { get; set; }

        public string ProjectName { get; set; }

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected string salesStaffName

        protected string salesStaffName;

        #endregion

        #region protected int forecastQuantity

        protected int forecastQuantity;

        #endregion

        #region protected int actualQuantity

        protected int actualQuantity;

        #endregion
    }
}
