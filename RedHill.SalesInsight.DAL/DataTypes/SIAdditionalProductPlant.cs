using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIAdditionalProductPlant
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SIAdditionalProductPlant()
        {
            additionalProductPlant = null;
            plantName = string.Empty;
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region public AdditionalProductPlant AdditionalProductPlant

        public AdditionalProductPlant AdditionalProductPlant
        {
            get
            {
                return additionalProductPlant;
            }
            set
            {
                additionalProductPlant = value;
            }
        }

        #endregion public AdditionalProductPlant AdditionalProductPlant

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

        #endregion public string PlantName

        //---------------------------------
        // Methods
        //---------------------------------

        #region IsUnique

        internal static bool IsUnique(List<SIAdditionalProductPlant> productPlants, Table<AdditionalProductPlant> contextTable, int productId, int plantId, int id)
        {
            // Verify that specified product/plant is the only one in the list
            if (productPlants.Count(pp => pp.AdditionalProductPlant.AdditionalProductId == productId && pp.AdditionalProductPlant.PlantId == plantId) > 1)
            {
                return false;
            }

            // Count the matching product/plant records in the context not including the specified id
            int count =
            (
                from ct in contextTable
                where ct.AdditionalProductId == productId && ct.PlantId == plantId && ct.Id != id
                select ct
             ).Count();

            return (count <= 0);
        }

        #endregion IsUnique

        //---------------------------------
        // Fields 
        //---------------------------------

        #region Fields

        protected AdditionalProductPlant additionalProductPlant;
        protected string plantName;

        #endregion Fields
    }
}
