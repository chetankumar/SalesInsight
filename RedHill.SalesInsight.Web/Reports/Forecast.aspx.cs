using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Reports
{
    public partial class Forecast : System.Web.UI.Page
    {
        //---------------------------------
        // Initialize
        //---------------------------------

        #region protected void Page_Load(object sender, EventArgs e)

        protected void Page_Load(object sender, EventArgs e)
        {
            // If this is the first load
            if(!IsPostBack)
            {
                // Parse the params
                Guid userId         = SIQueryString.GetGuid(Request, "userid");
                string[] sortList   = SIQueryString.GetStringCollection(Request, "sorts", ',', null);
                string search       = SIQueryString.GetString(Request, "search", null);
                int[] districtIds   = SIQueryString.GetIntCollection(Request, "districtids", ',', null);
                int[] statusIds     = SIQueryString.GetIntCollection(Request, "statusids", ',', null);
                int[] plantIds      = SIQueryString.GetIntCollection(Request, "plantids", ',', null);
                int[] staffIds      = SIQueryString.GetIntCollection(Request, "staffids", ',', null);

                // Generate the report
                GenerateReport
                (
                    userId,
                    sortList,
                    search,
                    districtIds,
                    statusIds,
                    plantIds,
                    staffIds
                );
            }
        }

        #endregion

        //---------------------------------
        // Methods
        //---------------------------------

        #region protected void GenerateReport(Guid userId, string[] sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds)

        protected void GenerateReport(Guid userId, string[] sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds)
        {
            // Get the service
            //SalesInsightClient client = SISalesInsightService.GetSalesInsightClient(Request);


            List<string> sortList = new List<string>();
            if (sorts != null) 
                sortList.AddRange(sorts);

            RedHill.SalesInsight.DAL.DataTypes.SIForecastProjects projects = SIDAL.GetForecast(userId, sortList, search, districtIds, projectStatusIds, plantIds, salesStaffIds, 0, 5000);

            
            // Create the datasource
            reportView.LocalReport.DataSources.Clear();
            reportView.LocalReport.DataSources.Add(new ReportDataSource("SIForecastProject", projects.Projects));

            // Create the projectionMonth list
            List<string> projectionMonths = new List<string>();

            // Add the month
            projectionMonths.Add(projects.ProjectionMonths[0].ToString("MMM") + " Actual");
            projectionMonths.Add(projects.ProjectionMonths[0].ToString("MMM") + " Projected");
            projectionMonths.Add(projects.ProjectionMonths[1].ToString("MMM"));
            projectionMonths.Add(projects.ProjectionMonths[2].ToString("MMM"));
            projectionMonths.Add(projects.ProjectionMonths[3].ToString("MMM"));
            projectionMonths.Add(projects.ProjectionMonths[4].ToString("MMM"));
            projectionMonths.Add(projects.ProjectionMonths[5].ToString("MMM"));
            projectionMonths.Add(projects.ProjectionMonths[6].ToString("MMM"));

            // Create the params
            List<ReportParameter> parameters = new List<ReportParameter>();

            // Add the columns
            parameters.Add(new ReportParameter("ProjectionMonths", projectionMonths.ToArray()));

            // Set the params
            reportView.LocalReport.SetParameters(parameters);

            // Refresh the report
            reportView.LocalReport.Refresh();
        }

        #endregion
    }
}
