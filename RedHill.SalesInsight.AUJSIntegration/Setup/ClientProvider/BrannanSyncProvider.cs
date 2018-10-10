using RedHill.SalesInsight.AUJSIntegration.Consumer;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Setup.ClientProvider
{
    public class BrannanSyncProvider : IInitialSyncManager
    {
        public int CompanyId { get; set; }
        private APIConsumer _apiConsumer = null;

        public BrannanSyncProvider(int companyId)
        {
            this.CompanyId = companyId;
        }

        private APIConsumer GetConsumer()
        {
            return _apiConsumer != null ? _apiConsumer : new APIConsumer("https://api.brannansandandgravel.com/goldgate/v1", "SalesInsight", "Qy5kQ6EUXUMNXNzjNsfbNajOKWxGP4rNbX7YXOMWQGoNNI4WFDgRDEIP4HVPphn");
        }

        public void SyncMarketSegments()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    foreach (var item in consumer.GetLookupItems(timeStamp: null))
                    {
                        if (item.Key.Equals(typeof(MarketSegment)))
                        {
                            List<MarketSegment> apiList = (List<MarketSegment>)item.Value;

                            List<MarketSegment> dbList = new List<MarketSegment>();
                            dbList = SIDAL.GetMarketSegmentWithNullsDID(this.CompanyId);

                            List<MarketSegment> dbUpdateList = new List<MarketSegment>();
                            foreach (var mSegment in dbList)
                            {
                                if (apiList.Any(x => x.Name.Equals(mSegment.Name)))
                                {
                                    //update it's dispatch id
                                    dbUpdateList.Add(new MarketSegment()
                                    {
                                        Name = mSegment.Name,
                                        DispatchId = apiList.Where(X => X.Name.Equals(mSegment.Name)).FirstOrDefault().DispatchId,
                                        CompanyId = this.CompanyId,
                                        MarketSegmentId = mSegment.MarketSegmentId
                                    });
                                }
                            }

                            if (dbUpdateList.Any())
                            {
                                //update the MarketSegments,
                                SIDAL.UpdateMarketSegmentsDispatchId(dbUpdateList);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Unable to update Market Segments, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncPlants()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    foreach (var item in consumer.GetLookupItems(timeStamp: null))
                    {
                        if (item.Key.Equals(typeof(Plant)))
                        {
                            List<Plant> apiList = (List<Plant>)item.Value;

                            List<Plant> dbList = new List<Plant>();
                            dbList = SIDAL.GetPlantsWithNullsDID();

                            List<Plant> dbUpdateList = new List<Plant>();
                            foreach (var plant in dbList)
                            {
                                if (apiList.Any(x => x.Name != null && x.Name.Equals(plant.Name, StringComparison.OrdinalIgnoreCase)))
                                {
                                    var apiObj = apiList.FirstOrDefault(x => x.Name.Equals(plant.Name, StringComparison.OrdinalIgnoreCase));

                                    //update it's dispatch id
                                    dbUpdateList.Add(new Plant()
                                    {
                                        Name = plant.Name,
                                        DispatchId = apiObj.DispatchId,
                                        CompanyId = this.CompanyId,
                                        PlantId = plant.PlantId
                                    });
                                }
                            }

                            if (dbUpdateList.Any())
                            {
                                //update the Plants,
                                SIDAL.UpdateDispatchIdsForPlants(dbUpdateList);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Unable to update Plants, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncRawMaterialTypes()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    foreach (var item in consumer.GetLookupItems(timeStamp: null))
                    {
                        if (item.Key.Equals(typeof(RawMaterialType)))
                        {
                            List<RawMaterialType> apiList = (List<RawMaterialType>)item.Value;

                            List<RawMaterialType> dbList = SIDAL.GetAllRawMaterialTypeWithNullDID();

                            List<RawMaterialType> dbUpdateList = new List<RawMaterialType>();

                            if (dbList.Any())
                            {
                                foreach (var rmt in dbList)
                                {
                                    if (apiList.Any(x => x.Name != null && x.Name.Equals(rmt.Name, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        var foundObj = apiList.FirstOrDefault(x => x.Name != null && x.Name.Equals(rmt.Name, StringComparison.OrdinalIgnoreCase));

                                        dbUpdateList.Add(
                                            new RawMaterialType()
                                            {
                                                Name = rmt.Name,
                                                DispatchId = foundObj.DispatchId
                                            });
                                    }
                                }
                            }

                            if (dbUpdateList.Any())
                            {
                                SIDAL.UpdateRawMaterialsDispatchId(dbUpdateList);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Problem in updating Raw Material Types, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncStatusTypes()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    foreach (var item in consumer.GetLookupItems(timeStamp: null))
                    {
                        if (item.Key.Equals(typeof(ProjectStatus)))
                        {
                            List<ProjectStatus> apiList = (List<ProjectStatus>)item.Value;

                            List<ProjectStatus> dbList = SIDAL.GetAllProjectStatusWithNullDID(this.CompanyId);

                            List<ProjectStatus> dbUpdateList = new List<ProjectStatus>();

                            if (dbList.Any())
                            {
                                foreach (var pt in dbList)
                                {
                                    if (apiList.Any(x => x.Name == pt.Name))
                                    {
                                        dbUpdateList.Add(
                                            new ProjectStatus()
                                            {
                                                Name = pt.Name,
                                                ProjectStatusId = pt.ProjectStatusId,
                                                DispatchId = apiList.Where(x => x.Name == pt.Name).FirstOrDefault().DispatchId
                                            });
                                    }
                                }
                            }

                            if (dbUpdateList.Any())
                            {
                                SIDAL.UpdateProjectStatusesDispatchId(dbUpdateList);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Problem in updating Project Status, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncTaxSchedules()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    foreach (var item in consumer.GetLookupItems(timeStamp: null))
                    {
                        if (item.Key.Equals(typeof(TaxCode)))
                        {
                            List<TaxCode> apiList = (List<TaxCode>)item.Value;

                            List<TaxCode> dbList = SIDAL.GetTaxCodesWithNullDID();

                            List<TaxCode> dbUpdateList = new List<TaxCode>();

                            if (dbList.Any())
                            {
                                foreach (var tc in dbList)
                                {
                                    if (apiList.Any(x => x.Code == tc.Code))
                                    {
                                        dbUpdateList.Add(
                                            new TaxCode()
                                            {
                                                Code = tc.Code,
                                                DispatchId = apiList.Where(x => x.Code == tc.Code).FirstOrDefault().DispatchId
                                            });
                                    }
                                }
                            }

                            if (dbUpdateList.Any())
                            {
                                SIDAL.UpdateTaxCodesDispatchId(dbUpdateList);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Problem in updating Tax Codes, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncUOMs()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    foreach (var item in consumer.GetLookupItems(timeStamp: null))
                    {
                        if (item.Key.Equals(typeof(Uom)))
                        {
                            List<Uom> apiList = (List<Uom>)item.Value;

                            List<Uom> dbList = new List<Uom>();
                            dbList = SIDAL.GetUOMWithNullsDispatchId();

                            List<Uom> dbUpdateList = new List<Uom>();
                            foreach (var uom in dbList)
                            {
                                if (apiList.Any(x => x.Name.Equals(uom.Name, StringComparison.OrdinalIgnoreCase)))
                                {
                                    var foundObj = apiList.Where(X => X.Name.Equals(uom.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                                    //update it's dispatch id
                                    dbUpdateList.Add(new Uom()
                                    {
                                        Name = uom.Name,
                                        DispatchId = foundObj.DispatchId,
                                        Category = foundObj.Category,
                                    });
                                }
                            }

                            if (dbUpdateList.Any())
                            {
                                SIDAL.UpdateUOMForDispatchId(dbUpdateList);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Unable to update UOM, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncRawMaterials()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    List<RawMaterial> apiList = consumer.GetRawMaterials();

                    List<RawMaterial> dbList = new List<RawMaterial>();
                    dbList = SIDAL.GetRawMaterialsWithNullsDID();

                    List<RawMaterial> dbUpdateList = new List<RawMaterial>();
                    foreach (var rawMaterial in dbList)
                    {
                        if (apiList.Any(x => x.Description != null && x.Description.Equals(rawMaterial.MaterialCode, StringComparison.OrdinalIgnoreCase)))
                        {
                            var foundObj = apiList.Where(x => x.Description.Equals(rawMaterial.MaterialCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                            //update it's dispatch id
                            dbUpdateList.Add(new RawMaterial()
                            {
                                MaterialCode = foundObj.Description,
                                DispatchId = foundObj.DispatchId
                            });
                        }
                    }

                    if (dbUpdateList.Any())
                    {
                        SIDAL.UpdateRawMaterialsForDispatchId(dbUpdateList);
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Unable to update Raw Materials, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncSalesStaff()
        {
            var consumer = GetConsumer();

            try
            {
                List<SalesStaff> apiList = consumer.GetSalesPeople(null);

                List<SalesStaff> exSalesStaffManager = SIDAL.GetSalesStaffWithNullDID(this.CompanyId);

                List<SalesStaff> dbPendingUpdates = new List<SalesStaff>();

                foreach (var item in exSalesStaffManager)
                {
                    if (apiList.Any(x => x.Name != null && x.Name.Equals(item.Name)))
                    {
                        var foundObj = apiList.Where(x => x.Name != null && x.Name.Equals(item.Name)).FirstOrDefault();

                        dbPendingUpdates.Add(
                            new SalesStaff()
                        {
                            Name = foundObj.Name,
                            DispatchId = foundObj.DispatchId
                        });
                    }
                }

                if (dbPendingUpdates.Any())
                {
                    SIDAL.UpdateSalesStaffForDID(dbPendingUpdates);
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Problem in updating Sales Staff, please try again. ( " + ex.Message + " ) ");
            }
        }

        public void SyncCustomers()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    List<Customer> apiList = (List<Customer>)consumer.GetCustomers();

                    List<Customer> dbList = new List<Customer>();
                    dbList = SIDAL.GetCustomersWithNullsDID();

                    List<Customer> dbUpdateList = new List<Customer>();
                    foreach (var customer in dbList)
                    {
                        if (apiList.Any(x => x.CustomerNumber.Equals(customer.CustomerNumber)))
                        {
                            var foundObj = apiList.Where(x => x.CustomerNumber == customer.CustomerNumber).FirstOrDefault();
                            //update it's dispatch id
                            dbUpdateList.Add(new Customer()
                            {
                                CustomerNumber = foundObj.CustomerNumber,
                                DispatchId = foundObj.DispatchId,
                            });
                        }
                    }

                    if (dbUpdateList.Any())
                    {
                        SIDAL.UpdateCustomersForDispatchId(dbUpdateList);
                    }

                }
                catch (Exception e)
                {
                    throw new ApplicationException("Unable to update Customers, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void SyncProducts()
        {

        }

        public void SyncStandardMix()
        {
            var consumer = GetConsumer();
            if (consumer != null)
            {
                try
                {
                    List<StandardMixConstituent> apiList = (List<StandardMixConstituent>)consumer.GetProducts();

                    List<StandardMix> dbList = new List<StandardMix>();
                    dbList = SIDAL.GetStandardMixWithNullsDID();

                    List<StandardMix> updateList = new List<StandardMix>();

                    if (apiList.Any())
                    {
                        foreach (var item in dbList)
                        {
                            if (apiList.Any(x => x.MixFormulation.StandardMix.Number != null
                                && x.MixFormulation.StandardMix.Number.Equals(item.Number, StringComparison.OrdinalIgnoreCase)))
                            {
                                var foundObj = apiList.Where(x => x.MixFormulation.StandardMix.Number != null
                                    && x.MixFormulation.StandardMix.Number.Equals(item.Number, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                                updateList.Add(
                                    new StandardMix()
                                    {
                                        Number = foundObj.MixFormulation.StandardMix.Number,
                                        DispatchId = foundObj.MixFormulation.StandardMix.DispatchId
                                    });
                            }
                        }

                        if (updateList.Any())
                        {
                            SIDAL.UpdateStandardMixes(updateList);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Unable to update Standard Mix Constituent, please try again. ( " + e.Message + " ) ");
                }
            }
        }

        public void StartSync()
        {
            //SyncMarketSegments();
            //SyncRawMaterialTypes();
            //SyncPlants();
            //SyncStatusTypes();
            //SyncTaxSchedules();
            //SyncUOMs();
            //SyncRawMaterials();
            SyncCustomers();
            //SyncStandardMix();
        }
    }
}
