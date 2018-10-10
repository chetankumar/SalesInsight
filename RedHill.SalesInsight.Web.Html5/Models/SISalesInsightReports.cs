using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web;

namespace RedHill.SalesInsight.Web.Silverlight.Code.Utilities
{
    public class SISalesInsightReports
    {
        //---------------------------------
        // Methods
        //---------------------------------


        #region protected static string GetAuthenticateParams(string password)



        #endregion

        #region protected static string GetPrintReportParams(Guid userId, string password, ObservableCollection<string> sorts, string search, ObservableCollection<int> districtIds, ObservableCollection<int> projectStatusIds, ObservableCollection<int> plantIds, ObservableCollection<int> salesStaffIds)

        public static string GetPrintReportParams(Guid userId, string[] sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds)
        {
            // Create the params
            StringBuilder paramList = new StringBuilder();

            // Add the items
            paramList.AppendFormat("userid={0}", HttpUtility.UrlEncode(userId.ToString()));

            // If we have sorts
            if (sorts != null && sorts.Length > 0)
            {
                // Create the sorts
                StringBuilder items = new StringBuilder();

                // For all of the sorts
                foreach (string sort in sorts)
                {
                    // Append the sort
                    items.AppendFormat("{0}{1}", items.Length <= 0 ? "" : ",", sort);
                }

                // Add the param
                paramList.AppendFormat
                (
                    "{0}sorts={1}",
                    paramList.Length <= 0 ? "" : "&",
                    HttpUtility.UrlEncode(items.ToString())
                );
            }

            // Add the search
            if (!string.IsNullOrEmpty(search))
            {
                paramList.AppendFormat
                (
                    "{0}search={1}",
                    paramList.Length <= 0 ? "" : "&",
                    HttpUtility.UrlEncode(search)
                );
            }

            // If we have districts to filter
            if (districtIds != null && districtIds.Length > 0)
            {
                // Create the sorts
                StringBuilder items = new StringBuilder();

                // For all of the sorts
                foreach (int id in districtIds)
                {
                    // Append the sort
                    items.AppendFormat("{0}{1}", items.Length <= 0 ? "" : ",", id);
                }

                // Add the param
                paramList.AppendFormat
                (
                    "{0}districtids={1}",
                    paramList.Length <= 0 ? "" : "&",
                    HttpUtility.UrlEncode(items.ToString())
                );
            }

            // If we have districts to filter
            if (projectStatusIds != null && projectStatusIds.Length > 0)
            {
                // Create the sorts
                StringBuilder items = new StringBuilder();

                // For all of the sorts
                foreach (int id in projectStatusIds)
                {
                    // Append the sort
                    items.AppendFormat("{0}{1}", items.Length <= 0 ? "" : ",", id);
                }

                // Add the param
                paramList.AppendFormat
                (
                    "{0}statusids={1}",
                    paramList.Length <= 0 ? "" : "&",
                    HttpUtility.UrlEncode(items.ToString())
                );
            }

            // If we have districts to filter
            if (plantIds != null && plantIds.Length > 0)
            {
                // Create the sorts
                StringBuilder items = new StringBuilder();

                // For all of the sorts
                foreach (int id in plantIds)
                {
                    // Append the sort
                    items.AppendFormat("{0}{1}", items.Length <= 0 ? "" : ",", id);
                }

                // Add the param
                paramList.AppendFormat
                (
                    "{0}plantids={1}",
                    paramList.Length <= 0 ? "" : "&",
                    HttpUtility.UrlEncode(items.ToString())
                );
            }

            // If we have districts to filter
            if (salesStaffIds != null && salesStaffIds.Length > 0)
            {
                // Create the sorts
                StringBuilder items = new StringBuilder();

                // For all of the sorts
                foreach (int id in salesStaffIds)
                {
                    // Append the sort
                    items.AppendFormat("{0}{1}", items.Length <= 0 ? "" : ",", id);
                }

                // Add the param
                paramList.AppendFormat
                (
                    "{0}staffids={1}",
                    paramList.Length <= 0 ? "" : "&",
                    HttpUtility.UrlEncode(items.ToString())
                );
            }

            // Add the authenticate params

            // reutrn the params
            return paramList.ToString();
        }

        #endregion
    }
}
