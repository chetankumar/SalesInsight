using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Redhill.SalesInsight.ESI.Mongo.Models;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Mongo.Models;
using RedHill.SalesInsight.DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Utils;
using Redhill.SalesInsight.ESI.Mongo.QueryBuilders;
using Redhill.SalesInsight.ESI.Enumerations;
using Redhill.SalesInsight.ESI.ReportModels;
using System.Diagnostics;
using RedHill.SalesInsight.Logger;
using System.Linq.Dynamic;

namespace RedHill.SalesInsight.ESI
{
    public class ESIDataManager
    {
        public const string TicketStatsTable = "TicketStats";
        public const string DriverDayStatsTable = "DriverDayStats";
        public const string PlantDayStatsTable = "PlantDayStats";
        public const string DailyPlantSummaryTable = "DailyPlantSummary";

        private IMongoDatabase GetDatabase()
        {
            IMongoClient _client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString);
            IMongoDatabase _database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDb"]);
            return _database;
        }

        private IMongoCollection<T> GetCollection<T>(string tableName)
        {
            var database = GetDatabase();
            var collection = database.GetCollection<T>(tableName);
            return collection;
        }

        public void AddOrUpdateTicketStats(TicketStats document)
        {
            var collection = GetCollection<TicketStats>(TicketStatsTable);

            collection.ReplaceOne<TicketStats>(x => x.Id == document.Id, document, new UpdateOptions { IsUpsert = true });
        }

        public bool AddOrUpdateTicketStatsCollection(List<TicketStats> documents)
        {
            List<BsonDocument> dataToInsert = new List<BsonDocument>();
            documents.ForEach(document =>
            {
                dataToInsert.Add(document.ToBsonDocument());
            });

            var collection = GetCollection<BsonDocument>(TicketStatsTable);
            collection.InsertMany(dataToInsert);
            return true;
        }

        public bool DeleteTicketStats(DateTime startDate, DateTime endDate)
        {
            var collection = GetCollection<TicketStats>(TicketStatsTable);
            var result = collection.DeleteMany<TicketStats>(x => x.Date >= startDate && x.Date <= endDate);
            return result.IsAcknowledged;
        }

        public void AddOrUpdatePlantDayStats(PlantDayStats document)
        {
            var collection = GetCollection<PlantDayStats>(PlantDayStatsTable);

            collection.ReplaceOne<PlantDayStats>(x => x.Id == document.Id, document, new UpdateOptions { IsUpsert = true });
        }

        public void ProcessEsiCache(int year, int month)
        {
            //Console.WriteLine("ProcessEsiCache");
            //Console.WriteLine("CleanTicketData");
            //SIDAL.CleanTicketDataNew();
            //SIDAL.TicketBulkInsertCleanup();

            //Console.WriteLine("CleanDriverLoginTimes");
            //SIDAL.CleanDriverLoginTimesNew();
            //SIDAL.UpdateDriverLoginTimesAsScrubbed();
            //SIDAL.TicketBulkInsertCleanup();

            //Console.WriteLine("ProcessOrphanTickets");
            //SIDAL.ProcessOrphanTickets();

            SIDAL.CleanTicketData(year, month);
            SIDAL.CleanDriverLoginTimes(year, month);
            SIDAL.ProcessOrphanTickets();
        }

        public void UploadPlantDayStats(int month, int year)
        {
            DateTime startDate = DateUtils.GetFirstOf(month, year);
            DateTime endDate = startDate.AddMonths(1);
            DateTime tmpDate = startDate;
            var totalDistribution = SIDAL.GetTotalWorkDays(startDate.AddDays(-1), endDate.AddDays(-1)); // The method excludes the first day, and includes the end date, so we need to decrease the end date by 1

            while (tmpDate < endDate)
            {
                Console.WriteLine($"UploadPlantDayStats YEAR: {year}, Month: {month}, Date : {tmpDate.ToShortDateString()}");
                var currentDistribution = SIDAL.GetWorkDayDistribution(tmpDate);
                foreach (Plant plant in SIDAL.GetPlantsList())
                {
                    Console.WriteLine($"UploadPlantDayStats YEAR: {year}, Month: {month}, Date : {tmpDate.ToShortDateString()}, plant : {plant.Name}");
                    PlantDayStats stats = new PlantDayStats();
                    stats.Date = tmpDate;
                    stats.PlantId = plant.PlantId;
                    stats.PlantName = plant.Name;
                    stats.DistrictId = plant.DistrictId;
                    stats.DistrictName = plant.District.Name;
                    stats.RegionId = plant.District.RegionId;
                    stats.RegionName = plant.District.Region.Name;
                    PlantBudget budget = SIDAL.GetPlantBudget(plant.PlantId, month, year);
                    // Lets populated the daily budget entries first

                    if (budget != null)
                    {
                        stats.BudgetVolume = budget.Budget.GetValueOrDefault() * (currentDistribution / totalDistribution);
                        stats.BudgetTrucks = budget.Trucks.GetValueOrDefault();
                        stats.BudgetStartupMinutes = budget.StartUp.GetValueOrDefault();
                        stats.BudgetTicketingMinutes = budget.Ticketing.GetValueOrDefault();
                        stats.BudgetLoadMinutes = budget.Loading.GetValueOrDefault();
                        stats.BudgetTemperMinutes = budget.Tempering.GetValueOrDefault();
                        stats.BudgetToJobMinutes = budget.ToJob.GetValueOrDefault();
                        stats.BudgetWaitMinutes = budget.Wait.GetValueOrDefault();
                        stats.BudgetUnloadMinutes = budget.Unload.GetValueOrDefault();
                        stats.BudgetWashMinutes = budget.Wash.GetValueOrDefault();
                        stats.BudgetFromJobMinutes = budget.FromJob.GetValueOrDefault();
                        stats.BudgetShutdownMinutes = budget.Shutdown.GetValueOrDefault();
                        //stats.BudgetLateMinutes = budget.LateMinutes.GetValueOrDefault();
                        stats.BudgetAvgLoad = budget.AvgLoad.GetValueOrDefault();
                        stats.BudgetAccidents = budget.Accidents.GetValueOrDefault() * (currentDistribution / totalDistribution);
                        stats.BudgetBatchTolerance = budget.BatchTolerance.GetValueOrDefault() * (currentDistribution / totalDistribution); ;
                        stats.BudgetCydHr = budget.CydHr.GetValueOrDefault();
                        stats.BudgetFirstLoadOnTimePercent = budget.FirstLoadOnTimePercent.GetValueOrDefault();
                        stats.BudgetInYardMinutes = budget.InYard.GetValueOrDefault();
                        stats.BudgetPlantInterruptions = budget.PlantInterruptions.GetValueOrDefault() * (currentDistribution / totalDistribution);
                        stats.BudgetTrucksDown = budget.TrucksDown.GetValueOrDefault();
                        stats.BudgetTrucksPercentOperable = budget.TrucksPercentOperable.GetValueOrDefault();
                    }

                    PlantFinancialBudget finBudget = SIDAL.GetPlantFinancialBudgets(plant.PlantId, month, year);
                    if (finBudget != null)
                    {
                        stats.BudgetRevenue = Convert.ToDouble(finBudget.Revenue.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                        stats.BudgetMaterialCost = Convert.ToDouble(finBudget.MaterialCost.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                        stats.BudgetSpread = stats.BudgetRevenue - stats.BudgetMaterialCost;
                        stats.BudgetProfit = Convert.ToDouble(finBudget.Profit.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                        stats.BudgetSGA = Convert.ToDouble(finBudget.SGA.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                        stats.BudgetDeliveryVariable = Convert.ToDouble(finBudget.DeliveryVariable.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                        stats.BudgetContribution = stats.BudgetSpread - stats.BudgetDeliveryVariable;
                        stats.BudgetPlantVariable = Convert.ToDouble(finBudget.PlantVariable.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                        stats.BudgetDeliveryFixed = Convert.ToDouble(finBudget.DeliveryFixed.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                        stats.BudgetPlantFixed = Convert.ToDouble(finBudget.PlantFixed.GetValueOrDefault()) * (currentDistribution / totalDistribution);
                    }
                    var collection = GetCollection<PlantDayStats>(PlantDayStatsTable);
                    collection.ReplaceOne(x => x.Id == stats.Id, stats, new UpdateOptions { IsUpsert = true });
                }
                tmpDate = tmpDate.AddDays(1);
            }
        }

        public bool UpdateMongoByEsiCacheNew()
        {
            Stopwatch watch = new Stopwatch();
            try
            {
                var drivers = SIDAL.GetDriversFromTickets();
                if (drivers != null && drivers.Any())
                {
                    List<TicketStats> stats = null;
                    foreach (var driverNumber in drivers)
                    {
                        watch.Start();
                        stats = new List<TicketStats>();
                        var tickets = SIDAL.GetTicketsByDriver(driverNumber);
                        var ticketsCount = 0;
                        if (tickets != null && tickets.Count > 0)
                        {
                            ticketsCount = tickets.Count;
                            tickets.ForEach(x =>
                            {
                                stats.Add(GetTicketStats(x));
                            });
                            AddOrUpdateTicketStatsCollection(stats);
                            SIDAL.UpdateTicketDetailsAsIsMongoUpdated(driverNumber);
                        }

                        watch.Stop();
                        Console.WriteLine($"Process take {watch.Elapsed.TotalSeconds} seconds to update mongo for driver number: {driverNumber} for total {ticketsCount} tickets.");
                        watch.Reset();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateMongoByEsiCache()
        {
            try
            {
                bool continueNext = true;
                while (continueNext)
                {
                    List<TicketDetail> tickets = SIDAL.GetMongoPendingTickets(5000);
                    if (tickets != null && tickets.Count > 0)
                    {
                        tickets.ForEach(x =>
                        {
                            AddOrUpdateTicketStats(GetTicketStats(x));
                            if (!x.IsMongoUpdated) SIDAL.UpdateTicketIsMongoUpdated(x.Id);
                        });
                    }
                    else
                    {
                        continueNext = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateMongoByDailyPlantSummary()
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                bool continueNext = true;
                while (continueNext)
                {
                    List<DailyPlantSummary> dailyPlantSummaryList = SIDAL.GetMongoPendingDailyPlantSummary(0);

                    if (dailyPlantSummaryList != null && dailyPlantSummaryList.Count > 0)
                    {
                        dailyPlantSummaryList.ForEach(x =>
                        {
                            AddOrUpdateDailyPlantSummaryStats(new DailyPlantSummaryStats(x));
                            if (!x.IsMongoUpdated)
                                SIDAL.UpdateDailyPlantSummaryIsMongoUpdated(x.Id);
                        });
                    }
                    else
                    {
                        continueNext = false;
                    }
                }
                // the code that you want to measure comes here
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private TicketStats GetTicketStats(TicketDetail ticketDetail)
        {
            DriverDetail driverInfo = SIDAL.GetDriverDetail(ticketDetail.DriverNumber);

            var stats = new TicketStats();
            stats.Id = ticketDetail.Id;
            stats.IsVoid = ticketDetail.VoidStatus;
            stats.TicketId = ticketDetail.TicketId;
            stats.TicketNumber = ticketDetail.TicketNumber;
            stats.Date = ticketDetail.TicketDate.Value;

            var plant = SIDAL.GetPlantByDispatchCode(ticketDetail.PlantNumber);
            if (plant != null)
            {
                stats.PlantId = plant.PlantId;
                stats.PlantName = plant.Name;
                stats.DistrictId = plant.DistrictId;
                stats.DistrictName = plant.District.Name;
                stats.RegionId = plant.District.RegionId;
                stats.RegionName = plant.District.Region.Name;
            }
            stats.FOB = ticketDetail.FOB.HasValue ? ticketDetail.FOB.Value : false;
            if (driverInfo != null)
                stats.DriverId = driverInfo.Id;
            stats.DriverNumber = ticketDetail.DriverNumber;
            stats.DriverDescription = ticketDetail.DriverDescription;
            stats.TruckNumber = ticketDetail.TruckNumber;
            stats.TruckType = ticketDetail.TruckType;

            var customer = SIDAL.GetCustomerByDispatchCode(ticketDetail.CustomerNumber);
            if (customer != null)
            {
                var customerMS = SIDAL.GetMarketSegmentByDispatch(ticketDetail.CustomerSegmentNumber);
                if (customerMS != null)
                {
                    stats.CustomerSegmentId = customerMS.MarketSegmentId;
                    stats.CustomerSegmentName = customerMS.Name;
                }
                stats.CustomerId = customer.CustomerId;
                stats.CustomerNumber = customer.DispatchId;
                stats.CustomerName = customer.Name;

                stats.CustomerCity = ticketDetail.CustomerCity;
                stats.CustomerZipCode = ticketDetail.CustomerZip;
            }

            var project = SIDAL.GetProjectByDispatchCode(ticketDetail.JobNumber);
            if (project != null)
            {
                var projectMS = SIDAL.GetMarketSegmentByDispatch(ticketDetail.JobSegmentNumber);
                if (projectMS != null)
                {
                    stats.JobSegmentId = projectMS.MarketSegmentId;
                }

                stats.JobId = project.ProjectId;
                stats.JobName = project.Name;
            }

            var salesStaff = SIDAL.GetSalesStaffByDispatchCode(ticketDetail.SalesPersonNumber);
            if (salesStaff != null)
            {
                stats.SalesStaffId = salesStaff.SalesStaffId;
                stats.SalesStaffName = salesStaff.Name;
                stats.SalesStaffNumber = ticketDetail.SalesPersonNumber;
            }

            stats.BatchmanNumber = ticketDetail.BatchmanNumber;
            stats.BatchmanDescription = ticketDetail.BatchmanDescription;
            stats.Volume = ticketDetail.DeliveredVolume.HasValue ? ticketDetail.DeliveredVolume.Value : 0;
            stats.Revenue = Convert.ToDouble(ticketDetail.TotalRevenue.HasValue ? ticketDetail.TotalRevenue.Value : 0);
            stats.MaterialCost = Convert.ToDouble(ticketDetail.MaterialCost.HasValue ? ticketDetail.MaterialCost.Value : 0);
            stats.Spread = stats.Revenue - stats.MaterialCost;

            stats.StartupMinutes = ticketDetail.StartupTime.GetValueOrDefault();
            stats.ShutdownMinutes = ticketDetail.ShutdownTime.GetValueOrDefault();
            stats.EstimatedClockHours = ticketDetail.EstimatedClockHours.GetValueOrDefault();

            stats.TotalMinutes = DateUtils.SubstractDates(ticketDetail.TimeTicketed.GetValueOrDefault(), ticketDetail.TimeArrivePlant.GetValueOrDefault());
            stats.TicketingMinutes = DateUtils.SubstractDates(ticketDetail.TimeTicketed.GetValueOrDefault(), ticketDetail.TimeBeginLoad.GetValueOrDefault());
            stats.LoadMinutes = DateUtils.SubstractDates(ticketDetail.TimeBeginLoad.GetValueOrDefault(), ticketDetail.TimeEndLoad.GetValueOrDefault());
            stats.Temper = DateUtils.SubstractDates(ticketDetail.TimeEndLoad.GetValueOrDefault(), ticketDetail.TimeLeavePlant.GetValueOrDefault());
            stats.ToJobMinutes = DateUtils.SubstractDates(ticketDetail.TimeLeavePlant.GetValueOrDefault(), ticketDetail.TimeArriveJob.GetValueOrDefault());
            stats.WaitMinutes = DateUtils.SubstractDates(ticketDetail.TimeArriveJob.GetValueOrDefault(), ticketDetail.TimeBeginUnload.GetValueOrDefault());
            stats.UnloadMinutes = DateUtils.SubstractDates(ticketDetail.TimeBeginUnload.GetValueOrDefault(), ticketDetail.TimeEndUnload.GetValueOrDefault());
            stats.UnloadMinutes = DateUtils.SubstractDates(ticketDetail.TimeEndUnload.GetValueOrDefault(), ticketDetail.TimeLeaveJob.GetValueOrDefault());
            stats.FromJobMinutes = DateUtils.SubstractDates(ticketDetail.TimeLeaveJob.GetValueOrDefault(), ticketDetail.TimeArrivePlant.GetValueOrDefault());
            stats.LateMinutes = DateUtils.SubstractDates(ticketDetail.TimeDueOnJob.GetValueOrDefault(), ticketDetail.TimeArrivePlant.GetValueOrDefault());
            stats.VariableDelivery = stats.TotalMinutes * Convert.ToDouble(plant.VariableCostPerMin.GetValueOrDefault()) * Convert.ToDouble(plant.Utilization.GetValueOrDefault(1));
            stats.Contribution = stats.Spread - stats.VariableDelivery;
            stats.FixedPlant = Convert.ToDouble(plant.PlantFixedCost.GetValueOrDefault()) * stats.Volume;
            stats.FixedDelivery = Convert.ToDouble(plant.DeliveryFixedCost.GetValueOrDefault()) * stats.Volume;
            stats.SGA = Convert.ToDouble(plant.SGA.GetValueOrDefault()) * stats.Volume;
            stats.Profit = stats.Contribution - stats.FixedPlant - stats.FixedDelivery - stats.SGA;

            return stats;
        }

        #region Daily Plant Summary SQL Importer

        public void ImportDailyPlantSummary(string filePath, int sheetNumber = 1)
        {
            ImportData<DailyPlantSummary> dailyPlantSummaryImport = new ImportData<DailyPlantSummary>(new string[] { "RefId", "DayDateTime", "PlantId", "ProducedVolume", "TrucksAssigned", "TruckAvailable", "DriversOnPayroll", "DriversAvailable", "PlantInterruptions", "BadOrRejectedLoads", "Accidents", "TotalLoads", "TotalOrders", "FirstLoadOnTimePercent", "DriverDeliveredVolume", "ScheduledVolume", "ScheduledTrucks", "TotalClockHours", "DriversUtilized", "AverageLoadSize", "StartUpTime", "ShutdownTime", "InYardTime", "TicketTime", "LoadTime", "TemperingTime", "ToJobTime", "WaitOnJobTime", "PourTime", "WashOnJobTime", "FromJobTime", "TruckBreakdowns", "NonDeliveryHours" });

            dailyPlantSummaryImport.FilePath = filePath;
            dailyPlantSummaryImport.SheetNumber = sheetNumber;
            //dailyPlantSummaryImport.RowsToCallBackList = 1000;
            dailyPlantSummaryImport.RowImporting += DailyPlantSummaryImport_RowImporting;
            dailyPlantSummaryImport.CellImporting += DailyPlantSummaryImport_CellImporting;
            dailyPlantSummaryImport.RowImportingListCallBack += DailyPlantSummaryImport_RowImportingListCallBack;
            dailyPlantSummaryImport.StartImport();

        }

        private void DailyPlantSummaryImport_CellImporting(object sender, ImportDataRowImportingEventArgs<DailyPlantSummary> e)
        {
        }

        private void DailyPlantSummaryImport_RowImporting(object sender, ImportDataRowImportingEventArgs<DailyPlantSummary> e)
        {
            if (!e.RowProcessedOnce)
            {
                if (e.Item == null)
                {
                    e.IgnoreRow = true;
                    return;
                }

                Console.WriteLine("ImportData_RowImporting save in db DayDateTime : " + e.Item.DayDateTime + ", PlantId : " + e.Item.PlantId);
                SIDAL.AddUpdateDailyPlantSummary(e.Item);
            }
        }

        private void DailyPlantSummaryImport_RowImportingListCallBack(object sender, ImportDataRowListEventArgs<DailyPlantSummary> e)
        {
            //if (e.Items != null && e.Items.Count > 0)
            //{
            //    Console.WriteLine("DailyPlantSummaryImport_RowImportingListCallBack save in db Current Ticket Count : " + e.Items.Count + ", Total Tickets Processed : " + e.RowsSucceeded);
            //    SIDAL.AddUpdateDailyPlantSummaryList(e.Items);
            //}
        }

        #endregion
        #region Driver Details SQL Importer

        public void ImportDriverDetails(string driverLoginFile, int sheetNumber = 1)
        {
            ImportData<DriverDetail> driverDetailImportData = new ImportData<DriverDetail>(new string[] { "LoginDate", "DriverNumber", "Driver_Name", "FirstPunchInTime", "LastPunchOutTime", "TotalClockHours" });

            driverDetailImportData.FilePath = driverLoginFile;   // Read posted file\
            driverDetailImportData.SheetNumber = sheetNumber;
            driverDetailImportData.RowsToCallBackList = 20000;
            driverDetailImportData.RowImporting += DriverDetailImportData_RowImporting;
            driverDetailImportData.CellImporting += DriverDetailImportData_CellImporting;
            driverDetailImportData.RowImportingListCallBack += DriverDetailImportData_RowImportingListCallBack;
            driverDetailImportData.StartImport();
        }

        public void ImportDriverDetails(HttpPostedFileBase driverLoginFile, int sheetNumber = 1)
        {
            ImportData<DriverDetail> driverDetailImportData = new ImportData<DriverDetail>(new string[] { "LoginDate", "DriverNumber", "Driver_Name", "FirstPunchInTime", "LastPunchOutTime", "TotalClockHours" });

            driverDetailImportData.PostedFile = driverLoginFile;
            driverDetailImportData.RowImporting += DriverDetailImportData_RowImporting;
            driverDetailImportData.CellImporting += DriverDetailImportData_CellImporting;
            driverDetailImportData.StartImport();

            //driverDetailImportData.PostedFile = driverLoginFile;   // Read posted file\
            //driverDetailImportData.SheetNumber = sheetNumber;
            //driverDetailImportData.RowsToCallBackList = 20000;
            //driverDetailImportData.RowImporting += DriverDetailImportData_RowImporting;
            //driverDetailImportData.CellImporting += DriverDetailImportData_CellImporting;
            //driverDetailImportData.RowImportingListCallBack += DriverDetailImportData_RowImportingListCallBack;
            //driverDetailImportData.StartImport();
        }

        private void DriverDetailImportData_CellImporting(object sender, ImportDataRowImportingEventArgs<DriverDetail> e)
        {

            if (!e.RowProcessedOnce)
            {
                switch (e.ColumnName)
                {
                    case "DriverNumber":
                        if (e.Value != null)
                        {
                            e.Item.DriverNumber = e.Value.ToString();
                        }
                        break;
                    case "Driver_Name":
                        if (e.Value != null)
                        {
                            e.Item.DriverName = e.Value.ToString();
                        }
                        break;
                }
            }
        }

        private void DriverDetailImportData_RowImporting(object sender, ImportDataRowImportingEventArgs<DriverDetail> e)
        {
            if (!e.RowProcessedOnce)
            {
                if (e.Item == null)
                {
                    e.IgnoreRow = true;
                    return;
                }

                Console.WriteLine("DriverDetailImportData_RowImporting save in db DriverNumber : " + e.Item.DriverNumber);
                SIDAL.AddUpdateDriverDetail(e.Item);
            }
        }

        private void DriverDetailImportData_RowImportingListCallBack(object sender, ImportDataRowListEventArgs<DriverDetail> e)
        {
            if (e.Items != null && e.Items.Count > 0)
            {
                Console.WriteLine("DriverDetailImportData_RowImportingListCallBack save in db Current Ticket Count : " + e.Items.Count + ", Total Tickets Processed : " + e.RowsSucceeded);
                SIDAL.AddUpdateDriverDetailList(e.Items);
            }
        }

        #endregion
        #region Driver Login Times SQL Importer

        public void ImportDriverLoginTimes(string driverLoginFile, int sheetNumber = 1)
        {
            ImportData<DriverLoginTime> driverLoginImportData = new ImportData<DriverLoginTime>(new string[] { "LoginDate", "DriverNumber", "Driver_Name", "FirstPunchInTime", "LastPunchOutTime", "TotalClockHours" });

            driverLoginImportData.FilePath = driverLoginFile;
            driverLoginImportData.SheetNumber = sheetNumber;
            driverLoginImportData.RowsToCallBackList = 1000;
            driverLoginImportData.RowImporting += DriverLoginImportData_RowImporting;
            driverLoginImportData.CellImporting += DriverLoginImportData_CellImporting;
            driverLoginImportData.RowImportingListCallBack += DriverLoginImportData_RowImportingListCallBack;
            driverLoginImportData.StartImport();
        }

        public void ImportDriverLoginTimes(HttpPostedFileBase driverLoginFile, int sheetNumber = 1)
        {
            ImportData<DriverLoginTime> driverLoginImportData = new ImportData<DriverLoginTime>(new string[] { "LoginDate", "DriverNumber", "Driver_Name", "FirstPunchInTime", "LastPunchOutTime", "TotalClockHours" });

            driverLoginImportData.PostedFile = driverLoginFile;
            driverLoginImportData.RowImporting += DriverLoginImportData_RowImporting;
            driverLoginImportData.CellImporting += DriverLoginImportData_CellImporting;
            driverLoginImportData.StartImport();
            //driverLoginImportData.PostedFile = driverLoginFile;
            //driverLoginImportData.SheetNumber = sheetNumber;
            //driverLoginImportData.RowsToCallBackList = 1000;
            //driverLoginImportData.RowImporting += DriverLoginImportData_RowImporting;
            //driverLoginImportData.CellImporting += DriverLoginImportData_CellImporting;
            //driverLoginImportData.RowImportingListCallBack += DriverLoginImportData_RowImportingListCallBack;
            //driverLoginImportData.StartImport();
        }

        private void DriverLoginImportData_CellImporting(object sender, ImportDataRowImportingEventArgs<DriverLoginTime> e)
        {

            if (!e.RowProcessedOnce)
            {
                switch (e.ColumnName)
                {
                    case "FirstPunchInTime":
                        if (e.Value != null)
                        {
                            e.Item.PunchInTime = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "LastPunchOutTime":
                        if (e.Value != null)
                        {
                            e.Item.PunchOutTime = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "TotalClockHours":
                        if (e.Value != null)
                        {
                            e.Item.TotalTime = Convert.ToDouble(e.Value) * 60;
                        }
                        break;
                }
            }
        }

        private void DriverLoginImportData_RowImporting(object sender, ImportDataRowImportingEventArgs<DriverLoginTime> e)
        {
            if (!e.RowProcessedOnce)
            {
                if (e.Item == null)
                {
                    e.IgnoreRow = true;
                    return;
                }

                Console.WriteLine("DriverLoginImportData_RowImporting save in db DriverNumber : " + e.Item.DriverNumber + ", LoginDate : " + e.Item.LoginDate);
                SIDAL.AddUpdateDriverLoginTime(e.Item);
            }
        }

        private void DriverLoginImportData_RowImportingListCallBack(object sender, ImportDataRowListEventArgs<DriverLoginTime> e)
        {
            if (e.Items != null && e.Items.Count > 0)
            {
                Console.WriteLine("DriverLoginImportData_RowImportingListCallBack save in db Current Ticket Count : " + e.Items.Count + ", Total Tickets Processed : " + e.RowsSucceeded);
                SIDAL.AddUpdateDriverLoginTimeList(e.Items);
            }
        }

        #endregion
        #region Ticket Details sql Importer

        public void ImportTicketData(HttpPostedFileBase file, int sheetNumber = 1)
        {
            ImportData<TicketDetail> ticketImportData = new ImportData<TicketDetail>(new string[] { "TicketId", "TicketNumber", "TicketDate", "Void_Status", "Time_Due_On_Job", "Time_Ticketed", "Time_Beg_Load", "Time_End_Load", "Time_Leave_Plant", "Time_Arrive_Job", "Time_Beg_Unload", "Time_End_Unload", "Time_Leave_Job", "Time_Arrive_Plant", "Ticket_Plant_Num", "Ticket_Plant_Desc", "fob", "Driver_Num", "Driver_Desc", "Driver_Type", "Driver_Home_Plant_Num", "Truck_Num", "Truck_Type", "Truck_Home_Plant_Num", "Cust_Num", "Cust_Desc", "Cust_Seg_Num", "Cust_Seg_Desc", "Cust_City", "Cust_Zip", "Job_Num", "Job_Desc", "Job_Seg_Num", "Job_Seg_Desc", "Delv_Addr", "Salesperson_Num", "Salesperson_Desc", "Batchman_Num", "Batchman_Desc", "Delivered_Volume", "Total_Revenue", "Material_Cost" });

            ticketImportData.PostedFile = file;
            ticketImportData.RowImporting += TicketImportData_RowImporting;
            ticketImportData.CellImporting += TicketImportData_CellImporting;
            ticketImportData.StartImport();

            //ticketImportData.PostedFile = file;
            //ticketImportData.SheetNumber = sheetNumber;
            //ticketImportData.RowsToCallBackList = 1000;
            //ticketImportData.RowImporting += TicketImportData_RowImporting;
            //ticketImportData.CellImporting += TicketImportData_CellImporting;
            //ticketImportData.RowImportingListCallBack += TicketImportData_RowImportingListCallBack;
            //ticketImportData.StartImport();
        }
        public void ImportTicketData(string ticketFile, int sheetNumber = 1)
        {
            ImportData<TicketDetail> ticketImportData = new ImportData<TicketDetail>(new string[] { "TicketId", "TicketNumber", "TicketDate", "Void_Status", "Time_Due_On_Job", "Time_Ticketed", "Time_Beg_Load", "Time_End_Load", "Time_Leave_Plant", "Time_Arrive_Job", "Time_Beg_Unload", "Time_End_Unload", "Time_Leave_Job", "Time_Arrive_Plant", "Ticket_Plant_Num", "Ticket_Plant_Desc", "fob", "Driver_Num", "Driver_Desc", "Driver_Type", "Driver_Home_Plant_Num", "Truck_Num", "Truck_Type", "Truck_Home_Plant_Num", "Cust_Num", "Cust_Desc", "Cust_Seg_Num", "Cust_Seg_Desc", "Cust_City", "Cust_Zip", "Job_Num", "Job_Desc", "Job_Seg_Num", "Job_Seg_Desc", "Delv_Addr", "Salesperson_Num", "Salesperson_Desc", "Batchman_Num", "Batchman_Desc", "Delivered_Volume", "Total_Revenue", "Material_Cost" });

            ticketImportData.FilePath = ticketFile;
            ticketImportData.SheetNumber = sheetNumber;
            ticketImportData.RowsToCallBackList = 1000;
            ticketImportData.RowImporting += TicketImportData_RowImporting;
            ticketImportData.CellImporting += TicketImportData_CellImporting;
            ticketImportData.RowImportingListCallBack += TicketImportData_RowImportingListCallBack;
            ticketImportData.StartImport();

        }


        private void TicketImportData_CellImporting(object sender, ImportDataRowImportingEventArgs<TicketDetail> e)
        {
            if (!e.RowProcessedOnce)
            {
                switch (e.ColumnName)
                {
                    case "Void_Status":
                        if (e.Value != null)
                        {
                            e.Item.VoidStatus = e.Value.ToString() == "Y";
                        }
                        break;
                    case "Time_Due_On_Job":
                        if (e.Value != null)
                        {
                            e.Item.TimeDueOnJob = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_Ticketed":
                        if (e.Value != null)
                        {
                            e.Item.TimeTicketed = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_Beg_Load":
                        if (e.Value != null)
                        {
                            e.Item.TimeBeginLoad = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_End_Load":
                        if (e.Value != null)
                        {
                            e.Item.TimeEndLoad = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_Leave_Plant":
                        if (e.Value != null)
                        {
                            e.Item.TimeLeavePlant = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_Arrive_Job":
                        if (e.Value != null)
                        {
                            e.Item.TimeArriveJob = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_Beg_Unload":
                        if (e.Value != null)
                        {
                            e.Item.TimeBeginUnload = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_End_Unload":
                        if (e.Value != null)
                        {
                            e.Item.TimeEndUnload = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_Leave_Job":
                        if (e.Value != null)
                        {
                            e.Item.TimeLeaveJob = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "Time_Arrive_Plant":
                        if (e.Value != null)
                        {
                            e.Item.TimeArrivePlant = DateTime.FromOADate(Convert.ToDouble(e.Value));
                        }
                        break;
                    case "fob":
                        if (e.Value != null)
                        {
                            e.Item.FOB = e.Value?.ToString() == "Y";
                        }
                        break;
                    case "Ticket_Plant_Num":
                        if (e.Value != null)
                        {
                            e.Item.PlantNumber = e.Value.ToString();
                        }
                        break;
                    case "Ticket_Plant_Desc":
                        if (e.Value != null)
                        {
                            e.Item.PlantDescription = e.Value.ToString();
                        }
                        break;
                    case "Driver_Num":
                        if (e.Value != null)
                        {
                            e.Item.DriverNumber = e.Value.ToString();
                        }
                        break;
                    case "Driver_Desc":
                        if (e.Value != null)
                        {
                            e.Item.DriverDescription = e.Value.ToString();
                        }
                        break;
                    case "Driver_Type":
                        if (e.Value != null)
                        {
                            e.Item.DriverType = e.Value.ToString();
                        }
                        break;
                    case "Driver_Home_Plant_Num":
                        if (e.Value != null)
                        {
                            e.Item.DriveHomePlantNumber = e.Value.ToString();
                        }
                        break;
                    case "Truck_Num":
                        if (e.Value != null)
                        {
                            e.Item.TruckNumber = e.Value.ToString();
                        }
                        break;
                    case "Truck_Type":
                        if (e.Value != null)
                        {
                            e.Item.TruckType = e.Value.ToString();
                        }
                        break;
                    case "Truck_Home_Plant_Num":
                        if (e.Value != null)
                        {
                            e.Item.TruckHomePlantNumber = e.Value.ToString();
                        }
                        break;
                    case "Cust_Num":
                        if (e.Value != null)
                        {
                            e.Item.CustomerNumber = e.Value.ToString();
                        }
                        break;
                    case "Cust_Desc":
                        if (e.Value != null)
                        {
                            e.Item.CustomerDescription = e.Value.ToString();
                        }
                        break;
                    case "Cust_Seg_Num":
                        if (e.Value != null)
                        {
                            e.Item.CustomerSegmentNumber = e.Value.ToString();
                        }
                        break;
                    case "Cust_Seg_Desc":
                        if (e.Value != null)
                        {
                            e.Item.CustomerSegmentDesc = e.Value.ToString();
                        }
                        break;
                    case "Cust_City":
                        if (e.Value != null)
                        {
                            e.Item.CustomerCity = e.Value.ToString();
                        }
                        break;
                    case "Cust_Zip":
                        if (e.Value != null)
                        {
                            e.Item.CustomerZip = e.Value.ToString();
                        }
                        break;
                    case "Job_Num":
                        if (e.Value != null)
                        {
                            e.Item.JobNumber = e.Value.ToString();
                        }
                        break;
                    case "Job_Desc":
                        if (e.Value != null)
                        {
                            e.Item.JobDescription = e.Value.ToString();
                        }
                        break;
                    case "Job_Seg_Num":
                        if (e.Value != null)
                        {
                            e.Item.JobSegmentNumber = e.Value.ToString();
                        }
                        break;
                    case "Job_Seg_Desc":
                        if (e.Value != null)
                        {
                            e.Item.JobSegmentDescription = e.Value.ToString();
                        }
                        break;
                    case "Delv_Addr":
                        if (e.Value != null)
                        {
                            e.Item.DeliveryAddress = e.Value.ToString();
                        }
                        break;
                    case "Salesperson_Num":
                        if (e.Value != null)
                        {
                            e.Item.SalesPersonNumber = e.Value.ToString();
                        }
                        break;
                    case "Salesperson_Desc":
                        if (e.Value != null)
                        {
                            e.Item.SalesPersonDescription = e.Value.ToString();
                        }
                        break;
                    case "Batchman_Num":
                        if (e.Value != null)
                        {
                            e.Item.BatchmanNumber = e.Value.ToString();
                        }
                        break;
                    case "Batchman_Desc":
                        if (e.Value != null)
                        {
                            e.Item.BatchmanDescription = e.Value.ToString();
                        }
                        break;
                    case "Delivered_Volume":
                        if (e.Value != null)
                        {
                            e.Item.DeliveredVolume = Convert.ToDouble(e.Value);
                        }
                        break;
                    case "Total_Revenue":
                        if (e.Value != null)
                        {
                            e.Item.TotalRevenue = Convert.ToDecimal(e.Value);
                        }
                        break;
                    case "Material_Cost":
                        if (e.Value != null)
                        {
                            e.Item.MaterialCost = Convert.ToDecimal(e.Value);
                        }
                        break;
                }
            }
        }

        private void TicketImportData_RowImporting(object sender, ImportDataRowImportingEventArgs<TicketDetail> e)
        {
            if (!e.RowProcessedOnce)
            {
                if (e.Item == null)
                {
                    e.IgnoreRow = true;
                    return;
                }

                Console.WriteLine("TicketImportData_RowImporting save in db Ticket Number : " + e.Item.TicketNumber + ", TicketId : " + e.Item.TicketId);
                SIDAL.AddUpdateTicketDetail(e.Item);
            }
        }

        private void TicketImportData_RowImportingListCallBack(object sender, ImportDataRowListEventArgs<TicketDetail> e)
        {
            if (e.Items != null && e.Items.Count > 0)
            {
                Console.WriteLine("TicketImportData_RowImportingListCallBack save in db Current Ticket Count : " + e.Items.Count + ", Total Tickets Processed : " + e.RowsSucceeded);
                SIDAL.AddUpdateTicketDetailList(e.Items);
            }
        }

        #endregion

        public void AddOrUpdateDailyPlantSummaryStats(DailyPlantSummaryStats document)
        {
            var collection = GetCollection<DailyPlantSummaryStats>(DailyPlantSummaryTable);
            collection.ReplaceOne(x => x.Id == document.Id, document, new UpdateOptions { IsUpsert = true });
        }

        internal void ProcessMetricRequest(MongoQuery query)
        {
            BsonDocument match = new BsonDocument();
            var filters = DataSourceMeta.ScrubFilters(query.CollectionName, query.Filters);
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    switch (filter.ComparisionType)
                    {
                        case ComparisionType.In:
                            BsonArray array = new BsonArray();
                            foreach (var item in filter.Value) { array.Add(item); }
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$in", array } } } });
                            break;
                        case ComparisionType.Equals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, filter.Value } });
                            break;
                        case ComparisionType.GreaterOrEquals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gte", filter.Value } } } });
                            break;
                        case ComparisionType.GreaterThan:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gt", filter.Value } } } });
                            break;
                        case ComparisionType.LessOrEquals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$lte", filter.Value } } } });
                            break;
                        case ComparisionType.LessThan:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$lt", filter.Value } } } });
                            break;
                        case ComparisionType.Range:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gte", filter.Value }, { "$lte", filter.Value2 } } } });
                            break;
                    }
                }
            }

            BsonDocument group = new BsonDocument();
            if (query.AggregationList != null && query.AggregationList.Count > 0)
            {
                group.AddRange(new BsonDocument { { $"_id", BsonNull.Value } });
                foreach (var agg in query.AggregationList)
                {
                    if (agg.AggregationType == AggregationType.count)
                        group.AddRange(new BsonDocument { { GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString()), new BsonDocument { { "$sum", 1 } } } });
                    if (agg.AggregationType == AggregationType.sum_multiply)
                        group.AddRange(new BsonDocument { { GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString()), new BsonDocument { { "$sum", new BsonDocument { { "$multiply", new BsonArray(new string[] { $"${agg.ColumnName}", $"${agg.ColumnName2}" }) } } } } } });
                    else
                        group.AddRange(new BsonDocument { { GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString()), new BsonDocument { { $"${agg.AggregationType}", $"${agg.ColumnName}" } } } });

                }
            }

            var database = GetDatabase();
            var collection = database.GetCollection<BsonDocument>(query.CollectionName);

            var AggregateQuery = collection.Aggregate().Match(match).Group(group);

            string logMessage = $"ProcessMetricRequest CollectionName: {query.CollectionName}, Query : {AggregateQuery.ToString()}";

            var results = AggregateQuery.ToList();

            if (results != null && results.Any())
            {
                logMessage += $", {results.Count} records Found";
                foreach (var agg in query.AggregationList)
                {
                    var key = GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString());
                    var value = results[0][key];
                    agg.QueryValue = value;
                }
            }
            LogQueryRequest(logMessage);
        }

        internal List<TicketStats> GetRawData(MongoQuery query, out long count)
        {
            BsonDocument match = new BsonDocument();
            var filters = DataSourceMeta.ScrubFilters(query.CollectionName, query.Filters);
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    //text search currently support only full text search
                    if (filter.search != null)
                    {
                        match.AddRange(new BsonDocument { { "$text", new BsonDocument { { "$search", filter.search } } } });
                    }

                    switch (filter.ComparisionType)
                    {
                        case ComparisionType.In:
                            BsonArray array = new BsonArray();
                            foreach (var item in filter.Value) { array.Add(item); }
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$in", array } } } });
                            break;
                        case ComparisionType.Equals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, filter.Value } });
                            break;
                        case ComparisionType.GreaterOrEquals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gte", filter.Value } } } });
                            break;
                        case ComparisionType.GreaterThan:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gt", filter.Value } } } });
                            break;
                        case ComparisionType.LessOrEquals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$lte", filter.Value } } } });
                            break;
                        case ComparisionType.LessThan:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$lt", filter.Value } } } });
                            break;
                        case ComparisionType.Range:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gte", filter.Value }, { "$lte", filter.Value2 } } } });
                            break;
                    }

                }
            }


            var database = GetDatabase();
            var collection = database.GetCollection<TicketStats>(query.CollectionName);
            var aggregateOptions = new AggregateOptions { AllowDiskUse = true };//for sorting more than 100 MB
            var AggregateQuery = collection.Aggregate(aggregateOptions).Match(match);

            for (int i = 0; i < query.Filters.Count; i++)
            {
                if (query.Filters[i].order != null)
                {
                    if (query.Filters[i].order[0].IsDescending == "false")
                    {
                        AggregateQuery = AggregateQuery.AppendStage<TicketStats>(new BsonDocument { { "$sort", new BsonDocument { { query.Filters[i].order[0].SortBy, 1 } } } }).Skip(query.Filters[i].SkipRecords).Limit(query.Filters[i].ItemsPerPage);
                    }
                    else
                    {
                        AggregateQuery = AggregateQuery.AppendStage<TicketStats>(new BsonDocument { { "$sort", new BsonDocument { { query.Filters[i].order[0].SortBy, -1 } } } }).Skip(query.Filters[i].SkipRecords).Limit(query.Filters[i].ItemsPerPage);
                    }
                }

            }
            count = collection.Count(match);
            LogQueryRequest($"GetRawData CollectionName: {query.CollectionName}, Query : {AggregateQuery.ToString()}");
            return AggregateQuery.ToList();
        }

        public void LogQueryRequest(string queryRequest)
        {
            ILogger logger = new FileLogger();
            logger.LogInfo(queryRequest);

        }

        public void ProcessMetricGroupRequest(MongoGroupQuery query)
        {
            BsonDocument match = new BsonDocument();
            var filters = DataSourceMeta.ScrubFilters(query.CollectionName, query.Filters);
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    switch (filter.ComparisionType)
                    {
                        case ComparisionType.In:
                            BsonArray array = new BsonArray();
                            foreach (var item in filter.Value) { array.Add(item); }
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$in", array } } } });
                            break;
                        case ComparisionType.Equals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, filter.Value } });
                            break;
                        case ComparisionType.GreaterOrEquals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gte", filter.Value } } } });
                            break;
                        case ComparisionType.GreaterThan:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gt", filter.Value } } } });
                            break;
                        case ComparisionType.LessOrEquals:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$lte", filter.Value } } } });
                            break;
                        case ComparisionType.LessThan:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$lt", filter.Value } } } });
                            break;
                        case ComparisionType.Range:
                            match.AddRange(new BsonDocument { { filter.PropertyName, new BsonDocument { { "$gte", filter.Value }, { "$lte", filter.Value2 } } } });
                            break;

                    }

                }
            }

            BsonDocument group = new BsonDocument();
            if (query.AggregationList != null && query.AggregationList.Count > 0)
            {
                if (query.IsDateGroup)
                {
                    switch (query.Granularity)
                    {
                        case RepeatPeriod.YEARLY:
                            group.AddRange(new BsonDocument { { $"_id",
                                  new BsonDocument { { "year", new BsonDocument { { "$year", $"${query.ColumnProperty}" } } } }
                            } });
                            break;
                        case RepeatPeriod.MONTHLY:
                            group.AddRange(new BsonDocument { { $"_id",
                                  new BsonDocument { { "year", new BsonDocument { { "$year", $"${query.ColumnProperty}" } } }, { "month", new BsonDocument { { "$month", $"${query.ColumnProperty}" } } } }
                            } });
                            break;
                        case RepeatPeriod.WEEKLY:
                            group.AddRange(new BsonDocument { { $"_id",
                                  new BsonDocument { { "year", new BsonDocument { { "$year", $"${query.ColumnProperty}" } } }, { "week", new BsonDocument { { "$week", $"${query.ColumnProperty}" } } } }
                            } });
                            break;
                        case RepeatPeriod.DAILY:
                            group.AddRange(new BsonDocument { { $"_id", $"${query.ColumnProperty}" } });
                            break;
                    }
                }
                else
                {
                    group.AddRange(new BsonDocument { { $"_id", $"${query.ColumnProperty}" } });
                }

                foreach (var agg in query.AggregationList)
                {
                    if (agg.AggregationType == AggregationType.count)
                        group.AddRange(new BsonDocument { { GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString()), new BsonDocument { { "$sum", 1 } } } });
                    if (agg.AggregationType == AggregationType.sum_multiply)
                        group.AddRange(new BsonDocument { { GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString()), new BsonDocument { { "$sum", new BsonDocument { { "$multiply", new BsonArray(new string[] { $"${agg.ColumnName}", $"${agg.ColumnName2}" }) } } } } } });
                    else
                        group.AddRange(new BsonDocument { { GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString()), new BsonDocument { { $"${agg.AggregationType}", $"${agg.ColumnName}" } } } });
                }
            }

            var database = GetDatabase();
            var collection = database.GetCollection<BsonDocument>(query.CollectionName);

            var AggregateQuery = collection.Aggregate().Match(match).Group(group);
            string logMessage = $"ProcessMetricGroupRequest CollectionName: {query.CollectionName}, Group By: {query.ColumnProperty}, IsDateGroup: {query.IsDateGroup}, {(query.IsDateGroup ? $"Granularity: { query.Granularity.ToString()}, " : "")} Query : {AggregateQuery.ToString()}";



            for (int i = 0; i < query.Filters.Count; i++)
            {
                if (query.Filters[i].SortType == "percent" && query.Filters[i].ItemsPerPage > 0)
                {
                    var count = AggregateQuery.ToList().Count();
                    int value, percentRecords = 0;
                    value = query.Filters[i].ItemsPerPage;
                    percentRecords = value * count / 100;
                    if (percentRecords == 0)
                    {
                        query.Filters[i].ItemsPerPage = percentRecords + value;
                    }
                    else
                    {
                        query.Filters[i].ItemsPerPage = percentRecords;
                    }
                }
                if (query.Filters[i].order != null && query.Filters[i].ItemsPerPage > 0)
                {
                    if (query.Filters[i].order[0].IsDescending == "false")
                    {
                        AggregateQuery = AggregateQuery.AppendStage<BsonDocument>(new BsonDocument { { "$sort", new BsonDocument { { query.Filters[i].order[0].SortBy, 1 } } } }).Limit(filters[i].ItemsPerPage);
                    }
                    else
                    {
                        AggregateQuery = AggregateQuery.AppendStage<BsonDocument>(new BsonDocument { { "$sort", new BsonDocument { { query.Filters[i].order[0].SortBy, -1 } } } }).Limit(filters[i].ItemsPerPage);
                    }
                }
            }

            var results = AggregateQuery.ToList();
            if (results != null && results.Any())
            {
                logMessage += $", {results.Count} records Found";
                foreach (var agg in query.AggregationList)
                {
                    var key = GetAggregationKey(agg.MetricDefinition.MetricName, agg.AggregationType.ToString());
                    agg.BucketValues = new List<MetricGroupBucket>();
                    foreach (var item in results)
                    {
                        MetricGroupBucket bucket = new MetricGroupBucket();
                        if (query.IsDateGroup)
                        {
                            switch (query.Granularity)
                            {
                                case RepeatPeriod.YEARLY:
                                    bucket.GroupName = item["_id"].ToString();
                                    break;
                                case RepeatPeriod.MONTHLY:
                                    bucket.GroupName = new DateTime(Convert.ToInt32(item["_id"]["year"]), Convert.ToInt32(item["_id"]["month"]), 1);
                                    break;
                                case RepeatPeriod.WEEKLY:
                                    bucket.GroupName = DateUtils.FirstDateOfWeekISO8601(Convert.ToInt32(item["_id"]["year"]), Convert.ToInt32(item["_id"]["week"]));
                                    break;
                                case RepeatPeriod.DAILY:
                                    bucket.GroupName = Convert.ToDateTime(item["_id"]);
                                    break;
                            }
                        }
                        else
                        {
                            bucket.GroupName = item["_id"].ToString();
                        }
                        bucket.Value = item[key];
                        agg.BucketValues.Add(bucket);
                    }
                }
            }
            logMessage += $", :, { AggregateQuery.ToString()}";
            LogQueryRequest(logMessage);
        }

        public string GetAggregationKey(string metricName, string aggregationType)
        {
            return $"{metricName}_{aggregationType}";
        }
    }
}
