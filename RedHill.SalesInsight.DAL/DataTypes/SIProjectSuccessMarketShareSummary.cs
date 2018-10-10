using System;
using System.Collections.Generic;
using System.Linq;


namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProjectSuccessMarketShareSummary
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SIProjectSuccessMarketShareSummary()
        {
            name = string.Empty;
            volume = int.MinValue;
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region Name

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        #endregion Name

        #region Volume

        public int Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }

        #endregion Volume
        
        //---------------------------------
        // Methods
        //---------------------------------

        #region public static List<SIProjectSuccessMarketShareSummary> Get(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo)

        public static List<SIProjectSuccessMarketShareSummary> Get(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo, DateTime? wlDateFrom, DateTime? wlDateTo)
        {
            // Validate the parameter(s)
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Create value and record parameter delimiter strings
                string valueDelimiter = System.Text.Encoding.ASCII.GetString(new byte[3] { 2, 2, 2 });
                string recordDelimiter = System.Text.Encoding.ASCII.GetString(new byte[3] { 3, 3, 3 });

                // Convert id int arrays to id delimited strings
                string delimitedRegionIds = (regionIds == null || regionIds.Length <= 0 ? string.Empty : string.Join(recordDelimiter, regionIds.Select(id => id.ToString()).ToArray()));
                string delimitedDistrictIds = (districtIds == null || districtIds.Length <= 0 ? string.Empty : string.Join(recordDelimiter, districtIds.Select(id => id.ToString()).ToArray()));
                string delimitedPlantIds = (plantIds == null || plantIds.Length <= 0 ? string.Empty : string.Join(recordDelimiter, plantIds.Select(id => id.ToString()).ToArray()));
                string delimitedSalesStaffIds = (salesStaffIds == null || salesStaffIds.Length <= 0 ? string.Empty : string.Join(recordDelimiter, salesStaffIds.Select(id => id.ToString()).ToArray()));

                // Get the results
                var result = context.GetProjectSuccessMarketShareSummary(userId, delimitedRegionIds, delimitedDistrictIds, delimitedPlantIds, delimitedSalesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo,wlDateFrom,wlDateTo, recordDelimiter, valueDelimiter);

                // Return the results
                return (result == null ? new List<SIProjectSuccessMarketShareSummary>(0) : result.ToList<SIProjectSuccessMarketShareSummary>());
            }
        }

        #endregion public static List<SIProjectSuccessMarketShareSummary> Get(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDate, DateTime? startDate)

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        protected string name;
        protected int volume;

        #endregion Fields
    }
}
