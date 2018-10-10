using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIConcreteProductPlant
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SIConcreteProductPlant()
        {
            concreteProductPlant = null;
            plantName = string.Empty;
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region public ConcreteProductPlant ConcreteProductPlant

        public ConcreteProductPlant ConcreteProductPlant
        {
            get
            {
                return concreteProductPlant;
            }
            set
            {
                concreteProductPlant = value;
            }
        }

        #endregion public ConcreteProductPlant ConcreteProductPlant

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

        internal static bool IsUnique(List<SIConcreteProductPlant> productPlants, Table<ConcreteProductPlant> contextTable, int productId, int plantId, int id)
        {
            // Verify that specified product/plant is the only one in the list
            if (productPlants.Count(pp => pp.ConcreteProductPlant.ConcreteProductId == productId && pp.ConcreteProductPlant.PlantId == plantId) > 1)
            {
                return false;
            }

            // Count the matching product/plant records in the context not including the specified id
            int count =
            (
                from ct in contextTable
                where ct.ConcreteProductId == productId && ct.PlantId == plantId && ct.Id != id
                select ct
             ).Count();

            return (count <= 0);
        }

        #endregion IsUnique

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        protected ConcreteProductPlant concreteProductPlant = null;
        protected string plantName = string.Empty;

        #endregion Fields
    }
}
