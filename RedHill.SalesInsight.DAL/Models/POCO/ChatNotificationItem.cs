using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.Models.POCO
{
    public class ChatNotificationItem
    {
        public Guid Id { get; set; }
        public Guid? MessageByUserId { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }
        public Guid ConversationId { get; set; }
        public int ProjectId { get; set; }
        public long? QuoteId { get; set; }

        public string Message { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string Date { get { return this.CreatedAt.ToString("yyyyMMddHHmm"); } }
    }
}
