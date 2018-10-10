using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Data
{
    public class DataManager
    {
        public DataManager(int companyId)
        {
            this.CompanyId = companyId;
        }

        public int CompanyId { get; set; }

        public void SaveMarketSegments(List<MarketSegment> marketSegments)
        {
            if (marketSegments != null)
            {
                var allDistricts = SIDAL.GetDistricts(this.CompanyId, true);
                foreach (var segment in marketSegments)
                {
                    MarketSegment duplicate = SIDAL.FindDuplicateByDispatchId<MarketSegment>(segment);

                    var isDuplicate = (duplicate != null);

                    SIMarketSegment siMarketSegment = new SIMarketSegment();
                    siMarketSegment.Districts = allDistricts;

                    if (isDuplicate)
                    {
                        SIDAL.Update<MarketSegment>(x => x.MarketSegmentId == duplicate.MarketSegmentId, segment, u => u.Name);

                        siMarketSegment.MarketSegment = duplicate;

                        SIDAL.UpdateMarketSegmentDistricts(siMarketSegment);
                    }
                    else
                    {
                        //Fill other properties
                        segment.CompanyId = this.CompanyId;
                        segment.Active = true;

                        siMarketSegment.MarketSegment = segment;

                        SIDAL.AddMarketSegments(siMarketSegment);
                    }
                }
            }
        }

        public void SaveCustomers(List<Customer> customers)
        {
            if (customers != null)
            {
                var allCustomerDistrict = SIDAL.GetDistricts(this.CompanyId, true);
                var disableCustomerStatusSync = SIDAL.GetCompanySettings().DisableCustomerStatusSync.GetValueOrDefault();

                foreach (var customer in customers)
                {
                    Customer duplicateCustomer = SIDAL.FindDuplicateByDispatchId<Customer>(customer);
                   
                    var isDuplicate = (duplicateCustomer != null);

                    if (isDuplicate)
                    {
                        if (!disableCustomerStatusSync)
                        {
                            if (duplicateCustomer.APIActiveStatus != customer.APIActiveStatus)
                            {
                                customer.OverrideAUStatus = null;
                            }
                            else
                            {
                                customer.OverrideAUStatus = duplicateCustomer.OverrideAUStatus;
                                customer.Active = duplicateCustomer.Active;
                            }
                        }
                        else
                        {
                            customer.APIActiveStatus = duplicateCustomer.APIActiveStatus;
                            customer.OverrideAUStatus = duplicateCustomer.OverrideAUStatus;
                            customer.Active = duplicateCustomer.Active;
                        }
                        SIDAL.Update<Customer>(x => x.CustomerId == duplicateCustomer.CustomerId, customer, c => c.Name, c => c.CustomerNumber, c => c.Address1, c => c.Address2, c => c.City, c => c.State, c => c.Zip, c => c.Active, c => c.APIActiveStatus, c => c.PurchaseConcrete, c => c.PurchaseAggregate, c => c.PurchaseBlock, c => c.OverrideAUStatus);
                    }
                    else
                    {
                        //Fill other properties of Customer object
                        customer.CompanyId = this.CompanyId;

                        SICustomer siCustomer = new SICustomer()
                        {
                            Customer = customer,
                            Districts = allCustomerDistrict
                        };
                        SIDAL.AddCustomer(siCustomer);
                    }
                }
            }
        }

        public void SaveSalesStaff(List<SalesStaff> salesPeople)
        {
            if (salesPeople != null)
            {
                var allDistricts = SIDAL.GetDistricts(this.CompanyId, true);
                foreach (var staff in salesPeople)
                {
                    SalesStaff duplicateStaff = SIDAL.FindDuplicateByDispatchId<SalesStaff>(staff);

                    var isDuplicate = (duplicateStaff != null);

                    if (isDuplicate)
                    {
                        SIDAL.Update<SalesStaff>(x => x.SalesStaffId == duplicateStaff.SalesStaffId, staff, c => c.Name, c => c.Email, c => c.Phone, c => c.Active);
                    }
                    else
                    {
                        //Fill other properties here
                        staff.CompanyId = this.CompanyId;
                        staff.Active = true;

                        SISalesStaff siSalesStaff = new SISalesStaff();
                        siSalesStaff.SalesStaff = staff;
                        siSalesStaff.Districts = allDistricts;

                        SIDAL.AddSalesStaff(siSalesStaff);
                    }
                }
            }
        }

        public void SavePlants(List<Plant> plants)
        {
            if (plants != null)
            {
                foreach (var plant in plants)
                {
                    Plant duplicate = SIDAL.FindDuplicateByDispatchId<Plant>(plant);

                    var isDuplicate = (duplicate != null);

                    SIOperation operation = new SIOperation();
                    if (isDuplicate)
                    {
                        SIDAL.Update<Plant>(x => x.PlantId == duplicate.PlantId, plant, u => u.Name, u => u.ProductTypeId, u => u.Active);
                    }
                    else
                    {
                        //Fill other properties here
                        plant.CompanyId = this.CompanyId;

                        District district = SIDAL.CreateOrFindDistrict(plant.CompanyId, "AUJS Dummy");

                        //Product Type Id is default to 1
                        //plant.ProductTypeId = 1;
                        //1 District must be created before adding these
                        plant.DistrictId = district.DistrictId;

                        operation.Type = SIOperationType.Add;
                        operation.Item = plant;
                        List<SIOperation> operations = new List<SIOperation>();
                        operations.Add(operation);

                        SIDAL.ExecuteOperations(operations);
                    }
                }
            }
        }

        public void SaveRawMaterialTypes(List<RawMaterialType> rawMaterialTypes)
        {
            if (rawMaterialTypes != null)
            {
                foreach (var rawMaterialType in rawMaterialTypes)
                {
                    RawMaterialType duplicate = SIDAL.FindDuplicateByDispatchId<RawMaterialType>(rawMaterialType);

                    var isDuplicate = (duplicate != null);
                    if (isDuplicate)
                    {
                        SIDAL.Update<RawMaterialType>(x => x.Id == duplicate.Id, rawMaterialType, u => u.Name);
                    }
                    else
                    {
                        //Fill other properties here
                        rawMaterialType.Active = true;

                        SIOperation operation = new SIOperation()
                        {
                            Type = SIOperationType.Add,
                            Item = rawMaterialType
                        };
                        List<SIOperation> operations = new List<SIOperation>();
                        operations.Add(operation);

                        SIDAL.ExecuteOperations(operations);
                    }
                }
            }
        }

        public void SaveRawMaterials(List<RawMaterial> rawMaterials)
        {
            if (rawMaterials != null)
            {
                //Initialize variables here
                List<RawMaterial> exRawMaterials = SIDAL.GetRawMaterials(true);
                List<Uom> exUoms = SIDAL.GetUOMS(null);
                List<Plant> exPlants = SIDAL.GetPlantsList();
                DateTime today = DateTime.Now;

                foreach (var rawMaterial in rawMaterials)
                {
                    //Compare Dispatch Id with Material Code because that's how we save it to the database
                    RawMaterial duplicateRawMaterial = exRawMaterials.FirstOrDefault(rm => rm.DispatchId == rawMaterial.DispatchId);

                    var isDuplicate = (duplicateRawMaterial != null);

                    //Get the Raw Material type id by it's Dispatch Id
                    RawMaterialType rawMaterialType = SIDAL.FindDuplicateByDispatchId<RawMaterialType>(new RawMaterialType() { DispatchId = rawMaterial.RawMaterialTypeDispatchId.ToString() });
                    if (rawMaterialType != null)
                    {
                        rawMaterial.RawMaterialTypeId = rawMaterialType.Id;
                        if (isDuplicate)
                        {
                            SIDAL.Update<RawMaterial>(x => x.Id == duplicateRawMaterial.Id, rawMaterial, u => u.MaterialCode, u => u.Description, u => u.RawMaterialTypeId, u => u.DispatchId, u => u.Active);
                        }
                        else
                        {
                            //Fill other properties of Customer object
                            rawMaterial.MeasurementType = "AUJS";

                            SIOperation operation = new SIOperation()
                            {
                                Type = SIOperationType.Add,
                                Item = rawMaterial
                            };
                            List<SIOperation> operations = new List<SIOperation>();
                            operations.Add(operation);

                            SIDAL.ExecuteOperations(operations);
                        }
                    }

                    //Save Raw Material Cost Projections
                    if (rawMaterial.CostProjections != null)
                    {
                        RawMaterialCostProjection rmCostProjection = null;
                        long rawMaterialId = SIDAL.GetRawMaterialByDispatchId(rawMaterial.DispatchId);
                        foreach (var item in rawMaterial.CostProjections)
                        {
                            rmCostProjection = new RawMaterialCostProjection();

                            Uom uom = exUoms.FirstOrDefault(x => x.DispatchId == item.UomDispatchId);

                            if (uom != null)
                                rmCostProjection.UomId = uom.Id;
                            rmCostProjection.RawMaterialId = rawMaterialId;
                            rmCostProjection.Cost = item.Cost;
                            rmCostProjection.PlantId = exPlants.Where(x => x.DispatchId == item.PlantDispatchId)
                                                               .Select(x => x.PlantId)
                                                               .FirstOrDefault();

                            rmCostProjection.ChangeDate = new DateTime(today.Year, today.Month, 1);

                            if (rmCostProjection.PlantId > 0 && rmCostProjection.RawMaterialId > 0 && rmCostProjection.UomId > 0)
                                SIDAL.AddUpdateRawMaterialProjection(rmCostProjection);
                        }
                    }
                }
            }
        }

        public void SaveProjectStatus(List<ProjectStatus> projectStatusList)
        {
            if (projectStatusList != null)
            {
                foreach (var projectStatus in projectStatusList)
                {
                    ProjectStatus duplicate = SIDAL.FindDuplicateByDispatchId<ProjectStatus>(projectStatus);

                    var isDuplicate = (duplicate != null);
                    if (isDuplicate)
                    {
                        SIDAL.Update<ProjectStatus>(x => x.ProjectStatusId == duplicate.ProjectStatusId, projectStatus, u => u.Name);
                    }
                    else
                    {
                        //Fill other properties here
                        //Set status type to 1 by default
                        projectStatus.CompanyId = this.CompanyId;
                        projectStatus.StatusType = 1;
                        projectStatus.SortOrder = 1;
                        projectStatus.Active = true;

                        SIOperation operation = new SIOperation();
                        operation.Type = SIOperationType.Add;
                        operation.Item = projectStatus;
                        List<SIOperation> operations = new List<SIOperation>();
                        operations.Add(operation);

                        SIDAL.ExecuteOperations(operations);
                    }
                }
            }
        }

        public void SaveTaxCodes(List<TaxCode> taxCodes)
        {
            if (taxCodes != null)
            {
                foreach (var taxCode in taxCodes)
                {
                    TaxCode duplicate = SIDAL.FindDuplicateByDispatchId<TaxCode>(taxCode);

                    var isDuplicate = (duplicate != null);
                    if (isDuplicate)
                    {
                        SIDAL.Update<TaxCode>(x => x.Id == duplicate.Id, taxCode, u => u.Description);
                    }
                    else
                    {
                        SIDAL.AddUpdateTaxCode(taxCode);
                    }
                }
            }
        }

        public void SaveUnitOfMeasures(List<Uom> unitOfMeasures)
        {
            if (unitOfMeasures != null)
            {
                foreach (var uom in unitOfMeasures)
                {
                    Uom duplicate = SIDAL.FindDuplicateByDispatchId<Uom>(uom);

                    var isDuplicate = (duplicate != null);
                    if (isDuplicate)
                    {
                        //Fill the object
                        SIDAL.Update<Uom>(x => x.Id == duplicate.Id, uom, u => u.Name);
                    }
                    else
                    {
                        //Fill other properties here
                        uom.BaseConversion = 0;
                        uom.Category = uom.Name;
                        uom.Active = true;
                        uom.Priority = 1;

                        SIOperation operation = new SIOperation();
                        operation.Type = SIOperationType.Add;
                        operation.Item = uom;
                        List<SIOperation> operations = new List<SIOperation>();
                        operations.Add(operation);

                        SIDAL.ExecuteOperations(operations);
                    }
                }
            }
        }

        public List<string> SaveStandardMixConstituents(List<StandardMixConstituent> standardMixConstituents)
        {
            List<string> errors = new List<string>();

            if (standardMixConstituents != null)
            {
                int count = int.MaxValue;
                List<StandardMix> exStandardMixes = SIDAL.GetStandardMixes(out count, true);

                //Initially mark all mix formulations as not scrubbed (or IsScrubbed = false) as they may be set to true during the last sync
                SIDAL.UpdateMixFormulationsScrubbedStatus(false);

                List<MixFormulation> exMixFormulations = SIDAL.GetMixFormulations();
                //List<StandardMixConstituent> exMixConstituents = SIDAL.GetStandardMixConstituents();
                List<long> cleanedMixFormulations = new List<long>();

                long mixFormulationId = 0, standardMixId = 0;
                foreach (var smConstituent in standardMixConstituents)
                {
                    try
                    {
                        //1. Check if standardMixes already contains the Standard Mix
                        //2. If not add the standardMix to the database and also add it to standardMixes list
                        MixFormulation mixFormulation = smConstituent.MixFormulation;
                        StandardMix standardMix = (mixFormulation?.StandardMix);

                        if (standardMix != null)
                        {
                            //check if standardMix is already in the database
                            if (exStandardMixes.Count(x => x.DispatchId == standardMix.DispatchId) > 0)
                            {
                                //standardMix = exStandardMixes.FirstOrDefault(x => x.DispatchId == standardMix.DispatchId);
                                standardMixId = exStandardMixes.Where(x => x.DispatchId == standardMix.DispatchId).Select(x => x.Id).FirstOrDefault();

                                //Update the existing standard mix properties
                                SIDAL.Update<StandardMix>(sm => sm.Id == standardMixId, standardMix, sm => sm.Number, sm => sm.Description, sm => sm.SalesDesc, sm => sm.PSI, sm => sm.Slump, sm => sm.Air, sm => sm.MD1, sm => sm.MD2, sm => sm.MD3, sm => sm.MD4, sm => sm.Active);
                            }
                            else
                            {
                                //Initialize Standard Mix
                                StandardMix newStandardMix = new StandardMix();
                                newStandardMix.DispatchId = standardMix.DispatchId;
                                newStandardMix.Number = standardMix.Number;
                                newStandardMix.Description = standardMix.Description;
                                newStandardMix.PSI = standardMix.PSI;
                                newStandardMix.Slump = standardMix.Slump;
                                newStandardMix.Air = standardMix.Air;
                                newStandardMix.MD1 = standardMix.MD1;
                                newStandardMix.MD2 = standardMix.MD2;
                                newStandardMix.MD3 = standardMix.MD3;
                                newStandardMix.MD4 = standardMix.MD4;
                                newStandardMix.Active = standardMix.Active;

                                //As per requirement doc
                                newStandardMix.SalesDesc = standardMix.Description;

                                SIDAL.AddUpdateStandardMix(newStandardMix);
                                exStandardMixes.Add(newStandardMix);

                                standardMixId = newStandardMix.Id;
                            }
                        }

                        //Delete Mix Formulation and Constituents for the current standard mix and plant combination
                        try
                        {
                            //SIDAL.DeleteMixFormulationAndConstituents(mixFormulation.PlantId, standardMixId);
                        }
                        catch { }

                        if (mixFormulation != null && exMixFormulations.Count(x => x.StandardMixId == standardMixId && x.PlantId == mixFormulation.PlantId) > 0)
                        {
                            mixFormulation = exMixFormulations.FirstOrDefault(x => x.StandardMixId == standardMixId && x.PlantId == mixFormulation.PlantId);
                            mixFormulationId = mixFormulation.Id;
                        }
                        else if (mixFormulation != null)
                        {
                            //Initialize New Mix Formulation
                            MixFormulation newMixFormulation = new MixFormulation();
                            newMixFormulation.StandardMixId = standardMixId;
                            newMixFormulation.PlantId = mixFormulation.PlantId;

                            SIDAL.AddUpdateMixFormulation(newMixFormulation);
                            //exMixFormulations.Add(newMixFormulation);
                            mixFormulationId = newMixFormulation.Id;
                        }

                        //3. Delete the MixConstituents first
                        if (!cleanedMixFormulations.Contains(mixFormulationId))
                        {
                            SIDAL.DeleteMixConstituents(mixFormulationId);
                            cleanedMixFormulations.Add(mixFormulationId);
                        }

                        //StandardMixConstituent mixConstituent = exMixConstituents.FirstOrDefault(x => x.MixFormulationId == mixFormulation.Id && x.RawMaterialId == smConstituent.RawMaterialId);

                        //If doesn't exist in the database add it
                        //if (mixConstituent == null)
                        //{

                        //Initialize New StandardMixConstituent
                        StandardMixConstituent mixConstituent = new StandardMixConstituent();
                        mixConstituent.MixFormulationId = mixFormulationId;
                        mixConstituent.RawMaterialId = smConstituent.RawMaterialId;
                        mixConstituent.UomId = smConstituent.UomId;
                        mixConstituent.Quantity = smConstituent.Quantity;

                        List<SIOperation> operations = new List<SIOperation>();

                        operations.Add(new SIOperation()
                        {
                            Item = mixConstituent,
                            Type = SIOperationType.Add
                        });

                        SIDAL.ExecuteOperations(operations);
                        //}
                        //Other wise update the required fields
                        //else
                        //{
                        //  SIDAL.Update<StandardMixConstituent>(x => x.MixFormulationId == mixFormulation.Id && x.RawMaterialId == smConstituent.RawMaterialId, smConstituent, u => u.UomId, u => u.Quantity);
                        //}

                        SIDAL.UpdateMixFormulationScrubbedStatus(mixFormulationId, true);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }

                //Delete Mix Formulations whhich are left unscrubbed
                SIDAL.DeleteUnScrubbedMixFormulations();
            }
            return errors;
        }

        public void SaveAggregateProducts(List<AggregateProduct> aggregateProducts)
        {
            if (aggregateProducts != null)
            {
                var existingProducts = SIDAL.GetAggregateProducts();
                var exProductPriceProjections = SIDAL.GetAllAggregateProductPriceProjections();

                AggregateProduct newAggProduct = null;
                AggregateProductPriceProjection aggProductPriceProjection = null;
                long aggregateProductId = 0;
                foreach (var aggProdApi in aggregateProducts)
                {
                    newAggProduct = new AggregateProduct()
                    {
                        Id = aggProdApi.Id,
                        DispatchId = aggProdApi.DispatchId,
                        Code = aggProdApi.Code,
                        Description = aggProdApi.Description,
                        Active = aggProdApi.Active,
                        CreatedAt = aggProdApi.CreatedAt
                    };

                    Uom uom = SIDAL.FindUOMByDispatchId(aggProdApi.UomDispatchId);
                    if (uom != null)
                        newAggProduct.UomId = uom.Id;

                    //check if the product already exists in the database
                    if (existingProducts.Count(x => x.DispatchId == aggProdApi.DispatchId) > 0)
                    {
                        aggregateProductId = existingProducts.Where(x => x.DispatchId == newAggProduct.DispatchId).Select(x => x.Id).FirstOrDefault();

                        //Update the existing product properties
                        SIDAL.Update<AggregateProduct>(sm => sm.Id == aggregateProductId, newAggProduct, ap => ap.Code, ap => ap.Description, ap => ap.UomId, ap => ap.Active);
                    }
                    else
                    {
                        SIDAL.AddUpdateAggregateProduct(newAggProduct);
                        existingProducts.Add(newAggProduct);

                        aggregateProductId = newAggProduct.Id;
                    }

                    if (aggProdApi.AggregateProductPriceProjections != null)
                    {
                        foreach (var aggProdPriceProjection in aggProdApi.AggregateProductPriceProjections)
                        {
                            aggProductPriceProjection = new AggregateProductPriceProjection()
                            {
                                AggregateProductId = aggregateProductId,
                                Price = aggProdPriceProjection.Price,
                                Active = newAggProduct.Active,
                                CreatedAt = DateTime.Now,
                                PlantId = aggProdPriceProjection.PlantId,
                                ChangeDate = aggProdPriceProjection.ChangeDate
                            };

                            if (exProductPriceProjections.Exists(x => x.AggregateProductId == aggregateProductId
                            && x.ChangeDate == aggProdPriceProjection.ChangeDate
                            && x.PlantId == aggProdPriceProjection.PlantId))
                            {
                                SIDAL.Update<AggregateProductPriceProjection>(x => x.AggregateProductId == aggregateProductId && x.ChangeDate == aggProdPriceProjection.ChangeDate, aggProductPriceProjection, appp => appp.Price);
                            }
                            else
                            {
                                SIDAL.Add<AggregateProductPriceProjection>(aggProductPriceProjection);
                            }
                        }
                    }
                }
            }
        }

        public void SaveBlockProducts(List<BlockProduct> blockProducts)
        {
            if (blockProducts != null)
            {
                var existingProducts = SIDAL.GetBlockProducts();
                var exProductPriceProjections = SIDAL.GetAllBlockProductPriceProjections();

                BlockProduct newBlockProd = null;
                BlockProductPriceProjection blockProductPriceProjection = null;
                long blockProductId = 0;
                foreach (var blockProdApi in blockProducts)
                {
                    newBlockProd = new BlockProduct()
                    {
                        Id = blockProdApi.Id,
                        DispatchId = blockProdApi.DispatchId,
                        Code = blockProdApi.Code,
                        Description = blockProdApi.Description,
                        Active = blockProdApi.Active,
                        CreatedAt = blockProdApi.CreatedAt
                    };

                    Uom uom = SIDAL.FindUOMByDispatchId(blockProdApi.UomDispatchId);
                    if (uom != null)
                        newBlockProd.UomId = uom.Id;

                    //check if the product already exists in the database
                    if (existingProducts.Count(x => x.DispatchId == blockProdApi.DispatchId) > 0)
                    {
                        blockProductId = existingProducts.Where(x => x.DispatchId == newBlockProd.DispatchId).Select(x => x.Id).FirstOrDefault();

                        //Update the existing product properties
                        SIDAL.Update<BlockProduct>(sm => sm.Id == blockProductId, newBlockProd, bp => bp.Code, bp => bp.Description, bp => bp.UomId, bp => bp.Active);
                    }
                    else
                    {
                        SIDAL.AddUpdateBlockProduct(newBlockProd);
                        existingProducts.Add(newBlockProd);

                        blockProductId = newBlockProd.Id;
                    }

                    if (blockProdApi.BlockProductPriceProjections != null)
                    {
                        foreach (var blockProdPriceProjection in blockProdApi.BlockProductPriceProjections)
                        {
                            blockProductPriceProjection = new BlockProductPriceProjection()
                            {
                                BlockProductId = blockProductId,
                                Price = blockProdPriceProjection.Price,
                                Active = blockProdApi.Active,
                                CreatedAt = DateTime.Now,
                                PlantId = blockProdPriceProjection.PlantId,
                                ChangeDate = blockProdPriceProjection.ChangeDate
                            };

                            if (exProductPriceProjections.Exists(x => x.BlockProductId == blockProductId
                            && x.ChangeDate == blockProdPriceProjection.ChangeDate
                            && x.PlantId == blockProdPriceProjection.PlantId))
                            {
                                SIDAL.Update<BlockProductPriceProjection>(x => x.BlockProductId == blockProductId && x.ChangeDate == blockProdPriceProjection.ChangeDate, blockProductPriceProjection, bppp => bppp.Price);
                            }
                            else
                            {
                                SIDAL.Add<BlockProductPriceProjection>(blockProductPriceProjection);
                            }
                        }
                    }
                }
            }
        }

        public void SaveAddOns(List<Addon> addOns)
        {
            if (addOns != null)
            {
                AddonPriceProjection addOnPriceProjection = null;
                List<District> activeDistricts = SIDAL.GetDistricts(this.CompanyId, true);
                List<AddonPriceProjection> priceProjectionsToAdd = null;

                foreach (var addOn in addOns)
                {
                    Addon duplicateAddOn = SIDAL.FindDuplicateByDispatchId<Addon>(addOn);

                    var isDuplicate = (duplicateAddOn != null);

                    Uom uom = SIDAL.GetUOMByDispatchId(addOn.UomDispatchId);

                    addOn.MixCostUomId = uom.Id;
                    addOn.QuoteUomId = uom.Id;

                    if (isDuplicate)
                    {
                        SIDAL.Update<Addon>(x => x.Id == duplicateAddOn.Id, addOn, u => u.Description, u => u.QuoteUomId, u => u.MixCostUomId, u => u.Code, u=> u.Active);

                        if (addOn.PriceProjections != null)
                        {
                            priceProjectionsToAdd = new List<AddonPriceProjection>();

                            foreach (var priceProjection in addOn.PriceProjections)
                            {
                                foreach (var district in activeDistricts)
                                {
                                    addOnPriceProjection = new AddonPriceProjection();
                                    addOnPriceProjection.Price = priceProjection.Price;
                                    addOnPriceProjection.PriceMode = "QUOTE";
                                    addOnPriceProjection.UomId = uom.Id;
                                    addOnPriceProjection.AddonId = duplicateAddOn.Id;
                                    addOnPriceProjection.DistrictId = district.DistrictId;
                                    addOnPriceProjection.ChangeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                                    priceProjectionsToAdd.Add(addOnPriceProjection);
                                }
                            }

                            SIDAL.SaveAddOnPriceProjections(priceProjectionsToAdd);
                        }
                    }
                    else
                    {
                        addOn.AddonType = "Product";

                        SIDAL.AddUpdateAddOn(addOn);

                        if (addOn.PriceProjections != null)
                        {
                            priceProjectionsToAdd = new List<AddonPriceProjection>();

                            foreach (var priceProjection in addOn.PriceProjections)
                            {
                                foreach (var district in activeDistricts)
                                {
                                    addOnPriceProjection = new AddonPriceProjection();
                                    addOnPriceProjection.Price = priceProjection.Price;
                                    addOnPriceProjection.PriceMode = "QUOTE";
                                    addOnPriceProjection.UomId = uom.Id;
                                    addOnPriceProjection.AddonId = addOn.Id;
                                    addOnPriceProjection.DistrictId = district.DistrictId;
                                    addOnPriceProjection.ChangeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                                    priceProjectionsToAdd.Add(addOnPriceProjection);
                                }
                            }

                            SIDAL.SaveAddOnPriceProjections(priceProjectionsToAdd);
                        }
                    }
                }
            }
        }
    }
}
