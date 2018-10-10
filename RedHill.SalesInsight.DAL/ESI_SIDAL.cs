using RedHill.SalesInsight.DAL.Constants;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Utils;
/**
*SIDAL file for ESI only
* */
namespace RedHill.SalesInsight.DAL
{
    public partial class SIDAL
    {
        #region Ticket and Driver Details Upload

        public static void CleanTicketDataNew()
        {
            List<TicketDetail> unScrubbedTickets = null;

            List<string> drivers = GetDriversFromTickets();
            if (drivers != null && drivers.Count > 0)
            {
                foreach (var driverNumber in drivers)
                {
                    unScrubbedTickets = GetTicketsByDriver(driverNumber);

                    if (unScrubbedTickets != null && unScrubbedTickets.Any())
                    {
                        foreach (var ticket in unScrubbedTickets)
                        {
                            #region Clean Date Values
                            List<DateTime?> listOfDates = new List<DateTime?>();
                            listOfDates.Add(ticket.TimeDueOnJob);
                            listOfDates.Add(ticket.TimeTicketed);
                            listOfDates.Add(ticket.TimeBeginLoad);
                            listOfDates.Add(ticket.TimeEndLoad);
                            listOfDates.Add(ticket.TimeLeavePlant);
                            listOfDates.Add(ticket.TimeArriveJob);
                            listOfDates.Add(ticket.TimeBeginUnload);
                            listOfDates.Add(ticket.TimeEndUnload);
                            listOfDates.Add(ticket.TimeLeaveJob);
                            listOfDates.Add(ticket.TimeArrivePlant);

                            ticket.HasInvalidData = DateUtils.SanitizeDates(listOfDates, ticket.TicketDate.Value);
                            int i = 0;
                            ticket.TimeDueOnJob = listOfDates[i++];
                            ticket.TimeTicketed = listOfDates[i++];
                            ticket.TimeBeginLoad = listOfDates[i++];
                            ticket.TimeEndLoad = listOfDates[i++];
                            ticket.TimeLeavePlant = listOfDates[i++];
                            ticket.TimeArriveJob = listOfDates[i++];
                            ticket.TimeBeginUnload = listOfDates[i++];
                            ticket.TimeEndUnload = listOfDates[i++];
                            ticket.TimeLeaveJob = listOfDates[i++];
                            ticket.TimeArrivePlant = listOfDates[i++];
                            ticket.IsScrubbed = true;
                            #endregion
                        }
                        foreach (var ticket in unScrubbedTickets)
                        {
                            #region Calculate InYard Time

                            var precedingTicket = unScrubbedTickets.Where(x => x.DriverNumber == ticket.DriverNumber && x.TimeTicketed < ticket.TimeTicketed).OrderByDescending(x => x.TimeTicketed).FirstOrDefault();

                            if (precedingTicket != null && ticket.TimeTicketed.HasValue && precedingTicket.TimeArrivePlant.HasValue)
                            {
                                ticket.InYardTime = (ticket.TimeTicketed.Value - precedingTicket.TimeArrivePlant.Value).TotalMinutes;
                            }
                            if (!ticket.InYardTime.HasValue || ticket.InYardTime.Value < 0 || ticket.InYardTime.Value > 240)
                                ticket.InYardTime = 0;
                            #endregion
                        }
                        BulkInsertList("TicketDetails_1", unScrubbedTickets.AsDataTable());
                    }
                }
            }
        }

        public static List<string> GetDriversFromTickets()
        {
            using (var context = GetContext())
            {
                return context.TicketDetails.Select(x => x.DriverNumber).Distinct().ToList();
            }
        }
        public static void CleanDriverLoginTimesNew()
        {
            List<string> drivers = GetDriversFromTickets();
            List<Plant> plants = GetPlantsList();

            List<TicketDetail> unScrubbedTickets = null;
            List<DriverLoginTime> unScrubbedDriverLogins = null;
            if (drivers != null && drivers.Count > 0)
            {
                foreach (var driverNumber in drivers)
                {
                    unScrubbedTickets = GetTicketsByDriver(driverNumber);
                    using (var context = GetContext())
                    {
                        unScrubbedDriverLogins = context.DriverLoginTimes.Where(x => x.DriverNumber == driverNumber).OrderBy(x => x.LoginDate).ToList();
                    }
                    if (unScrubbedDriverLogins != null && unScrubbedDriverLogins.Any())
                    {
                        foreach (var driverLogin in unScrubbedDriverLogins)
                        {
                            var invalidPunchIn = !driverLogin.PunchInTime.HasValue || Math.Abs((driverLogin.LoginDate - driverLogin.PunchInTime.Value).TotalDays) > 2;

                            var invalidPunchOut = !driverLogin.PunchOutTime.HasValue || Math.Abs((driverLogin.LoginDate - driverLogin.PunchOutTime.Value).TotalDays) > 2;

                            if (invalidPunchIn || invalidPunchOut || driverLogin.PunchInTime.Value >= driverLogin.PunchOutTime.Value)
                            {
                                driverLogin.HasInvalidData = true;
                            }
                            else
                            {
                                var tickets = unScrubbedTickets.Where(x => x.DriverNumber == driverLogin.DriverNumber && x.TimeTicketed >= driverLogin.PunchInTime.Value.AddMinutes(-30) && x.TimeTicketed <= driverLogin.PunchOutTime.Value).OrderBy(x => x.TimeTicketed).ToList();

                                if (tickets.Any())
                                {
                                    var startupTime = DateUtils.SubstractDates(driverLogin.PunchInTime.GetValueOrDefault(), tickets.First().TimeTicketed.GetValueOrDefault());
                                    if (startupTime <= 0 || startupTime > 240)
                                        startupTime = 22.22;

                                    var shutdownTime = DateUtils.SubstractDates(tickets.Last().TimeArrivePlant.GetValueOrDefault(), driverLogin.PunchOutTime.GetValueOrDefault());
                                    if (shutdownTime <= 0 || shutdownTime > 240)
                                        shutdownTime = 22.22;


                                    var totalActualTicketTime = tickets.Sum(x => DateUtils.SubstractDates(x.TimeTicketed.GetValueOrDefault(), x.TimeArrivePlant.GetValueOrDefault()));
                                    var totalLoginTime = driverLogin.TotalTime.GetValueOrDefault();
                                    double utilizationFactor = 0;
                                    if (totalActualTicketTime > 0)
                                        utilizationFactor = totalLoginTime / totalActualTicketTime;
                                    if (utilizationFactor < 1)// Fetch Utilisation factor from Plant
                                        utilizationFactor = GetInverseUtilisationFactorByPlant(plants.FirstOrDefault(x => x.DispatchId == tickets.First().PlantNumber));

                                    foreach (var ticketDetail in tickets)
                                    {
                                        ticketDetail.StartupTime = startupTime;
                                        ticketDetail.ShutdownTime = shutdownTime;
                                        ticketDetail.DriverLoginTimeId = driverLogin.Id;
                                        var totalTime = DateUtils.SubstractDates(ticketDetail.TimeTicketed.GetValueOrDefault(), ticketDetail.TimeArrivePlant.GetValueOrDefault());
                                        ticketDetail.EstimatedClockHours = totalTime * utilizationFactor;
                                        ticketDetail.IsProcessed = true;
                                    }
                                }
                                driverLogin.IsProcessed = true;
                            }
                            driverLogin.IsScrubbed = true;
                        }
                    }
                    if (unScrubbedTickets != null && unScrubbedTickets.Any())
                    {
                        BulkInsertList("TicketDetails_1", unScrubbedTickets.AsDataTable());
                    }
                }
            }

        }
        public static void TicketBulkInsertCleanup()
        {
            using (var context = GetContext())
            {

                context.ExecuteCommand("select *  INTO TicketDetails_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + " from TicketDetails");
                context.ExecuteCommand("Truncate table TicketDetails");
                context.SubmitChanges();

                context.ExecuteCommand("INSERT INTO [dbo].[TicketDetails]([TicketId],[TicketNumber],[TicketDate],[VoidStatus],[TimeDueOnJob],[TimeTicketed],[TimeBeginLoad],[TimeEndLoad],[TimeLeavePlant],[TimeArriveJob],[TimeBeginUnload],[TimeEndUnload],[TimeLeaveJob],[TimeArrivePlant],[PlantNumber],[PlantDescription],[FOB],[DriverNumber],[DriverDescription],[DriverType],[DriveHomePlantNumber],[TruckNumber],[TruckType],[TruckHomePlantNumber],[CustomerNumber],[CustomerDescription],[CustomerSegmentNumber],[CustomerSegmentDesc],[CustomerCity],[CustomerZip],[JobNumber],[JobDescription],[JobSegmentNumber],[JobSegmentDescription],[DeliveryAddress],[SalesPersonNumber],[SalesPersonDescription],[BatchmanNumber],[BatchmanDescription],[DeliveredVolume],[TotalRevenue],[MaterialCost],[IsMongoUpdated],[IsScrubbed],[IsProcessed],[HasInvalidData],[DriverLoginTimeId],[StartupTime],[ShutdownTime],[InYardTime],[EstimatedClockHours])"
+ "select[TicketId],[TicketNumber],[TicketDate],[VoidStatus],[TimeDueOnJob],[TimeTicketed],[TimeBeginLoad],[TimeEndLoad],[TimeLeavePlant],[TimeArriveJob],[TimeBeginUnload],[TimeEndUnload],[TimeLeaveJob],[TimeArrivePlant],[PlantNumber],[PlantDescription],[FOB],[DriverNumber],[DriverDescription],[DriverType],[DriveHomePlantNumber],[TruckNumber],[TruckType],[TruckHomePlantNumber],[CustomerNumber],[CustomerDescription],[CustomerSegmentNumber],[CustomerSegmentDesc],[CustomerCity],[CustomerZip],[JobNumber],[JobDescription],[JobSegmentNumber],[JobSegmentDescription],[DeliveryAddress],[SalesPersonNumber],[SalesPersonDescription],[BatchmanNumber],[BatchmanDescription],[DeliveredVolume],[TotalRevenue],[MaterialCost],[IsMongoUpdated],[IsScrubbed],[IsProcessed],[HasInvalidData],[DriverLoginTimeId],[StartupTime],[ShutdownTime],[InYardTime],[EstimatedClockHours] from TicketDetails_1");
                context.SubmitChanges();

                context.ExecuteCommand("Truncate table TicketDetails_1");
                context.SubmitChanges();

            }
        }
        public static void UpdateTicketDetailsAsIsMongoUpdated(string driverNumber)
        {
            using (var context = GetContext())
            {

                context.ExecuteCommand("UPDATE TicketDetails SET IsMongoUpdated = 1 WHERE DriverNumber = '" + driverNumber + "'");
                context.SubmitChanges();

            }
        }
        public static void UpdateDriverLoginTimesAsScrubbed()
        {
            using (var context = GetContext())
            {

                context.ExecuteCommand("Update DriverLoginTimes set IsProcessed = 1 where HasInvalidData = 0");
                context.ExecuteCommand("Update DriverLoginTimes set IsScrubbed = 1");
                context.SubmitChanges();
            }
        }

        public static void CleanTicketData(int year)
        {
            using (var context = GetContext())
            {
                var drivers = context.TicketDetails.Select(x => x.DriverNumber).Distinct().ToList();
                if (drivers != null && drivers.Count > 0)
                {
                    foreach (var driverNumber in drivers)
                    {
                        var unScrubbedTickets = context.TicketDetails.Where(x => x.DriverNumber == driverNumber && x.IsScrubbed == false && x.TicketDate.Value.Year == year).OrderBy(x => x.DriverNumber).ThenBy(x => x.TimeTicketed);
                        if (unScrubbedTickets != null && unScrubbedTickets.Any())
                        {
                            foreach (var ticket in unScrubbedTickets)
                            {
                                #region Clean Date Values
                                List<DateTime?> listOfDates = new List<DateTime?>();
                                listOfDates.Add(ticket.TimeDueOnJob);
                                listOfDates.Add(ticket.TimeTicketed);
                                listOfDates.Add(ticket.TimeBeginLoad);
                                listOfDates.Add(ticket.TimeEndLoad);
                                listOfDates.Add(ticket.TimeLeavePlant);
                                listOfDates.Add(ticket.TimeArriveJob);
                                listOfDates.Add(ticket.TimeBeginUnload);
                                listOfDates.Add(ticket.TimeEndUnload);
                                listOfDates.Add(ticket.TimeLeaveJob);
                                listOfDates.Add(ticket.TimeArrivePlant);

                                ticket.HasInvalidData = DateUtils.SanitizeDates(listOfDates, ticket.TicketDate.Value);
                                int i = 0;
                                ticket.TimeDueOnJob = listOfDates[i++];
                                ticket.TimeTicketed = listOfDates[i++];
                                ticket.TimeBeginLoad = listOfDates[i++];
                                ticket.TimeEndLoad = listOfDates[i++];
                                ticket.TimeLeavePlant = listOfDates[i++];
                                ticket.TimeArriveJob = listOfDates[i++];
                                ticket.TimeBeginUnload = listOfDates[i++];
                                ticket.TimeEndUnload = listOfDates[i++];
                                ticket.TimeLeaveJob = listOfDates[i++];
                                ticket.TimeArrivePlant = listOfDates[i++];
                                ticket.IsScrubbed = true;
                                #endregion
                            }
                            context.SubmitChanges();

                            foreach (var ticket in unScrubbedTickets)
                            {
                                #region Calculate InYard Time

                                var precedingTicket = context.TicketDetails.Where(x => x.DriverNumber == ticket.DriverNumber && x.TimeTicketed < ticket.TimeTicketed).OrderByDescending(x => x.TimeTicketed).FirstOrDefault();

                                if (precedingTicket != null && ticket.TimeTicketed.HasValue && precedingTicket.TimeArrivePlant.HasValue)
                                {
                                    ticket.InYardTime = (ticket.TimeTicketed.Value - precedingTicket.TimeArrivePlant.Value).TotalMinutes;
                                }
                                if (!ticket.InYardTime.HasValue || ticket.InYardTime.Value < 0 || ticket.InYardTime.Value > 240)
                                    ticket.InYardTime = 0;
                                #endregion
                            }
                            context.SubmitChanges();
                        }
                    }
                }
            }
        }

        public static void CleanDriverLoginTimes(int year)
        {
            using (var context = GetContext())
            {
                bool continueNext = true;
                while (continueNext)
                {
                    var plants = context.Plants.ToList();
                    var unScrubbedDriverLogins = context.DriverLoginTimes.Where(x => x.IsProcessed == false && x.LoginDate.Year == year).OrderBy(x => x.DriverNumber).ThenBy(x => x.LoginDate).Take(1000);
                    if (unScrubbedDriverLogins != null && unScrubbedDriverLogins.Any())
                    {
                        foreach (var driverLogin in unScrubbedDriverLogins)
                        {
                            var invalidPunchIn = !driverLogin.PunchInTime.HasValue || Math.Abs((driverLogin.LoginDate - driverLogin.PunchInTime.Value).TotalDays) > 2;

                            var invalidPunchOut = !driverLogin.PunchOutTime.HasValue || Math.Abs((driverLogin.LoginDate - driverLogin.PunchOutTime.Value).TotalDays) > 2;

                            if (invalidPunchIn || invalidPunchOut || driverLogin.PunchInTime.Value >= driverLogin.PunchOutTime.Value)
                            {
                                driverLogin.HasInvalidData = true;
                            }
                            else
                            {
                                var tickets = context.TicketDetails.Where(x => x.DriverNumber == driverLogin.DriverNumber && x.TimeTicketed >= driverLogin.PunchInTime.Value.AddMinutes(-30) && x.TimeTicketed <= driverLogin.PunchOutTime.Value).OrderBy(x => x.TimeTicketed).ToList();

                                if (tickets.Any())
                                {
                                    var startupTime = DateUtils.SubstractDates(driverLogin.PunchInTime.GetValueOrDefault(), tickets.First().TimeTicketed.GetValueOrDefault());
                                    if (startupTime <= 0 || startupTime > 240)
                                        startupTime = 22.22;

                                    var shutdownTime = DateUtils.SubstractDates(tickets.Last().TimeArrivePlant.GetValueOrDefault(), driverLogin.PunchOutTime.GetValueOrDefault());
                                    if (shutdownTime <= 0 || shutdownTime > 240)
                                        shutdownTime = 22.22;


                                    var totalActualTicketTime = tickets.Sum(x => DateUtils.SubstractDates(x.TimeTicketed.GetValueOrDefault(), x.TimeArrivePlant.GetValueOrDefault()));
                                    var totalLoginTime = driverLogin.TotalTime.GetValueOrDefault();
                                    double utilizationFactor = 0;
                                    if (totalActualTicketTime > 0)
                                        utilizationFactor = totalLoginTime / totalActualTicketTime;
                                    if (utilizationFactor < 1)// Fetch Utilisation factor from Plant
                                        utilizationFactor = GetInverseUtilisationFactorByPlant(plants.FirstOrDefault(x => x.DispatchId == tickets.First().PlantNumber));

                                    foreach (var ticketDetail in tickets)
                                    {
                                        ticketDetail.StartupTime = startupTime;
                                        ticketDetail.ShutdownTime = shutdownTime;
                                        ticketDetail.DriverLoginTimeId = driverLogin.Id;
                                        var totalTime = DateUtils.SubstractDates(ticketDetail.TimeTicketed.GetValueOrDefault(), ticketDetail.TimeArrivePlant.GetValueOrDefault());
                                        ticketDetail.EstimatedClockHours = totalTime * utilizationFactor;
                                        ticketDetail.IsProcessed = true;
                                    }
                                }
                                driverLogin.IsProcessed = true;
                            }
                            driverLogin.IsScrubbed = true;
                            context.SubmitChanges();
                        }
                    }
                    else
                        continueNext = false;
                }
            }
        }
        public static void ProcessOrphanTickets()
        {
            using (var context = GetContext())
            {
                bool continueNext = true;
                while (continueNext)
                {
                    var plants = context.Plants.ToList();
                    var orphanTickets = context.TicketDetails.Where(x => x.IsProcessed == false).OrderBy(x => x.DriverNumber).ThenBy(x => x.TimeTicketed).Take(20000);
                    if (orphanTickets != null && orphanTickets.Any())
                    {
                        foreach (var ticket in orphanTickets)
                        {
                            ticket.StartupTime = 22.2;
                            ticket.ShutdownTime = 22.2;
                            double utilizationFactor = GetInverseUtilisationFactorByPlant(plants.FirstOrDefault(x => x.DispatchId == ticket.PlantNumber));
                            var totalTime = DateUtils.SubstractDates(ticket.TimeTicketed.GetValueOrDefault(), ticket.TimeArrivePlant.GetValueOrDefault());
                            ticket.EstimatedClockHours = totalTime * utilizationFactor;
                            ticket.IsProcessed = true;
                            ticket.HasInvalidData = true;
                        }
                        context.SubmitChanges();
                    }
                    else
                        continueNext = false;
                }
            }
        }

        public static double GetInverseUtilisationFactorByPlant(Plant plant)
        {
            double utilizationFactor = 1;
            if (plant != null && plant.Utilization.HasValue && plant.Utilization.Value > 0)
            {
                utilizationFactor = Convert.ToDouble(1 / plant.Utilization.GetValueOrDefault());
            }
            return utilizationFactor;
        }
        #endregion

        public static List<MetricDefinition> MetricDefinitions = null;

        public static MetricDefinition GetMetricDefinition(int id)
        {
            using (var context = GetContext())
            {
                if (MetricDefinitions == null)
                {
                    MetricDefinitions = context.MetricDefinitions.ToList();
                }
                if (MetricDefinitions != null)
                {
                    return MetricDefinitions.FirstOrDefault(x => x.Id == id);
                }
                return context.MetricDefinitions.FirstOrDefault(x => x.Id == id);
            }
        }

        public static MetricDefinition GetMetricDefinitionByName(string metricName)
        {
            using (var context = GetContext())
            {
                return context.MetricDefinitions.FirstOrDefault(x => metricName == x.MetricName);
            }
        }

        public static List<MetricDefinition> GetMetricDefinitions(string[] metricNames)
        {
            using (var context = GetContext())
            {
                return context.MetricDefinitions.Where(x => metricNames.Contains(x.MetricName)).ToList();
            }
        }

        #region Generate PlantBudgets



        #endregion

        /// <summary>
        /// Get Widget Start-End Date
        /// </summary>
        /// <param name="wId"></param>
        /// <returns></returns>
        public static DateTime[] GetWidgetStartEndDate(long? wId, string reportType)
        {
            DateTime[] startEnd = null;
            using (var context = GetContext())
            {
                if (wId != null && wId != 0)
                {
                    var widget = context.WidgetSettings.Where(x => x.WidgetId == wId).FirstOrDefault();
                    if (widget != null)
                    {
                        var startDate = context.DashboardSettings.Where(x => x.Id == widget.DashboardId).FirstOrDefault().StartDate;

                        if (reportType == ESIReportType.GOAL_ANALYSIS)
                        {
                            startEnd = new DateTime[2];
                            startEnd[0] = startDate.GetValueOrDefault();
                            startEnd[1] = startDate.GetValueOrDefault();
                        }
                        else
                        {
                            startEnd = DateUtils.GetStartAndEndDateForPeriodType(widget.PrimaryMetricPeriod, startDate.GetValueOrDefault(DateTime.Today));
                        }
                    }
                }
            }
            return startEnd;
        }
    }
}
