namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProjectedActualAssetProductivity : SIPlantProjections
    {
        //---------------------------------
        // Properties
        //---------------------------------

        public double? TargetTruckCount { get; set; }

        public double ActualTruckCount
        {
            get
            {
                if (TargetTruckCount != null && TargetTruckCount.Value!=0)
                {
                    return TargetTruckCount.GetValueOrDefault(0);
                }
                else
                {
                    return TruckCount;
                }
            }
        }

        #region public int TruckCount

        public int TruckCount
        {
            get
            {
                return truckCount;
            }
            set
            {
                truckCount = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected int TruckCount

        protected int truckCount = 0;

        #endregion
    }
}
