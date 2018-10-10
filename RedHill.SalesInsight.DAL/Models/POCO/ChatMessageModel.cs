using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.Models.POCO
{
    public class ChatMessageModel
    {
        public ChatMessageModel()
        {
        }

        public ChatMessageModel(ChatMessage chatMessage, string userName)
        {
            if (chatMessage != null)
            {
                this.Id = chatMessage.Id;
                this.ChatConversationId = chatMessage.ChatConversationId;
                this.CompanyUserId = chatMessage.CompanyUserId;
                this.Message = chatMessage.Message;
                this.CreatedAt = chatMessage.CreatedAt.GetValueOrDefault();
                this.UserName = userName;
            }
        }

        public Guid Id { get; set; }
        public Guid? ChatConversationId { get; set; }
        public int? CompanyUserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string TimeStamp
        {
            get
            {
                return this.CreatedAt.ToString("yyyyMMddHHmm");
            }
        }
    }
}
