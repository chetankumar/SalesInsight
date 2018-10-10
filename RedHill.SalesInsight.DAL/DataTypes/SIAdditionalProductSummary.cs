using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIAdditionalProductSummary
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SIAdditionalProductSummary()
        {
            id = int.MinValue;
            companyId = int.MinValue;
            companyName = string.Empty;
            unitOfMeasureId = int.MinValue;
            productTypeId = SIProductType.MinValue;
            code = string.Empty;
            description = null;
            active = true;
            standardProduct = false;
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

        #region StandardProduct

        public bool StandardProduct
        {
            get
            {
                return standardProduct;
            }
            set
            {
                standardProduct = value;
            }
        }

        #endregion StandardProduct

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

        #region public bool FlatRate

        public bool FlatRate
        {
            get
            {
                return flatRate;
            }
            set
            {
                flatRate = value;
            }
        }

        #endregion

        //---------------------------------
        // Methods
        //---------------------------------

        #region Get

        internal static List<SIAdditionalProductSummary> Get(int companyId, int? plantId, SalesInsightDataContext context)
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

            List<SIAdditionalProductSummary> additionalProductSummaries;

            if (plantId.HasValue)
            {
                var result =
                (
                    from c in context.Companies
                    join ap in context.AdditionalProducts on c.CompanyId equals ap.CompanyId
                    join app in context.AdditionalProductPlants on ap.Id equals app.AdditionalProductId
                    where c.CompanyId == companyId && app.PlantId == plantId.Value
                    select new SIAdditionalProductSummary
                    {
                        Id = ap.Id,
                        CompanyId = ap.CompanyId,
                        CompanyName = ap.Company.Name,
                        Code = ap.Code,
                        Description = ap.Description,
                        ProductActive = ap.Active,
                        StandardProduct = ap.StandardProduct,
                        ProductPlantId = app.Id,
                        PlantId = app.PlantId,
                        PlantName = app.Plant.Name,
                        Price = app.Price,
                        Cost = app.Cost,
                        ProductPlantActive = app.Active,
                        FlatRate = ap.FlatRate
                    }
                );

                additionalProductSummaries = (result == null ? new List<SIAdditionalProductSummary>(0) : result.ToList());
            }
            else
            {
                var result =
                (
                    from c in context.Companies
                    join ap in context.AdditionalProducts on c.CompanyId equals ap.CompanyId
                    where c.CompanyId == companyId
                    select new SIAdditionalProductSummary
                    {
                        Id = ap.Id,
                        CompanyId = ap.CompanyId,
                        CompanyName = ap.Company.Name,
                        Code = ap.Code,
                        Description = ap.Description,
                        ProductActive = ap.Active,
                        StandardProduct = ap.StandardProduct,
                        FlatRate = ap.FlatRate
                    }
                );

                additionalProductSummaries = (result == null ? new List<SIAdditionalProductSummary>(0) : result.ToList());
            }

            return additionalProductSummaries;
        }

        #endregion Get

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        protected int id;
        protected int companyId;
        protected string companyName;
        protected int unitOfMeasureId;
        protected SIProductType productTypeId;
        protected string code;
        protected string description;
        protected bool active;
        protected bool standardProduct;
        protected int? productPlantId;
        protected int? plantId;
        protected string plantName;
        protected decimal? price;
        protected decimal? cost;
        protected bool? productPlantActive;
        protected bool flatRate;

        #endregion Fields
    }
}
