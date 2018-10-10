using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using RedHill.SalesInsight.DAL.DataTypes;

namespace RedHill.SalesInsight.Web.Reports
{
    public partial class WonLost : System.Web.UI.Page
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
                    // Parse the params
                    Guid userId = SIQueryString.GetGuid(Request, "userid");
                    int[] districtIds = SIQueryString.GetIntCollection(Request, "districtids", ',', null);
                    int[] plantIds = SIQueryString.GetIntCollection(Request, "plantids", ',', null);
                    int[] regionIds = SIQueryString.GetIntCollection(Request, "regionids", ',', null);
                    int[] salesStaffIds = SIQueryString.GetIntCollection(Request, "staffids", ',', null);
                    long bidDateUtcFrom = SIQueryString.GetLong(Request, "biddatefrom", long.MinValue);
                    long bidDateUtcTo = SIQueryString.GetLong(Request, "biddateto", long.MinValue);
                    long startDateUtcFrom = SIQueryString.GetLong(Request, "startdatefrom", long.MinValue);
                    long startDateUtcTo = SIQueryString.GetLong(Request, "startdateto", long.MinValue);
                    DateTime? bidDateFrom = bidDateUtcFrom != long.MinValue ? (DateTime?)DateTime.FromFileTimeUtc(bidDateUtcFrom) : null;
                    DateTime? bidDateTo = bidDateUtcTo != long.MinValue ? (DateTime?)DateTime.FromFileTimeUtc(bidDateUtcTo) : null;
                    DateTime? startDateFrom = startDateUtcFrom != long.MinValue ? (DateTime?)DateTime.FromFileTimeUtc(startDateUtcFrom) : null;
                    DateTime? startDateTo = startDateUtcTo != long.MinValue ? (DateTime?)DateTime.FromFileTimeUtc(startDateUtcTo) : null;

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
