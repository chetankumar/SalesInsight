using OfficeOpenXml;
using OfficeOpenXml.Style;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Models.TargetModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class TargetController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region Monthly Productivity Targets
        public ActionResult MonthlyProductivityTargets()
        {
            MonthlyProductivityTargetsView model = new MonthlyProductivityTargetsView(GetUser().UserId);
            return View(model);
        }

        [HttpPost]
        public ActionResult MonthlyProductivityTargets(MonthlyProductivityTargetsView model)
        {
            model.UserId = GetUser().UserId;
            return View(model);
        }

        public string UpdateMonthlyProductivityTarget(int plantId, int year, int month, string metric, float value)
        {
            SIDAL.UpdateMonthlyProductivityTarget(plantId, year, month, metric, value);
            return "OK";
        }

        #endregion

        public ActionResult MonthlyFinancialTargets()
        {
            MonthlyFinancialTargetsView model = new MonthlyFinancialTargetsView(GetUser().UserId);
            return View(model);
        }

        [HttpPost]
        public ActionResult MonthlyFinancialTargets(MonthlyFinancialTargetsView model)
        {
            model.UserId = GetUser().UserId;
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadMonthlyFinancialTargets()
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
                            string budgetYear = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string budgetMonth = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string plantName = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string revenue = Convert.ToString(worksheet.Cells[i, 4].Value);
                            string materialCost = Convert.ToString(worksheet.Cells[i, 5].Value);
                            string deliveryVariable = Convert.ToString(worksheet.Cells[i, 6].Value);
                            string plantVariable = Convert.ToString(worksheet.Cells[i, 7].Value);
                            string deliveryFixed = Convert.ToString(worksheet.Cells[i, 8].Value);
                            string plantFixed = Convert.ToString(worksheet.Cells[i, 9].Value);
                            string sga = Convert.ToString(worksheet.Cells[i, 10].Value);
                            string profit = Convert.ToString(worksheet.Cells[i, 11].Value);
                            string error = SIDAL.ScrubMonthlyFinancialBudget(budgetYear, budgetMonth, plantName, revenue, materialCost, deliveryVariable, plantVariable, deliveryFixed, plantFixed, sga, profit);
                            if (error != null)
                            {
                                worksheet.Cells[i, 12].Value = error;
                                worksheet.Cells[i, 1, i, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, 11].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                worksheet.Cells[i, 1, i, 11].Style.Font.Color.SetColor(Color.White);
                            }
                            else
                            {
                                worksheet.Cells[i, 12].Value = "IMPORTED SUCCESSFULLY";
                                worksheet.Cells[i, 1, i, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, 11].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                worksheet.Cells[i, 1, i, 11].Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 1, i, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[i, 1, i, 11].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            worksheet.Cells[i, 1, i, 11].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[i, 12].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }
                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("MonthlyFinancialTargets");
        }

        public string UpdateMonthlyFinancialTarget(int plantId, int year, int month, string metric, float value)
        {
            SIDAL.UpdateMonthlyFinancialTarget(plantId, year, month, metric, value);
            return "OK";
        }

    }
}
