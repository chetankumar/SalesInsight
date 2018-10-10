using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL;
using System.Web.Security;
using RedHill.SalesInsight.Web.Html5.Models.ESI;
using Redhill.SalesInsight.ESI.ReportModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Redhill.SalesInsight.ESI;
using RedHill.SalesInsight.DAL.Constants;
using RedHill.SalesInsight.Web.Html5.Models.ESI.Payload;
using System.Text;
using RedHill.SalesInsight.DAL.Mongo.Models;
using RedHill.SalesInsight.Web.Html5.Models.JsonModels;
using RedHill.SalesInsight.Web.Html5.Models.ESI.POCO;
using System.Linq;
using RedHill.SalesInsight.Web.Html5.Helpers;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class ESIController : BaseController
    {
        //
        // GET: /ESI/
        public EsiReportManager reportManager = null;

        public ESIController()
        {
            reportManager = new EsiReportManager();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public ActionResult Dashboard(long Id = 0, bool isNewDashboard = false, bool persistFilter = false)
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            DashboardSettingsView boardSetting = new DashboardSettingsView();
            UserDashboardSetting userDashSetting = SIDAL.GetDashboardSetting(userId, Id);
            DashboardSetting dashboardSet = SIDAL.GetUserDashboardSetting(userId, Id, userDashSetting);
            ViewBag.IsNewDashboard = isNewDashboard;
            ViewBag.DashboardList = SIDAL.GetDashboardSettingGroupWise(userId);
            ViewBag.IsAdmin = RoleAccess.IsAdmin;

            if (dashboardSet != null)
            {
                if (!persistFilter)
                {
                    Session["DashboardFilters"] = null;
                    Session["DashboardStartDate"] = null;
                }
                boardSetting = new DashboardSettingsView(dashboardSet, (DashboardFilterSettingView)Session["DashboardFilters"], userId, userDashSetting, (DateTime?)Session["DashboardStartDate"]);

                Session["DashboardFilters"] = boardSetting.DashboardFilter;
            }
            return View(boardSetting);
        }

        [HttpPost]
        public ActionResult Dashboard(WidgetSettingsView widgetSetting)
        {
            WidgetSetting widSet = new WidgetSetting();
            widSet.WidgetId = widgetSetting.WidgetId;
            widSet.DashboardId = widgetSetting.DashboardId;
            widSet.Title = widgetSetting.Title;
            widSet.MetricType = widgetSetting.MetricType;
            widSet.DecimalPlaces = widgetSetting.DecimalPlaces;
            widSet.BackgroundColor = widgetSetting.BackgroundColor;
            widSet.PrimaryMetricDefinitionId = widgetSetting.PrimaryMetricDefinitionId;
            widSet.PrimaryMetricPeriod = widgetSetting.PrimaryMetricPeriod;
            widSet.ComparisonMetricDefinitionId = widgetSetting.ComparisonMetricDefinitionId;
            widSet.ComparisonMetricPeriod = widgetSetting.ComparisonMetricPeriod;
            widSet.ShowActionIcon = widgetSetting.ShowActionIcon;
            widSet.SuccessLimitPercent = widgetSetting.SuccessLimitPercent;
            widSet.AlertLimitPercent = widgetSetting.AlertLimitPercent;
            widSet.GoalAnalysisReportId = widgetSetting.GoalAnalysisReportId;
            widSet.TrendAnalysisReportId = widgetSetting.TrendAnalysisReportId;
            widSet.BenchmarkReportId = widgetSetting.BenchmarkReportId;
            widSet.DrillInReportId = widgetSetting.DrillInReportId;
            widSet.Position = widgetSetting.Position;
            widgetSetting.VisualIndicationType = widgetSetting.VisualIndicationType.ToLower();
            switch (widgetSetting.VisualIndicationType)
            {
                case "hasbargraph":
                    widSet.HasBarGraph = true;
                    widSet.BarGraphDaysPerBar = widgetSetting.BarGraphDaysPerBar;
                    break;
                case "hasfrequencydistribution":
                    widSet.HasFrequencyDistribution = true;
                    break;
                case "haslinegraph":
                    widSet.HasLineGraph = true;
                    widSet.LineGraphRangeInDays = widgetSetting.LineGraphRangeInDays;
                    break;
                case "haspopsummary":
                    widSet.HasPOPSummary = true;
                    break;
                case "hasstaticmessage":
                    widSet.HasStaticMessage = true;
                    widSet.StaticMessage = widgetSetting.StaticMessage;
                    break;
            }

            SIDAL.AddUpdateWidget(widSet);
            return RedirectToAction("Dashboard", new { @Id = widSet.DashboardId });
        }

        [HttpPost]
        public ActionResult UpdateWidgetDetails(WidgetSettingsView widgetSetting)
        {
            JsonResponse res = new JsonResponse();
            try
            {
                WidgetSetting widSet = new WidgetSetting();
                widSet.WidgetId = widgetSetting.WidgetId;
                widSet.DashboardId = widgetSetting.DashboardId;
                widSet.Title = widgetSetting.Title;
                widSet.MetricType = widgetSetting.MetricType;
                widSet.DecimalPlaces = widgetSetting.DecimalPlaces;
                widSet.BackgroundColor = widgetSetting.BackgroundColor;
                widSet.PrimaryMetricDefinitionId = widgetSetting.PrimaryMetricDefinitionId;
                widSet.PrimaryMetricPeriod = widgetSetting.PrimaryMetricPeriod;
                widSet.ComparisonMetricDefinitionId = widgetSetting.ComparisonMetricDefinitionId;
                widSet.ComparisonMetricPeriod = widgetSetting.ComparisonMetricPeriod;
                widSet.ShowActionIcon = widgetSetting.ShowActionIcon;
                widSet.SuccessLimitPercent = widgetSetting.SuccessLimitPercent;
                widSet.AlertLimitPercent = widgetSetting.AlertLimitPercent;
                widSet.GoalAnalysisReportId = widgetSetting.GoalAnalysisReportId;
                widSet.TrendAnalysisReportId = widgetSetting.TrendAnalysisReportId;
                widSet.BenchmarkReportId = widgetSetting.BenchmarkReportId;
                widSet.DrillInReportId = widgetSetting.DrillInReportId;
                widSet.Position = widgetSetting.Position;
                widgetSetting.VisualIndicationType = widgetSetting.VisualIndicationType.ToLower();
                switch (widgetSetting.VisualIndicationType)
                {
                    case "hasbargraph":
                        widSet.HasBarGraph = true;
                        widSet.BarGraphDaysPerBar = widgetSetting.BarGraphDaysPerBar;
                        break;
                    case "hasfrequencydistribution":
                        widSet.HasFrequencyDistribution = true;
                        break;
                    case "haslinegraph":
                        widSet.HasLineGraph = true;
                        widSet.LineGraphRangeInDays = widgetSetting.LineGraphRangeInDays;
                        break;
                    case "haspopsummary":
                        widSet.HasPOPSummary = true;
                        break;
                    case "hasstaticmessage":
                        widSet.HasStaticMessage = true;
                        widSet.StaticMessage = widgetSetting.StaticMessage;
                        break;
                }

                SIDAL.AddUpdateWidget(widSet);
                res.Success = true;
                res.Data = new JObject();
                res.Data.WidgetId = widSet.WidgetId;
                res.Data.Title = widSet.Title;
                res.Data.BackgroundColor = widSet.BackgroundColor;
                res.Data.GoalAnalysisReportId = widSet.GoalAnalysisReportId;
                res.Data.BenchmarkAnalysisReportId = widSet.BenchmarkReportId;
                res.Data.TrendAnalysisReportId = widSet.TrendAnalysisReportId;
                res.Data.DrillInReportId = widSet.DrillInReportId;

                res.Message = "Widget details updated successfully";
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
            }
            return Json(res.ToString());
        }

        public ActionResult DeleteDashboard(long Id)
        {
            var userId = GetUser().UserId;
            try
            {
                SIDAL.DeleteDashboard(Id, userId);
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Dashboard");
        }

        private Guid UserId
        {
            get
            {
                Guid userId = Guid.Empty;
                if (Session["UserId"] != null)
                {
                    userId = (Guid)Session["UserId"];
                }
                else
                {
                    userId = GetUser().UserId;
                    Session["UserId"] = userId;
                }
                return userId;
            }
        }

        public ActionResult GetWidgetData(long widgetId, long dashboardId, DateTime? date = null)
        {
            //TODO: This can be changed. Added for handling of date if passed null
            date = date ?? new DateTime(2016, 8, 31);
            WidgetResponsePayload payload = new WidgetResponsePayload(widgetId, date);

            var filters = (DashboardFilterSettingView)Session["DashboardFilters"];

            if (filters == null)
            {
                try
                {
                    filters = new DashboardFilterSettingView(SIDAL.GetDashboardFilterSetting(dashboardId), UserId);
                    Session["DashboardFilters"] = filters;
                }
                catch { }
            }
            payload.Filters = filters;

            return Content(payload.GetResponse(), "application/json");
        }

        [HttpPost]
        public ActionResult UpdateWidgetOrder(long dashboardId, IEnumerable<WidgetOrder> order)
        {
            JsonResponse response = new JsonResponse();

            try
            {
                if (order != null)
                {
                    Dictionary<long, int> widgetOrder = new Dictionary<long, int>();
                    foreach (var wo in order)
                    {
                        widgetOrder.Add(wo.WidgetId, wo.Position);
                    }
                    SIDAL.UpdateWidgetOrder(dashboardId, widgetOrder);
                }

                response.Success = true;
                response.Message = "Widget order saved successfully";
            }
            catch
            {
                response.Message = "Could not save the widget order";
                throw;
            }

            return Json(response.ToString());
        }

        [HttpPost]
        public ActionResult DeleteWidget(long widgetId)
        {
            var deleteStaus = false;
            try
            {
                deleteStaus = SIDAL.DeleteDashboardWidget(widgetId);
            }
            catch (Exception ex)
            {
            }
            return Content(deleteStaus.ToString());
        }

        [HttpPost]
        public ActionResult GetWidgetDetails(long widgetId)
        {
            WidgetSetting widgetSetting = SIDAL.GetWidgetSettings(widgetId);
            JObject o = new JObject();
            o["widgetId"] = widgetSetting.WidgetId;
            o["title"] = widgetSetting.Title;
            o["metricType"] = widgetSetting.MetricType;
            o["decimalPlaces"] = widgetSetting.DecimalPlaces;
            o["backgroundColor"] = widgetSetting.BackgroundColor;
            o["primaryMetric"] = widgetSetting.PrimaryMetric;
            o["primaryMetricPeriod"] = widgetSetting.PrimaryMetricPeriod;
            o["comparisonMetric"] = widgetSetting.ComparisonMetric;
            o["comparisonMetricPeriod"] = widgetSetting.ComparisonMetricPeriod;
            o["showActionIcon"] = widgetSetting.ShowActionIcon;
            o["successLimitPercent"] = widgetSetting.SuccessLimitPercent;
            o["alertLimitPercent"] = widgetSetting.AlertLimitPercent;
            o["hasBarGraph"] = widgetSetting.HasBarGraph;
            o["barGraphDaysPerBar"] = widgetSetting.BarGraphDaysPerBar;
            o["hasFrequencyDistribution"] = widgetSetting.HasFrequencyDistribution;
            o["hasLineGraph"] = widgetSetting.HasLineGraph;
            o["lineGraphRangeInDays"] = widgetSetting.LineGraphRangeInDays;
            o["hasPOPSummary"] = widgetSetting.HasPOPSummary;
            o["hasStaticMessage"] = widgetSetting.HasStaticMessage;
            o["staticMessage"] = widgetSetting.StaticMessage;
            o["goalAnalysisReportId"] = widgetSetting.GoalAnalysisReportId;
            o["trendAnalysisReportId"] = widgetSetting.TrendAnalysisReportId;
            o["benchmarkReportId"] = widgetSetting.BenchmarkReportId;
            o["drillInReportId"] = widgetSetting.DrillInReportId;
            o["primaryMetricDefinitionId"] = widgetSetting.PrimaryMetricDefinitionId;
            o["comparisonMetricDefinitionId"] = widgetSetting.ComparisonMetricDefinitionId;
            o["position"] = widgetSetting.Position;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GoalAnalysis(bool? persistFilter, long? wId = 0, long Id = 0, bool isNewReport = false)
        {
            Guid userId = GetUser().UserId;
            ViewBag.IsAdmin = RoleAccess.IsAdmin;
            GoalAnalysisReportView reportModel = new GoalAnalysisReportView();
            var reportType = ESIReportType.GOAL_ANALYSIS;
            UserReportSetting userReportSetting = SIDAL.GetReportSetting(userId, reportType, Id);
            ReportSetting report = SIDAL.GetUserReport(userId, reportType, Id, userReportSetting);
            ViewBag.ReportType = reportType;
            if (report != null)
                reportModel = new GoalAnalysisReportView(report, userId, persistFilter, wId, (ReportFilterSettingView)Session["ReportFilterSetting"], userReportSetting, (DateTime?)Session["ReportStartDate"], (DateTime?)Session["ReportEndDate"]);

            reportModel.LoadData();
            ViewBag.SavedReports = SIDAL.GetSavedReports(GetUser().UserId, reportType);
            ViewBag.IsNewReport = isNewReport;
            Session["ReportFilterSettings"] = reportModel.ReportFilterSetting;
            return View(reportModel);
        }

        [HttpPost]
        public ActionResult GoalAnalysis(GoalAnalysisReportView reportModel)
        {
            Guid userId = GetUser().UserId;

            var reportType = ESIReportType.GOAL_ANALYSIS;

            ViewBag.ReportType = reportType;

            reportModel.LoadData();

            ViewBag.SavedReports = SIDAL.GetSavedReports(GetUser().UserId, reportType);

            return View(reportModel);
        }

        public ActionResult BenchmarkAnalysis(bool? persistFilter, long? wId = 0, long Id = 0, bool isNewReport = false)
        {
            Guid userId = GetUser().UserId;
            ViewBag.IsAdmin = RoleAccess.IsAdmin;
            BenchmarkAnalysisReportView reportModel = new BenchmarkAnalysisReportView();
            var reportType = ESIReportType.BENCHMARK_ANALYSIS;
            UserReportSetting userReportSetting = SIDAL.GetReportSetting(userId, reportType, Id);
            ReportSetting report = SIDAL.GetUserReport(userId, reportType, Id, userReportSetting);
            ViewBag.ReportType = reportType;
            if (report != null)
                reportModel = new BenchmarkAnalysisReportView(report, userId, persistFilter, wId, (ReportFilterSettingView)Session["ReportFilterSetting"], userReportSetting, (DateTime?)Session["ReportStartDate"], (DateTime?)Session["ReportEndDate"]);
            reportModel.LoadData();
            ViewBag.SavedReports = SIDAL.GetSavedReports(GetUser().UserId, reportType);
            ViewBag.IsNewReport = isNewReport;
            return View(reportModel);
        }

        [HttpPost]
        public ActionResult BenchmarkAnalysis(BenchmarkAnalysisReportView reportModel)
        {
            Guid userId = GetUser().UserId;
            var reportType = ESIReportType.BENCHMARK_ANALYSIS;

            ViewBag.ReportType = reportType;

            reportModel.LoadData();

            ViewBag.SavedReports = SIDAL.GetSavedReports(GetUser().UserId, reportType);

            return View(reportModel);
        }

        public ActionResult TrendAnalysis(bool? persistFilter, long? wId = 0, long Id = 0, bool isNewReport = false)
        {
            Guid userId = GetUser().UserId;
            ViewBag.IsAdmin = RoleAccess.IsAdmin;
            TrendAnalysisReportView reportModel = new TrendAnalysisReportView();
            var reportType = ESIReportType.TREND_ANALYSIS;
            UserReportSetting userReportSetting = SIDAL.GetReportSetting(userId, reportType, Id);
            ReportSetting report = SIDAL.GetUserReport(userId, reportType, Id, userReportSetting);
            ViewBag.ReportType = reportType;
            if (report != null)
                reportModel = new TrendAnalysisReportView(report, userId, persistFilter, wId, (ReportFilterSettingView)Session["ReportFilterSetting"], userReportSetting, (DateTime?)Session["ReportStartDate"], (DateTime?)Session["ReportEndDate"]);

            try
            {
                reportModel.Initialize();
                reportModel.LoadData();
            }
            catch { }

            ViewBag.SavedReports = SIDAL.GetSavedReports(GetUser().UserId, reportType);
            ViewBag.IsNewReport = isNewReport;
            //reportManager.GetData();

            return View(reportModel);
        }

        public ActionResult DrillIn(bool? persistFilter, long? wId = 0, long Id = 0, bool isNewReport = false)
        {
            Guid userId = GetUser().UserId;
            ViewBag.IsAdmin = RoleAccess.IsAdmin;
            DrillInReportView reportModel = new DrillInReportView();
            var reportType = ESIReportType.DRILL_IN;
            UserReportSetting userReportSetting = SIDAL.GetReportSetting(userId, reportType, Id);
            ReportSetting report = SIDAL.GetUserReport(userId, reportType, Id, userReportSetting);
            ViewBag.ReportType = reportType;
            if (report != null)
                reportModel = new DrillInReportView(report, userId, persistFilter, wId, (ReportFilterSettingView)Session["ReportFilterSetting"], userReportSetting, (DateTime?)Session["ReportStartDate"], (DateTime?)Session["ReportEndDate"]);

            try
            {
                reportModel.LoadData();
            }
            catch (Exception e)
            {
            }

            ViewBag.SavedReports = SIDAL.GetSavedReports(GetUser().UserId, reportType);
            ViewBag.IsNewReport = isNewReport;
            return View(reportModel);
        }

        public ActionResult DrillInDetails(string dimensionName, string metricName)
        {
            return View();
        }

        [HttpPost]
        public ActionResult DrillInDetails(DrillInTicketStatsView reportModel)
        {
            if (Request.Form["order[0][SortBy]"] != null)
            {
                reportModel.order[0].SortBy = Request.Form["order[0][SortBy]"];
                reportModel.order[0].IsDescending = Request.Form["order[0][IsDescending]"];
            }

            long totalRecords = 0, totalFiltered = 0;
            var data = DrillInDetailsJson(reportModel, out totalRecords, out totalFiltered);
            var finalData = new JObject();
            finalData.Add("recordsFiltered", totalRecords);
            finalData.Add("recordsTotal", totalRecords);

            finalData.Add("data", data);
            var jsonData = Content(finalData.ToString(), "application/json");
            return jsonData;
        }

        private JArray DrillInDetailsJson(DrillInTicketStatsView reportModel, out long totalRecords, out long totalFiltered)
        {
            Guid userId = GetUser().UserId;
            long dimensionId = 0;

            try
            {
                dimensionId = SIDAL.GetEntityIdByName(reportModel.DimensionName, reportModel.MetricName);
                reportModel.LoadData(dimensionId, reportModel.MetricName, reportModel);
            }
            catch (Exception e)
            {

            }

            JArray array = new JArray();
            if (reportModel.Response != null)
            {
                reportModel.TicketStats = reportModel.Response.FirstOrDefault();

                foreach (var rowsResponse in reportModel.Response)
                {
                    JObject obj = new JObject();
                    foreach (var item in rowsResponse.GetType().GetProperties())
                    {
                        var itemPropertyType = item.PropertyType.Name;

                        if (itemPropertyType == "DateTime")
                        {
                            var val = Convert.ToDateTime(item.GetValue(reportModel.TicketStats, null) ?? new DateTime());
                            obj.Add(item.Name, val.ToString("MM/dd/yyyy"));
                        }
                        else if (itemPropertyType.Equals("Double"))
                        {
                            var val = Convert.ToDouble(item.GetValue(reportModel.TicketStats, null) ?? 0);
                            obj.Add(item.Name, Math.Round(val, 2));
                        }
                        else
                        {
                            string dataVal = null;
                            if (reportModel.TicketStats != null)
                            {
                                var val = Convert.ToString(item.GetValue(reportModel.TicketStats, null));
                                dataVal = val;
                            }
                            if (dataVal != null)
                            {
                                obj.Add(item.Name, dataVal);
                            }
                        }
                    }
                    array.Add(obj);
                }
            }
            totalFiltered = reportModel.recordsFiltered;
            totalRecords = reportModel.recordsTotal;
            return array;
        }


        //This might move to the Admin Section
        public ActionResult AlertsManagement()
        {
            return View();
        }

        public ActionResult CustomReport()
        {
            return View();
        }

        public ActionResult UpdateDashboardSetting(bool isDefault, int maxColumn, bool isFavorite, long dashboardId)
        {
            bool updateStatus = true;
            try
            {
                Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                updateStatus = SIDAL.UpdateDashBoardSettings(isDefault, maxColumn, isFavorite, dashboardId, userId);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return Content(updateStatus.ToString());
        }

        public ActionResult UpdateReportSetting(bool isDefault, bool isFavorite, string reportType, long reportId)
        {
            bool updateStatus = true;
            try
            {
                Guid userId = GetUser().UserId;
                updateStatus = SIDAL.UpdateReportSettings(isDefault, isFavorite, reportType, reportId, userId);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return Content(updateStatus.ToString());
        }

        public ActionResult UpdateReportName(long reportId, string reportType, string reportName)
        {
            var updateStatus = SIDAL.ChangeReportName(reportId, reportType, reportName);
            dynamic output = new
            {
                updateStatus = updateStatus,
            };
            return Json(output);
        }

        public ActionResult AddNewDashboard(DashboardSettingsView dashboard)
        {
            long dashboardId = 0;
            try
            {
                Guid userId = GetUser().UserId;

                DashboardSetting dashboardSetting = new DashboardSetting();
                UserDashboardSetting userDashSetting = new UserDashboardSetting();
                dashboardSetting.AccessType = dashboard.AccessType;
                dashboardSetting.Name = dashboard.Name;
                userDashSetting.IsFavourite = false;
                userDashSetting.UserId = userId;

                dashboardSetting.Id = dashboard.Id;
                var dashboardExist = SIDAL.GetDashboardSettingGroupWise(userId);
                userDashSetting.IsDefault = dashboardExist == null ? true : false;
                dashboardSetting.UserId = GetUser().UserId;
                dashboardSetting.CreatedAt = DateTime.Now;
                userDashSetting.MaxColumn = 7;

                dashboardId = SIDAL.AddDashBoard(dashboardSetting);
                userDashSetting.DashboardId = dashboardId;
                SIDAL.AddUserDashboardSetting(userDashSetting);

            }
            catch (Exception)
            {

            }
            return RedirectToAction("Dashboard", new { @Id = dashboardId, @isNewDashboard = true });
        }

        [HttpPost]
        public ActionResult UpdateDashboardStartDate(long dashboardId, DateTime startDate, bool nonAdminStandardReport)
        {
            var updateStatus = true;

            if (!nonAdminStandardReport)
            {
                try
                {
                    updateStatus = SIDAL.ChangeDashboardStartDate(dashboardId, startDate);
                }
                catch (Exception)
                {
                    updateStatus = false;
                }
            }
            Session["DashboardStartDate"] = startDate;
            dynamic output = new
            {
                updateStatus = updateStatus,
            };
            return Json(output);
        }

        public ActionResult UpdateAccessType(long dashboardId, string accessType)
        {
            var updateStatus = SIDAL.ChangeDashboardAccessType(dashboardId, accessType);
            dynamic output = new
            {
                updateStatus = updateStatus,
            };
            return Json(output);
        }

        public ActionResult UpdateDashboardName(long dashboardId, string dashboardName)
        {
            var updateStatus = SIDAL.ChangeDashboardName(dashboardId, dashboardName);
            dynamic output = new
            {
                updateStatus = updateStatus,
            };
            return Json(output);
        }

        public ActionResult AddNewReport(ReportSettingView postReport)
        {
            long reportId = 0;
            try
            {
                ReportSetting report = new ReportSetting();
                report.AccessType = postReport.AccessType;
                report.ReportName = postReport.ReportName;
                report.Type = postReport.Type;
                //report.IsFavourite = false;
                report.Id = postReport.ReportId;
                Guid userId = GetUser().UserId;
                var reportType = postReport.Type;
                //var reportExist = SIDAL.GetUserReport(userId, reportType, report.Id);
                //If there is no report available then make this report as default
                //report.IsDefault = reportExist == null ? true : false;

                report.UserId = userId;
                report.CreatedAt = DateTime.Now;
                //if (report.Id > 0)
                //{
                //    reportId = SIDAL.EditReport(report);
                //}
                //else
                //{
                reportId = SIDAL.AddReport(report);
                //}

                UserReportSetting userReportSetting = new UserReportSetting();
                userReportSetting.ReportId = reportId;
                userReportSetting.UserId = userId;
                userReportSetting.Type = report.Type;
                userReportSetting.IsDefault = false;
                userReportSetting.IsFavourite = false;
                SIDAL.AddUserReportSetting(userReportSetting);

            }
            catch (Exception e)
            {

            }
            var actionName = "";
            switch (postReport.Type)
            {
                case ESIReportType.GOAL_ANALYSIS:
                    actionName = "GoalAnalysis";
                    break;
                case ESIReportType.BENCHMARK_ANALYSIS:
                    actionName = "BenchmarkAnalysis";
                    break;
                case ESIReportType.TREND_ANALYSIS:
                    actionName = "TrendAnalysis";
                    break;
                case ESIReportType.DRILL_IN:
                    actionName = "DrillIn";
                    break;
            }


            return RedirectToAction(actionName, new { @Id = reportId, @IsNewReport = true });
        }

        #region Add Goal Analysis and Benchmark Analysis Report  Settings Configuration
        public ActionResult AddReportConfiguration(long reportId, string metricTypeList, string columnTypeList, string horizontalSeprator, string thresholdValueList)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var metricList = jsonSerializer.Deserialize<List<ReportConfiguration>>(metricTypeList);
            var columnList = jsonSerializer.Deserialize<List<ReportConfiguration>>(columnTypeList);
            var actionSepratorList = jsonSerializer.Deserialize<List<ReportConfiguration>>(horizontalSeprator);
            var thresholdList = new List<ReportConfiguration>();
            //var data = thresholdValueList.Split(new[] { ',','\\' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
            if (thresholdValueList != "[]")
            {
                thresholdList = jsonSerializer.Deserialize<List<ReportConfiguration>>(thresholdValueList);
            }

            //Delete if record available for the report.
            SIDAL.DeleteReportConfiguration(reportId);

            foreach (var item in columnList)
            {
                SIDAL.AddReportColumnConfig(item);
            }
            foreach (var item in metricList)
            {
                //   long horizontalMetricId = horizontalSeprator.Contains(item.ColumnId)
                SIDAL.AddReportRowConfig(item);
            }
            if (horizontalSeprator.Length > 0)
            {
                foreach (var item in actionSepratorList)
                {
                    SIDAL.UpdateSepratorStatus(item);
                }
            }
            if (horizontalSeprator.Length > 0)
            {
                foreach (var item in actionSepratorList)
                {
                    SIDAL.UpdateSepratorStatus(item);
                }
            }
            if (thresholdList.Count > 0)
            {
                foreach (var item in thresholdList)
                {
                    SIDAL.UpdateThresholdValues(item);
                }
            }

            return Content("OK");
        }
        #endregion

        #region Add Drill in Report Settings Configuration
        [HttpPost]
        public ActionResult AddDrillinReportConfiguration(long reportId, string dimensionDetails, string columnTypeList, string customFilter)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var dimensionDetail = jsonSerializer.Deserialize<List<ReportConfiguration>>(dimensionDetails);
            var columnDetails = jsonSerializer.Deserialize<List<ReportConfiguration>>(columnTypeList);
            var specialFilterDetail = new ReportConfiguration();
            try
            {
                specialFilterDetail = jsonSerializer.Deserialize<ReportConfiguration>(customFilter);
            }
            catch (Exception e) { }
            //Delete if record available for the report.
            SIDAL.DeleteDrillInConfiguration(reportId);

            foreach (var item in columnDetails)
            {
                SIDAL.AddReportColumnConfig(item);
            }

            foreach (var item in dimensionDetail)
            {
                //   long horizontalMetricId = horizontalSeprator.Contains(item.ColumnId)
                SIDAL.AddDrillinReportRowConfig(item);
            }

            if (specialFilterDetail.CustomFilterDimension != "")
            {
                SIDAL.AddDrillinCustomReportConfig(specialFilterDetail);
            }

            return Content("OK");
        }

        public ActionResult AddCustomReportConfiguration(long reportId, string specialFilters)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var dimensionDetail = jsonSerializer.Deserialize<List<ReportConfiguration>>(specialFilters);


            //Delete if record available for the report.
            SIDAL.DeleteDrillInCustomConfiguration(reportId);


            foreach (var item in dimensionDetail)
            {
                //   long horizontalMetricId = horizontalSeprator.Contains(item.ColumnId)
                SIDAL.AddDrillinCustomReportConfig(item);
            }

            return Content("OK");
        }

        #endregion

        #region Add Trend Analysis Configuration
        public ActionResult ApplyTrendReportConfig(TrendAnalysisReportView model, bool? PersistFilter, long? WidgetId)
        {

            //Delete existing configuration 
            SIDAL.DeleteTrendAnalysisConfiguration(model.ReportId);

            // Add the new configuration
            SIDAL.AddTrendAnalysisConfiguration(model.ReportId, model.MetricDefinitionId, model.TargetDimensionId, model.UpperControlLimit, model.LowerControlLimit, model.OmitPeriodsWithNoData, model.IsScallingAutoFit);

            return RedirectToAction("TrendAnalysis", new { @Id = model.ReportId, @persistFilter = PersistFilter, @wId = WidgetId });
        }
        #endregion

        public ActionResult ApplyReportFilter(ReportSettingView reportSetting)
        {
            var persistFilter = false;
            Dictionary<string, List<long>> reportFilterList = new Dictionary<string, List<long>>();
            if (reportSetting.ReportFilterSetting != null)
            {
                if (reportSetting.ReportFilterSetting.Regions != null)
                    reportFilterList.Add("Region", reportSetting.ReportFilterSetting.Regions);

                if (reportSetting.ReportFilterSetting.Districts != null)
                    reportFilterList.Add("District", reportSetting.ReportFilterSetting.Districts);

                if (reportSetting.ReportFilterSetting.Plants != null)
                    reportFilterList.Add("Plant", reportSetting.ReportFilterSetting.Plants);

                if (reportSetting.ReportFilterSetting.MarketSegments != null)
                    reportFilterList.Add("Market", reportSetting.ReportFilterSetting.MarketSegments);

                if (reportSetting.ReportFilterSetting.Customers != null)
                    reportFilterList.Add("Customer", reportSetting.ReportFilterSetting.Customers);

                if (reportSetting.ReportFilterSetting.SalesStaffs != null)
                    reportFilterList.Add("SalesStaff", reportSetting.ReportFilterSetting.SalesStaffs);
            }
            var user = SIDAL.GetUser(UserId);

            if (user.Role != "System Admin" && reportSetting.AccessType == "Standard")
            {
                Session["ReportFilterSetting"] = reportSetting.ReportFilterSetting;
                Session["ReportStartDate"] = reportSetting.StartDate;
                Session["ReportEndDate"] = reportSetting.EndDate;
                persistFilter = true;
            }
            else
            {
                SIDAL.UpdateReportFilterSettings(reportFilterList, reportSetting.ReportId, reportSetting.StartDate, reportSetting.EndDate);
            }

            var actionName = "";
            switch (reportSetting.Type)
            {
                case ESIReportType.GOAL_ANALYSIS:
                    actionName = "GoalAnalysis";
                    break;
                case ESIReportType.BENCHMARK_ANALYSIS:
                    actionName = "BenchmarkAnalysis";
                    break;
                case ESIReportType.TREND_ANALYSIS:
                    actionName = "TrendAnalysis";
                    break;
                case ESIReportType.DRILL_IN:
                    actionName = "DrillIn";
                    break;
            }

            return RedirectToAction(actionName, new { @Id = reportSetting.ReportId, @persistFilter = persistFilter });
        }

        public ActionResult DeleteReport(int Id, string reportType)
        {
            var userId = GetUser().UserId;
            var deleteStatus = false;
            try
            {
                deleteStatus = SIDAL.DeleteReport(Id, reportType, userId);
            }
            catch (Exception)
            {

            }
            var actionName = "";
            switch (reportType)
            {
                case ESIReportType.GOAL_ANALYSIS:
                    actionName = "GoalAnalysis";
                    break;
                case ESIReportType.BENCHMARK_ANALYSIS:
                    actionName = "BenchmarkAnalysis";
                    break;
                case ESIReportType.TREND_ANALYSIS:
                    actionName = "TrendAnalysis";
                    break;
                case ESIReportType.DRILL_IN:
                    actionName = "DrillIn";
                    break;
            }
            return RedirectToAction(actionName);
        }

        public ActionResult UpdateReportAccessType(string type, string accessType, long reportId)
        {
            var updateStatus = SIDAL.ChangeDashboardAccessType(type, accessType, reportId);
            dynamic output = new
            {
                updateStatus = updateStatus,
            };
            return Json(output);
        }

        [HttpPost]
        public ActionResult ApplyDashbaordFilter(DashboardSettingsView dashboardSetting, string submitValue)
        {
            var user = SIDAL.GetUser(UserId);
            if (submitValue == "Save")
            {
                Dictionary<string, List<long>> dashboardFilterList = new Dictionary<string, List<long>>();
                if (dashboardSetting.DashboardFilter != null)
                {
                    if (dashboardSetting.DashboardFilter.Regions != null)
                    {
                        dashboardFilterList.Add("Region", dashboardSetting.DashboardFilter.Regions);
                    }
                    if (dashboardSetting.DashboardFilter.Districts != null)
                    {
                        dashboardFilterList.Add("District", dashboardSetting.DashboardFilter.Districts);
                    }
                    if (dashboardSetting.DashboardFilter.Plants != null)
                    {
                        dashboardFilterList.Add("Plant", dashboardSetting.DashboardFilter.Plants);
                    }
                    if (dashboardSetting.DashboardFilter.MarketSegments != null)
                    {
                        dashboardFilterList.Add("Market", dashboardSetting.DashboardFilter.MarketSegments);
                    }
                    if (dashboardSetting.DashboardFilter.Customers != null)
                    {
                        dashboardFilterList.Add("Customer", dashboardSetting.DashboardFilter.Customers);
                    }
                    if (dashboardSetting.DashboardFilter.SalesStaffs != null)
                    {
                        dashboardFilterList.Add("SalesStaff", dashboardSetting.DashboardFilter.SalesStaffs);
                    }
                }
                try
                {
                    if (user.Role != "System Admin" && dashboardSetting.AccessType == "Standard")
                    {

                    }
                    else
                    {
                        SIDAL.UpdateDashboardFilterSettings(dashboardFilterList, dashboardSetting.Id);
                    }

                }
                catch (Exception ex)
                {
                }
            }
            Session["DashboardFilters"] = dashboardSetting.DashboardFilter;
            return RedirectToAction("Dashboard", new { @Id = dashboardSetting.Id, @persistFilter = true });
        }

        [HttpPost]
        public ActionResult GetFiltersDistrictWise(string[] districtId, long dashboardId)
        {
            var PlantList = new List<SelectListItem>();

            foreach (Plant plant in SIDAL.GetPlantsForDistricts(districtId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = plant.Name;
                item.Value = plant.PlantId.ToString();
                //item.Selected = boardSetting.Plants.Contains(plant.PlantId);
                PlantList.Add(item);
            }

            var MarketSegmentList = new List<SelectListItem>();

            foreach (MarketSegment segment in SIDAL.GetMarketSegmentsForDistricts(districtId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = segment.Name;
                item.Value = segment.MarketSegmentId.ToString();
                //item.Selected = boardSetting.MarketSegments.Contains(segment.MarketSegmentId);
                MarketSegmentList.Add(item);
            }

            var CustomerList = new List<SelectListItem>();

            foreach (Customer customer in SIDAL.GetCustomersForDistricts(districtId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = customer.Name;
                item.Value = customer.CustomerId.ToString();
                //item.Selected = boardSetting.Customers.Contains(customer.CustomerId);
                CustomerList.Add(item);
            }

            var SalesStaffList = new List<SelectListItem>();

            foreach (SalesStaff staff in SIDAL.GetSalesStaffForDistricts(districtId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = staff.Name;
                item.Value = staff.SalesStaffId.ToString();
                //item.Selected = boardSetting.SalesStaffs.Contains(staff.SalesStaffId);
                SalesStaffList.Add(item);
            }

            dynamic output = new
            {
                plants = PlantList,
                marketSegments = MarketSegmentList,
                customers = CustomerList,
                salesStaffs = SalesStaffList,
            };

            return Json(output);
        }

        [HttpPost]
        public ActionResult GetDistrictRegionWise(string[] regionId, long dashboardId)
        {
            DashboardFilterSettingView boardSetting = new DashboardFilterSettingView(UserId, dashboardId);

            List<SelectListItem> DistrictList = new List<SelectListItem>();
            foreach (District district in SIDAL.GetDistrictsForRegions(regionId, UserId))
            {
                SelectListItem item = new SelectListItem();
                item.Text = district.Name;
                item.Value = district.DistrictId.ToString();
                item.Selected = boardSetting.Districts.Contains(district.DistrictId);
                DistrictList.Add(item);
            }

            return Json(DistrictList);
        }

        [HttpPost]
        public ActionResult GetCustomConfig(long reportId)
        {
            var data = SIDAL.GetReportRowConfig(reportId);

            List<ReportRowConfig> list = new List<ReportRowConfig>();
            foreach (var item in data.Where(x => x.ShowActionIcons == true))
            {
                var rowConfig = new ReportRowConfig();
                rowConfig.OkLimit = item.OkLimit;
                rowConfig.CautionLimit = item.CautionLimit;
                rowConfig.WarningLimit = item.WarningLimit;
                rowConfig.ShowActionIcons = item.ShowActionIcons;
                rowConfig.ComparisonMetricId = item.ComparisonMetricId;
                //rowConfig.
                list.Add(rowConfig);
            }

            return Json(list);
        }
    }
}
