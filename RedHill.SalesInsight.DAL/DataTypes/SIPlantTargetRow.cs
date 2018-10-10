using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIPlantTargetRow
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public Dictionary<string, SIPlantTargetColumn> Columns

        public Dictionary<string, SIPlantTargetColumn> Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
            }
        }

        #endregion
        
        //---------------------------------
        // Fields
        //---------------------------------

        #region protected Dictionary<string, SIPlantTarget> columns

        protected Dictionary<string, SIPlantTargetColumn> columns = new Dictionary<string, SIPlantTargetColumn>();

        #endregion

        /*//---------------------------------
        // Properties
        //---------------------------------

        #region public List<Dictionary<string, string>> Values

        public List<Dictionary<string, string>> Values
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected List<Dictionary<string, string>> values

        protected List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();

        #endregion*/

        /*//---------------------------------
        // Properties
        //---------------------------------

        #region public List<SIPlantTarget> PlantTargets

        public List<SIPlantTarget> PlantTargets
        {
            get
            {
                return plantTargets;
            }
            set
            {
                plantTargets = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected List<SIPlantTarget> plantTargets

        protected List<SIPlantTarget> plantTargets = new List<SIPlantTarget>();

        #endregion*/
    }
}
