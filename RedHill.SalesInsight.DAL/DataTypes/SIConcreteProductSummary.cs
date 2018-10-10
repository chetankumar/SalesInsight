using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIConcreteProductSummary
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SIConcreteProductSummary()
        {
            id = int.MinValue;
            companyId = int.MinValue;
            companyName = string.Empty;
            code = string.Empty;
            description = null;
            active = true;
            productPlantId = null;
            plantId = null;
            plantName = null;
            price = null;
            cost = null;
            productPlantActive = null;
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region Id

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        #endregion Id

        #region CompanyId

        public int CompanyId
        {
            get
            {
                return companyId;
            }
            set
            {
                companyId = value;
            }
        }

        #endregion CompanyId

        #region CompanyName

        public string CompanyName
        {
            get
            {
                return companyName;
            }
            set
            {
                companyName = value;
            }
        }

        #endregion CompanyName

        #region Code

        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        #endregion Code

        #region Description

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        #endregion Description

        #region Active

        public bool Active
        {
            get
            {
                return (productPlantActive.HasValue ? productPlantActive.Value : active);
            }
        }

        #endregion Active

        #region ProductActive

        internal bool ProductActive
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }

        #endregion ProductActive

        #region ProductPlantId

        public int? ProductPlantId
        {
            get
            {
                return productPlantId;
            }
            set
            {
                productPlantId = value;
            }
        }

        #endregion ProductPlantId

        #region PlantId

        public int? PlantId
        {
            get
            {
                return plantId;
            }
            set
            {
                plantId = value;
            }
        }

        #endregion PlantId

        #region PlantName

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

        #endregion PlantName

        #region Price

        public decimal? Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        #endregion Price

        #region Cost

        public decimal? Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
            }
        }

        #endregion Cost

        #region ProductPlantActive

        internal bool? ProductPlantActive
        {
            get
            {
                return productPlantActive;
            }
            set
            {
                productPlantActive = value;
            }
        }

        #endregion ProductPlantActive

        //---------------------------------
        // Methods
        //---------------------------------

        #region Get

        internal static List<SIConcreteProductSummary> Get(int companyId, int? plantId, SalesInsightDataContext context)
        {
            // Validate the parameter(s)
            if (companyId <= 0)
            {
                throw new ArgumentOutOfRangeException("companyId", companyId.ToString(), "The valid CompanyId range is: > 0.");
            }
            if (plantId.HasValue && plantId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException("plantId", plantId.ToString(), "The valid PlantId range is: null or > 0.");
            }

            List<SIConcreteProductSummary> concreteProductSummaries;

            if (plantId.HasValue)
            {
                var result =
                (
                    from c in context.Companies
                    join cp in context.ConcreteProducts on c.CompanyId equals cp.CompanyId
                    join cpp in context.ConcreteProductPlants on cp.Id equals cpp.ConcreteProductId
                    where c.CompanyId == companyId && cpp.PlantId == plantId.Value
                    select new SIConcreteProductSummary
                    {
                        Id = cp.Id,
                        CompanyId = cp.CompanyId,
                        CompanyName = cp.Company.Name,
                        Code = cp.Code,
                        Description = cp.Description,
                        ProductActive = cp.Active,
                        ProductPlantId = (int?)cpp.Id,
                        PlantId = (int?)cpp.PlantId,
                        PlantName = cpp.Plant.Name,
                        Price = cpp.Price,
                        Cost = cpp.Cost,
                        ProductPlantActive = (bool?)cpp.Active
                    }
                );

                concreteProductSummaries = (result == null ? new List<SIConcreteProductSummary>(0) : result.ToList());
            }
            else
            {
                var result =
                (
                    from c in context.Companies
                    join cp in context.ConcreteProducts on c.CompanyId equals cp.CompanyId
                    where c.CompanyId == companyId
                    select new SIConcreteProductSummary
                    {
                        Id = cp.Id,
                        CompanyId = cp.CompanyId,
                        CompanyName = cp.Company.Name,
                        Code = cp.Code,
                        Description = cp.Description,
                        ProductActive = cp.Active
                    }
                );

                concreteProductSummaries = (result == null ? new List<SIConcreteProductSummary>(0) : result.ToList());
            }

            return concreteProductSummaries;
        }

        #endregion Get

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        protected int id;
        protected int companyId;
        protected string companyName;
        protected string code;
        protected string description;
        protected bool active;
        protected int? productPlantId;
        protected int? plantId;
        protected string plantName;
        protected decimal? price;
        protected decimal? cost;
        protected bool? productPlantActive;

        #endregion Fields
    }
}
