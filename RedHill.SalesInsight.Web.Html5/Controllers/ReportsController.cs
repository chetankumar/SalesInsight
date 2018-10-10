using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Web.Html5.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using RedHill.SalesInsight.Web.Html5.Helpers;
namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class ReportsController : BaseController
    {

        public ActionResult Index()
        {
            if (RoleAccess.HasReportsAccess != SIRolePermissionLevelConstants.FULL_ACCESS)
            {
                return RedirectToAction("Pipeline", "Home");
            }
            ViewBag.ReportsActive = "active";
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ReportView model = new ReportView(userId);
            model.SalesForecastFilter.ProjectionMonth = DateTime.Now.ToString("MMM, yyyy");
            return View(model);
        }

        #region Settings

        public ActionResult Settings()
        {
            return View();
        }

        #endregion

        #region Indicators
        public ActionResult Indicators()
        {
            IndicatorsView model = new IndicatorsView();
            return View(model);
        }

        public String UpdateMetricAllowances(string metric, string attribute, float value)
        {
            SIDAL.UpdateMetricIndicator(metric, value, attribute);
            return "OK";
        }

        public String UpdateTargetAllowances(string metric, string attribute, float value)
        {
            SIDAL.UpdateTargetIndicator(metric, value, attribute);
            return "OK";
        }
        #endregion

        #region Uploads

        // Productivity Section
        public ActionResult CustomerProductivity()
        {
            CustomerProductivityPage model = new CustomerProductivityPage();
            model.LoadRows();
            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerProductivity(CustomerProductivityPage model)
        {
            model.LoadRows();
            return View(model);
        }
        public ActionResult UploadCustomerProductivity()
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    int numRows = 16;
                    List<CustomerProductivity> entities = new List<CustomerProductivity>();
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string ticketDate = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string orderCode = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string customerCode = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string ticketCode = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string isFOB = Convert.ToString(worksheet.Cells[i, 5].Value);
                            string plantCode = Convert.ToString(worksheet.Cells[i, 6].Value);
                            string truckCode = Convert.ToString(worksheet.Cells[i, 7].Value);
                            string quantity = Convert.ToString(worksheet.Cells[i, 8].Value);
                            string ticketing = Convert.ToString(worksheet.Cells[i, 9].Value);
                            string loadTemper = Convert.ToString(worksheet.Cells[i, 10].Value);
                            string toJob = Convert.ToString(worksheet.Cells[i, 11].Value);
                            string wait = Convert.ToString(worksheet.Cells[i, 12].Value);
                            string unload = Convert.ToString(worksheet.Cells[i, 13].Value);
                            string wash = Convert.ToString(worksheet.Cells[i, 14].Value);
                            string fromJob = Convert.ToString(worksheet.Cells[i, 15].Value);
                            string segmentId = Convert.ToString(worksheet.Cells[i, 16].Value);

                            CustomerProductivity entity = new CustomerProductivity();
                            string error = SIDAL.ScrubCustomerProductivity(ticketDate, orderCode, customerCode, ticketCode, isFOB, plantCode, truckCode, quantity, ticketing, loadTemper, toJob, wait, unload, wash, fromJob, segmentId, out entity);

                            if (error != null)
                            {
                                worksheet.Cells[i, numRows + 1].Value = error;
                                worksheet.Cells[i, 1, i, numRows].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numRows].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, numRows].Style.Font.Color.SetColor(Color.White);
                            }
                            else
                            {
                                entities.Add(entity); // If there is no error, keep the entity to save later.
                                worksheet.Cells[i, numRows + 1].Value = "IMPORTED SUCCESSFULLY";
                                worksheet.Cells[i, 1, i, numRows].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numRows].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                worksheet.Cells[i, 1, i, numRows].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 1, i, numRows].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i, 1, i, numRows].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            worksheet.Cells[i, 1, i, numRows].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[i, numRows + 1].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }
                    if (entities.Count() > 0)
                    {
                        SIDAL.RefreshCustomerProductivities(entities);
                    }
                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("CustomerProductivity");
        }

        // Profitibility Section        
        public ActionResult CustomerProfitability()
        {
            CustomerProfitabilityPage model = new CustomerProfitabilityPage();
            model.LoadRows();
            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerProfitability(CustomerProfitabilityPage model)
        {
            model.LoadRows();
            return View(model);
        }
        public ActionResult UploadCustomerProfitability()
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    int numCols = 7;
                    List<CustomerProfitability> entities = new List<CustomerProfitability>();
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string reportDate = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string customerNumber = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string customerName = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string segmentId = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string revenue = Convert.ToString(worksheet.Cells[i, 5].Value);
                            string materialCost = Convert.ToString(worksheet.Cells[i, 6].Value);
                            string plantCode = Convert.ToString(worksheet.Cells[i, 7].Value);

                            CustomerProfitability entity = new CustomerProfitability();
                            string error = SIDAL.ScrubCustomerProfitablity(reportDate, customerNumber, customerName, segmentId, revenue, materialCost, plantCode, out entity);

                            if (error != null)
                            {
                                worksheet.Cells[i, numCols + 1].Value = error;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            }
                            else
                            {
                                entities.Add(entity); // If there is no error, keep the entity to save later.
                                worksheet.Cells[i, numCols + 1].Value = "IMPORTED SUCCESSFULLY";
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[i, numCols + 1].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }
                    if (entities.Count() > 0)
                    {
                        SIDAL.RefreshCustomerProfitabilities(entities);
                    }
                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("CustomerProfitability");
        }

        // Order Change Section        
        public ActionResult OrderChanges()
        {
            OrderChangePage model = new OrderChangePage();
            model.LoadRows();
            return View(model);
        }
        [HttpPost]
        public ActionResult OrderChanges(OrderChangePage model)
        {
            model.LoadRows();
            return View(model);
        }
        public ActionResult UploadOrderChanges()
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    int numCols = 5;
                    List<CustomerOrderChange> entities = new List<CustomerOrderChange>();
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string orderId = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string customerNumber = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string productId = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string volume = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string reportDate = Convert.ToString(worksheet.Cells[i, 5].Value);

                            CustomerOrderChange entity = new CustomerOrderChange();
                            string error = SIDAL.ScrubCustomerOrderChanges(reportDate, orderId, customerNumber, productId, volume, out entity);

                            if (error != null)
                            {
                                worksheet.Cells[i, numCols + 1].Value = error;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            }
                            else
                            {
                                entities.Add(entity); // If there is no error, keep the entity to save later.
                                worksheet.Cells[i, numCols + 1].Value = "IMPORTED SUCCESSFULLY";
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[i, numCols + 1].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }
                    if (entities.Count() > 0)
                    {
                        SIDAL.RefreshCustomerOrderChanges(entities);
                    }
                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("OrderChanges");
        }

        // Aging Section        
        public ActionResult CustomerAgings()
        {
            CustomerAgingPage model = new CustomerAgingPage();
            model.LoadRows();
            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerAgings(CustomerAgingPage model)
        {
            model.LoadRows();
            return View(model);
        }
        public ActionResult UploadCustomerAgings()
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    int numCols = 10;
                    List<CustomerAging> entities = new List<CustomerAging>();
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string customerNumber = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string customerName = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string balance = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string current = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string over1mo = Convert.ToString(worksheet.Cells[i, 5].Value);
                            string over2mo = Convert.ToString(worksheet.Cells[i, 6].Value);
                            string over3mo = Convert.ToString(worksheet.Cells[i, 7].Value);
                            string over4mo = Convert.ToString(worksheet.Cells[i, 8].Value);
                            string dso = Convert.ToString(worksheet.Cells[i, 9].Value);
                            string reportDate = Convert.ToString(worksheet.Cells[i, 10].Value);

                            CustomerAging entity = new CustomerAging();
                            string error = SIDAL.ScrubCustomerAging(reportDate, customerNumber, customerName, balance, current, over1mo, over2mo, over3mo, over4mo, dso, out entity);

                            if (error != null)
                            {
                                worksheet.Cells[i, numCols + 1].Value = error;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            }
                            else
                            {
                                entities.Add(entity); // If there is no error, keep the entity to save later.
                                worksheet.Cells[i, numCols + 1].Value = "IMPORTED SUCCESSFULLY";
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 1, i, numCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i, 1, i, numCols].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            worksheet.Cells[i, 1, i, numCols].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[i, numCols + 1].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }
                    if (entities.Count() > 0)
                    {
                        SIDAL.RefreshCustomerAging(entities);
                    }
                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("CustomerAgings");
        }

        #endregion

        #region Duplicate Projections
        public ActionResult RemoveDuplicateProjections()
        {
            List<DuplicateView> duplicates = new List<DuplicateView>();
            foreach (List<ProjectProjection> dup in SIDAL.GetProjectProjectionDuplicates())
            {
                duplicates.Add(new DuplicateView(dup));
            }
            return View(duplicates);
        }

        // Save this projection and remove the other duplicates
        public String SaveProjection(int id)
        {
            SIDAL.SaveProjectProjection(id);
            return "OK";
        }

        #endregion

        #region Won Loss Report
        [HttpGet]
        public ActionResult WonLossReport()
        {
            if (RoleAccess.HasReportsAccess != SIRolePermissionLevelConstants.FULL_ACCESS)
            {
                return RedirectToAction("Pipeline", "Home");
            }
            ViewBag.ReportsActive = "active";
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ReportView model = new ReportView(userId);
            return View("WonLost", model);
        }

        [HttpPost]
        public ActionResult WonLossReport(ReportFilter model)
        {

            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            if (model.Districts == null || model.Districts.Count() == 0)
            {
                model.Districts = SIDAL.GetDistricts(userId).Select(x => x.DistrictId).ToArray();
            }

            DateTime maxEndDate = SqlDateTime.MaxValue.Value;
            DateTime minEndDate = SqlDateTime.MinValue.Value;

            if (model.BidDateBegin != null || model.BidDateEnd != null)
            {
                model.BidDateEnd = model.BidDateEnd == null ? model.BidDateEnd = maxEndDate : model.BidDateEnd;
                model.BidDateBegin = model.BidDateBegin == null ? model.BidDateBegin = minEndDate : model.BidDateBegin;
            }

            if (model.StartDateBegin != null || model.StartDateEnd != null)
            {
                model.StartDateEnd = model.StartDateEnd == null ? model.StartDateEnd = maxEndDate : model.StartDateEnd;
                model.StartDateBegin = model.StartDateBegin == null ? model.StartDateBegin = minEndDate : model.StartDateBegin;
            }

            if (model.WLDateBegin != null || model.WLDateEnd != null)
            {
                model.WLDateEnd = model.WLDateEnd == null ? model.WLDateEnd = maxEndDate : model.WLDateEnd;
                model.WLDateBegin = model.WLDateBegin == null ? model.WLDateBegin = minEndDate : model.WLDateBegin;
            }


            model.FillSelectItems(userId);

            //Filter Plants based on the Product Type
            if (model.ProductTypeValue > 0)
            {
                var productPlants = SIDAL.GetPlants(userId, false, model.SelectProductTypeId);
                if (productPlants != null)
                {
                    model.Plants = productPlants.Select(x => x.PlantId).ToArray();
                }
            }

            List<SIProjectSuccessMarketShareSummary> summaries = SIDAL.GetProjectSuccessMarketShareSummary(userId, model.Regions, model.Districts, model.Plants, model.Staffs, model.BidDateBegin, model.BidDateEnd, model.StartDateBegin, model.StartDateEnd, model.WLDateBegin, model.WLDateEnd);
            string PieChartJson = ChartData.GetMarketShareSummaryJson(summaries);
            ViewBag.Summaries = PieChartJson;

            List<SIProjectSuccessSalesStaffSummary> salesStaffs = SIDAL.GetProjectSuccessSalesStaffSummary(userId, model.Regions, model.Districts, model.Plants, model.Staffs, model.BidDateBegin, model.BidDateEnd, model.StartDateBegin, model.StartDateEnd, model.WLDateBegin, model.WLDateEnd);
            SIProjectSuccessSalesStaffSummary totalSales = new SIProjectSuccessSalesStaffSummary();
            totalSales.Name = "Total";
            totalSales.VolumeSold = salesStaffs.Sum(m => m.VolumeSold);
            totalSales.VolumeLost = salesStaffs.Sum(m => m.VolumeLost);
            salesStaffs.Add(totalSales);
            ViewBag.SalesStaffs = salesStaffs;

            List<SIProjectSuccessMajorJobSummary> majorJobs = SIDAL.GetProjectSuccessDashboardMajorJobSummary(userId, model.Regions, model.Districts, model.Plants, model.Staffs, model.BidDateBegin, model.BidDateEnd, model.StartDateBegin, model.StartDateEnd, model.WLDateBegin, model.WLDateEnd);
            //SIProjectSuccessMajorJobSummary totals = new SIProjectSuccessMajorJobSummary();
            //totals.CustomerName = "Total";
            //totals.Volume = majorJobs.Sum(m => m.Volume);
            //totals.MixPrice = majorJobs.Average(m => m.MixPrice);
            //totals.CompetitorNames = new List<string>();            
            //majorJobs.Add(totals);       
            int totalVolume = majorJobs.Sum(m => m.Volume).GetValueOrDefault();

            //Get the lost Projects
            var lostProjects = majorJobs.Where(x => x.PriceLost > 0);

            int totalLostVolume = lostProjects.Sum(x => x.Volume).GetValueOrDefault();
            decimal totalPriceLost = 0, mixPrice = 0; ;

            if (totalLostVolume > 0)
            {
                totalPriceLost = lostProjects.Sum(x => (x.PriceLost.GetValueOrDefault() * x.Volume.GetValueOrDefault())) / totalLostVolume;
            }

            int totalPriceVolume = majorJobs.Where(x => x.MixPrice > 0).Sum(m => m.Volume).GetValueOrDefault();

            if (totalPriceVolume > 0)
            {
                mixPrice = majorJobs.Where(x => x.MixPrice > 0)
                                    .Sum(m => (m.MixPrice.GetValueOrDefault() * m.Volume.GetValueOrDefault())) / totalPriceVolume;
            }

            ViewBag.Volume = totalVolume;
            ViewBag.MixPrice = mixPrice.ToString("N2");
            ViewBag.MajorJobs = majorJobs;
            ViewBag.TotalPriceLost = totalPriceLost.ToString("N2");

            if (model.Print != null)
                return View("WonLossReport_Print", model);
            else
                return View(model);
        }

        #endregion

        #region Sales Forecast Report

        [HttpGet]
        public ActionResult SalesForecastReport()
        {
            if (RoleAccess.HasReportsAccess != SIRolePermissionLevelConstants.FULL_ACCESS)
            {
                return RedirectToAction("Pipeline", "Home");
            }
            ViewBag.ReportsActive = "active";
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ReportView model = new ReportView(userId);
            model.SalesForecastFilter.ProjectionMonth = DecryptCookie("FORECAST_MONTH");
            if (model.SalesForecastFilter.ProjectionMonth == null)
            {
                model.SalesForecastFilter.ProjectionMonth = DateTime.Now.ToString("MMM, yyyy");
            }
            return View("SalesForecast", model);
        }

        [HttpPost]
        public ActionResult SalesForecastReport(ReportFilter model)
        {
            EncryptCookie("FORECAST_MONTH", model.ProjectionMonth, 7);
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            if (model.Districts == null || model.Districts.Count() == 0)
            {
                model.Districts = SIDAL.GetDistricts(userId).Select(x => x.DistrictId).ToArray();
            }

            model.FillSelectItems(userId);

            //Filter Plants based on the Product Type
            if (model.ProductTypeValue > 0)
            {
                var productPlants = SIDAL.GetPlants(userId, false, model.SelectProductTypeId);
                if (productPlants != null)
                {
                    model.Plants = productPlants.Select(x => x.PlantId).ToArray();
                }
            }

            // Forecast verses plan report
            List<SIForecastVersusPlan> forecastVPlan = SIDAL.GetForecastVersusPlan(userId, model.Districts, model.Plants, model.ProjectionDateTime);
            ViewBag.forecastPlans = ChartData.GetForecastVersesPlanData(forecastVPlan);

            List<String> districtNames = forecastVPlan.Select(m => m.DistrictName).Distinct().ToList();
            ViewBag.DistrictNames = districtNames;


            // Projection Accuracy Report
            List<SIProjectionAccuracy> projectionAccuracy = SIDAL.GetProjectionAccuracy(userId, model.Districts, model.Plants, model.ProjectionDateTime);
            SIProjectionAccuracy total = new SIProjectionAccuracy();
            total.ActualQuantity = projectionAccuracy.Sum(m => m.ActualQuantity);
            total.ForecastQuantity = projectionAccuracy.Sum(m => m.ForecastQuantity);
            total.SalesStaffName = "Current Month Total";
            projectionAccuracy.Add(total);
            ViewBag.ProjectionAccuracy = projectionAccuracy;


            // Projection Plant Report
            List<SIPlantProjections> plantProjections = SIDAL.GetPlantProjections(userId, model.Districts, model.Plants, model.ProjectionDateTime);
            List<SIPlantProjections> backlogProjections = SIDAL.GetPlantBacklog(userId, model.Districts, model.Plants, model.ProjectionDateTime, model.FilterBacklogLimit);

            plantProjections.AddRange(backlogProjections);

            List<string> projectionMonths = plantProjections.Select(m => m.MonthName).Distinct().ToList();
            ViewBag.PlantProjectionMonths = projectionMonths;

            foreach (SIPlantProjections pp in plantProjections)
            {
                pp.DisplayQuantity = pp.ForecastQuantity.ToString("N0");
            }

            foreach (String month in projectionMonths)
            {
                int totalPP = (int)plantProjections.Where(m => m.MonthName == month).Sum(m => m.ForecastQuantity);
                int totalPPBudget = (int)plantProjections.Where(m => m.MonthName == month).Sum(m => m.TargetQuantity);
                int varianceCYDs = totalPP - totalPPBudget;
                double variance = (totalPP - totalPPBudget) / (totalPPBudget * 1.00) * 100.00;

                SIPlantProjections sipp = new SIPlantProjections();
                sipp.PlantName = "Total";
                sipp.DisplayQuantity = totalPP.ToString("N0");
                sipp.MonthName = month;
                plantProjections.Add(sipp);

                sipp = new SIPlantProjections();
                sipp.PlantName = "Budget";
                sipp.DisplayQuantity = totalPPBudget.ToString("N0");
                sipp.MonthName = month;
                plantProjections.Add(sipp);

                sipp = new SIPlantProjections();
                sipp.PlantName = "Variance CYDs";
                sipp.DisplayQuantity = varianceCYDs.ToString("###,###");
                plantProjections.Add(sipp);
                sipp.MonthName = month;

                sipp = new SIPlantProjections();
                sipp.PlantName = "Variance";
                sipp.DisplayQuantity = variance.ToString("##.##") + "%";
                sipp.MonthName = month;
                plantProjections.Add(sipp);
            }

            List<string> projectionPlants = plantProjections.Select(m => m.PlantName).Distinct().ToList();
            ViewBag.PlantProjectionPlants = projectionPlants;

            ViewBag.PlantProjections = plantProjections;

            // Asset Productivity Report
            List<SIProjectedActualAssetProductivity> assetProductivity = SIDAL.GetProjectedActualAssetProductivity(userId, model.Districts, model.Plants, model.ProjectionDateTime);
            List<String> months = assetProductivity.Select(m => m.MonthName).Distinct().ToList();
            TableModel table = new TableModel();
            table.AddHeader("Plant");
            table.AddHeader("Trucks");

            foreach (string month in months)
                table.AddHeader(month + " " + ConfigurationHelper.Company.DeliveryQtyUomSingular + "/truck");

            List<String> plants = assetProductivity.Select(m => m.PlantName).Distinct().ToList();

            // If the both targets in 0, then grab it from the plants
            // If one is 0 and other is not then take an average.
            // If both are non-zero then take the average.
            int totalTrucks = 0;
            foreach (String plant in plants)
            {
                List<String> row = new List<string>();
                row.Add(plant);
                int trucks = (int)assetProductivity.Where(m => m.PlantName == plant).Select(m => m.ActualTruckCount).Average();
                totalTrucks += trucks;

                bool valid = trucks != 0;
                row.Add(trucks.ToString());
                int i = 0;
                foreach (string month in months)
                {
                    int forecast = assetProductivity.Where(m => m.PlantName == plant).Where(m => m.MonthName == month).Select(m => m.ForecastQuantity).First();
                    if (i == 0)
                    {
                        forecast = assetProductivity.Where(m => m.PlantName == plant).Where(m => m.MonthName == month).Select(m => m.ActualQuantity).First();
                    }
                    valid = valid & forecast != 0;
                    if (trucks == 0)
                        row.Add("");
                    else
                        row.Add((forecast / (21 * trucks * 1d)).ToString("N2"));
                    i++;
                }
                if (valid)
                    table.AddRow(row);
            }
            List<String> totalsRow = new List<string>();
            totalsRow.Add("Total");
            totalsRow.Add(totalTrucks.ToString());
            totalsRow.Add((assetProductivity.Where(m => m.MonthName == months[0]).Sum(m => m.ActualQuantity) / (21 * totalTrucks * 1d)).ToString("N2"));
            totalsRow.Add((assetProductivity.Where(m => m.MonthName == months[1]).Sum(m => m.ForecastQuantity) / (21 * totalTrucks * 1d)).ToString("N2"));

            table.AddRow(totalsRow);

            ViewBag.AssetProductivity = table;


            // MarketSegmentation Analysis
            List<SISegmentationAnalysis> segmentationAnalysis = SIDAL.GetSegmentationAnalysis(userId, model.Districts, model.Plants, model.ProjectionDateTime);
            List<SISegmentationAnalysis> backlogAnalysis = SIDAL.GetSegmentationAnalysisBacklog(userId, model.Districts, model.Plants, model.ProjectionDateTime, model.FilterBacklogLimit);
            segmentationAnalysis.AddRange(backlogAnalysis);

            List<string> segMonths = segmentationAnalysis.Select(m => m.MonthName).Distinct().ToList();
            ViewBag.SegMonths = segMonths;

            foreach (String month in segMonths)
            {
                int totalPP = (int)segmentationAnalysis.Where(m => m.MonthName == month).Sum(m => m.ForecastQuantity);

                SISegmentationAnalysis sipp = new SISegmentationAnalysis();
                sipp.MarketSegmentName = "Total";
                sipp.ForecastQuantity = totalPP;
                sipp.MonthName = month;
                segmentationAnalysis.Add(sipp);
            }

            List<string> segmentNames = segmentationAnalysis.Where(x => x.MarketSegmentName != null).Select(m => m.MarketSegmentName).Distinct().ToList();
            ViewBag.SegmentNames = segmentNames;
            ViewBag.SegmentationAnalysis = segmentationAnalysis;

            // Current Month Segmentation Analysis

            List<SISegmentationAnalysis> currentSegments = segmentationAnalysis.Where(m => m.MonthName == (model.ProjectionDateTime.Value.ToString("MMM") + " Act")).ToList();
            currentSegments.Remove(currentSegments.Where(m => m.MarketSegmentName == "Total").First());
            ViewBag.CurrentSegments = ChartData.GetCurrentSegmentsData(currentSegments);

            if (model.Print != null)
                return View("SalesForecastReport_Print", model);
            else
                return View(model);
        }

        #endregion

        #region Geo Analysis Report
        [HttpGet]
        public ActionResult GeoMarketAnalysis()
        {
            GeoMarketAnalysisReport model = new GeoMarketAnalysisReport(GetUser().UserId);
            return View(model);
        }
        
        [HttpPost]
        public ActionResult GeoMarketAnalysis(GeoMarketAnalysisReport model)
        {
            model.UserId = GetUser().UserId;
            return View(model);
        }
        #endregion

        #region Customer Diamond Report
        [HttpGet]
        public ActionResult CustomerDiamondAnalysis()
        {
            DiamondReport report = new DiamondReport(GetUser().UserId);
            report.StartDate = DateTime.Now.AddMonths(-2);
            report.EndDate = DateTime.Now;
            return View(report);
        }

        [HttpPost]
        public ActionResult CustomerDiamondAnalysis(DiamondReport report)
        {
            report.UserId = GetUser().UserId;
            if (report.CustomerIds == null || report.CustomerIds.Count() == 0)
            {
                ViewBag.Message = "Please select customers";
                return RedirectToAction("CustomerDiamondAnalysis");
            }
            report.LoadReports();
            return View(report);
        }
        #endregion

        #region Sales Dashboard Report
        [HttpGet]
        public ActionResult SalesDashboard()
        {
            SalesDashboard report = new SalesDashboard(GetUser().UserId);
            report.StartDate = DateTime.Now.AddDays(-1 * DateTime.Now.Day + 1);
            return View(report);
        }

        [HttpPost]
        public ActionResult SalesDashboard(SalesDashboard report)
        {
            report.UserId = GetUser().UserId;
            if (report.SelectedDistricts == null || report.SelectedDistricts.Count() == 0)
            {
                return RedirectToAction("SalesDashboard");
            }
            report.LoadReports();
            return View(report);
        }
        #endregion
    }

}
