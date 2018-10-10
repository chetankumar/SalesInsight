using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIConcreteProduct
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public ConcreteProduct ConcreteProduct

        public ConcreteProduct ConcreteProduct
        {
            get
            {
                return concreteProduct;
            }
            set
            {
                concreteProduct = value;
            }
        }

        #endregion

        #region public List<SIConcreteProductPlant> ConcreteProductPlants

        public List<SIConcreteProductPlant> ConcreteProductPlants
        {
            get
            {
                return concreteProductPlants;
            }
            set
            {
                concreteProductPlants = value;
            }
        }

        #endregion

        //---------------------------------
        // Methods
        //---------------------------------

        #region Get

        internal static SIConcreteProduct Get(int concreteProductId)
        {
            // Validate the parameter(s)
            if (concreteProductId <= 0)
            {
                return null;
            }

            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Get the result
                SIConcreteProduct result =
                (
                    from p in context.ConcreteProducts
                    where p.Id == concreteProductId
                    select new SIConcreteProduct
                    {
                        ConcreteProduct = p,
                        ConcreteProductPlants =
                        (
                            from pp in p.ConcreteProductPlants
                            select new SIConcreteProductPlant
                            {
                                ConcreteProductPlant = pp,
                                PlantName = pp.Plant.Name
                            }
                        ).ToList()
                    }
                ).SingleOrDefault();

                // Return the result
                return result;
            }
        }

        #endregion Get

        #region Save

        internal SISavedProduct Save()
        {
            // Validate the parameter(s) and class data
            if (concreteProduct == null)
            {
                throw new NullReferenceException("The value 'null' was found where an instance of the ConcreteProduct class was required.");
            }

            if (concreteProduct.Id <= 0)
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
                if (!SIConcreteProduct.IsUnique(context.ConcreteProducts, this.concreteProduct.CompanyId, this.concreteProduct.Code))
                {
                    return new SISavedProduct(SaveProductResult.DuplicateCompanyIdProductCode);
                }

                // Insert the product
                concreteProduct.Id = 0;
                context.ConcreteProducts.InsertOnSubmit(concreteProduct);

                // Commit to get the product id
                context.SubmitChanges();

                // Insert all product child entities to the context
                SaveProductResult result;
                if ((result = InsertPlants(context.ConcreteProductPlants)) == SaveProductResult.Success)
                {
                    // Commit all product child entities to the context
                    context.SubmitChanges();
                }

                return new SISavedProduct(result, concreteProduct.Id);
            }
        }

        #endregion Insert

        #region InsertPlants

        private SaveProductResult InsertPlants(Table<ConcreteProductPlant> contextTable)
        {
            if (concreteProductPlants != null)
            {
                // Update all product plants with retrieved product id & insert to the context
                foreach (SIConcreteProductPlant concreteProductPlant in concreteProductPlants)
                {
                    ConcreteProductPlant entity = concreteProductPlant.ConcreteProductPlant;

                    // Ensure product plant to insert is unique
                    if (!SIConcreteProductPlant.IsUnique(concreteProductPlants, contextTable, entity.ConcreteProductId, entity.PlantId, entity.Id))
                    {
                        return SaveProductResult.DuplicateProductIdPlantId;
                    }

                    entity.Id = 0;
                    entity.ConcreteProductId = concreteProduct.Id;
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
                if (!SIConcreteProduct.IsUnique(context.ConcreteProducts, this.concreteProduct.CompanyId, this.concreteProduct.Code, this.concreteProduct.Id))
                {
                    return new SISavedProduct(SaveProductResult.DuplicateCompanyIdProductCode);
                }

                // Get product from context
                SIConcreteProduct productFromContext = SIConcreteProduct.Get(concreteProduct.Id);

                // Update the product
                context.ConcreteProducts.Attach(concreteProduct);
                context.Refresh(RefreshMode.KeepCurrentValues, concreteProduct);

                // Update all product child entities to the context
                SaveProductResult result;
                if ((result = UpdatePlants(context, productFromContext.ConcreteProductPlants)) == SaveProductResult.Success)
                {
                    // Commit all product child entities to the context
                    context.SubmitChanges();
                }

                return new SISavedProduct(result, concreteProduct.Id);
            }
        }

        #endregion Update

        #region UpdatePlants

        private SaveProductResult UpdatePlants(SalesInsightDataContext context, List<SIConcreteProductPlant> plantsFromContext)
        {
            // Set new/old lists to empty list if null to ensure proper inserts/updates/deletes below
            if (concreteProductPlants == null)
            {
                concreteProductPlants = new List<SIConcreteProductPlant>(0);
            }
            if (plantsFromContext == null)
            {
                plantsFromContext = new List<SIConcreteProductPlant>(0);
            }

            // Delete all product plants in the context but not the parameter entity
            foreach (SIConcreteProductPlant plantFromContext in plantsFromContext)
            {
                if (!concreteProductPlants.Exists(cpp => cpp.ConcreteProductPlant.Id == plantFromContext.ConcreteProductPlant.Id))
                {
                    context.ConcreteProductPlants.DeleteOnSubmit(plantFromContext.ConcreteProductPlant);
                }
            }

            // Update or insert all product plants in the parameter entity
            foreach (SIConcreteProductPlant concreteProductPlant in concreteProductPlants)
            {
                ConcreteProductPlant entity = concreteProductPlant.ConcreteProductPlant;

                // Ensure product plant to update or insert is unique
                if (!SIConcreteProductPlant.IsUnique(concreteProductPlants, context.ConcreteProductPlants, concreteProduct.Id, entity.PlantId, entity.Id))
                {
                    return SaveProductResult.DuplicateProductIdPlantId;
                }

                if (plantsFromContext.Exists(pfc => pfc.ConcreteProductPlant.Id == entity.Id))
                {
                    // Update the product plant
                    context.ConcreteProductPlants.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                else
                {
                    // Insert the product plant
                    entity.Id = 0;
                    entity.ConcreteProductId = concreteProduct.Id;
                    context.ConcreteProductPlants.InsertOnSubmit(entity);
                }
            }

            return SaveProductResult.Success;
        }

        #endregion UpdatePlants

        #region IsUnique

        internal static bool IsUnique(Table<ConcreteProduct> contextTable, int companyId, string code)
        {
            return SIConcreteProduct.IsUnique(contextTable, companyId, code, 0);
        }

        internal static bool IsUnique(Table<ConcreteProduct> contextTable, int companyId, string code, int id)
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

        #region protected ConcreteProduct concreteProduct

        protected ConcreteProduct concreteProduct = null;

        #endregion

        #region protected List<SIConcreteProductPlant> concreteProductPlants

        protected List<SIConcreteProductPlant> concreteProductPlants = null;

        #endregion
    }
}
