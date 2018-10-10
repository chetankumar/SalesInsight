using System;
using Microsoft.Reporting.WebForms;
using RedHill.SalesInsight.DAL.DataTypes;

namespace RedHill.SalesInsight.Web.Reports
{
    public partial class Dashboard : System.Web.UI.Page
    {
        //---------------------------------
        // Initialize
        //---------------------------------

        #region protected void Page_Load(object sender, EventArgs e)

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (SIAuthenticate.AuthenticateByRequest(Request))
                {

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
