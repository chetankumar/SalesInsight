using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RedHill.SalesInsight.AUJSIntegration.Data;
using RedHill.SalesInsight.AUJSIntegration.Setup.ClientProvider;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Logger;
using RedHill.SalesInsight.Web.Html5.Helpers;
using RedHill.SalesInsight.Web.Html5.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL.Utilities;
using RedHill.SalesInsight.ESI;
using RedHill.SalesInsight.Web.Html5.Models.ESI;
using System.Web.Security;
using System.Configuration;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class AdminController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region Raw Material Types
        [APIFilter]
        public ActionResult ManageRawMaterialTypes(long? id)
        {
            ManageRawMaterialTypes model = new ManageRawMaterialTypes(id);
            return View(model);
        }

        [HttpPost]
        [APIFilter]
        public ActionResult ManageRawMaterialTypes(ManageRawMaterialTypes model)
        {
            model.LoadValues();
            return View(model);
        }

        public ActionResult UpdateRawMaterialType(ManageRawMaterialTypes model)
        {
            if (ModelState.IsValid)
            {
                RawMaterialType entity = model.SelectedRawMaterialType.ToEntity();
                SIDAL.UpdateRawMaterialType(entity);
                model.SelectedRawMaterialType = new RawMaterialTypeView();
                return RedirectToAction("ManageRawMaterialTypes");
            }
            model.LoadValues();
            model.ShowModal = true;
            return View("ManageRawMaterialTypes", model);
        }
        #endregion

        #region 5SK Pricing
        public ActionResult ManageFSKPrices(long? id)
        {
            ManageFSKPrices model = new ManageFSKPrices(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult ManageFSKPrices(ManageFSKPrices model)
        {
            model.LoadValues();
            return View(model);
        }

        public ActionResult UpdateFSKPrice(ManageFSKPrices model)
        {
            if (ModelState.IsValid)
            {
                FSKPrice entity = model.SelectedFSKPrice.ToEntity();
                SIDAL.UpdateFSKPrice(entity);
                model.SelectedFSKPrice = new FSKPriceView();
                return RedirectToAction("ManageFSKPrices");
            }
            model.LoadValues();
            model.ShowModal = true;
            return View("ManageFSKPrices", model);
        }

        public ActionResult DeleteFSKPrice(long id)
        {
            SIDAL.DeleteFSKPrice(id);
            return RedirectToAction("ManageFSKPrices");
        }
        #endregion

        #region Raw Materials

        [APIFilter]
        public ActionResult ManageRawMaterials(long? id)
        {
            ManageRawMaterials model = new ManageRawMaterials(id);
            model.CompanyName = GetCompany().Name;
            model.LoadValues();
            return View(model);
        }

        [HttpPost]
        [APIFilter]
        public ActionResult ManageRawMaterials(ManageRawMaterials model)
        {
            model.CompanyName = GetCompany().Name;
            model.LoadValues();
            return View(model);
        }

        [HttpGet]
        public string GetFSKDetail(long id)
        {
            RawMaterialType mat = SIDAL.FindRawMaterialType(id);
            return mat.IncludeInFSK.GetValueOrDefault() ? "SHOW" : "HIDE";
        }

        public ActionResult UpdateRawMaterial(ManageRawMaterials model)
        {
            ModelState.Remove("SelectedRawMaterialId");
            ModelState.Remove("Active");
            model.SelectedRawMat.Id = model.SelectedRawMaterialId;
            var existing = SIDAL.FindRawMaterialByCode(model.SelectedRawMat.MaterialCode);
            if (existing != null && existing.Id != model.SelectedRawMaterialId)
            {
                ModelState["SelectedRawMat.MaterialCode"].Errors.Add("The code is already taken by another raw material.");
            }
            if (ModelState.IsValid)
            {
                RawMaterial entity = model.SelectedRawMat.ToEntity();
                SIDAL.UpdateRawMaterial(entity);
                return RedirectToAction("ManageRawMaterials");
            }
            else
            {
                model.CompanyName = GetCompany().Name;
                model.LoadValues();
                model.ShowModal = true;
                return View("ManageRawMaterials", model);
            }
        }

        public ActionResult ManageRawMaterialProjections(long id)
        {
            ManageRawMaterialCostProjections model = new ManageRawMaterialCostProjections(id);
            model.User = GetUser();
            model.LoadValues();
            return View("ManageRawMaterialProjections", model);
        }

        [HttpPost]
        public ActionResult ManageRawMaterialProjections(ManageRawMaterialCostProjections model)
        {
            model.User = GetUser();
            model.UpdatePrice();
            model.LoadValues();
            ModelState.Clear();
            return View("ManageRawMaterialProjections", model);
        }

        public FileContentResult RawMaterialPricingExcel()
        {
            string filename = "RawMaterialPrice_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            using (ExcelPackage p = new ExcelPackage(new System.IO.FileInfo(Server.MapPath("~/SampleUploads/Raw material Prices.xlsx")), true))
            {
                p.Workbook.Properties.Author = User.Identity.Name;
                p.Workbook.Properties.Title = "Raw Material Price";
                p.Workbook.Worksheets.Add("Raw Material Price");

                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                ws.Cells.Style.Font.Size = 10;

                //  Set Cell Headers 
                ws.Cells[1, 1].Value = "Material Code";
                ws.Cells[1, 2].Value = "Plant";
                ws.Cells[1, 3].Value = "Unit Cost";
                ws.Cells[1, 4].Value = "Unit of Measure";
                ws.Cells[1, 5].Value = "Date";

                int index = 2;
                foreach (RawMaterialCostProjection cp in SIDAL.GetLatestRawMaterialCostProjections())
                {
                    ws.Cells[index, 1].Value = cp.RawMaterial.MaterialCode;
                    ws.Cells[index, 2].Value = cp.Plant.Name;
                    ws.Cells[index, 3].Value = cp.Cost;
                    ws.Cells[index, 4].Value = cp.Uom.Name;
                    ws.Cells[index, 5].Value = cp.ChangeDate.ToString("M/d/yyyy hh:mm:ss tt");
                    index++;
                }

                Byte[] bin = p.GetAsByteArray();
                // return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                return File(bin, "application/vnd.ms-excel", filename);
            }
        }

        [HttpPost]
        public ActionResult UploadRawMaterialCosts()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string materialCode = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string plant = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string unitCost = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string costUom = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string date = Convert.ToString(worksheet.Cells[i, 5].Value);

                            string error = SIDAL.ScrubRawMaterialProjection(materialCode, plant, unitCost, costUom, date);
                            if (error != null)
                            {
                                worksheet.Cells[i, 6].Value = error;
                                worksheet.Cells[i, 1, i, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, 5].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, 5].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 6].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }
                    //Byte[] bin = xlPackage.GetAsByteArray();
                    //return File(bin, "application/vnd.ms-excel", file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("ManageRawMaterials");
        }


        #endregion

        #region Mix Designs

        [APIFilter]
        public ActionResult ManageMixDesigns()
        {
            ManageMixDesign model = new ManageMixDesign();
            model.CurrentPage = 0;
            model.RowsPerPage = 10;
            model.InitCustomFields();
            model.LoadValues();
            return View(model);
        }

        [HttpPost]
        [APIFilter]
        public ActionResult ManageMixDesigns(ManageMixDesign model)
        {
            model.UserId = GetUser().UserId;
            model.LoadValues();
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateUDFDisplay(bool md1, bool md2, bool md3, bool md4)
        {
            string data = "";
            try
            {
                ManageMixDesign mixDesign = new ManageMixDesign();
                mixDesign.MD1Show = md1;
                mixDesign.MD2Show = md2;
                mixDesign.MD3Show = md3;
                mixDesign.MD4Show = md4;

                data = "{ success: true }";
            }
            catch (Exception ex)
            {
                data = "{ success: false }";
            }
            return Json(data);
        }

        [APIFilter]
        public ActionResult AddEditStandardMix(long? id)
        {
            StandardMixView model = null;
            if (id == null)
                model = new StandardMixView();
            else
            {
                StandardMix entity = SIDAL.FindStandardMix(id.Value);
                model = new StandardMixView(entity);
            }
            model.User = GetUser();
            return View(model);
        }

        [APIFilter]
        public ActionResult UpdateStandardMix(StandardMixView model)
        {
            if (ModelState.IsValid)
            {
                StandardMix entity = model.ToEntity();
                bool inUse = SIDAL.CheckDuplicateMixNumber(entity.Id, entity.Number);
                if (inUse)
                {
                    model.User = GetUser();
                    ModelState.AddModelError("Number", "The mix number must be unique.");
                    return View("AddEditStandardMix", model);
                }
                entity.UpdatedBy = GetUser().Username;
                entity.UpdatedOn = DateTime.Now;
                SIDAL.UpdateStandardMix(entity);
                return RedirectToAction("AddEditStandardMix", new { @id = entity.Id });
            }
            else
            {
                model.User = GetUser();
                return View("AddEditStandardMix", model);
            }
        }

        public FileContentResult MixDesignsExcel()
        {
            string filename = "MixDesigns_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            using (ExcelPackage p = new ExcelPackage(new FileInfo(Server.MapPath("~/SampleUploads/Mix Designs.xlsx")), true))
            {
                p.Workbook.Properties.Author = User.Identity.Name;
                p.Workbook.Properties.Title = "Mix Designs";
                p.Workbook.Worksheets.Add("Mix Designs");

                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                ws.Cells.Style.Font.Size = 10;

                //  Set Cell Headers 
                ws.Cells[1, 1].Value = "Mix Number";
                ws.Cells[1, 2].Value = "Mix Description";
                ws.Cells[1, 3].Value = "Sales Description";
                ws.Cells[1, 4].Value = "PSI";
                ws.Cells[1, 5].Value = "Slump";
                ws.Cells[1, 6].Value = "PCT_AIR";
                ws.Cells[1, 7].Value = "UDF_MD1";
                ws.Cells[1, 8].Value = "UDF_MD2";
                ws.Cells[1, 9].Value = "UDF_MD3";
                ws.Cells[1, 10].Value = "UDF_MD4";

                int index = 2;
                int count = 0;
                foreach (StandardMix mix in SIDAL.GetStandardMixes(out count, false, null, 0, Int32.MaxValue))
                {
                    ws.Cells[index, 1].Value = mix.Number;
                    ws.Cells[index, 2].Value = mix.Description;
                    ws.Cells[index, 3].Value = mix.SalesDesc;
                    ws.Cells[index, 4].Value = mix.PSI;
                    ws.Cells[index, 5].Value = mix.Slump;
                    ws.Cells[index, 6].Value = mix.Air;
                    ws.Cells[index, 7].Value = mix.MD1;
                    ws.Cells[index, 8].Value = mix.MD2;
                    ws.Cells[index, 9].Value = mix.MD3;
                    ws.Cells[index, 10].Value = mix.MD4;
                    index++;
                }

                Byte[] bin = p.GetAsByteArray();
                return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        [HttpPost]
        public ActionResult UploadMixDesigns()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string number = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string mixDescription = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string SalesDescription = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string PSI = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string Slump = Convert.ToString(worksheet.Cells[i, 5].Value);
                            string Air = Convert.ToString(worksheet.Cells[i, 6].Value);
                            string UDF1 = Convert.ToString(worksheet.Cells[i, 7].Value);
                            string UDF2 = Convert.ToString(worksheet.Cells[i, 8].Value);
                            string UDF3 = Convert.ToString(worksheet.Cells[i, 9].Value);
                            string UDF4 = Convert.ToString(worksheet.Cells[i, 10].Value);

                            string error = SIDAL.ScrubMixDesigns(number, mixDescription, SalesDescription, PSI, Slump, Air, UDF1, UDF2, UDF3, UDF4, GetUser().Username);
                            if (error != null)
                            {
                                worksheet.Cells[i, 11].Value = error;
                                worksheet.Cells[i, 1, i, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, 10].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, 10].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 1, i, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i, 1, i, 10].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            worksheet.Cells[i, 1, i, 10].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[i, 11].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }

                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("ManageRawMaterials");
        }

        #endregion

        #region Mix Formulations

        public FileContentResult MixFormulationExcel()
        {
            string filename = "MixFormulations_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            using (ExcelPackage p = new ExcelPackage(new FileInfo(Server.MapPath("~/SampleUploads/Mix Formulations.xlsx")), true))
            {
                p.Workbook.Properties.Author = User.Identity.Name;
                p.Workbook.Properties.Title = "Mix Formulations";
                p.Workbook.Worksheets.Add("Mix Formulations");

                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                ws.Cells.Style.Font.Size = 10;

                //  Set Cell Headers 
                ws.Cells[1, 1].Value = "Mix Number";
                ws.Cells[1, 2].Value = "Formulation Number";
                ws.Cells[1, 3].Value = "Formulation Desc";
                ws.Cells[1, 4].Value = "Plant";
                ws.Cells[1, 5].Value = "Material Code";
                ws.Cells[1, 6].Value = "UOM";
                ws.Cells[1, 7].Value = "Per CW";
                ws.Cells[1, 8].Value = "Qty";


                int index = 2;
                var mixFormulations = SIDAL.GetAllMixFormulations();
                var mixConstituents = SIDAL.GetMixConstituents();

                var results = mixConstituents.Join(mixFormulations, a => a.MixFormulationId, b => b.Id, (a, b) => new { b, a.RawMaterial, b.StandardMix, a.Uom, a.PerCementWeight, a.Quantity, b.Plant })
                    .Select(x => new
                    {
                        StandardMixNumber = x.StandardMix.Number,
                        FormulationNumber = x.b.Number,
                        FormulationDescription = x.b.Description,
                        PlantName = x.Plant.Name,
                        RawMaterialCode = x.RawMaterial.MaterialCode,
                        UomName = x.Uom.Name,
                        PerCementWeight = x.PerCementWeight.GetValueOrDefault() ? "Yes" : "No",
                        Quantity = x.Quantity
                    }).ToList();

                foreach (var item in results)
                {
                    ws.Cells[index, 1].Value = item.StandardMixNumber;
                    ws.Cells[index, 2].Value = item.FormulationNumber;
                    ws.Cells[index, 3].Value = item.FormulationDescription;
                    ws.Cells[index, 4].Value = item.PlantName;
                    ws.Cells[index, 5].Value = item.RawMaterialCode;
                    ws.Cells[index, 6].Value = item.UomName;
                    ws.Cells[index, 7].Value = item.PerCementWeight;
                    ws.Cells[index, 8].Value = item.Quantity;
                    index++;
                }
                //foreach (MixFormulation formulation in mixFormulations)
                //{
                //    foreach (StandardMixConstituent s in mixConstituents.Where(x => x.MixFormulationId == formulation.Id))
                //    {
                //        ws.Cells[index, 1].Value = formulation.StandardMix.Number;
                //        ws.Cells[index, 2].Value = formulation.Number;
                //        ws.Cells[index, 3].Value = formulation.Description;
                //        ws.Cells[index, 4].Value = formulation.Plant.Name;
                //        ws.Cells[index, 5].Value = s.RawMaterial.MaterialCode;
                //        ws.Cells[index, 6].Value = s.Uom.Name;
                //        ws.Cells[index, 7].Value = s.PerCementWeight.GetValueOrDefault() ? "Yes" : "No";
                //        ws.Cells[index, 8].Value = s.Quantity;
                //        index++;
                //    }
                //}

                Byte[] bin = p.GetAsByteArray();
                return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        [HttpPost]
        public ActionResult UploadMixFormulations()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    var deletedMixes = new List<long>();
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string mixNumber = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string formulaNumber = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string formulaDescription = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string plant = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string materialCode = Convert.ToString(worksheet.Cells[i, 5].Value);
                            string uom = Convert.ToString(worksheet.Cells[i, 6].Value);
                            bool perCw = Convert.ToString(worksheet.Cells[i, 7].Value).Equals("Yes");
                            string qty = Convert.ToString(worksheet.Cells[i, 8].Value);

                            string error = SIDAL.ScrubMixFormulations(mixNumber, formulaNumber, formulaDescription, plant, materialCode, uom, perCw, qty, GetUser().Username, deletedMixes);
                            if (error != null)
                            {
                                worksheet.Cells[i, 9].Value = error;
                                worksheet.Cells[i, 1, i, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, 8].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, 8].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception ex)
                        {
                            worksheet.Cells[i, 1, i, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i, 1, i, 8].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            worksheet.Cells[i, 1, i, 8].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[i, 9].Value = "System Exception : " + ex.Message;
                        }
                    }

                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("ManageRawMaterials");
        }

        public ActionResult CopyMixFormulation(int SourcePlantId, long Mix, int[] PlantIds)
        {
            foreach (int plantId in PlantIds)
            {
                SIDAL.CopyFormulation(Mix, SourcePlantId, plantId);
            }
            return RedirectToAction("AddEditStandardMix", new { @id = Mix });
        }

        [APIFilter]
        public ActionResult AddEditMixFormulation(long id, int plant)
        {
            MixFormulationView model = new MixFormulationView(id, plant);
            model.Load();
            return View(model);
        }

        public ActionResult DeleteMixFormulation(long id, int plant)
        {
            SIDAL.DeleteMixFormulation(plant, id);
            return RedirectToAction("AddEditStandardMix", new { @id = id });
        }

        [HttpPost]
        public ActionResult UpdateMixFormulation(MixFormulationView model)
        {
            MixFormulation entity = model.Formulation;
            entity.UpdatedBy = GetUser().Username;
            entity.UpdatedOn = DateTime.Now;
            entity.StandardMixId = model.StandardMixId;
            entity.PlantId = model.PlantId;
            if (entity.Number != null)
            {
                SIDAL.UpdateMixFormulation(entity);
                return RedirectToAction("AddEditMixFormulation", new { @id = model.StandardMixId, @plant = model.PlantId });
            }
            else
            {
                ViewBag.Error = "The number cannot be empty";
                model.Load();
                return View("AddEditMixFormulation", model);
            }
        }

        [HttpPost]
        public ActionResult UpdateMixConstituent(MixFormulationView model)
        {
            StandardMixConstituent entity = model.SelectedConstituent.ToEntity();
            MixFormulation formulation = SIDAL.FindOrCreateFormulation(model.PlantId, model.StandardMixId);
            entity.MixFormulationId = formulation.Id;
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                SIDAL.UpdateStandardMixConstituent(entity);
                SIDAL.SetMixFormulationAuditTrail(formulation.Id, GetUser().Username, DateTime.Now);
                SIDAL.UpdateMixFormulationCalculatedFields(formulation.Id);
                SIDAL.UpdateMixFormulationCostProjections(formulation.Id);
                return RedirectToAction("AddEditMixFormulation", new { @id = model.StandardMixId, @plant = model.PlantId });
            }
            else
            {
                model.Load();
                return View("AddEditMixFormulation", model);
            }
        }



        [HttpGet]

        public ActionResult EditMixConstituent(long id, long mix, int plant)
        {
            MixFormulationView model = new MixFormulationView(mix, plant);
            model.MixConstituentId = id;
            model.Load();
            return View("AddEditMixFormulation", model);
        }

        [HttpGet]
        public ActionResult DeleteMixConstituent(long id, long mix, int plant)
        {
            long formulationId = SIDAL.FindMixConstituent(id).MixFormulationId;
            SIDAL.DeleteMixConstituent(id);
            SIDAL.SetMixFormulationAuditTrail(formulationId, GetUser().Username, DateTime.Now);
            SIDAL.UpdateMixFormulationCalculatedFields(formulationId);
            ModelState.Clear();
            return RedirectToAction("AddEditMixFormulation", new { @id = mix, @plant = plant });
        }

        #endregion

        #region Addons

        [HttpGet]
        [APIFilter]
        public ActionResult ManageAddons(long? id = null, string mode = null)
        {
            ManageAddons model = new ManageAddons(id);
            model.PriceMode = (mode == "QUOTE" || mode == null) ? "QUOTE" : "MIX";
            model.User = GetUser();
            model.CompanyName = GetCompany().Name;
            return View(model);
        }

        [HttpPost]
        [APIFilter]
        public ActionResult ManageAddons(ManageAddons model)
        {
            model.User = GetUser();
            model.UpdatePrice();
            model.LoadValues();
            ModelState.Clear();
            return View(model);
        }

        public ActionResult UpdateAddon(ManageAddons model)
        {
            ModelState.Remove("SelectedAddon.Id");
            var existing = SIDAL.FindAddonByCode(model.SelectedAddon.Code);
            if (existing != null && existing.Id != model.SelectedAddon.Id)
            {
                ModelState["SelectedAddon.Code"].Errors.Add("The code is already taken by another addon.");
            }
            if (ModelState.IsValid)
            {
                Addon entity = model.SelectedAddon.ToEntity();
                SIDAL.UpdateAddon(entity);
                return RedirectToAction("ManageAddons");
            }
            else
            {
                model.ShowEditModal = true;
                model.LoadValues();
                return View("ManageAddons", model);
            }
        }

        [HttpGet]
        public string MarkAddonDefault(long addonId, int districtId, bool value, string defaultType)
        {
            SIDAL.MarkAddonDefault(addonId, districtId, value, defaultType);
            return "OK";
        }

        public string GetUOMs(string addonType, string mode)
        {
            var uoms = SIDAL.GetAddonUOMS(addonType, mode).Select(x => new { Name = x.Name, Value = x.Id }).ToList();
            JArray array = new JArray();
            foreach (var item in uoms)
            {
                JObject o = new JObject();
                o.Add("text", item.Name);
                o.Add("value", item.Value.ToString());
                array.Add(o);
            }
            return array.ToString();
        }

        public FileContentResult AddonPricingExcel()
        {
            string filename = "AddonPrice_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            using (ExcelPackage p = new ExcelPackage(new System.IO.FileInfo(Server.MapPath("~/SampleUploads/Addon Prices.xlsx")), true))
            {
                p.Workbook.Properties.Author = User.Identity.Name;
                p.Workbook.Properties.Title = "Add-On Price";
                p.Workbook.Worksheets.Add("Add-On Price");

                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                ws.Cells.Style.Font.Size = 10;

                //  Set Cell Headers 
                ws.Cells[1, 1].Value = "Addon Code";
                ws.Cells[1, 2].Value = "District";
                ws.Cells[1, 3].Value = "Unit Cost";
                ws.Cells[1, 4].Value = "Unit of Measure";
                ws.Cells[1, 5].Value = "Applicability";
                ws.Cells[1, 6].Value = "Effective Date";

                int index = 2;
                foreach (AddonPriceProjection cp in SIDAL.GetLatestAddonCostProjections())
                {
                    ws.Cells[index, 1].Value = cp.Addon.Code;
                    ws.Cells[index, 2].Value = cp.District.Name;
                    ws.Cells[index, 3].Value = cp.Price;
                    ws.Cells[index, 4].Value = cp.Uom.Name;
                    ws.Cells[index, 5].Value = cp.PriceMode;
                    ws.Cells[index, 6].Value = cp.ChangeDate.ToString("M/d/yyyy hh:mm:ss tt");
                    index++;
                }

                Byte[] bin = p.GetAsByteArray();
                return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        public ActionResult UploadAddonPrices()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string addonCode = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string district = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string unitCost = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string costUom = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string priceMode = Convert.ToString(worksheet.Cells[i, 5].Value);
                            string date = Convert.ToString(worksheet.Cells[i, 6].Value);

                            string error = SIDAL.ScrubAddonProjection(addonCode, district, unitCost, costUom, priceMode, date);
                            if (error != null)
                            {
                                worksheet.Cells[i, 7].Value = error;
                                worksheet.Cells[i, 1, i, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, 6].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, 6].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 7].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }

                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("ManageAddons");
        }

        #endregion

        #region Misc Settings

        [APIFilter]
        public ActionResult ManageMiscSettings(long? id)
        {
            MiscView model = new MiscView();
            if (id != null)
            {
                model.SelectedTaxCode = new TaxCodeView(SIDAL.FindTaxCode(id.Value));
                //model.UserPasswordSettings = new RedHill.SalesInsight.DAL.DataTypes.SISuperUserSettings(SIDAL.FindSuperUserSettings);
            }
            else
                model.SelectedTaxCode = new TaxCodeView();

            model.LoadTaxCodes();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUpdateTaxCodes(MiscView model)
        {
            if (ModelState.IsValid)
            {
                TaxCode tc = model.SelectedTaxCode.ToEntity();
                SIDAL.AddUpdateTaxCode(tc);
                return RedirectToAction("ManageMiscSettings");
            }
            else
            {
                model.LoadTaxCodes();
                return View("ManageMiscSettings", model);
            }
        }

        [HttpGet]
        public ActionResult DeleteTaxCode(long id)
        {
            bool success = SIDAL.DeleteTaxCode(id);
            if (!success)
            {
                TempData["TaxError"] = "Cannot not delete this Tax Code as it may be bound to existing Quotes.";
            }
            return RedirectToAction("ManageMiscSettings");
        }

        [HttpPost]
        public ActionResult ManageMiscSettings(MiscView model)
        {
            SIDAL.UpdateGlobalSetting(model.ToEntity());
            return RedirectToAction("ManageMiscSettings");
        }

        public ActionResult UploadLogo(HttpPostedFileBase LogoFile)
        {
            try
            {
                if (LogoFile.ContentLength > 0)
                {
                    var fileName = "logo.jpg";
                    var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                    LogoFile.SaveAs(path);
                }
            }
            catch
            {

            }
            return RedirectToAction("ManageMiscSettings");
        }
        #endregion

        #region Sync Module

        [HttpPost]
        public ActionResult ImportEntity(string entityType)
        {
            dynamic data = new { };

            entityType = entityType.ToLower();

            SyncResponse syncResponse = null;

            AUJSIntegration.Data.SyncManager syncManager = new AUJSIntegration.Data.SyncManager(SIDAL.DefaultCompany().CompanyId, new FileLogger());

            try
            {
                switch (entityType)
                {
                    case "marketsegment":
                        syncResponse = syncManager.ImportMarketSegments();
                        break;
                    case "rawmaterialtype":
                        syncResponse = syncManager.ImportRawMaterialTypes();
                        break;
                    case "plant":
                        syncResponse = syncManager.ImportPlants();
                        break;
                    case "projectstatus":
                        syncResponse = syncManager.ImportStatusTypes();
                        break;
                    case "taxcode":
                        syncResponse = syncManager.ImportTaxCodes();
                        break;
                    case "uom":
                        syncResponse = syncManager.ImportUnitOfMeasures();
                        break;
                    case "rawmaterial":
                        syncResponse = syncManager.ImportRawMaterials();
                        break;
                    case "customer":
                        syncResponse = syncManager.ImportCustomers();
                        break;
                    case "salesstaff":
                        syncResponse = syncManager.ImportSalesStaff();
                        break;
                    case "standardmixconstituent":
                        //Import Standard Mixes
                        syncResponse = syncManager.ImportProducts();

                        //Import Aggregate Products
                        syncResponse = syncManager.ImportAggregateProducts();

                        //Import Block Products
                        syncResponse = syncManager.ImportBlockProducts();

                        //Import Add-Ons
                        syncResponse = syncManager.ImportAddOns();
                        break;
                }

                if (syncResponse != null && syncResponse.SyncStatus != SyncStatus.Error)
                    data = new { success = syncResponse.SyncStatus == SyncStatus.Complete, message = "Sync Completed" };
                else
                    data = new { success = false, message = syncResponse.Message };
            }
            catch (Exception ex)
            {
                data = new { success = false, message = syncResponse.Message };
            }
            return Json(data);
        }

        public ActionResult LoadFetchHistory()
        {
            var res = SIDAL.GetLastSyncDates();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FirstTimeSetup()
        {
            try
            {
                BrannanSyncProvider bProvider = new BrannanSyncProvider(GetCompany().CompanyId);
                bProvider.StartSync();

                TempData["Success"] = "Initial setup complete";
            }
            catch (Exception e)
            {
                TempData["Fail"] = e.Message;
            }
            return RedirectToAction("InitialSetup");
        }

        [APIFilter]
        public ActionResult InitialSetup()
        {
            return View();
        }

        #endregion

        #region ResetPassword

        [HttpPost]
        public ActionResult ResetPasswordRules(MiscView model)
        {
            var currentUser = SIDAL.FindUserByName(User.Identity.Name);
            SuperUserSetting setting = new SuperUserSetting();
            setting.Id = model.UserPasswordSettings.Id;
            setting.RequireOneCaps = model.UserPasswordSettings.RequireOneCaps;
            setting.RequireOneDigit = model.UserPasswordSettings.RequireOneDigit;
            setting.RequireOneLower = model.UserPasswordSettings.RequireOneLower;
            setting.RequireSpecialChar = model.UserPasswordSettings.RequireSpecialChar;
            setting.MaximumPasswordAge = model.UserPasswordSettings.MaximumPasswordAge;
            setting.MinimumLength = model.UserPasswordSettings.MinimumLength;
            setting.PasswordHistoryLimit = model.UserPasswordSettings.PasswordHistoryLimit;

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ExpirePasswordForExistingUsers)
                    {
                        SIDAL.ExpirePassword(currentUser.UserId, model.UserPasswordSettings.MaximumPasswordAge);
                    }
                    SIDAL.UpdateSuperUserSettings(setting);
                    ViewBag.Message = String.Format("Your settings has been updated successfully.");
                }
                catch (Exception e)
                {
                    ViewBag.Error = "Super User Setting Could not be saved. Please try again";
                    return View(model);
                }
            }

            TempData["Message"] = "Saved Successfully";
            return RedirectToAction("ManageMiscSettings");
        }
        #endregion

        #region User Support

        public ActionResult UserSupport()
        {
            return View();
        }

        #endregion

        #region Ticket And Driver Upload
        public ActionResult ManageESIData()
        {
            return View();
        }

        public ActionResult UploadTickets()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                ESIDataManager manager = new ESIDataManager();
                manager.ImportTicketData(file);
                //manager.UpdateMongoByEsiCache();
                //manager.UpdateMongoByEsiCacheNew();

            }
            return RedirectToAction("ManageESIData");
        }

        public ActionResult UploadDriverLogins()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                ESIDataManager manager = new ESIDataManager();
                manager.ImportDriverDetails(file);
                manager.ImportDriverLoginTimes(file);
                //manager.ProcessEsiCache(Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear"]));
                //manager.UpdateMongoByEsiCacheNew();
                //manager.UpdateMongoByDailyPlantSummary();
                //for (int i = 1; i <= 12; i++)
                //{
                //    //Console.WriteLine("UploadPlantDayStats 2014");
                //    manager.UploadPlantDayStats(i, 2015);
                //    //Console.WriteLine("UploadPlantDayStats 2015"); //Step 5 :  Same as step 1 
                //    manager.UploadPlantDayStats(i, 2016);
                //    //Console.WriteLine("UploadPlantDayStats 2016");
                //    manager.UploadPlantDayStats(i, 2017);
                //}
            }
            return RedirectToAction("ManageESIData");
        }

        [HttpPost]
        public ActionResult UploadToDatabase()
        {
            var uploadStatus = true;
            ESIDataManager manager = new ESIDataManager();
            try
            {
                //manager.ProcessEsiCache(Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear"]));
                manager.UpdateMongoByEsiCacheNew();
                manager.UpdateMongoByDailyPlantSummary();
                for (int i = 1; i <= 12; i++)
                {
                    //Console.WriteLine("UploadPlantDayStats 2014");
                    manager.UploadPlantDayStats(i, 2015);
                    //Console.WriteLine("UploadPlantDayStats 2015"); //Step 5 :  Same as step 1 
                    manager.UploadPlantDayStats(i, 2016);
                    //Console.WriteLine("UploadPlantDayStats 2016");
                    manager.UploadPlantDayStats(i, 2017);
                }
            }
            catch (Exception ex)
            {
                uploadStatus = false;
            }

            JObject o = new JObject();
            o["status"] = uploadStatus;

            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region WorkDaysDistribution 
        public ActionResult WorkDayDistribution()
        {
            WorkDayCalendar calendar = new WorkDayCalendar();
            ViewBag.CanEdit = true;
            return View(calendar);
        }

        public string UpdateWeekDayDistribution(int monday, int tuesday, int wednesday, int thursday, int friday, int saturday, int sunday)
        {
            SIDAL.UpdateWeekDayDistribution(monday, tuesday, wednesday, thursday, friday, saturday, sunday);
            SIDAL.ApplyExceptions();
            return "OK";
        }

        public ActionResult EditException(int id)
        {
            WorkDayCalendar calendar = new WorkDayCalendar();
            calendar.WorkDayException = SIDAL.FindWorkDayException(id);
            ViewBag.CanEdit = true;
            return View("WorkDayDistribution", calendar);
        }

        public ActionResult DeleteException(int id)
        {
            WorkDayException exception = SIDAL.FindWorkDayException(id);
            SIDAL.DeleteException(exception.ExceptionDate);
            return RedirectToAction("WorkDayDistribution");
        }
        public ActionResult AddException(WorkDayCalendar calendar)
        {
            if (calendar.WorkDayException.ExceptionDate == null)
            {
                ViewBag.Error = "Date cannot be empty";
                return RedirectToAction("WorkDayDistribution");
            }

            SIDAL.AddException(calendar.WorkDayException.ExceptionDate, calendar.WorkDayException.Description, calendar.WorkDayException.Distribution);
            return RedirectToAction("WorkDayDistribution");
        }


        #endregion

        #region Custom Metric Type
        //public ActionResult CustomMetrics()
        //{
        //    CustomMetricsModel customMetric = new CustomMetricsModel();
        //    ViewBag.Plants = SIDAL.GetPlantsList();
        //    return View(customMetric);
        //}

        //public string AddUpdateCustomMetrics(DateTime day, int plantId, string metricType, string metricValue)
        //{
        //    bool success = false;
        //    success = SIDAL.AddUpdateCustomMetric(day, plantId, metricType, metricValue);

        //    return (success.ToString());

        //}

        //public ActionResult LoadCustomMetricValues(DateTime day, int plantId)
        //{
        //    DailyPlantSummaryModel model = new DailyPlantSummaryModel(SIDAL.FindDailyPlantSummary(day, plantId));
        //    return Json(model.ToJson());
        //}
        #endregion
    }
}
