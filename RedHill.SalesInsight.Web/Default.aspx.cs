using System;

namespace RedHill.SalesInsight.Web
{
    public partial class Default : System.Web.UI.Page
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region protected void Page_Load(object sender, EventArgs e)

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set the initialize params
            xamlSalesInsight.InitParameters = string.Format
            (
                "SalesInsightServiceUrl={0},SalesInsightReportsUrl={1}",
                SISalesInsightService.GetServiceUrl(Request),
                SISalesInsightReports.GetReportsUrl(Request)
            );
        }

        #endregion
    }
}
