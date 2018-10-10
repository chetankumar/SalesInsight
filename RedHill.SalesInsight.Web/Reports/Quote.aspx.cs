using System;
using Microsoft.Reporting.WebForms;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Reports
{
    public partial class Quote : System.Web.UI.Page
    {
        //---------------------------------
        // Initialize
        //---------------------------------

        #region protected void Page_Load(object sender, EventArgs e)

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if(SIAuthenticate.AuthenticateByRequest(Request))
                {
                    // Parse the params
                    Guid userId = SIQueryString.GetGuid(Request, "userid");
                    int quoteId = SIQueryString.GetInt(Request, "quoteid");
            
                    // Generate the report
                    
                }
                else
                {
                    throw (new UnauthorizedAccessException());
                }            
            }
        }

        #endregion

        //---------------------------------
        // Methods
        //---------------------------------

        
    }
}
