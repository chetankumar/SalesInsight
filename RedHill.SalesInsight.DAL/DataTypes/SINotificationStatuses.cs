using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SINotificationStatuses
    {
        public const string NOTIFICATION_TYPE_ENABLE_QUOTATION_EDIT = "ENABLE_QUOTATION_EDITING";
        public const string NOTIFICATION_TYPE_QUOTATION_APPROVAL = "QUOTATION_APPROVAL";
        public const string NOTIFICATION_TYPE_PROJECT_ENTRY = "PROJECT_ENTRY";
        public const string NOTIFICATION_TYPE_COMMENT = "QUOTATION_COMMENT";
        public const string NOTIFICATION_TYPE_APPROVED = "QUOTE_APPROVED";
        //public const string NOTIFICATION_TYPE_QUOTATION_EDITING = "QUOTATION_EDIT_ENBLED";


        public const string APPROVAL_STATUS_NEW = "NEW";
        public const string APPROVAL_STATUS_APPROVED = "APPROVED";
        public const string QUOTATION_EDITING_STATUS_ENABLED = "ENABLED";
    }
}
