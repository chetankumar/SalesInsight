using RedHill.SalesInsight.AUJSIntegration.Attributes;
using RedHill.SalesInsight.AUJSIntegration.Consumer;
using RedHill.SalesInsight.Logger;
using RedHill.SalesInsight.AUJSIntegration.Model;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Data
{
    public enum SyncStatus { Complete, Error, Pending, Processing }

    public class SyncManager
    {
        private APIConsumer _consumer = null;
        private CompanySetting _companySettings = null;

        public ILogger Logger { get; set; }

        public CompanySetting CompanySettings
        {
            get
            {
                if (this._companySettings == null)
                    this._companySettings = SIDAL.GetCompanySettings();
                return this._companySettings;
            }
        }

        public SyncManager(int companyId)
        {
            this.CompanyId = companyId;
        }

        public SyncManager(int companyId, ILogger logger)
        {
            this.CompanyId = companyId;
            this.Logger = logger;
        }

        public int CompanyId { get; set; }

        private bool LookItemsFetched { get; set; }

        private List<MarketSegment> MarketSegments { get; set; }
        private List<ProjectStatus> ProjectStatusList { get; set; }
        private List<RawMaterialType> RawMaterialTypes { get; set; }
        private List<Plant> Plants { get; set; }
        private List<TaxCode> TaxCodes { get; set; }
        private List<Uom> UnitOfMeasures { get; set; }

        private List<StandardMixConstituent> StandardMixConstituents { get; set; }

        private APIConsumer GetConsumer()
        {
            if (this.CompanySettings == null)
                throw new ApplicationException("Company settings not defined");

            if (this._consumer == null)
                this._consumer = new APIConsumer(this.CompanySettings.APIEndPoint, this.CompanySettings.ClientId, this.CompanySettings.ClientKey);

            if (!this._consumer.LoginSuccessful)
                throw new ApplicationException("[API] Could not log in to the API");

            return this._consumer;
        }

        private List<T> ParseLookUpItem<T>(Dictionary<Type, object> section)
        {
            if (section == null)
                return default(List<T>);

            return (List<T>)Convert.ChangeType(section[typeof(T)], typeof(List<T>));
        }

        public void FetchLookUpItems()
        {
            var consumer = GetConsumer();

            try
            {
                //Fetch Look up items
                var lookUpItems = consumer.GetLookupItems();

                if (lookUpItems != null)
                {
                    this.MarketSegments = ParseLookUpItem<MarketSegment>(lookUpItems);
                    this.RawMaterialTypes = ParseLookUpItem<RawMaterialType>(lookUpItems);
                    this.Plants = ParseLookUpItem<Plant>(lookUpItems);
                    this.ProjectStatusList = ParseLookUpItem<ProjectStatus>(lookUpItems);
                    this.TaxCodes = ParseLookUpItem<TaxCode>(lookUpItems);
                    this.UnitOfMeasures = ParseLookUpItem<Uom>(lookUpItems);
                }

                this.LookItemsFetched = true;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here

            }
        }

        [MethodExecutionSequence(1)]
        public SyncResponse ImportMarketSegments()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(MarketSegment).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                if (!this.LookItemsFetched)
                    this.FetchLookUpItems();

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveMarketSegments(this.MarketSegments);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);

            return syncResponse;
        }

        [MethodExecutionSequence(2)]
        public SyncResponse ImportRawMaterialTypes()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(RawMaterialType).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                if (!this.LookItemsFetched)
                    this.FetchLookUpItems();

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveRawMaterialTypes(this.RawMaterialTypes);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        [MethodExecutionSequence(3)]
        public SyncResponse ImportStatusTypes()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(ProjectStatus).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                if (!this.LookItemsFetched)
                    this.FetchLookUpItems();

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveProjectStatus(this.ProjectStatusList);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        [MethodExecutionSequence(4)]
        public SyncResponse ImportTaxCodes()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(TaxCode).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                if (!this.LookItemsFetched)
                    this.FetchLookUpItems();

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveTaxCodes(this.TaxCodes);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        [MethodExecutionSequence(6)]
        public SyncResponse ImportUnitOfMeasures()
        {
            SyncResponse syncResponse = new SyncResponse();
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(Uom).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                if (!this.LookItemsFetched)
                    this.FetchLookUpItems();

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveUnitOfMeasures(this.UnitOfMeasures);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        [MethodExecutionSequence(5)]
        public SyncResponse ImportPlants()
        {
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(Plant).Name, DateTime.Now);
            SyncResponse syncResponse = new SyncResponse();
            syncResponse.SyncStatus = SyncStatus.Pending;
            string message = null;
            try
            {
                if (!this.LookItemsFetched)
                    this.FetchLookUpItems();

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SavePlants(this.Plants);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                message = ex.Message + "\n" + ex.StackTrace;
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        [MethodExecutionSequence(7)]
        public SyncResponse ImportCustomers()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(Customer).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                var consumer = GetConsumer();

                //Find last fetch history
                var lastImportTimeStamp = GetLastFetchTimeStamp(typeof(Customer).Name);
                var customers = consumer.GetCustomers(lastImportTimeStamp);

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveCustomers(customers);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        /// <summary>
        /// Gets the last fetched history date for the given entity
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private long? GetLastFetchTimeStamp(string name)
        {
            DateTime date = SIDAL.GetAPIFetchHistory(name);
            return date != DateTime.MinValue ? (long?)(date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds : null;
        }

        public SyncResponse ImportSalesStaff()
        {
            SyncResponse syncResponse = new SyncResponse();
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(SalesStaff).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                var consumer = GetConsumer();

                //Get the last import date time
                var lastImportTimeStamp = GetLastFetchTimeStamp(typeof(SalesStaff).Name);
                var salesPeople = consumer.GetSalesPeople(lastImportTimeStamp);

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveSalesStaff(salesPeople);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        public SyncResponse ImportRawMaterials()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(RawMaterial).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                var consumer = GetConsumer();

                //TODO: Get the last import date time
                var lastImportTimeStamp = GetLastFetchTimeStamp(typeof(RawMaterial).Name);
                var rawMaterials = consumer.GetRawMaterials(lastImportTimeStamp);

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveRawMaterials(rawMaterials);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        public SyncResponse ImportProducts()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(StandardMixConstituent).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                var consumer = GetConsumer();

                //Get the last import date time
                var lastImportTimeStamp = GetLastFetchTimeStamp(typeof(StandardMixConstituent).Name);
                var standardMixConstituents = consumer.GetProducts(lastImportTimeStamp);

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveStandardMixConstituents(standardMixConstituents);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        public SyncResponse ImportBlockProducts()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(BlockProduct).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                var consumer = GetConsumer();

                //Get the last import date time
                var lastImportTimeStamp = GetLastFetchTimeStamp(typeof(BlockProduct).Name);
                var blockProducts = consumer.GetBlockProducts(lastImportTimeStamp);

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveBlockProducts(blockProducts);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        public SyncResponse ImportAggregateProducts()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(AggregateProduct).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                var consumer = GetConsumer();

                //Get the last import date time
                var lastImportTimeStamp = GetLastFetchTimeStamp(typeof(AggregateProduct).Name);
                var aggregateProducts = consumer.GetAggregateProducts(lastImportTimeStamp);

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveAggregateProducts(aggregateProducts);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                //TODO: Log ex here
                Logger.LogError(ex.ToString());
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        public SyncResponse ImportAddOns()
        {
            SyncResponse syncResponse = new SyncResponse();
            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(Addon).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                var consumer = GetConsumer();

                //Get the last import date time
                var lastImportTimeStamp = GetLastFetchTimeStamp(typeof(Addon).Name);
                var addOns = consumer.GetAddOns(lastImportTimeStamp);

                fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString());

                DataManager dm = new DataManager(this.CompanyId);
                dm.SaveAddOns(addOns);

                syncResponse.SyncStatus = SyncStatus.Complete;
            }
            catch (Exception ex)
            {
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
                //Log ex here
                Logger.LogError(ex.ToString());
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return syncResponse;
        }

        public bool PushQuote(QuotePayload qp)
        {
            bool success = false;
            SyncResponse syncResponse = new SyncResponse();

            //Create an Entry for API SyncLog
            APIFetchHistory fetchHistory = APIFetchHistory.FindOrCreate(null, typeof(Quotation).Name, DateTime.Now);
            syncResponse.SyncStatus = SyncStatus.Pending;
            try
            {
                //Logging payload
                Logger.LogInfo(qp.ToJson().ToString(), "QuotePayload");
                var consumer = GetConsumer();
                success = consumer.PushQuote(qp);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString(),"QuotePayload_Error");
                syncResponse.SyncStatus = SyncStatus.Error;
                syncResponse.Message = ex.Message;
                syncResponse.StackTrace = ex.StackTrace;
            }

            DateTime? syncDate = null;
            if (syncResponse.SyncStatus == SyncStatus.Complete)
                syncDate = DateTime.Now;

            fetchHistory.UpdateStatus(syncResponse.SyncStatus.ToString(), syncDate, syncResponse.Message);
            return success;
        }

        public void Import()
        {
            //Import Market Segments
            this.ImportMarketSegments();

            //Import Raw material types
            this.ImportRawMaterialTypes();

            //Import Plants
            this.ImportPlants();

            //Import Status Types
            this.ImportStatusTypes();

            //Import Tax Codes
            this.ImportTaxCodes();

            //Import Unit of Measures
            this.ImportUnitOfMeasures();

            //Import Raw Materials
            this.ImportRawMaterials();

            //Import Customers
            this.ImportCustomers();

            //Import Sales Staff
            this.ImportSalesStaff();

            //Import Products
            this.ImportProducts();

            //Import Add Ons
            this.ImportAddOns();
        }
    }
}
