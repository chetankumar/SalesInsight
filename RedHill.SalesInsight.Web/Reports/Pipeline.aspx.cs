using System;
using Microsoft.Reporting.WebForms;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;
using System.Collections.Generic;

namespace RedHill.SalesInsight.Web.Reports
{
    public partial class Pipeline : System.Web.UI.Page
    {
        //---------------------------------
        // Initialize
        //---------------------------------

        #region protected void Page_Load(object sender, EventArgs e)

        protected void Page_Load(object sender, EventArgs e)
        {
            // If this is the first load
            if (!IsPostBack)
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
            
            // Get the projects

            List<string> sortList = new List<string>();
            if (sorts!=null)
                sortList.AddRange(sorts);

            List<RedHill.SalesInsight.DAL.DataTypes.SIPipelineProject> projects = SIDAL.GetPipeline(userId, sortList , search, districtIds, projectStatusIds, plantIds, salesStaffIds, 0, 5000);
            

            // Create the datasource
            reportView.LocalReport.DataSources.Clear();
            reportView.LocalReport.DataSources.Add(new ReportDataSource("SIPipelineProject", projects));

            // Refresh the report
            reportView.LocalReport.Refresh();
        }

        #endregion
    }
}
