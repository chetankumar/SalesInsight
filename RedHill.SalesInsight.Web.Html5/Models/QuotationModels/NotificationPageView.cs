using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class NotificationPageView
    {
        public Guid UserId { get; set; }
        public List<NotificationModel> Notifications { get; set; }
        public PipelineFilter Filter { get; set; }
        public string NotificationType { get; set; }
        public int NumResults { get; set; }
        public int[] Districts { get; set; }
        public int[] Plants { get; set; }
        public int[] Staffs { get; set; }

        public List<SelectListItem> NotificationTypes 
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                
                item.Value = SINotificationStatuses.NOTIFICATION_TYPE_QUOTATION_APPROVAL;
                item.Text = "Quotation Approval Requests";
                item.Selected = this.NotificationType == item.Value;
                items.Add(item);

                item = new SelectListItem();
                item.Value = SINotificationStatuses.NOTIFICATION_TYPE_PROJECT_ENTRY;
                item.Text = "Project Entry Form Notifications";
                item.Selected = this.NotificationType == item.Value;
                items.Add(item);

                return items;
            }
        }

        public NotificationPageView()
        {
            
        }

        public void Load()
        {
            int count = 0;
            List<Notification> notifications = SIDAL.GetNotifications(this.UserId,this.NotificationType, this.Districts, this.Plants, this.Staffs, Filter.SearchTerm, this.Filter.CurrentStart, Filter.RowsPerPage, out count);
            this.NumResults = count;
            this.Notifications = new List<NotificationModel>();
            foreach (Notification notification in notifications)
            {
                this.Notifications.Add(new NotificationModel(notification));
            }
        }

        public object HumanName(string status)
        {
            if (status == SINotificationStatuses.NOTIFICATION_TYPE_PROJECT_ENTRY)
                return "Project entry notification";
            if (status == SINotificationStatuses.NOTIFICATION_TYPE_QUOTATION_APPROVAL)
                return "Quotation Approval Request";
            if (status == SINotificationStatuses.NOTIFICATION_TYPE_COMMENT)
                return "Quotation Comments";
            if (status == SINotificationStatuses.NOTIFICATION_TYPE_APPROVED)
                return "Quotation Approved";
            if (status == SINotificationStatuses.NOTIFICATION_TYPE_ENABLE_QUOTATION_EDIT)
                return "Enable Quotation Editing";
            return "";
        }
    }
}