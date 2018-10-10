using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class NotificationModel
    {
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public long QuotationId { get; set; }
        public int ProjectId { get; set; }
        public long ConversationId { get; set; }
        public string NotificationType { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid UserId { get; set; }
        public bool IsRead { get; set; }
        public string MessageSubject { get; set; }
        public string MessageText { get; set; }

        public NotificationModel()
        {
        }

        public NotificationModel(Notification entity)
        {
            this.Id = entity.Id;
            this.CreatedBy = entity.CreatedByUser;
            this.QuotationId = entity.QuotationId; //.GetValueOrDefault();
            //this.ProjectId = entity.ProjectId; //.GetValueOrDefault();
            //this.ConversationId = entity.ConversationId.GetValueOrDefault();
            this.NotificationType = entity.NotificationType;
            this.Status = entity.Status;
            this.CreatedOn = entity.CreatedOn;
            this.IsRead = entity.IsRead.GetValueOrDefault(false) ;
            this.UserId = entity.UserId;
            this.MessageSubject = entity.MessageSubject;
            this.MessageText = entity.MessageText;
        }

        public Notification ToEntity()
        {
            Notification entity = new Notification();
            entity.Id = this.Id;
            entity.CreatedByUser = this.CreatedBy;
            entity.QuotationId = this.QuotationId;
            //entity.ProjectId = this.ProjectId;
            //entity.ConversationId = this.ConversationId;
            entity.NotificationType = this.NotificationType;
            entity.Status = this.Status;
            entity.CreatedOn = this.CreatedOn;
            entity.IsRead = this.IsRead;
            entity.UserId = this.UserId;
            entity.MessageSubject = this.MessageSubject;
            entity.MessageText = this.MessageText;
            return entity;
        }

        public bool IsProjectEntryType
        {
            get
            {
                return NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_PROJECT_ENTRY;
            }
        }

        public bool IsEnableQuoteEdit
        {
            get
            {
                return NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_ENABLE_QUOTATION_EDIT;
            }
        }

        public bool IsCommentType
        {
            get
            {
                return NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_COMMENT;
            }
        }

        public bool IsApprovedType
        {
            get
            {
                return NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_APPROVED;
            }
        }

        public bool IsApprovalType
        {
            get
            {
                return NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_QUOTATION_APPROVAL;
            }
        }
    }
}