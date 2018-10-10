using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIAdditionalProduct
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public AdditionalProduct AdditionalProduct

        public AdditionalProduct AdditionalProduct
        {
            get
            {
                return additionalProduct;
            }
            set
            {
                additionalProduct = value;
            }
        }

        #endregion

        #region public List<SIAdditionalProductPlant> AdditionalProductPlants

        public List<SIAdditionalProductPlant> AdditionalProductPlants
        {
            get
            {
                return additionalProductPlants;
            }
            set
            {
                additionalProductPlants = value;
            }
        }

        #endregion

        #region public string UnitOfMeasureCode

        public string UnitOfMeasureCode
        {
            get
            {
                return unitOfMeasureCode;
            }
            set
            {
                unitOfMeasureCode = value;
            }
        }

        #endregion

        #region public string UnitOfMeasureDescription

        public string UnitOfMeasureDescription
        {
            get
            {
                return unitOfMeasureDescription;
            }
            set
            {
                unitOfMeasureDescription = value;
            }
        }

        #endregion
        
        //---------------------------------
        // Methods
        //---------------------------------

        #region Get

        internal static SIAdditionalProduct Get(int additionalProductId)
        {
            // Validate the parameter(s)
            if (additionalProductId <= 0)
            {
                return null;
            }

            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Get the results
                SIAdditionalProduct result =
                (
                    from p in context.AdditionalProducts
                    where p.Id == additionalProductId
                    select new SIAdditionalProduct
                    {
                        AdditionalProduct = p,
                        AdditionalProductPlants =
                        (
                            from pp in p.AdditionalProductPlants
                            select new SIAdditionalProductPlant
                            {
                                AdditionalProductPlant = pp, PlantName = pp.Plant.Name
                            }
                        ).ToList()
                    }
                ).SingleOrDefault();

                // Return the resul
                return result;
            }
        }

        #endregion Get

        #region Get

        internal static List<SIAdditionalProduct> Get(int companyId, int? plantId, bool? standardProducts)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                if(plantId != null && standardProducts != null)
                {
                    // Get the results
                    List<SIAdditionalProduct> result =
                    (
                        from a in context.AdditionalProducts
                        join p in context.AdditionalProductPlants on a.Id equals p.AdditionalProductId
                        where a.CompanyId == companyId && p.PlantId == plantId && a.StandardProduct == standardProducts
                        select new SIAdditionalProduct
                        {
                            UnitOfMeasureCode = a.UnitOfMeasure.Code,
                            UnitOfMeasureDescription = a.UnitOfMeasure.Description,
                            AdditionalProduct = a,
                            AdditionalProductPlants =
                            (
                                from pp in a.AdditionalProductPlants
                                select new SIAdditionalProductPlant
                                {
                                    AdditionalProductPlant = pp, PlantName = pp.Plant.Name
                                }
                            ).ToList()
                        }
                    ).ToList();

                    // Return the resul
                    return result;                    
                }
                else if(plantId != null)
                {
                    // Get the results
                    List<SIAdditionalProduct> result =
                    (
                        from a in context.AdditionalProducts
                        join p in context.AdditionalProductPlants on a.Id equals p.AdditionalProductId
                        where a.CompanyId == companyId && p.PlantId == plantId
                        select new SIAdditionalProduct
                        {
                            UnitOfMeasureCode = a.UnitOfMeasure.Code,
                            UnitOfMeasureDescription = a.UnitOfMeasure.Description,
                            AdditionalProduct = a,
                            AdditionalProductPlants =
                            (
                                from pp in a.AdditionalProductPlants
                                select new SIAdditionalProductPlant
                                {
                                    AdditionalProductPlant = pp,
                                    PlantName = pp.Plant.Name
                                }
                            ).ToList()
                        }
                    ).ToList();

                    // Return the resul
                    return result;
                }
                else if (standardProducts != null)
                {
                    // Get the results
                    List<SIAdditionalProduct> result =
                    (
                        from a in context.AdditionalProducts
                        where a.CompanyId == companyId && a.StandardProduct == standardProducts
                        select new SIAdditionalProduct
                        {
                            UnitOfMeasureCode = a.UnitOfMeasure.Code,
                            UnitOfMeasureDescription = a.UnitOfMeasure.Description,
                            AdditionalProduct = a,
                            AdditionalProductPlants =
                            (
                                from pp in a.AdditionalProductPlants
                                select new SIAdditionalProductPlant
                                {
                                    AdditionalProductPlant = pp,
                                    PlantName = pp.Plant.Name
                                }
                            ).ToList()
                        }
                    ).ToList();

                    // Return the resul
                    return result;
                }
                else
                {
                    // Get the results
                    List<SIAdditionalProduct> result =
                    (
                        from a in context.AdditionalProducts
                        where a.CompanyId == companyId
                        select new SIAdditionalProduct
                        {
                            UnitOfMeasureCode = a.UnitOfMeasure.Code,
                            UnitOfMeasureDescription = a.UnitOfMeasure.Description,
                            AdditionalProduct = a,
                            AdditionalProductPlants =
                            (
                                from pp in a.AdditionalProductPlants
                                select new SIAdditionalProductPlant
                                {
                                    AdditionalProductPlant = pp,
                                    PlantName = pp.Plant.Name
                                }
                            ).ToList()
                        }
                    ).ToList();

                    // Return the resul
                    return result;
                }
            }
        }

        #endregion Get

        #region Save

        internal SISavedProduct Save()
        {
            // Validate the parameter(s) and class data
            if (additionalProduct == null)
            {
                throw new NullReferenceException("The value 'null' was found where an instance of the AdditionalProduct class was required.");
            }

            if (additionalProduct.Id <= 0)
            {
                return Insert();
            }
            else
            {
                return Update();
            }
        }

        #endregion Save

        #region Insert

        private SISavedProduct Insert()
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Ensure product to insert is unique
                if (!SIAdditionalProduct.IsUnique(context.AdditionalProducts, this.additionalProduct.CompanyId, this.additionalProduct.Code))
                {
                    return new SISavedProduct(SaveProductResult.DuplicateCompanyIdProductCode);
                }

                // Insert the product
                additionalProduct.Id = 0;
                context.AdditionalProducts.InsertOnSubmit(additionalProduct);

                // Commit to get the product id
                context.SubmitChanges();

                // Insert all product child entities to the context
                SaveProductResult result;
                if ((result = InsertPlants(context.AdditionalProductPlants)) == SaveProductResult.Success)
                {
                    // Commit all product child entities to the context
                    context.SubmitChanges();
                }

                return new SISavedProduct(result, additionalProduct.Id);
            }
        }

        #endregion Insert

        #region InsertPlants

        private SaveProductResult InsertPlants(Table<AdditionalProductPlant> contextTable)
        {
            if (additionalProductPlants != null)
            {
                // Update all product plants with retrieved product id & insert to the context
                foreach (SIAdditionalProductPlant additionalProductPlant in additionalProductPlants)
                {
                    AdditionalProductPlant entity = additionalProductPlant.AdditionalProductPlant;

                    // Ensure product plant to insert is unique
                    if (!SIAdditionalProductPlant.IsUnique(additionalProductPlants, contextTable, entity.AdditionalProductId, entity.PlantId, entity.Id))
                    {
                        return SaveProductResult.DuplicateProductIdPlantId;
                    }

                    entity.Id = 0;
                    entity.AdditionalProductId = additionalProduct.Id;
                    contextTable.InsertOnSubmit(entity);
                }
            }

            return SaveProductResult.Success;
        }

        #endregion InsertPlants

        #region Update

        private SISavedProduct Update()
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Ensure product to update is unique
                if (!SIAdditionalProduct.IsUnique(context.AdditionalProducts, this.additionalProduct.CompanyId, this.additionalProduct.Code, this.additionalProduct.Id))
                {
                    return new SISavedProduct(SaveProductResult.DuplicateCompanyIdProductCode);
                }

                // Get product from context
                SIAdditionalProduct productFromContext = SIAdditionalProduct.Get(additionalProduct.Id);

                // Update the product
                context.AdditionalProducts.Attach(additionalProduct);
                context.Refresh(RefreshMode.KeepCurrentValues, additionalProduct);

                // Update all product child entities to the context
                SaveProductResult result;
                if ((result = UpdatePlants(context, productFromContext.AdditionalProductPlants)) == SaveProductResult.Success)
                {
                    // Commit all product child entities to the context
                    context.SubmitChanges();
                }

                return new SISavedProduct(result, additionalProduct.Id);
            }
        }

        #endregion Update

        #region UpdatePlants

        private SaveProductResult UpdatePlants(SalesInsightDataContext context, List<SIAdditionalProductPlant> plantsFromContext)
        {
            // Set new/old lists to empty list if null to ensure proper inserts/updates/deletes below
            if (additionalProductPlants == null)
            {
                additionalProductPlants = new List<SIAdditionalProductPlant>(0);
            }
            if (plantsFromContext == null)
            {
                plantsFromContext = new List<SIAdditionalProductPlant>(0);
            }

            // Delete all product plants in the context but not the parameter entity
            foreach (SIAdditionalProductPlant plantFromContext in plantsFromContext)
            {
                if (!additionalProductPlants.Exists(cpp => cpp.AdditionalProductPlant.Id == plantFromContext.AdditionalProductPlant.Id))
                {
                    context.AdditionalProductPlants.DeleteOnSubmit(plantFromContext.AdditionalProductPlant);
                }
            }

            // Update or insert all product plants in the parameter entity
            foreach (SIAdditionalProductPlant additionalProductPlant in additionalProductPlants)
            {
                AdditionalProductPlant entity = additionalProductPlant.AdditionalProductPlant;

                // Ensure product plant to update or insert is unique
                if (!SIAdditionalProductPlant.IsUnique(additionalProductPlants, context.AdditionalProductPlants, additionalProduct.Id, entity.PlantId, entity.Id))
                {
                    return SaveProductResult.DuplicateProductIdPlantId;
                }

                if (plantsFromContext.Exists(pfc => pfc.AdditionalProductPlant.Id == entity.Id))
                {
                    // Update the product plant
                    context.AdditionalProductPlants.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                else
                {
                    // Insert the product plant
                    entity.Id = 0;
                    entity.AdditionalProductId = additionalProduct.Id;
                    context.AdditionalProductPlants.InsertOnSubmit(entity);
                }
            }

            return SaveProductResult.Success;
        }

        #endregion UpdatePlants

        #region IsUnique

        internal static bool IsUnique(Table<AdditionalProduct> contextTable, int companyId, string code)
        {
            return SIAdditionalProduct.IsUnique(contextTable, companyId, code, 0);
        }

        internal static bool IsUnique(Table<AdditionalProduct> contextTable, int companyId, string code, int id)
        {
            int count =
            (
                from ct in contextTable
                where ct.CompanyId == companyId && ct.Code == code && ct.Id != id
                select ct
             ).Count();

            return (count <= 0);
        }

        #endregion IsUnique

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected AdditionalProduct additionalProduct

        protected AdditionalProduct additionalProduct = null;

        #endregion

        #region protected List<SIAdditionalProductPlant> additionalProductPlants

        protected List<SIAdditionalProductPlant> additionalProductPlants = null;

        #endregion

        #region protected string unitOfMeasureCode

        protected string unitOfMeasureCode = null;

        #endregion

        #region protected string unitOfMeasureDescription

        protected string unitOfMeasureDescription = null;

        #endregion
    }
}
