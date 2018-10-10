using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Web.Html5.Models;
using RedHill.SalesInsight.Web.Html5.Models.QuotationModels;
using RedHill.SalesInsight.Web.Html5.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Utils;
using RedHill.SalesInsight.Web.Html5.Models.JsonModels;
using RedHill.SalesInsight.Web.Html5.Utils;
using RedHill.SalesInsight.AUJSIntegration.Consumer;
using RedHill.SalesInsight.AUJSIntegration.Data;
using RedHill.SalesInsight.AUJSIntegration.Model;
using System.Text;
using RedHill.SalesInsight.Logger;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class QuoteController : BaseController
    {
        #region Quotation page

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.QuotationsActive = "active";
            QuotationPageView model = new QuotationPageView();
            model.UserId = GetUser().UserId;
            model.Filter = new PipelineFilter(GetUser().UserId);
            model.Filter.SortColumns = new string[] { "StatusName" };
            model.Filter.ParentPage = "Pipeline";
            model.Filter.FillSelectItems(GetUser().UserId);
            if (DecryptCookie("NUM_PAGES") != null)
            {
                int numPages = 10;
                Int32.TryParse(DecryptCookie("NUM_PAGES"), out numPages);
                model.Filter.RowsPerPage = numPages;
            }
            else
            {
                EncryptCookie("NUM_PAGES", model.Filter.RowsPerPage + "", 7);
            }
            model.LoadQuotation();
            ViewBag.CurrentPage = (model.Filter.CurrentStart / model.Filter.RowsPerPage) + 1;
            ViewBag.NumPages = (model.NumResults / model.Filter.RowsPerPage) + 1;
            ViewBag.RowCount = model.NumResults;
            ViewBag.Mode = "all";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(QuotationPageView model, string showFilter)
        {
            ViewBag.QuotationsActive = "active";
            if (model.Districts != null)
                model.Filter.Districts = model.Districts;
            if (model.Plants != null)
                model.Filter.Plants = model.Plants;
            if (model.Staffs != null)
                model.Filter.Staffs = model.Staffs;
            if (model.Filter.SearchTerm != null && showFilter == "")
            {
                ModelState.Remove("Filter.ShowInactives");
                model.Filter.ShowInactives = true;
            }
            model.Filter.FillSelectItems(GetUser().UserId);
            model.UserId = GetUser().UserId;
            EncryptCookie("NUM_PAGES", model.Filter.RowsPerPage + "", 7);
            if (model.Filter.DoPrint == null)
            {
                model.LoadQuotation();
                ViewBag.CurrentPage = (model.Filter.CurrentStart / model.Filter.RowsPerPage) + 1;
                ViewBag.NumPages = (model.NumResults / model.Filter.RowsPerPage) + 1;
                ViewBag.RowCount = model.NumResults;
                ViewBag.Mode = "all";
                return View(model);
            }
            else
            {
                model.Filter.CurrentStart = 0;
                model.Filter.RowsPerPage = Int32.MaxValue;
                model.LoadQuotation();
                string fileName = ExportQuotations(model.QuoteReport);
                return new FilePathResult(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        private string ExportQuotations(List<QuotationReportItem> quotations)
        {
            string filename = "Quotations_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            using (ExcelPackage p = new ExcelPackage())
            {
                p.Workbook.Properties.Author = User.Identity.Name;
                p.Workbook.Properties.Title = "Quotations";
                p.Workbook.Worksheets.Add("Quotations");

                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                ws.Cells.Style.Font.Size = 10;

                //  Set Cell Headers 
                ws.Cells[1, 1].Value = "Quote ID";
                ws.Cells[1, 2].Value = "Awarded";
                ws.Cells[1, 3].Value = "Project";
                ws.Cells[1, 4].Value = "Customer";
                ws.Cells[1, 5].Value = "Quote Date";
                ws.Cells[1, 6].Value = "Acceptance Expiration Date";
                ws.Cells[1, 7].Value = "Quote Expiration Date";
                ws.Cells[1, 8].Value = "Market Segment";
                ws.Cells[1, 9].Value = "Plant";
                ws.Cells[1, 10].Value = "Sales Staff";
                ws.Cells[1, 11].Value = "Volume";
                ws.Cells[1, 12].Value = "Revenue";
                ws.Cells[1, 13].Value = "Price";
                ws.Cells[1, 14].Value = "Spread";
                ws.Cells[1, 15].Value = "Profit";
                ws.Cells[1, 16].Value = "Qc Requirement";
                ws.Cells[1, 17].Value = "Backup PlantId";

                int index = 2;
                if (quotations != null)
                {
                    foreach (QuotationReportItem profile in quotations)
                    {
                        ws.Cells[index, 1].Value = profile.QuoteRefNumber;
                        ws.Cells[index, 2].Value = profile.Awarded ? "Yes" : "No";
                        ws.Cells[index, 3].Value = profile.ProjectName;
                        ws.Cells[index, 4].Value = profile.CustomerName;
                        ws.Cells[index, 5].Value = profile.QuoteDate != null ? profile.QuoteDate.Value.ToShortDateString() : "";
                        ws.Cells[index, 6].Value = profile.AcceptanceExpirationDate != null ? profile.AcceptanceExpirationDate.Value.ToShortDateString() : "";
                        ws.Cells[index, 7].Value = profile.QuoteExpirationDate != null ? profile.QuoteExpirationDate.Value.ToShortDateString() : "";
                        ws.Cells[index, 8].Value = profile.MarketSegment;
                        ws.Cells[index, 9].Value = profile.PlantName;
                        ws.Cells[index, 10].Value = profile.SalesStaffName;
                        ws.Cells[index, 11].Value = profile.TotalVolume;
                        ws.Cells[index, 12].Value = profile.TotalRevenue;
                        ws.Cells[index, 13].Value = profile.AvgSellingPrice;
                        ws.Cells[index, 14].Value = profile.Spread;
                        ws.Cells[index, 15].Value = profile.Profit;
                        ws.Cells[index, 16].Value = SIDAL.GetProjectQCRequirementNameList(profile.ProjectId);
                        ws.Cells[index, 17].Value = profile.BackupPlant;
                        index++;
                    }
                }


                Byte[] bin = p.GetAsByteArray();
                string file = Server.MapPath("~/Exports/" + filename);
                System.IO.File.WriteAllBytes(file, bin);

                return file;
            }
        }

        public ActionResult CleanUpQuotes()
        {
            SIDAL.CleanUpQuotesForReport();
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Quick Quote

        public ActionResult QuickQuote()
        {
            QuickQuoteModel model = new QuickQuoteModel(GetUser().UserId);
            return View(model);
        }

        // For quick quote
        public string QuickAddQuotationStandardMixForQQ(int plantId, int marketSegmentId, long formulationId, decimal toJob, decimal waitOnJob, decimal washMinutes, decimal returnMinutes, DateTime pricingMonth, int? fskPriceId = null, decimal? fskBasePrice = null)
        {
            MixFormulation mf = SIDAL.FindFormulation(formulationId);
            var standardMixId = mf.StandardMixId;
            QuotationStandardMixView model = new QuotationStandardMixView(plantId, marketSegmentId, standardMixId);
            QuotationMix mix = model.ToEntity();
            mix.Volume = 100;

            mix.MixCost = SIDAL.CalculateCurrentMixCost(standardMixId, plantId, pricingMonth);

            JObject mixObject = new JObject();
            if (ModelState.IsValid)
            {
                if (fskPriceId == null)
                {
                    mix.Price = mix.MixCost + mix.Spread;
                    mixObject["Price"] = mix.Price;
                }
                else
                {
                    SIMixIngredientPriceSheet sheet = new SIMixIngredientPriceSheet(standardMixId, plantId, pricingMonth);
                    FskCalculation calculation = new FskCalculation(sheet, fskPriceId.GetValueOrDefault(), fskBasePrice.GetValueOrDefault());
                    mixObject["Price"] = calculation.FinalPrice;
                    mixObject["Content"] = calculation.Content;
                }
                mix.QuotedDescription = mf.StandardMix.SalesDesc;
            }
            mix = SIDAL.CalculateQuotationStandardMixProperties(mix, plantId, toJob, waitOnJob, washMinutes, returnMinutes);

            mixObject["Id"] = mix.Id; //mix.Id;
            mixObject["MixId"] = mix.StandardMixId; //mix.StandardMixId;
            mixObject["Description"] = mix.QuotedDescription;
            mixObject["Cost"] = mix.MixCost;    //mix.MixCost;
            mixObject["Volume"] = 100;
            mixObject["Unload"] = mix.Unload;
            mixObject["AvgLoad"] = mix.AvgLoad;
            return mixObject.ToString();
        }

        public ActionResult Show5skCalculationsForPlant(int plantId, long standardMixId, long fskPriceId, decimal fskBasePrice, DateTime pricingMonth)
        {
            SIMixIngredientPriceSheet sheet = new SIMixIngredientPriceSheet(standardMixId, plantId, pricingMonth);
            FskCalculation calculation = new FskCalculation(sheet, fskPriceId, fskBasePrice);
            return View("FskCalculation", calculation);
        }

        public String GetPlantDefaults(int plantId)
        {
            Plant plant = SIDAL.GetPlant(plantId);
            District district = SIDAL.GetDistrict(plant.DistrictId);
            JObject plantObject = new JObject();
            plantObject["FskId"] = plant.FSKId;
            plantObject["FskPrice"] = plant.FSKPrice != null ? plant.FSKPrice.BasePrice : 0;
            plantObject["Wait"] = plant.WaitMinutes.GetValueOrDefault();
            plantObject["ToJob"] = district.ToJob.GetValueOrDefault();
            plantObject["Wash"] = district.Wash.GetValueOrDefault();
            plantObject["Return"] = district.Return.GetValueOrDefault();
            plantObject["FixedPrice"] = plant.DeliveryFixedCost.GetValueOrDefault() + plant.PlantFixedCost.GetValueOrDefault() + plant.SGA.GetValueOrDefault();
            plantObject["PlantTime"] = plant.TicketMinutes.GetValueOrDefault() + plant.LoadMinutes.GetValueOrDefault() + plant.TemperMinutes.GetValueOrDefault();
            plantObject["Utilization"] = plant.Utilization.GetValueOrDefault(1);
            plantObject["VariableCost"] = plant.VariableCostPerMin.GetValueOrDefault();
            return plantObject.ToString();
        }

        public String GetDistrictDefaults(int plantId, int marketSegmentId)
        {
            Plant plant = SIDAL.GetPlant(plantId);
            DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(marketSegmentId, plant.DistrictId);

            JObject districtDefaultsObject = new JObject();
            if (dms != null)
            {
                districtDefaultsObject["Spread"] = dms.Spread.GetValueOrDefault();
                districtDefaultsObject["Contribution"] = dms.ContMarg.GetValueOrDefault();
                districtDefaultsObject["CYDHr"] = dms.CydHr.GetValueOrDefault();
                districtDefaultsObject["Profit"] = dms.Profit.GetValueOrDefault();
            }

            return districtDefaultsObject.ToString();
        }

        public string Get5skBasePrice(long fskPriceId)
        {
            return SIDAL.FindFSKPrice(fskPriceId).BasePrice.ToString("N2");
        }

        public string Find5skPriceForPlant(int plantId, long standardMixId, long fskPriceId, decimal fskBasePrice, DateTime pricingMonth)
        {
            try
            {
                SIMixIngredientPriceSheet sheet = new SIMixIngredientPriceSheet(standardMixId, plantId);
                FskCalculation calculation = new FskCalculation(sheet, fskPriceId, fskBasePrice);
                decimal mixCost = SIDAL.CalculateCurrentMixCost(standardMixId, plantId, pricingMonth);
                JObject response = new JObject();
                response["price"] = calculation.FinalPrice.ToString("N2");
                response["mixCost"] = mixCost.ToString("N2");
                response["content"] = calculation.Content;
                response["name"] = SIDAL.FindStandardMix(standardMixId).SalesDesc;
                return response.ToString();
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public string FindStandardMixCostForPlant(int plantId, int marketSegmentId, long standardMixId, DateTime pricingMonth)
        {
            try
            {
                decimal mixCost = SIDAL.CalculateCurrentMixCost(standardMixId, plantId, pricingMonth);
                StandardMix mix = SIDAL.FindStandardMix(standardMixId);
                Plant p = SIDAL.GetPlant(plantId);
                DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(marketSegmentId, p.DistrictId);
                decimal spread = dms == null ? 0 : dms.Spread.GetValueOrDefault();
                JObject response = new JObject();
                response["mixCost"] = mixCost.ToString("N2");
                response["addonCost"] = 0;
                response["price"] = (mixCost + 0 + spread).ToString("N2");
                response["name"] = mix.SalesDesc;
                return response.ToString();
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        #endregion

        #region Customer / Contact / Basic info

        public ActionResult NewQuoteFromProject(ProjectView model)
        {
            Quotation q = new Quotation();

            //If no project selected 
            q.ProjectId = (model.NewQuoteProject > 0 ? model.NewQuoteProject : model.ProjectId);
            model.ProjectId = q.ProjectId.GetValueOrDefault();

            if (model.CopyFromQuoteId > 0)
            {
                //If no project selected 
                q.ProjectId = (model.NewQuoteProject > 0 ? model.NewQuoteProject : model.ProjectId);

                if (model.NewQuoteCustomer > 0)
                {
                    q.CustomerId = model.NewQuoteCustomer;
                    var cc = SIDAL.GetDefaultQuoteContact(q.CustomerId.GetValueOrDefault());
                    if (cc != null)
                        q.CustomerContactId = cc.Id;
                }
                q.TaxCodeId = SIDAL.GetTaxCodes().FirstOrDefault().Id;
                q.UserId = GetUser().UserId;

                SIDAL.UpdateQuotation(q);
                SIDAL.CopyQuotation(q.Id, model.CopyFromQuoteId, GetUser().UserId);
                return RedirectToAction("AddEditQuote", new { @id = q.Id });
            }
            else
            {
                QuotationPageModel newModel = new QuotationPageModel();
                newModel.UserId = GetUser().UserId;

                if (model.NewQuoteCustomer > 0)
                {
                    newModel.CustomerView.CustomerId = model.NewQuoteCustomer;
                    CustomerContact cc = SIDAL.GetDefaultQuoteContact(newModel.CustomerView.CustomerId);
                    if (cc != null)
                    {
                        newModel.CustomerView.CustomerContactId = cc.Id;
                        newModel.CustomerView.CustomerContactInfo = new CustomerContactView(cc);
                    }
                }

                if (model.ProjectId > 0)
                {
                    newModel.LoadList(model.ProjectId);
                    int projectId = model.ProjectId;
                    Project p = SIDAL.GetProject(projectId);
                    newModel.ProjectView.ProjectView = new ProjectView(p);
                    newModel.ProjectView.SalesStaffId = p.ProjectSalesStaffs.FirstOrDefault().SalesStaffId;
                    newModel.PlantId = SIDAL.GetQuotationConcretePlantId(p.ConcretePlantId.GetValueOrDefault(), p.ProjectId);
                    newModel.BackupPlantId = p.BackupPlantId;
                }
                ModelState.Remove("NewQuoteCustomer");
                ModelState.Remove("ProjectName");
                ModelState.Remove("ProjectId");
                return View("AddEditQuote", newModel);
            }
        }

        public ActionResult DeleteQuote(long id, int? projectId = null)
        {
            SIDAL.DeleteQuotation(id);
            if (projectId != null)
            {
                return RedirectToAction("EditProject", "Home", new { @id = projectId.Value, @selected = "quotes" });
            }
            else
            {
                return RedirectToAction("Index", "Quote");
            }
        }

        [APIFilter]
        public ActionResult AddEditQuote(long? id, string error = null)
        {
            ViewBag.Error = error;
            QuotationPageModel model = null;
            SIUser u = GetUser();
            if (id != null)
            {
                model = new QuotationPageModel(id.Value);
                //Check Margin Limit Violation Limits
                var role = "";
                if (model.Quote.EditingEnabledBy != null)
                    role = SIDAL.GetUser(model.Quote.EditingEnabledBy.GetValueOrDefault()).Role;
                else
                    role = u.Role;

                bool hasApprovalAccess = SIDAL.CheckApprovalAccess(id.GetValueOrDefault(), u.UserId);
                ViewBag.LimitViolations = SIDAL.GetMarginLimitViolations(id.GetValueOrDefault(), u.Role);
                if (hasApprovalAccess)
                {
                    ViewBag.UserCanApprove = hasApprovalAccess && (ViewBag.LimitViolations == null?true: ViewBag.LimitViolations.Count == 0);
                    //ViewBag.UserCanApprove = hasApprovalAccess;
                }
               
                model.UserId = u.UserId;
                model.Load();
                if (model.CustomerView.TaxCodeId == 0)
                    model.CustomerView.TaxCodeId = SIDAL.GetTaxCodes().FirstOrDefault().Id;
            }
            else
            {
                model = new QuotationPageModel();
                model.UserId = u.UserId;
                model.Load();
            }
            ViewBag.ChatProjectId = model.Project.ProjectId;
            ViewBag.ChatQuoteId = id;
            return View(model);
        }

        public ActionResult AddNewCustomerContact(QuotationPageModel model)
        {
            CustomerContact c = new CustomerContact();
            c.Name = model.CustomerView.NewCustomerContactInfo.Name;
            c.Phone = model.CustomerView.NewCustomerContactInfo.Phone;
            c.Fax = model.CustomerView.NewCustomerContactInfo.Fax;
            c.Email = model.CustomerView.NewCustomerContactInfo.Email;
            c.CustomerId = model.CustomerView.CustomerId;
            c.IsQuoteDefault = true;

            SIOperation operation = new SIOperation();

            operation.Type = SIOperationType.Add;

            operation.Item = c;
            List<SIOperation> operations = new List<SIOperation>();
            operations.Add(operation);

            SIDAL.ExecuteOperations(operations);

            Quotation q = null;
            if (model.QuotationId > 0)
            {
                SIDAL.UpdateQuotationCustomerContact(model.QuotationId, c.Id);
                SIDAL.UpdateQuotationCustomer(model.QuotationId, c.CustomerId);
                return RedirectToAction("AddEditQuote", new { @id = model.QuotationId });
            }
            else
            {
                q = new Quotation();
                q.UserId = GetUser().UserId;
                q.CustomerId = c.CustomerId;
                q.CustomerContactId = c.Id;
                SIDAL.UpdateQuotation(q);
                return RedirectToAction("AddEditQuote", new { @id = q.Id });
            }
        }

        public string UpdateQuoteMixDescription(long id, string description)
        {
            SIDAL.UpdateQuoteMixDescription(id, description);
            try
            {
                var quoteMix = SIDAL.FindQuotationMix(id);
                SIDAL.AddQuoteAuditLog(quoteMix.QuotationId.GetValueOrDefault(), "Mixes Updated", GetUser().Username, true);
            }
            catch { }
            return "OK";
        }

        public string UpdateQuoteMixPublicNotes(long id, string publicNotes)
        {
            SIDAL.UpdateQuoteMixPublicNotes(id, publicNotes);
            try
            {
                var quoteMix = SIDAL.FindQuotationMix(id);
                SIDAL.AddQuoteAuditLog(quoteMix.QuotationId.GetValueOrDefault(), "Mixes Updated", GetUser().Username, true);
            }
            catch { }
            return "OK";
        }

        [HttpPost]
        public ActionResult UpdateQuotation(QuotationPageModel model, List<long> QcReqIds)
        {
            if (model.CustomerView.NewCustomerInfo.Name != null)
            {
                ModelState.Remove("CustomerView.CustomerId");
            }
            else if (model.CustomerView.CustomerId > 0)
            {
                ModelState.Remove("CustomerView.NewCustomerInfo.Name");
                ModelState.Remove("CustomerView.NewCustomerInfo.Number");
            }
            else
            {
                ModelState.Remove("CustomerView.NewCustomerInfo.Name");
                ModelState.Remove("CustomerView.NewCustomerInfo.Number");
            }

            ModelState.Remove("CustomerView.NewCustomerInfo.Districts");
            ModelState.Remove("CustomerView.CustomerContactId");
            ModelState.Remove("CustomerView.NewCustomerContactInfo.Name");

            if (model.DistrictQcRequirement)
            {
                if (QcReqIds == null || QcReqIds.Contains(0))
                {
                    ModelState.AddModelError("QcReqIds", "Selection of QC Requirements is required");
                }
                else if (QcReqIds != null && !QcReqIds.Contains(5) && !QcReqIds.Contains(0) && model.BackupPlantId == null)
                {
                    ModelState.AddModelError("BackupPlantId", "Selection of Backup Plant is required");
                    model.BackupPlantId = null;
                }
            }

            if (ModelState.IsValid)
            {
                ProjectView p = model.ProjectView.ProjectView;
                if (model.PlantId > 0)
                {
                    Plant pl = SIDAL.GetPlant(model.PlantId);
                    District d = SIDAL.GetDistrict(pl.DistrictId);
                    if (p.ToJobMinutes == null)
                        p.ToJobMinutes = Convert.ToInt32(d.ToJob.GetValueOrDefault(0));
                    if (p.WashMinutes == null)
                        p.WashMinutes = Convert.ToInt32(d.Wash.GetValueOrDefault(0));
                    if (p.ReturnMinutes == null)
                        p.ReturnMinutes = Convert.ToInt32(d.Return.GetValueOrDefault(0));
                    if (p.WaitOnJob == null)
                        p.WaitOnJob = Convert.ToInt32(pl.WaitMinutes.GetValueOrDefault(0));
                }
                if (p.ProjectId > 0)
                {
                    SIDAL.UpdateProjectInfo(p.ProjectId, p.ProjectName, p.Address, p.City, p.State, p.Zipcode, p.MarketSegmentId,
                        p.CustomerRefName, model.PlantId.GetValueOrDefault(), p.ProjectStatusId, p.DistanceToJob, p.ToJobMinutes, p.WaitOnJob,
                        p.ReturnMinutes, p.WashMinutes, p.ExcludeFromReports, p.Active,model.BackupPlantId.GetValueOrDefault());
                }
                else
                {
                    Project project = new Project();
                    project.Name = p.ProjectName;
                    project.Address = p.Address;
                    project.City = p.City;
                    project.State = p.State;
                    project.ZipCode = p.Zipcode;
                    project.ConcretePlantId = model.PlantId;
                    project.MarketSegmentId = p.MarketSegmentId;
                    project.CustomerRefName = p.CustomerRefName;
                    project.ProjectStatusId = p.ProjectStatusId;
                    project.DistanceToJob = p.DistanceToJob;
                    Plant plant = SIDAL.GetPlant(project.ConcretePlantId);
                    District district = SIDAL.GetDistrict(plant.DistrictId);

                    if (p.ToJobMinutes != null)
                        project.ToJobMinutes = p.ToJobMinutes;
                    else
                        project.ToJobMinutes = Convert.ToInt32(district.ToJob.GetValueOrDefault(0));

                    if (p.WaitOnJob != null)
                        project.WaitOnJob = p.WaitOnJob;
                    else
                        project.WaitOnJob = Convert.ToInt32(plant.WaitMinutes.GetValueOrDefault(0));

                    if (p.WashMinutes != null)
                        project.WashMinutes = p.WashMinutes;
                    else
                        project.WashMinutes = Convert.ToInt32(district.Wash.GetValueOrDefault(0));

                    if (p.ReturnMinutes != null)
                        project.ReturnMinutes = p.ReturnMinutes;
                    else
                        project.ReturnMinutes = Convert.ToInt32(district.Return.GetValueOrDefault(0));
                    // Assing Customer on Project if Quote is created with new Project
                    if (model.CustomerView.CustomerId > 0)
                        project.CustomerId = model.CustomerView.CustomerId;

                    project.ExcludeFromReports = p.ExcludeFromReports;
                    project.Active = p.Active;
                    project.BackupPlantId = model.BackupPlantId.GetValueOrDefault();

                    SIOperation operation = new SIOperation();
                    operation.Type = SIOperationType.Add;
                    operation.Item = project;
                    SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));

                    p.ProjectId = project.ProjectId;

                    if (model.ProjectView.SalesStaffId > 0)
                        SIDAL.UpdateProjectSalesStaff(model.ProjectView.SalesStaffId, project.ProjectId);

                    if (project.ConcretePlantId > 0)
                    {
                        ProjectPlant pp = new ProjectPlant();
                        pp.PlantId = project.ConcretePlantId.GetValueOrDefault();
                        pp.ProjectId = project.ProjectId;
                        pp.Volume = 0;
                        SIDAL.UpdateProjectPlant(pp);
                    }
                }

                if (model.ActionClicked == "NewCustomer" && model.CustomerView.NewCustomerInfo.Name != null)
                {
                    Customer c = new Customer();
                    c.Active = true;
                    c.Name = model.CustomerView.NewCustomerInfo.Name;
                    c.CustomerNumber = model.CustomerView.NewCustomerInfo.Number;
                    c.CompanyId = GetUser().Company.CompanyId;

                    SIOperation operation = new SIOperation();
                    operation.Type = SIOperationType.Add;
                    operation.Item = c;
                    List<SIOperation> operations = new List<SIOperation>();
                    operations.Add(operation);

                    SIDAL.ExecuteOperations(operations);
                    SIDAL.UpdateCustomerDistrictsFromUser(c.CustomerId, GetUser().UserId);

                    model.CustomerView.CustomerId = c.CustomerId;
                }

                if (model.ActionClicked == "NewCustomerContact" && model.CustomerView.NewCustomerContactInfo.Name != null)
                {
                    CustomerContact c = new CustomerContact();
                    c.Name = model.CustomerView.NewCustomerContactInfo.Name;
                    c.Phone = model.CustomerView.NewCustomerContactInfo.Phone;
                    c.Fax = model.CustomerView.NewCustomerContactInfo.Fax;
                    c.Email = model.CustomerView.NewCustomerContactInfo.Email;
                    c.CustomerId = model.CustomerView.CustomerId;
                    c.IsQuoteDefault = true;

                    SIOperation operation = new SIOperation();

                    operation.Type = SIOperationType.Add;

                    operation.Item = c;
                    List<SIOperation> operations = new List<SIOperation>();
                    operations.Add(operation);

                    SIDAL.ExecuteOperations(operations);

                    model.CustomerView.CustomerContactId = c.Id;
                }

                if (model.QuotationId > 0)
                {
                    var user = GetUser();

                    SIDAL.UpdateQuotationBasicInfo(model.QuotationId, user.UserId, model.PlantId.GetValueOrDefault(), model.PricingMonthActual,
                                                    model.CustomerView.CustomerId, model.CustomerView.CustomerContactId,
                                                    model.CustomerView.TaxCodeId, model.CustomerView.TaxExcemptReason, model.ProjectView.SalesStaffId,model.DetailsView.CustomerNumberOnPDF);
                    SIDAL.UpdateQuotationProject(model.QuotationId, p.ProjectId);
                    SIDAL.AddQuoteAuditLog(model.QuotationId, "Job/Customer Information Updated", user.Username, true);
                }
                else
                {
                    Quotation q = new Quotation();
                    q.ProjectId = p.ProjectId;
                    q.PlantId = model.PlantId.GetValueOrDefault();
                    q.PricingMonth = model.PricingMonthActual;
                    if (model.CustomerView.CustomerContactId > 0)
                        q.CustomerContactId = model.CustomerView.CustomerContactId;

                    if (model.CustomerView.CustomerId > 0)
                        q.CustomerId = model.CustomerView.CustomerId;

                    if (model.ProjectView.ProjectView.PlantId > 0)
                        q.PlantId = model.ProjectView.ProjectView.PlantId;

                    if (model.CustomerView.TaxCodeId > 0)
                        q.TaxCodeId = model.CustomerView.TaxCodeId;

                    q.TaxExemptReason = model.CustomerView.TaxExcemptReason;
                    q.CreatedOn = DateTime.Today;
                    q.QuoteDate = DateTime.Today;
                    q.SalesStaffId = model.ProjectView.SalesStaffId;
                    District d = null;
                    if (model.PlantId > 0)
                    {
                        Plant pl = SIDAL.GetPlant(model.PlantId);
                        d = SIDAL.GetDistrict(pl.DistrictId);
                        q.AcceptanceExpirationDate = q.QuoteDate.Value.AddDays(d.AcceptanceExpiration.GetValueOrDefault(30));
                        q.QuoteExpirationDate = q.QuoteDate.Value.AddDays(d.QuoteExpiration.GetValueOrDefault(60));
                    }
                    q.UserId = GetUser().UserId;
                    q.Active = true;
                    q.BiddingDate = p.BidDate;
                    if (d != null)
                    {
                        q.IncludeAsLettingDate = d.LettingDate;
                        //q.CustomerNumberOnPDF = d.CustomerNumberOnPDF;
                    }
                    //if (model.DetailsView.CustomerNumberOnPDF)
                    //{
                    //    q.CustomerNumberOnPDF = true;
                    //}
                    //else
                    //{
                        q.CustomerNumberOnPDF = null;
                    //}
                    var defaultQuoteProductList = SIDAL.GetDefaultQuoteProducts(q.UserId.ToString()).Select(x => x.ProductTypeId);
                    foreach (var item in defaultQuoteProductList)
                    {
                        if (item == (int)ProductType.Aggregate)
                        {
                            q.AggregateEnabled = true;
                        }
                        else if (item == (int)ProductType.Block)
                        {
                            q.BlockEnabled = true;
                        }
                        else if (item == (int)ProductType.Concrete)
                        {
                            q.ConcreteEnabled = true;
                        }
                    }
                    if (q.PlantId > 0)
                    {
                        //Update Biddate if null

                        SIDAL.UpdateQuotation(q);
                        SIDAL.UpdateBidDateIfNull(q.ProjectId);
                        SIDAL.AddDefaultAddons(q.Id);
                    }
                    model.QuotationId = q.Id;
                }
                //SIDAL.UpdateProjectPriceSpreadProfit(p.ProjectId,model.QuotationId);
                SIDAL.UpdateProjectPlantVolume(p.ProjectId, model.QuotationId);
                SIDAL.UpdateProjectQCRequirement(p.ProjectId, QcReqIds);

                return RedirectToAction("AddEditQuote", new { @id = model.QuotationId });
            }
            else
            {
                model.UserId = GetUser().UserId;
                model.Load();
                return View("AddEditQuote", model);
            }
        }

        #endregion

        #region Job Information

        public ActionResult AddEditQuoteProject(long id)
        {
            QuotationProjectView model = null;
            model = new QuotationProjectView(id);
            model.UserId = GetUser().UserId;
            model.Load();
            return View(model);
        }

        public ActionResult AddNewProject(QuotationProjectView model)
        {
            Project p = new Project();
            p.Name = model.ProjectView.ProjectName;
            p.CustomerRefName = model.ProjectView.CustomerRefName;
            p.Address = model.ProjectView.Address;
            p.City = model.ProjectView.City;
            p.MarketSegmentId = model.ProjectView.MarketSegmentId;
            p.ConcretePlantId = model.ProjectView.PlantId;
            p.ToJobMinutes = model.ProjectView.ToJobMinutes.GetValueOrDefault();
            p.WaitOnJob = model.ProjectView.WaitOnJob.GetValueOrDefault();
            p.WashMinutes = model.ProjectView.WashMinutes.GetValueOrDefault();
            p.ReturnMinutes = model.ProjectView.ReturnMinutes.GetValueOrDefault();
            p.DistanceToJob = model.ProjectView.DistanceToJob;
            p.DeliveryInstructions = model.ProjectView.DeliveryInstructions;

            p.ProjectStatusId = SIDAL.GetProjectStatusForType(SIStatusType.Pipeline.Id).FirstOrDefault().ProjectStatusId;
            SIOperation operation = new SIOperation();

            operation.Type = SIOperationType.Add;

            operation.Item = p;
            List<SIOperation> operations = new List<SIOperation>();
            operations.Add(operation);
            SIDAL.ExecuteOperations(operations);

            SIDAL.CheckUpdateSingleProjectPlant(p.ProjectId);
            SIDAL.UpdateProjectSalesStaff(model.SalesStaffId, p.ProjectId);

            if (model.QuotationId > 0)
            {
                SIDAL.UpdateQuotationProject(model.QuotationId, p.ProjectId);
            }
            return RedirectToAction("AddEditQuoteProject", new { @id = model.QuotationId });
        }

        public ActionResult UpdateQuotationProject(QuotationProjectView model)
        {
            if (model.ProjectId > 0)
                SIDAL.UpdateQuotationProject(model.QuotationId, model.ProjectId);
            return RedirectToAction("AddEditQuoteProject", new { @id = model.QuotationId });
        }

        #endregion

        #region Quote Information

        public ActionResult AddEditQuoteDetails(long id)
        {
            QuotationDetailsView model = null;
            model = new QuotationDetailsView(id);
            model.UserId = GetUser().UserId;
            model.Load();
            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult UpdateQuoteDetails(QuotationPageModel model,string projectId)
        {
            if (model.QuotationId > 0)
            {
                SIDAL.UpdateQuotationDetails(
                    model.QuotationId, model.DetailsView.Active,
                    model.DetailsView.QuoteDate, model.DetailsView.AcceptanceExpirationDate,
                    model.DetailsView.QuoteExpiration,
                    model.DetailsView.PriceChangeDate1, model.DetailsView.PriceIncrease1,
                    model.DetailsView.PriceChangeDate2, model.DetailsView.PriceIncrease2,
                    model.DetailsView.PriceChangeDate3, model.DetailsView.PriceIncrease3,
                    model.DetailsView.PrivateNotes, model.DetailsView.PublicNotes,
                    model.DetailsView.Disclaimers,model.DetailsView.Disclosures,
                    model.DetailsView.TermsAndConditions,model.DetailsView.BiddingDate,model.DetailsView.IncludeAsLettingDate, Convert.ToInt32(projectId)
                );
                //SIDAL.UpdateQuotationAddons(model.QuotationId, model.AddOnsView.SelectedAddons);
            }

            SIDAL.AddQuoteAuditLog(model.QuotationId, "Quote Information Updated", GetUser().Username, true);
            return RedirectToAction("AddEditQuote", new { @id = model.QuotationId });
        }

        #endregion

        #region Quote Addons

        public ActionResult AddEditQuoteAddons(long id)
        {
            QuotationAddonsView model = null;
            model = new QuotationAddonsView(id);
            model.UserId = GetUser().UserId;
            model.Load();
            return View(model);
        }

        public ActionResult UpdateQuoteAddons(QuotationAddonsView model)
        {
            if (model.QuotationId > 0)
                SIDAL.UpdateQuotationAddons(model.QuotationId, model.SelectedAddons);
            return RedirectToAction("AddEditQuoteAddons", new { @id = model.QuotationId });
        }

        #endregion

        #region Quote Mixes

        public ActionResult AddEditQuoteMixes(long id)
        {
            QuotationMixView model = null;
            model = new QuotationMixView(id);
            model.UserId = GetUser().UserId;
            return View(model);
        }

        public string UpdateMixPositions(long[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                SIDAL.UpdateQuoteMixPosition(ids[i], i);
            }
            return "OK";
        }

        #endregion

        #region AddEditQuotationStandardMix

        [HttpPost]
        public String SearchMixFormulations(int plantId, int[] psi, string[] airs, string[] slumps, double[] ashes, double[] fineAggs, double[] sacks, string[] md1s, string[] md2s, string[] md3s, string[] md4s, long[] rawMaterialsIncluded, long[] rawMaterialsExcluded, DateTime? pricingMonth = null,long quoteId = 0)
        {
            //ILogger logger = new FileLogger();
            //string date = "Given Date = " + pricingMonth.ToString();
            //logger.LogError(date);
            if (quoteId == 0)
            {
                pricingMonth = DateUtils.GetFirstOf(pricingMonth.Value);
            }
            else
            {
                pricingMonth = SIDAL.FindQuotation(quoteId).PricingMonth;
            }
            //date = date + " ####"+quoteId+"##### Changed Date = " + pricingMonth.ToString();

            //logger.LogError(date);

            var formulations = SIDAL.SearchMixFormulations(plantId, pricingMonth.Value, psi, airs, slumps, ashes, fineAggs, sacks, md1s, md2s, md3s, md4s, rawMaterialsIncluded, rawMaterialsExcluded);
            var list = formulations.Select(x => x.FormulationId).ToList();
            var rawMaterials = SIDAL.GetCommonRawMaterialsForFomulations(list);
            var result = JsonConvert.SerializeObject(new { RawMaterials = rawMaterials, Formulations = formulations });
            return result;
        }

        public ActionResult AddQuotationStandardMix(long id)
        {
            Quotation q = SIDAL.FindQuotation(id);
            QuotationStandardMixView model = new QuotationStandardMixView(q);
            return View(model);
        }

        public string QuickAddQuotationStandardMix(long id, long? fskPriceId = null, long? formulationId = null)
        {
            Quotation q = SIDAL.FindQuotation(id);
            QuotationStandardMixView model = new QuotationStandardMixView(q);
            if (formulationId == null)
            {
                var mixes = model.ChooseStandardMix;

                if (mixes.Count() > 0)
                    model.StandardMixId = Int64.Parse(model.ChooseStandardMix.First().Value);
            }
            else
            {
                MixFormulation mf = SIDAL.FindFormulation(formulationId.Value);
                model.StandardMixId = mf.StandardMixId;
            }
            QuotationMix mix = model.ToEntity();
            mix.MixCost = SIDAL.CalculateCurrentMixCost(model.StandardMixId, model.PlantId, q.PricingMonth);
            JObject mixObject = new JObject();
            if (ModelState.IsValid)
            {
                if (fskPriceId == null)
                {
                    mix.Price = mix.MixCost + mix.Spread;
                    mixObject["Price"] = mix.Price;
                }
                else
                {
                    SIMixIngredientPriceSheet sheet = new SIMixIngredientPriceSheet(model.StandardMixId, q.PlantId.GetValueOrDefault());
                    FskCalculation calculation = new FskCalculation(sheet, fskPriceId.GetValueOrDefault(), q.FskBasePrice.GetValueOrDefault());
                    mixObject["Price"] = calculation.FinalPrice;
                    mixObject["Content"] = calculation.Content;
                }
                SIDAL.UpdateQuotationMix(mix);
                mix.QuotedDescription = SIDAL.ResetDefaultQuoteMixDescription(mix.Id);
            }
            SIDAL.UpdateQuotationStandardMixCalculations(mix.Id);

            try
            {
                SIDAL.AddQuoteAuditLog(id, "Standard Mix Details Updated", GetUser().Username, true);
            }
            catch
            {

            }

            mixObject["Id"] = mix.Id; //mix.Id;
            mixObject["MixId"] = mix.StandardMixId; //mix.StandardMixId;
            mixObject["Description"] = mix.QuotedDescription;
            mixObject["Cost"] = mix.MixCost;    //mix.MixCost;
            mixObject["Volume"] = mix.Volume;
            mixObject["Unload"] = mix.Unload;
            mixObject["AvgLoad"] = mix.AvgLoad;
            return mixObject.ToString();
        }

        public ActionResult Show5skCalculations(long id, long quoteMixId, long fskPriceId)
        {
            Quotation q = SIDAL.FindQuotation(id);
            long standardMixId = SIDAL.FindQuotationMix(quoteMixId).StandardMixId.GetValueOrDefault(0);
            SIMixIngredientPriceSheet sheet = new SIMixIngredientPriceSheet(standardMixId, q.PlantId.GetValueOrDefault(), q.PricingMonth);
            FskCalculation calculation = new FskCalculation(sheet, fskPriceId, q.FskBasePrice.GetValueOrDefault());
            return View("FskCalculation", calculation);
        }

        public string UpdateMixParameters(long mixId, double volume, decimal price, int unload, double averageLoad, long? standardMixId = null)
        {
            SIUser u = GetUser();
            JObject quoteObj = new JObject();
            bool status = false;
            try
            {
                QuotationMix mix = SIDAL.FindQuotationMix(mixId);

                try
                {
                    SIDAL.UpdateQuotationMix(mixId, standardMixId, volume, price, unload, averageLoad);
                    if (mix.StandardMixId == null)
                    {
                        SIDAL.UpdateQuotationCustomMixCalculations(mixId);
                    }
                    else
                    {
                        SIDAL.UpdateQuotationStandardMixCalculations(mixId);
                    }
                    quoteObj["Update"] = true;

                    try
                    {
                        SIDAL.AddQuoteAuditLog(mix.QuotationId.GetValueOrDefault(), "Mixes Updated", GetUser().Username, true);
                    }
                    catch { }
                }
                catch (Exception)
                {

                    quoteObj["Update"] = false;
                }
                Quotation quote = SIDAL.FindQuotation(mix.QuotationId.GetValueOrDefault());
                SIUser user = null;
                if (quote.EditingEnabledBy != null)
                    user = SIDAL.GetUser(quote.EditingEnabledBy.GetValueOrDefault());
                else
                    user = u;
                
                ViewBag.LimitViolations = SIDAL.GetMarginLimitViolations(mix.QuotationId.Value, user.Role);
                bool hasApprovalAccess = SIDAL.CheckApprovalAccess(mix.QuotationId.Value, user.UserId);
                if (hasApprovalAccess)
                {
                    status = hasApprovalAccess && (ViewBag.LimitViolations == null ? true : ViewBag.LimitViolations.Count == 0);
                    if (status)
                        quoteObj["ViolationError"] = "[]";
                    else
                        quoteObj["ViolationError"] = JsonConvert.SerializeObject(ViewBag.LimitViolations);
                }
                else
                {
                    if (quote.EditingEnabledBy != null)
                        status = true;

                    quoteObj["ViolationError"] = "[]";
                }
            }
            catch (Exception ex)
            {

            }
            quoteObj["ApprovalStatus"] = status;
            return quoteObj.ToString();
        }

        public ActionResult DeleteQuotationMix(long id, long? quotationId = null)
        {
            long quoteId = quotationId.GetValueOrDefault();
            QuotationMix mix = SIDAL.FindQuotationMix(id);
            if (mix != null)
            {
                quoteId = mix.QuotationId.GetValueOrDefault();
                SIDAL.DeleteQuotationMix(mix.Id);
            }
            return RedirectToAction("AddEditQuote", new { @id = quoteId });
        }

        public ActionResult FilterStandardMixes(QuotationStandardMixView model)
        {
            QuotationStandardMixView newModel = null;
            QuotationMix qm = SIDAL.FindQuotationMix(model.QuotationMixId);
            if (qm != null)
                newModel = new QuotationStandardMixView(qm);
            else
                newModel = new QuotationStandardMixView(SIDAL.FindQuotation(model.QuotationId));
            newModel.PSI = model.PSI;
            newModel.Air = model.Air;
            newModel.Slump = model.Slump;

            newModel.MD1 = model.MD1;
            newModel.MD2 = model.MD2;
            newModel.MD3 = model.MD3;
            newModel.MD4 = model.MD4;

            newModel.LoadProfile();
            ModelState.Clear();

            return View("AddQuotationStandardMix", newModel);
        }

        public ActionResult EditQuotationStandardMix(long id)
        {
            QuotationMix mix = SIDAL.FindQuotationMix(id);
            QuotationStandardMixView model = new QuotationStandardMixView(mix);
            model.SelectedAddon = new MixLevelAddonView();
            return View("AddQuotationStandardMix", model);
        }

        public string GetStandardMixFromFormulation(long formulationId)
        {
            return JsonConvert.SerializeObject(SIDAL.FindFormulation(formulationId).StandardMix);
        }

        public string FindStandardMixCost(long id, long quoteMixId, long standardMixId, decimal addonCost = 0)
        {
            Quotation q = SIDAL.FindQuotation(id);
            try
            {
                decimal mixCost = SIDAL.CalculateCurrentMixCost(standardMixId, q.PlantId.GetValueOrDefault(), q.PricingMonth);
                StandardMix mix = SIDAL.FindStandardMix(standardMixId);
                Plant p = SIDAL.GetPlant(q.PlantId.GetValueOrDefault());
                Project pr = SIDAL.GetProject(q.ProjectId.GetValueOrDefault());
                DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(pr.MarketSegmentId.GetValueOrDefault(), p.DistrictId);
                decimal spread = dms == null ? 0 : dms.Spread.GetValueOrDefault();
                JObject response = new JObject();
                response["mixCost"] = mixCost.ToString("N2");
                response["addonCost"] = addonCost.ToString("N2");
                response["price"] = (mixCost + addonCost + spread).ToString("N2");
                response["name"] = SIDAL.GenerateDefaultQuoteMixDescription(quoteMixId, standardMixId);
                return response.ToString();
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        [HttpGet]
        public string Update5skPriceInQuote(long id, long? fskPriceId = null)
        {
            var price = SIDAL.Update5skPrice(id, fskPriceId).ToString("N2");

            try
            {
                SIDAL.AddQuoteAuditLog(id, "Mixes Updated", GetUser().Username, true);
            }
            catch { }

            return price;
        }

        [HttpGet]
        public string Update5skBasePrice(long id, decimal fskPrice = 0)
        {
            var res = SIDAL.Update5skPrice(id, fskPrice);
            try
            {
                SIDAL.AddQuoteAuditLog(id, "Mixes Updated", GetUser().Username);
            }
            catch
            { }
            return res.ToString("N2"); ;
        }

        public string Find5skPrice(long id, long quoteMixId, long standardMixId, long fskPriceId)
        {
            Quotation q = SIDAL.FindQuotation(id);
            try
            {
                SIMixIngredientPriceSheet sheet = new SIMixIngredientPriceSheet(standardMixId, q.PlantId.GetValueOrDefault(), q.PricingMonth);
                FskCalculation calculation = new FskCalculation(sheet, fskPriceId, q.FskBasePrice.GetValueOrDefault());
                decimal mixCost = SIDAL.CalculateCurrentMixCost(standardMixId, q.PlantId.GetValueOrDefault(), q.PricingMonth);
                JObject response = new JObject();
                response["price"] = calculation.FinalPrice.ToString("N2");
                response["mixCost"] = mixCost.ToString("N2");
                response["content"] = calculation.Content;
                response["name"] = SIDAL.GenerateDefaultQuoteMixDescription(quoteMixId, standardMixId);
                return response.ToString();
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public string CalculateMixMetrics(long id, double volume, decimal price, decimal mixCost, decimal addonCost, double unload, double averageLoad)
        {
            Quotation q = SIDAL.FindQuotation(id);
            try
            {
                decimal spread = price - addonCost - mixCost;
                decimal fixedCost = SIDAL.FindPlantFixedCosts(q.PlantId.GetValueOrDefault());
                int numberOfTrips = (int)Math.Ceiling(volume / averageLoad);

                Plant plant = SIDAL.GetPlant(q.PlantId.GetValueOrDefault());
                Project project = SIDAL.GetProject(q.ProjectId.GetValueOrDefault());
                double ticketedTime = plant.TicketMinutes.GetValueOrDefault(0) + plant.LoadMinutes.GetValueOrDefault(0) + plant.TemperMinutes.GetValueOrDefault(0);
                ticketedTime = ticketedTime + project.ToJobMinutes.GetValueOrDefault(0) + project.WaitOnJob.GetValueOrDefault(0) + project.WashMinutes.GetValueOrDefault(0) + project.ReturnMinutes.GetValueOrDefault(0);
                ticketedTime = ticketedTime + unload;
                double roundTripTime = (double)(ticketedTime / Convert.ToDouble(plant.Utilization.GetValueOrDefault(1)));
                double perHourRate = averageLoad / roundTripTime * 60.0;
                double totalTimeSpent = numberOfTrips * roundTripTime;
                decimal deliveryCost = plant.VariableCostPerMin.GetValueOrDefault(0) * Convert.ToDecimal(totalTimeSpent);

                decimal variableDeliveryCost = deliveryCost / Convert.ToDecimal(volume);

                decimal contribution = spread - variableDeliveryCost;
                decimal profit = contribution - fixedCost;

                JObject response = new JObject();
                response["spread"] = spread.ToString("N2");
                response["contribution"] = contribution.ToString("N2");
                response["profit"] = profit.ToString("N2");
                return response.ToString();

            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public ActionResult CreateQuotationStandardMix(QuotationStandardMixView model)
        {
            QuotationMix mix = model.ToEntity();
            bool hasZeroCostContituents = !SIDAL.CheckConstituentValidity(model.StandardMixId, model.PlantId);
            if (hasZeroCostContituents)
            {
                ModelState.AddModelError("", "The choosen mix has constituents without costs, and cannot be added. Please review the cost setup.");
            }
            mix.MixCost = SIDAL.CalculateCurrentMixCost(model.StandardMixId, model.PlantId);
            if (ModelState.IsValid)
            {
                SIDAL.UpdateQuotationMix(mix);
                if (model.QuotationMixId == 0)
                {
                    SIDAL.ResetDefaultQuoteMixDescription(mix.Id);
                    mix.Price = mix.MixCost + mix.Spread;
                }
                SIDAL.UpdateQuotationStandardMixCalculations(mix.Id);
                try
                {
                    SIDAL.AddQuoteAuditLog(mix.QuotationId.GetValueOrDefault(), "Standard Mix Details Updated", GetUser().Username, true);
                }
                catch
                { }
                return RedirectToAction("EditQuotationStandardMix", new { @id = mix.Id });
            }
            else
            {
                model.LoadProfile();
                model.SelectedAddon = new MixLevelAddonView();
                return View("AddQuotationStandardMix", model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Quotation Id</param>
        /// <returns></returns>
        public ActionResult GetRawMaterials(long id)
        {
            Quotation quote = SIDAL.FindQuotation(id);

            var rawMaterials = SIDAL.GetNonZeroRawMaterials(quote.PlantId.GetValueOrDefault()).Select(s => new
            {
                Name = s.RawMaterialType.Name + " - " + s.MaterialCode + " - " + s.Description,
                Id = s.Id
            });
            return Json(rawMaterials, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Quotation Mix Id</param>
        /// <returns></returns>
        public ActionResult GetAddOns(long id)
        {
            QuotationMix m = SIDAL.FindQuotationMix(id);
            var list = SIDAL.GetActiveAddons("Mix", m.Quotation.PlantId.GetValueOrDefault())
                            .Where(x => x.MixUom.Name != "N.A.")
                            .Select(x => new
                            {
                                Text = x.Code + " - " + x.Description + " (Per " + x.MixUom.Name + ")",
                                Value = x.Id
                            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuantityUoms()
        {
            var list = SIDAL.GetUOMS()
                            .Where(x => x.Category == "Weight" || x.Category == "Volume")
                            .OrderBy(x => x.Category)
                            .ThenBy(x => x.Priority2)
                            .Select(x => new
                            {
                                Id = x.Id,
                                Name = x.Name
                            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCostUoms()
        {
            var list = SIDAL.GetUOMS()
                            .Where(x => x.Category == "Weight" || x.Category == "Volume")
                            .OrderBy(x => x.Category)
                            .ThenBy(x => x.Priority)
                            .Select(x => new
                            {
                                Id = x.Id,
                                Name = x.Name
                            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCostUomsByPriority2()
        {
            var list = SIDAL.GetUOMS()
                            .Where(x => x.Category == "Weight" || x.Category == "Volume")
                            .OrderBy(x => x.Category)
                            .ThenBy(x => x.Priority2)
                            .Select(x => new
                            {
                                Id = x.Id,
                                Name = x.Name
                            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveCustomMixConstituents(string data)
        {
            var constituents = JsonConvert.DeserializeObject<CustomMixConstituentsPayload>(data);
            constituents.Save();
            return Content("OK");
        }

        public ActionResult EditMixLevelAddon(long id)
        {
            MixLevelAddon entity = SIDAL.FindMixLevelAddon(id);
            QuotationMix mix = SIDAL.FindQuotationMix(entity.QuotationMixId);
            QuotationStandardMixView model = new QuotationStandardMixView(mix);
            model.SelectedAddon = new MixLevelAddonView(entity);
            return View("AddQuotationStandardMix", model);
        }

        public ActionResult DeleteMixLevelAddon(long id)
        {
            MixLevelAddon entity = SIDAL.FindMixLevelAddon(id);
            QuotationMix mix = SIDAL.FindQuotationMix(entity.QuotationMixId);
            QuotationStandardMixView model = new QuotationStandardMixView(mix);
            model.SelectedAddon = new MixLevelAddonView();
            SIDAL.DeleteMixLevelAddon(id);
            SIDAL.UpdateQuotationStandardMixCalculations(model.QuotationMixId, true);
            return RedirectToAction("EditQuotationStandardMix", new { @id = mix.Id });
        }

        public ActionResult RefreshQuotationMixDescription(long id)
        {
            SIDAL.ResetDefaultQuoteMixDescription(id);
            return RedirectToAction("EditQuotationStandardMix", new { @id = id });
        }

        public ActionResult SaveMixLevelAddon(QuotationStandardMixView model)
        {
            MixLevelAddon mixAddon = model.SelectedAddon.ToEntity();
            mixAddon.QuotationMixId = model.QuotationMixId;
            Quotation q = SIDAL.FindQuotation(model.QuotationId);
            Plant p = SIDAL.GetPlant(q.PlantId.Value);
            mixAddon.Cost = SIDAL.FindAddonMixCost(mixAddon.AddonId, p.DistrictId, q.PricingMonth);
            if (mixAddon.Cost > 0)
            {
                SIDAL.UpdateMixLevelAddon(mixAddon);
                SIDAL.ResetDefaultQuoteMixDescription(model.QuotationMixId);
                SIDAL.UpdateQuotationStandardMixCalculations(model.QuotationMixId, true);
                try
                {
                    SIDAL.AddQuoteAuditLog(q.Id, "Standard Mix Details Updated", GetUser().Username, true);
                }
                catch
                { }

            }
            else
            {
                TempData["AddonError"] = "Addons with 0 cost cannot be added";
            }
            return RedirectToAction("EditQuotationStandardMix", new { @id = model.QuotationMixId });
        }

        #endregion

        #region AddEditQuotationCustomMix

        public ActionResult AddQuotationCustomMix(long id)
        {
            Quotation q = SIDAL.FindQuotation(id);
            QuotationCustomMixView model = new QuotationCustomMixView(q);
            model.LoadProfile();
            model.AddonConstituent = new CustomMixConstituentView();
            model.RawMaterialConstituent = new CustomMixConstituentView();
            model.NonStandardConstituent = new CustomMixConstituentView();
            ViewBag.SelectedTab = "Addon";
            return View(model);
        }

        public ActionResult EditQuotationCustomMix(long id, string showTab)
        {
            QuotationMix mix = SIDAL.FindQuotationMix(id);
            QuotationCustomMixView model = new QuotationCustomMixView(mix);
            model.AddonConstituent = new CustomMixConstituentView(mix.Id);
            model.RawMaterialConstituent = new CustomMixConstituentView(mix.Id);
            model.NonStandardConstituent = new CustomMixConstituentView(mix.Id);

            model.RawMaterialConstituent.PlantId = mix.Quotation.PlantId.GetValueOrDefault();
            ViewBag.ShowTab = showTab;
            return View("AddQuotationCustomMix", model);
        }

        public ActionResult CreateQuotationCustomMix(QuotationCustomMixView model)
        {
            QuotationMix mix = model.ToEntity();
            if (ModelState.IsValid)
            {
                SIDAL.UpdateQuotationMix(mix);
                SIDAL.UpdateQuotationCustomMixCalculations(mix.Id);
            }
            if (mix.Id > 0)
            {
                try
                {
                    SIDAL.AddQuoteAuditLog(mix.QuotationId.GetValueOrDefault(), "Custom Mix Details Updated", GetUser().Username, true);
                }
                catch { }
                return RedirectToAction("EditQuotationCustomMix", new { @id = mix.Id });
            }
            else
            {
                model.LoadProfile();
                return View("AddQuotationCustomMix", model);
            }
        }

        public ActionResult EditCustomMixConstituent(long id)
        {
            CustomMixConstituent constituent = SIDAL.FindCustomMixConstituent(id);
            QuotationMix mix = SIDAL.FindQuotationMix(constituent.QuotationMixId);
            QuotationCustomMixView model = new QuotationCustomMixView(mix);

            if (constituent.AddonId != null)
            {
                ViewBag.ShowTab = "Addon";
                model.AddonConstituent = new CustomMixConstituentView(constituent);
            }
            else
                model.AddonConstituent = new CustomMixConstituentView(mix.Id);

            if (constituent.RawMaterialId != null)
            {
                ViewBag.ShowTab = "RawMat";
                model.RawMaterialConstituent = new CustomMixConstituentView(constituent);
            }
            else
                model.RawMaterialConstituent = new CustomMixConstituentView(mix.Id);

            model.RawMaterialConstituent.PlantId = mix.Quotation.PlantId.GetValueOrDefault();

            if (constituent.RawMaterialId == null && constituent.AddonId == null)
            {
                ViewBag.ShowTab = "CusMat";
                model.NonStandardConstituent = new CustomMixConstituentView(constituent);
            }
            else
                model.NonStandardConstituent = new CustomMixConstituentView(mix.Id);

            ViewBag.ScrollToTabs = true;
            return View("AddQuotationCustomMix", model);
        }

        public ActionResult DeleteCustomMixConstituent(long id)
        {
            try
            {
                var query = SIDAL.FindCustomMixConstituent(id);
                SIDAL.DeleteCustomMixConstituent(id);
                try
                {
                    long quotationId = SIDAL.GetQuotationIdByMixConstituentId(id);
                    SIDAL.AddQuoteAuditLog(quotationId, "Custom Mix Details Updated", GetUser().Username, true);
                }
                catch { }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
            //return RedirectToAction("EditQuotationCustomMix", new { @id = query.QuotationMixId });
        }

        public ActionResult UpdateCustomMixAddon(QuotationCustomMixView model)
        {
            CustomMixConstituent entity = model.AddonConstituent.ToEntity();
            Quotation q = SIDAL.FindQuotation(model.QuotationId);
            District d = SIDAL.GetDistrict(SIDAL.GetPlant(q.PlantId).DistrictId);
            Addon addon = SIDAL.FindAddon(model.AddonConstituent.AddonId);
            entity.QuotationMixId = model.QuotationMixId;
            entity.Description = addon.Code + " - " + addon.Description;
            entity.Cost = SIDAL.FindAddonMixCost(model.AddonConstituent.AddonId, d.DistrictId);
            if (entity.Cost > 0)
            {
                SIDAL.UpdateCustomMixConstituent(entity);
                ViewBag.ScrollToTabs = true;
            }
            else
            {
                TempData["AddonError"] = "Addons with 0 cost cannot be added";
            }

            return RedirectToAction("EditQuotationCustomMix", new { @id = entity.QuotationMixId, @showTab = "Addon" });
        }

        public ActionResult UpdateCustomRawMaterial(QuotationCustomMixView model)
        {
            CustomMixConstituent entity = model.RawMaterialConstituent.ToEntity();
            RawMaterial material = SIDAL.FindRawMaterial(model.RawMaterialConstituent.RawMaterialId);
            Quotation q = SIDAL.FindQuotation(model.QuotationId);
            Plant p = SIDAL.GetPlant(q.PlantId);
            entity.QuotationMixId = model.QuotationMixId;
            entity.Description = material.MaterialCode + " - " + material.Description;
            RawMaterialCostProjection projection = SIDAL.FindRawMaterialCost(model.RawMaterialConstituent.RawMaterialId, p.PlantId);
            if (projection != null && projection.Cost > 0)
            {
                entity.Cost = projection.Cost;
                entity.CostUomId = projection.UomId;
                entity.IsCementitious = material.RawMaterialType.IsCementitious;
                SIDAL.UpdateCustomMixConstituent(entity);
                ViewBag.ScrollToTabs = true;
            }
            else
            {
                TempData["RawMatError"] = "Cannot add Raw Material with 0 cost.";
            }
            return RedirectToAction("EditQuotationCustomMix", new { @id = entity.QuotationMixId, @showTab = "RawMat" });
        }

        public ActionResult UpdateCustomNonStandardConstituent(QuotationCustomMixView model)
        {
            CustomMixConstituent entity = model.NonStandardConstituent.ToEntity();
            entity.QuotationMixId = model.QuotationMixId;
            if (entity.Cost > 0)
            {
                SIDAL.UpdateCustomMixConstituent(entity);
                ViewBag.ScrollToTabs = true;
            }
            else
            {
                TempData["CusMatError"] = "The cost of the ingredient must be greater than 0";
            }
            return RedirectToAction("EditQuotationCustomMix", new { @id = entity.QuotationMixId, @showTab = "CusMat" });
        }

        #region Quick Add

        [HttpPost]
        public ActionResult QuickAddRawMaterial(long quotationMixId)
        {
            QuoteCustomMixConstituentResponse data = new QuoteCustomMixConstituentResponse();
            try
            {
                var mixConst = SIDAL.QuickAddRawMaterialCustomMixConstituent(quotationMixId);
                data.CustomMixConstId = mixConst.Id;
                data.Quantity = mixConst.Quantity;
                data.Cost = mixConst.Cost.GetValueOrDefault();
                data.CostUomName = SIDAL.GetUOMS().Where(x => x.Id == mixConst.CostUomId).Select(x => x.Name).FirstOrDefault();
                data.Success = true;
            }
            catch
            {
                data.Message = "Could not add Raw Material";
            }
            return Json(data);
        }

        [HttpPost]
        public ActionResult QuickAddAddOn(long quotationMixId)
        {
            QuoteCustomMixConstituentResponse data = new QuoteCustomMixConstituentResponse();
            try
            {
                CustomMixConstituent mixConst = SIDAL.QuickAddAddOnCustomMixConstituent(quotationMixId);
                data.CustomMixConstId = mixConst.Id;
                data.Cost = mixConst.Cost.GetValueOrDefault();
                data.CostUomName = SIDAL.GetUOMS().Where(x => x.Id == mixConst.CostUomId).Select(x => x.Name).FirstOrDefault(); ;
                data.Quantity = mixConst.Quantity;
                data.Success = true;
            }
            catch (ApplicationException ex)
            {
                data.Message = ex.Message;
            }
            catch (Exception ex)
            {
                data.Message = "Could not add Add On";
            }
            return Json(data);
        }

        [HttpPost]
        public ActionResult QuickAddNonStandardConstituent(long quotationMixId)
        {
            QuoteCustomMixConstituentResponse data = new QuoteCustomMixConstituentResponse();
            try
            {
                CustomMixConstituent mixConst = SIDAL.QuickAddNonStandardConstMixConstituent(quotationMixId);
                data.CustomMixConstId = mixConst.Id;
                data.Success = true;
                data.Description = mixConst.Description;
                data.Quantity = mixConst.Quantity;
                data.Cost = mixConst.Cost.GetValueOrDefault();
                data.Message = "Mix constituent added successfully";
            }
            catch
            {
                data.Message = "Could not add Non Standard Mix Constituent";
            }
            return Json(data);
        }

        [HttpPost]
        public ActionResult SaveCustomMixConstituentRawMaterial(CustomMixConstituentView mixConstituent)
        {
            QuoteCustomMixConstituentResponse res = new QuoteCustomMixConstituentResponse();
            try
            {
                QuotationMix qMix = SIDAL.FindQuotationMix(mixConstituent.QuotationMixId);
                Quotation q = SIDAL.FindQuotation(qMix.QuotationId.GetValueOrDefault());
                Plant p = SIDAL.GetPlant(q.PlantId);

                CustomMixConstituent cMixConst = new CustomMixConstituent();
                cMixConst.Id = mixConstituent.Id;
                cMixConst.RawMaterialId = mixConstituent.RawMaterialId;
                cMixConst.QuantityUomId = mixConstituent.QuantityUomId;
                cMixConst.Quantity = mixConstituent.Quantity;
                cMixConst.PerCementWeight = mixConstituent.PerCementWeight;
                cMixConst.QuotationMixId = mixConstituent.QuotationMixId;

                //CustomMixConstituent entity = model.RawMaterialConstituent.ToEntity();
                RawMaterial material = SIDAL.FindRawMaterial(mixConstituent.RawMaterialId);
                cMixConst.Description = material.MaterialCode + " - " + material.Description;
                RawMaterialCostProjection projection = SIDAL.FindRawMaterialCost(mixConstituent.RawMaterialId, p.PlantId, q.PricingMonth);
                if (projection != null && projection.Cost > 0)
                {
                    cMixConst.Cost = projection.Cost;
                    cMixConst.CostUomId = projection.UomId;
                    cMixConst.IsCementitious = material.RawMaterialType.IsCementitious;
                }

                try
                {
                    long quotationId = SIDAL.GetQuotationIdByMixConstituentId(cMixConst.Id);
                    SIDAL.AddQuoteAuditLog(quotationId, "Custom Mix Details Updated", GetUser().Username, true);
                }
                catch { }

                SIDAL.UpdateCustomMixConstituent(cMixConst, true);

                res.Cost = cMixConst.Cost.GetValueOrDefault();
                res.CostUomName = SIDAL.FindUOM(cMixConst.CostUomId.GetValueOrDefault()).Name;
                res.Success = true;
                res.Message = "Mix Constituent saved successfully";
            }
            catch
            {
                res.Message = "Could not save the Mix Constituent";
            }

            return Json(res);
        }

        [HttpPost]
        public ActionResult SaveCustomMixConstituentAddon(CustomMixConstituentView mixConstituent)
        {
            QuoteCustomMixConstituentResponse res = new QuoteCustomMixConstituentResponse();
            try
            {
                QuotationMix qMix = SIDAL.FindQuotationMix(mixConstituent.QuotationMixId);
                Quotation q = SIDAL.FindQuotation(qMix.QuotationId.GetValueOrDefault());
                Plant p = SIDAL.GetPlant(q.PlantId);
                District d = SIDAL.GetDistrict(SIDAL.GetPlant(q.PlantId).DistrictId);

                CustomMixConstituent custMixConst = new CustomMixConstituent();
                Addon addon = SIDAL.FindAddon(mixConstituent.AddonId);
                custMixConst.Id = mixConstituent.Id;
                custMixConst.AddonId = mixConstituent.AddonId;
                custMixConst.QuotationMixId = qMix.Id;
                custMixConst.Quantity = mixConstituent.Quantity;
                custMixConst.Description = addon.Code + " - " + addon.Description;
                //custMixConst.Cost = SIDAL.FindAddonMixCost(mixConstituent.AddonId, d.DistrictId, q.PricingMonth);
                var priceProjection = SIDAL.FindAddonMixPriceProjection(mixConstituent.AddonId, d.DistrictId, q.PricingMonth);
                if (priceProjection != null)
                {
                    custMixConst.Cost = priceProjection.Price;
                    custMixConst.CostUomId = priceProjection.UomId;
                }
                else
                {
                    custMixConst.Cost = 0;
                }
                if (custMixConst.Cost > 0)
                {
                    SIDAL.UpdateCustomMixConstituent(custMixConst, true);
                    try
                    {
                        SIDAL.AddQuoteAuditLog(q.Id, "Custom Mix Details Updated", GetUser().Username, true);
                    }
                    catch { }
                }
                res.Message = "Mix Constituent saved successfully";

                res.CustomMixConstId = custMixConst.Id;
                res.Quantity = custMixConst.Quantity;
                res.Cost = custMixConst.Cost.GetValueOrDefault();
                res.CostUomName = SIDAL.GetUOMS().Where(x => x.Id == custMixConst.CostUomId).Select(x => x.Name).FirstOrDefault(); ;
                res.Success = true;
            }
            catch
            {
                res.Message = "Could not save the Mix Constituent";
            }

            return Json(res);
        }

        [HttpPost]
        public ActionResult SaveCustomMixConstituentNonStandardConst(CustomMixConstituentView mixConstituent)
        {
            QuoteCustomMixConstituentResponse res = new QuoteCustomMixConstituentResponse();
            try
            {
                CustomMixConstituent entity = new CustomMixConstituent();
                entity.Id = mixConstituent.Id;
                entity.QuotationMixId = mixConstituent.QuotationMixId;
                entity.Description = mixConstituent.Description;
                entity.IsCementitious = mixConstituent.IsCementitious;
                entity.Quantity = mixConstituent.Quantity;
                entity.QuantityUomId = mixConstituent.QuantityUomId;
                entity.Cost = mixConstituent.Cost;
                entity.CostUomId = mixConstituent.CostUomId;
                entity.PerCementWeight = mixConstituent.PerCementWeight;
                if (entity.Cost > 0)
                {
                    SIDAL.UpdateCustomMixConstituent(entity, true);

                    try
                    {
                        long quotationId = SIDAL.GetQuotationIdByMixConstituentId(entity.Id);
                        SIDAL.AddQuoteAuditLog(quotationId, "Custom Mix Details Updated", GetUser().Username, true);
                    }
                    catch { }
                }

                res.Description = entity.Description;
                res.Quantity = entity.Quantity;
                res.Cost = entity.Cost.GetValueOrDefault();
                res.Message = "Mix Constituent saved successfully";
                res.Success = true;
            }
            catch
            {
                res.Message = "Could not save the Mix Constituent";
            }
            return Json(res);
        }

        [HttpPost]
        public ActionResult GetQuotationMixCalculations(long id)
        {
            QuotationMix qm = SIDAL.FindQuotationMix(id);
            JObject obj = new JObject();
            obj.Add("MixCost", qm.MixCost.GetValueOrDefault().ToString("N2"));
            obj.Add("AddOnCost", qm.AddonCost.GetValueOrDefault().ToString("N2"));
            obj.Add("Spread", qm.Spread.GetValueOrDefault().ToString("N2"));
            obj.Add("Contribution", qm.Contribution.GetValueOrDefault().ToString("N2"));
            obj.Add("Profit", qm.Profit.GetValueOrDefault().ToString("N2"));

            obj.Add("Price", SIDAL.UpdateCustomMixDefaultPrice(id));

            return Json(JsonConvert.SerializeObject(obj));
        }

        #endregion

        #endregion

        #region AuditLogs

        public ActionResult ShowAuditLogs(long id)
        {
            QuotationAuditLogView model = null;
            model = new QuotationAuditLogView(id);
            model.UserId = GetUser().UserId;
            model.Load();
            return View(model);
        }

        #endregion

        #region Preview Quotation

        [HttpGet]
        public ActionResult PreviewQuotation(long id)
        {
            QuotationPreview model = new QuotationPreview(id);
            model.Load();
            SIUser u = GetUser();
            bool hasApprovalAccess = SIDAL.CheckApprovalAccess(id, u.UserId);
            if (hasApprovalAccess)
            {
                ViewBag.ForApproval = true;
            }
            try
            {
                ViewBag.ChatProjectId = model.Profile.ProjectId;
                ViewBag.ChatQuoteId = model.Profile.QuoteId;
            }
            catch
            {
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult PreviewQuotation(QuotationPreview model)
        {
            // Save the email to customer data from the model before loading the profile
            String[] emailRecipients = null;
            string[] ccRecipients = null;
            string emailSubject = null;
            string emailText = null;
            var includeSupplement = model.IncludeQuoteSupplement;
            var utils = new AwsUtils();

            if (model.SelectedAction == "Email")
            {
                emailRecipients = model.Profile.EmailRecipients.Split(new char[] { ',' });
                ccRecipients = model.Profile.CCRecipients;
                emailSubject = model.Profile.EmailSubject;
                emailText = model.Profile.EmailText;
            }

            // Load the profile now.
            model.Load(false);
            if (model.SelectedAction == "QuoteSetting")
            {
                QuotationFormSetting qfs = new QuotationFormSetting();
                qfs.Id = model.QuoteFormSettingId;
                qfs.QuoteId = model.Id;
                qfs.PriceInclude = model.PriceInclude;
                qfs.PriceSequence = model.PriceSequence;
                qfs.QuantityInclude = model.QuantityInclude;
                qfs.QuantitySequence = model.QuantitySequence;
                qfs.MixIdInclude = model.MixIdInclude;
                qfs.MixIdSequence = model.MixIdSequence;
                qfs.DescriptionInclude = model.DescriptionInclude;
                qfs.DescriptionSequence = model.DescriptionSequence;
                qfs.PsiInclude = model.PsiInclude;
                qfs.PsiSequence = model.PsiSequence;
                qfs.PublicCommentsInclude = model.PublicCommentsInclude;
                qfs.PublicCommentsSequence = model.PublicCommentsSequence;
                qfs.SlumpInclude = model.SlumpInclude;
                qfs.SlumpSequence = model.SlumpSequence;
                qfs.AirInclude = model.AirInclude;
                qfs.AirSequence = model.AirSequence;
                qfs.AshInclude = model.AshInclude;
                qfs.AshSequence = model.AshSequence;
                qfs.FineAggInclude = model.FineAggInclude;
                qfs.FineAggSequence = model.FineAggSequence;
                qfs.SacksInclude = model.SacksInclude;
                qfs.SacksSequence = model.SacksSequence;
                qfs.MD1Include = model.MD1Include;
                qfs.MD1Sequence = model.MD1Sequence;
                qfs.MD2Include = model.MD2Include;
                qfs.MD2Sequence = model.MD2Sequence;
                qfs.MD3Include = model.MD3Include;
                qfs.MD3Sequence = model.MD3Sequence;
                qfs.MD4Include = model.MD4Include;
                qfs.MD4Sequence = model.MD4Sequence;

                SIDAL.UpdateQuoteFormSetting(qfs);

            }
            SIUser u = GetUser();
            bool hasApprovalAccess = SIDAL.CheckApprovalAccess(model.Profile.QuoteId, u.UserId);
            if (hasApprovalAccess)
            {
                ViewBag.ForApproval = true;
            }
            string tempPath = Server.MapPath("~/tempAttachmentFiles/" + u.Username + "");
            if (model.SelectedAction == "Print" || model.SelectedAction == "Email")
            {
                string logoPath = Server.MapPath("~/Content/images/logo.jpg");
                string quotePDFPath = Server.MapPath("~/tmp/tempQuotation.pdf");
                string finalQuotePDFPath = Server.MapPath("~/tmp/Quotation_" + model.Profile.QuoteRefNumber + ".pdf");
                model.GenerateQuotePDF(quotePDFPath, logoPath);

                List<string> mergePathList = new List<string>();
                mergePathList.Add(quotePDFPath);

                if (includeSupplement)
                {
                    byte[] supplementFile = utils.DownloadNoteFile(model.SupplementFilePath);
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    using (FileStream fs = new FileStream(tempPath + "/" + model.SupplementFileName, FileMode.OpenOrCreate))
                    {
                        fs.Write(supplementFile, 0, supplementFile.Length);
                    }
                    mergePathList.Add(tempPath + "/" + model.SupplementFileName);
                }

                PdfUtils.MergePDFs(mergePathList, finalQuotePDFPath);

                if (includeSupplement)
                {
                    Directory.Delete(Server.MapPath("~/tempAttachmentFiles/" + u.Username + ""), true);
                }

                if (model.SelectedAction == "Print")
                {
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Disposition", "inline;filename=" + @"QuotationForm.pdf");
                    Response.ContentType = "application/pdf";
                    Response.WriteFile(finalQuotePDFPath);
                    Response.Flush();
                    Response.Clear();
                }

                if (model.SelectedAction == "Email")
                {
                    List<string> pathList = new List<string>();
                    string signature = "\n \n ------------------" + "\n" + u.Username + "\n" + u.Email;
                    foreach (HttpPostedFileBase item in model.Files)
                    {
                        var stringCheck = item;

                        if (stringCheck != null)
                        {
                            if (item.ContentLength > 0)
                            {
                                var fileName = item.FileName;
                                if (!Directory.Exists(tempPath))
                                    Directory.CreateDirectory(tempPath);
                                string tempFile = Path.Combine(tempPath, item.FileName);
                                item.SaveAs(tempFile);
                                pathList.Add(tempFile);
                            }
                        }
                    }
                    SendMultipleAttachmentMail(emailRecipients, emailSubject, emailText + signature, pathList, finalQuotePDFPath, u.Username, ccRecipients);

                    try
                    {
                        string msg = "Quotation Emailed to ";

                        msg += string.Format("{0}", String.Join(",", emailRecipients));

                        if (ccRecipients != null)
                            msg += string.Format("| CC: {0}", String.Join(",", ccRecipients));

                        if (!string.IsNullOrWhiteSpace(emailSubject))
                            msg += string.Format("| Subject: {0}", emailSubject);

                        if (!string.IsNullOrWhiteSpace(emailText))
                            msg += string.Format("| Message: {0}", emailText);

                        //only quote PDF not the attachments with the mail to customer
                        string key = AwsUtils.GenerateKey("companies", u.Company.CompanyId, "users", u.Username, "quotationPreview", model.Profile.QuoteId, DateTime.Now.Ticks + "_Quotation_" + model.Profile.QuoteRefNumber + ".pdf");
                        var s3Url = utils.GetPreSignedURL(key, DateTime.Now.AddMinutes(60));
                        utils.UploadFile(quotePDFPath, key, "application/pdf");
                        //
                        SIDAL.AddQuoteAuditLog(model.Profile.QuoteId, msg, u.Username, false, s3Url);
                    }
                    catch { }
                }
            }

            try
            {
                ViewBag.ChatProjectId = model.Profile.ProjectId;
                ViewBag.ChatQuoteId = model.Profile.QuoteId;
            }
            catch
            {
            }
            return View(model);
            //return RedirectToAction("PreviewQuotation", new { @id = model.QuoteId });
        }

        //[HttpPost]
        //public ActionResult GetPreSigendURL(string url)
        //{

        //    //var utils = new AwsUtils();
        //    //var s3Url = utils.GetPreSignedURL(url, DateTime.Now.AddMinutes(60));
        //    //return View();
        //    dynamic output = new
        //    {
        //        Url = url
        //    };
        //    return output;
        //}

        [HttpGet]
        public ActionResult ProjectEntryForm(long id)
        {
            QuotationPreview model = new QuotationPreview(id);
            model.Load(false);
            SIDAL.MarkNotificationRead(SINotificationStatuses.NOTIFICATION_TYPE_PROJECT_ENTRY, id, GetUser().UserId);
            if (model.Profile.CreatedBy != GetUser().UserId)
            {
                ViewBag.ForReview = true;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ProjectEntryPDF(long id)
        {
            QuotationPreview model = new QuotationPreview(id);
            SIDAL.ProjectEntryFormPDFGeneratedDate(id);
            model.Load(true);
            string logoPath = Server.MapPath("~/Content/images/logo.jpg");
            string path = Server.MapPath("~/tmp/Download.pdf");
            model.GenerateProjectEntryPDF(path, logoPath);
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "inline;filename=" + @"ProjectEntryForm.pdf");
            Response.ContentType = "application/pdf";
            Response.WriteFile(path);
            Response.Flush();
            Response.Clear();
            return View();
        }

        #endregion

        #region Approval and Notifications

        public ActionResult RequestApproval(QuotationProfile model)
        {
            SIDAL.QueueQuotationForApproval(GetUser().Username, model.QuoteId, model.ApprovalRecipients, model.ApprovalText, model.ApprovalSubject);

            List<SIUser> users = SIDAL.GetUsersForUserIds(model.ApprovalRecipients);
            SIUser user = GetUser();
            string signature = "\n Quote Link --> " + GetHostUrl + "/Quote/AddEditQuote/" + model.QuoteId;
            signature += "\n \n ------------------" + "\n" + user.Username + "\n" + user.Email;
            SendMail(users.Select(x => x.Email).ToArray(), model.ApprovalSubject, model.ApprovalText + signature);

            try
            {
                string msg = "Approval Request sent to ";
                for (int i = 0; i < users.Count; i++)
                {
                    msg += users[i].Username;

                    if (i != (users.Count - 1))
                        msg += ",";
                }

                SIDAL.AddQuoteAuditLog(model.QuoteId, msg, user.Username);
            }
            catch { }

            var referer = Request.ServerVariables["http_referer"];
            if (referer.Contains("AddEditQuote"))
            {
                return RedirectToAction("AddEditQuote", new { @id = model.QuoteId });
            }
            else
            {
                return RedirectToAction("PreviewQuotation", new { @id = model.QuoteId });
            }
        }

        public ActionResult SendQuoteToCustomers(QuotationProfile model)
        {
            string logoPath = Server.MapPath("~/Content/images/logo.jpg");
            string path = Server.MapPath("~/tmp/Quotation_" + model.QuoteRefNumber + ".pdf");
            QuotationPreview preview = new QuotationPreview(model.QuoteId);
            preview.Load(true);
            preview.GenerateQuotePDF(path, logoPath);
            SIUser user = GetUser();
            string signature = "\n \n ------------------" + "\n" + user.Username + "\n" + user.Email;
            SendMail(model.EmailRecipients.Split(new char[] { ',' }), model.EmailSubject, model.EmailText + signature, path, model.CCRecipients);
            return RedirectToAction("PreviewQuotation", new { @id = model.QuoteId });
        }

        public ActionResult SendProjectEntryNotification(QuotationProfile model)
        {
            SIDAL.SendProjectEntryNotifications(GetUser().Username, model.QuoteId, model.NotificationRecipients, model.NotificationText, model.NotificationSubject);
            var users = SIDAL.GetUsersForUserIds(model.NotificationRecipients);
            string[] recipients = users.Select(x => x.Email).ToArray();
            string logoPath = Server.MapPath("~/Content/images/logo.jpg");
            string path = Server.MapPath("~/tmp/Project_Entry_Form_" + model.QuoteId + ".pdf");
            QuotationPreview preview = new QuotationPreview(model.QuoteId);
            preview.Load(true);
            preview.GenerateProjectEntryPDF(path, logoPath);
            SIUser user = GetUser();
            string signature = "\n \n ------------------" + "\n" + user.Username + "\n" + user.Email;
            SendMail(recipients, model.NotificationSubject, model.NotificationText + signature, path);

            try
            {
                string msg = "Project Entry Form sent to ";
                for (int i = 0; i < users.Count; i++)
                {
                    msg += users[i].Username;

                    if (i != (users.Count - 1))
                        msg += ",";
                }

                SIDAL.AddQuoteAuditLog(model.QuoteId, msg, user.Username);
            }
            catch { }

            return RedirectToAction("ProjectEntryForm", new { @id = model.QuoteId });
        }

        public ActionResult SendComments(QuotationProfile model)
        {
            SIDAL.SendCommentNotification(GetUser().Username, model.QuoteId, model.CommentRecipients, model.CommentText, model.CommentSubject);
            string[] recipients = SIDAL.GetUsersForUserIds(model.CommentRecipients).Select(x => x.Email).ToArray();
            QuotationPreview preview = new QuotationPreview(model.QuoteId);
            preview.Load(true);
            SIUser user = GetUser();
            string signature = "\n \n ------------------" + "\n" + user.Username + "\n" + user.Email;
            SendMail(recipients, model.CommentSubject, model.CommentText + signature, null);
            var referer = Request.ServerVariables["http_referer"];
            if (referer.Contains("AddEditQuote"))
            {
                return RedirectToAction("AddEditQuote", new { @id = model.QuoteId });
            }
            else
            {
                return RedirectToAction("PreviewQuotation", new { @id = model.QuoteId });
            }
        }

        public ActionResult ApproveQuote(QuotationProfile model, string notifyUser)
        {
            SIUser user = GetUser();
            bool hasApprovalAccess = SIDAL.CheckApprovalAccess(model.QuoteId, user.UserId);
            if (!hasApprovalAccess)
            {
                return RedirectToAction("AddEditQuote", new { @id = model.QuoteId, @error = "You don't have allowed volume permission to approve this quote." });
            }
            SIDAL.ApproveQuote(model.QuoteId, user);
            try
            {
                SIDAL.AddQuoteAuditLog(model.QuoteId, "Quote Approved", user.Username);
            }
            catch { }
            if (notifyUser == "withNotify")
            {
                SIDAL.SendApprovalNotification(GetUser().Username, model.QuoteId, model.ApprovalNotificationRecipients, model.ApprovalNotificationText, model.ApprovalNotificationSubject);
                string[] recipients = SIDAL.GetUsersForUserIds(model.ApprovalNotificationRecipients).Select(x => x.Email).ToArray();
                QuotationPreview preview = new QuotationPreview(model.QuoteId);
                preview.Load(true);
                string signature = "\n Quote Link --> " + GetHostUrl + "/Quote/AddEditQuote/" + model.QuoteId;
                signature += "\n \n ------------------" + "\n" + user.Username + "\n" + user.Email;

                SendMail(recipients, model.ApprovalNotificationSubject, model.ApprovalNotificationText + signature, null);
            }
            SIDAL.PopulateProjectMixWithHighestQuotedVolume(model.ProjectId, model.QuoteId);
            var referer = Request.ServerVariables["http_referer"];
            if (referer.Contains("AddEditQuote"))
            {
                return RedirectToAction("AddEditQuote", new { @id = model.QuoteId });
            }
            else
            {
                return RedirectToAction("PreviewQuotation", new { @id = model.QuoteId });
            }
        }

        public ActionResult EnableQuoteEditing(QuotationProfile model)
        {
            SIUser user = GetUser();
            if (model.UserIsApprover == false || (model.UserIsApprover && model.EnableWithoutNotification == false))
            {
                List<string> userIdList = new List<string>();
                if (model.EnableEditRecipients == null)
                {
                    userIdList.Add(model.ApprovedBy.ToString());
                }
                else
                {
                    foreach (var item in model.EnableEditRecipients)
                    {
                        userIdList.Add(item);
                    }
                    userIdList.Add(model.ApprovedBy.ToString());
                }
                SIDAL.SendQuotationEditingNotification(GetUser().Username, model.QuoteId, userIdList.ToArray(), model.EnableEditText, model.EnableEditSubject);
                //Mail
                var users = SIDAL.GetUsersForUserIds(userIdList.ToArray());

                string[] recipients = users.Select(x => x.Email).ToArray();
                QuotationPreview preview = new QuotationPreview(model.QuoteId);
                preview.Load(true);
                string signature = "\n Quote Link --> " + GetHostUrl + "/Quote/AddEditQuote/" + model.QuoteId;
                signature += "\n \n ------------------" + "\n" + user.Username + "\n" + user.Email;
                SendMail(recipients, model.EnableEditSubject, model.EnableEditText + signature, null);
            }
            if (model.UserIsApprover || model.IsUserQuotationLimit)
            {
                SIDAL.EnableEditQuote(model.QuoteId, user.UserId, withoutNotification: model.EnableWithoutNotification);

                try
                {
                    SIDAL.AddQuoteAuditLog(model.QuoteId, "Quote Editing Re-Enabled", user.Username);
                }
                catch { }
            }

            var referer = Request.ServerVariables["http_referer"];
            if (referer.Contains("AddEditQuote"))
            {
                return RedirectToAction("AddEditQuote", new { @id = model.QuoteId });
            }
            else
            {
                return RedirectToAction("PreviewQuotation", new { @id = model.QuoteId });
            }
        }

        [HttpGet]
        public ActionResult EnableQuoteEdit(int Id, bool EnableEdit = true)
        {
            SIUser user = GetUser();
            SIDAL.EnableEditQuote(Id, user.UserId, EnableEdit);
            try
            {
                SIDAL.AddQuoteAuditLog(Id, string.Format("Quote Editing {0}", (EnableEdit ? "Re-Enabled" : "Re-Disabled")), user.Username);
            }
            catch { }
            var referer = Request.ServerVariables["http_referer"];
            if (referer.Contains("AddEditQuote"))
            {
                return RedirectToAction("AddEditQuote", new { @id = Id });
            }
            else
            {
                return RedirectToAction("Notifications");
            }
        }

        [HttpGet]
        public ActionResult Notifications()
        {
            ViewBag.NotificationsActive = "active";
            NotificationPageView model = new NotificationPageView();
            model.UserId = GetUser().UserId;
            model.Filter = new PipelineFilter(GetUser().UserId);
            model.Filter.SortColumns = new string[] { "StatusName" };
            model.Filter.ParentPage = "Pipeline";
            model.Filter.FillSelectItems(GetUser().UserId);
            model.Load();
            ViewBag.CurrentPage = (model.Filter.CurrentStart / model.Filter.RowsPerPage) + 1;
            ViewBag.NumPages = (model.NumResults / model.Filter.RowsPerPage) + 1;
            ViewBag.RowCount = model.NumResults;
            ViewBag.Mode = "all";
            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteNotification(long id)
        {
            SIDAL.DeleteNotification(id);
            return RedirectToAction("Notifications");
        }

        [HttpPost]
        public ActionResult Notifications(NotificationPageView model)
        {
            ViewBag.NotificationsActive = "active";
            if (model.Districts != null)
                model.Filter.Districts = model.Districts;
            if (model.Plants != null)
                model.Filter.Plants = model.Plants;
            if (model.Staffs != null)
                model.Filter.Staffs = model.Staffs;
            model.Filter.FillSelectItems(GetUser().UserId);
            model.UserId = GetUser().UserId;
            model.Load();
            ViewBag.CurrentPage = (model.Filter.CurrentStart) + 1;
            ViewBag.NumPages = (model.NumResults / model.Filter.RowsPerPage) + 1;
            ViewBag.RowCount = model.NumResults;
            return View(model);
        }

        #endregion

        #region AUJS API

        public ActionResult PushToAPI(long id)
        {
            PushQuoteModel pqModel = new PushQuoteModel(this.GetCompany().CompanyId, id);
            var user = GetUser();

            TempData["PushQuoteSuccess"] = pqModel.PushQuote(user.Username);

            try
            {
                SIDAL.AddQuoteAuditLog(id, "Quote Sent to Dispatch", user.Username, true);
            }
            catch
            {

            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        #endregion

        #region Replace Quote Non Synced Customer With Synced Customer
        public ActionResult UpdateQuoteWithSyncCustomer(long customerId, long newCustomerId, long quoteId)
        {
            SIDAL.UpdateQuoteWithSyncCustomer(customerId, newCustomerId);
            return RedirectToAction("PushToAPI", new { @id = quoteId });
        }

        #endregion

        #region Quote Audit Logs

        [HttpPost]
        public ActionResult AuditLogs(long id, int? count = null)
        {
            var auditLogs = SIDAL.GetQuoteAuditLogs(id);
            var logs = new List<dynamic>();
            if (auditLogs != null)
            {
                var utils = new AwsUtils();
                auditLogs.ForEach(x => logs.Add(new
                {
                    Id = x.Id,
                    User = x.Username,
                    DateFormatted = x.CreatedOn.ToString("dd MMM, yyyy HH:mm"),
                    DateUtc = x.CreatedOn.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss tt UTC"),
                    Date = x.CreatedOn.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    Description = x.Text,
                    RefLink = (string.IsNullOrEmpty(x.ReferenceLink) ? "" : utils.GetQuotePreSignedURL(x.ReferenceLink, DateTime.Now.AddMinutes(60)))
                }));
            }
            dynamic output = new
            {
                total = auditLogs.Count,
                logs = logs
            };

            return Json(output);
        }

        #endregion

        #region Add Add-On
        public string QuickAddQuotationAddOn(long quotationId)
        {
            JObject addObject = new JObject();
            addObject["Update"] = true;

            QuotationAddonsView qaView = new QuotationAddonsView(quotationId);
            qaView.Load();
            Addon addOn = qaView.AllAddons.FirstOrDefault();
            QuotationAddon qaon = new QuotationAddon();
            qaon.QuotationId = quotationId;
            qaon.AddonId = addOn.Id;
            try
            {
                SIDAL.AddQuotationAddOn(qaon);
            }
            catch (Exception)
            {
                addObject["Update"] = false;
            }
            addObject["QuoteAddOnId"] = qaon.AddonId;
            addObject["QuotationId"] = qaon.QuotationId;
            addObject["Description"] = qaon.Description;
            addObject["Price"] = qaon.Price;
            addObject["IsIncludeTable"] = qaon.IsIncludeTable;
            return addObject.ToString();
        }

        public ActionResult DeleteQuotationAddon(long quoteAddonId, long addonId)
        {
            bool deleteStatus = true;
            var description = "";
            var code = "";
            try
            {

                SIDAL.DeleteQuotationAddon(quoteAddonId);
                var addon = SIDAL.FindAddon(addonId);
                description = addon.Description;
                code = addon.Code;
            }
            catch (Exception)
            {
                deleteStatus = false;
            }
            dynamic result = new
            {
                deleteStatus = deleteStatus,
                Description = description,
                Code = code,
                lastId = addonId
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Block Aggregate
        public ActionResult DeleteQuotationAggregate(long quoteAggregateId)
        {
            bool deleteStatus = true;
            try
            {
                SIDAL.DeleteQuotationAggregate(quoteAggregateId);
            }
            catch (Exception)
            {
                deleteStatus = false;
            }
            dynamic result = new
            {
                deleteStatus = deleteStatus
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteQuotationAggregateAddon(long quoteAggregateAddonId)
        {
            bool deleteStatus = true;
            try
            {
                SIDAL.DeleteQuotationAggregateAddon(quoteAggregateAddonId);
            }
            catch (Exception)
            {
                deleteStatus = false;
            }
            dynamic result = new
            {
                deleteStatus = deleteStatus
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteQuotationBlockAddon(long quoteBlockAddonId)
        {
            bool deleteStatus = true;
            try
            {
                SIDAL.DeleteQuotationBlockAddon(quoteBlockAddonId);
            }
            catch (Exception)
            {
                deleteStatus = false;
            }
            dynamic result = new
            {
                deleteStatus = deleteStatus
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteQuotationBlock(long quoteBlockId)
        {
            bool deleteStatus = true;

            try
            {
                SIDAL.DeleteQuotationBlock(quoteBlockId);
            }
            catch (Exception)
            {
                deleteStatus = false;
            }
            dynamic result = new
            {
                deleteStatus = deleteStatus,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateQuoteProductType(string productType, long quoteId)
        {
            var plantNames = "";
            bool updateStatus = true;
            try
            {
                SIDAL.UpdateQuoteProductType(productType, quoteId);
                plantNames = SIDAL.GetQuotationPlantsName(quoteId);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic result = new
            {
                updateStatus = updateStatus,
                plants = plantNames
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateQuotationAggregatePlant(int plantId, long quoteId)
        {
            List<QuotationAggregateAddonModel> aggAddonList = new List<QuotationAggregateAddonModel>();
            List<QuotationAggregateAddon> addonList = SIDAL.UpdateQuotationAggregatePlant(plantId, quoteId);
            QuotationAggregateAddonModel quoteAggAddon;
            foreach (var item in addonList)
            {
                quoteAggAddon = new QuotationAggregateAddonModel(item);
                aggAddonList.Add(quoteAggAddon);
            }

            var districtAddonDefaultList = SIDAL.GetAllDistrictAggregateAddons(quoteId);
            List<AddonView> AllAggregateAddonsList = new List<AddonView>();
            if (districtAddonDefaultList != null)
            {
                foreach (var item in districtAddonDefaultList)
                {
                    Addon addon = SIDAL.FindAddon(item.AddonId.GetValueOrDefault());
                    AllAggregateAddonsList.Add(new AddonView(addon));
                }
            }
            dynamic output = new
            {
                aggAddonList = aggAddonList,
                addonList = AllAggregateAddonsList
            };

            return Json(output);
        }

        public ActionResult UpdateQuotationBlockPlant(int plantId, long quoteId)
        {
            List<QuotationBlockAddonModel> blockAddonList = new List<QuotationBlockAddonModel>();
            List<QuotationBlockAddon> addonList = SIDAL.UpdateQuotationBlockPlant(plantId, quoteId);
            QuotationBlockAddonModel quoteBlockAddon;
            foreach (var item in addonList)
            {
                quoteBlockAddon = new QuotationBlockAddonModel(item);
                blockAddonList.Add(quoteBlockAddon);
            }

            var districtAddonDefaultList = SIDAL.GetAllDistrictBlockAddons(quoteId);
            List<AddonView> AllBlockAddonsList = new List<AddonView>();
            if (districtAddonDefaultList != null)
            {
                foreach (var item in districtAddonDefaultList)
                {
                    Addon addon = SIDAL.FindAddon(item.AddonId.GetValueOrDefault());
                    AllBlockAddonsList.Add(new AddonView(addon));
                }
            }
            dynamic output = new
            {
                blockAddonList = blockAddonList,
                addonList = AllBlockAddonsList
            };
            return Json(output);
        }

        #endregion

        public ActionResult QuotePayload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuotePayload(string data)
        {
            QuotePayload qp = JsonConvert.DeserializeObject<QuotePayload>(data);

            SyncManager syncManager = new SyncManager(ConfigurationHelper.Company.CompanyId);
            bool success = syncManager.PushQuote(qp);

            JObject res = new JObject();

            if (success)
            {
                res.Add("success", true);
                res.Add("message", "Successfully pushed the code");
            }
            else
            {
                res.Add("success", false);
                res.Add("message", "Could not push the code");
            }

            return Json(res.ToString());
        }

        [HttpPost]
        public ActionResult GetProjectList()
        {
            Guid userId = GetUser().UserId;
            var projectList = SIDAL.GetProjectList(userId).OrderBy(x => x.ProjectName).ToList();

            dynamic output = null;
            output = new
            {
                projects = projectList
            };
            return Json(output);
        }


        [HttpPost]
        public ActionResult GetCustomerList()
        {
            Guid userId = GetUser().UserId;
            var customerList = SIDAL.GetCustomerList(userId).OrderBy(x => x.Name).ToList();

            dynamic output = null;
            output = new
            {
                customers = customerList
            };
            var jsonResult = Json(output);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetTaxCodeList()
        {
            var taxCodeList = SIDAL.GetTaxCodes().Select(x => new { Text = x.Code + " - " + x.Description, Value = x.Id });
            dynamic output = null;
            output = new
            {
                taxCodes = taxCodeList
            };
            return Json(output);
        }

        public ActionResult UpdateQuotationAdjustMixPrice(double? adjustMixPrice, long quoteId)
        {
            var updateStatus = SIDAL.UpdateQuotationAdjustMixPrice(adjustMixPrice, quoteId);
            dynamic output = new
            {
                updateStatus = updateStatus
            };
            return Json(output);
        }
    }
}
